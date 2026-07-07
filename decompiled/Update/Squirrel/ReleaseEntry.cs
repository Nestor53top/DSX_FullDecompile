using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NuGet;
using Squirrel.SimpleSplat;

namespace Squirrel;

[DataContract]
internal class ReleaseEntry : IEnableLogger, IReleaseEntry
{
	private static readonly Regex packageNameRegex = new Regex("^([\\w-]+)-\\d+\\..+\\.nupkg$");

	private static readonly Regex entryRegex = new Regex("^([0-9a-fA-F]{40})\\s+(\\S+)\\s+(\\d+)[\\r]*$");

	private static readonly Regex commentRegex = new Regex("\\s*#.*$");

	private static readonly Regex stagingRegex = new Regex("#\\s+(\\d{1,3})%$");

	[DataMember]
	public string SHA1 { get; protected set; }

	[DataMember]
	public string BaseUrl { get; protected set; }

	[DataMember]
	public string Filename { get; protected set; }

	[DataMember]
	public string Query { get; protected set; }

	[DataMember]
	public long Filesize { get; protected set; }

	[DataMember]
	public bool IsDelta { get; protected set; }

	[DataMember]
	public float? StagingPercentage { get; protected set; }

	[IgnoreDataMember]
	public string EntryAsString
	{
		get
		{
			if (StagingPercentage.HasValue)
			{
				return string.Format("{0} {1}{2} {3} # {4}", new object[5]
				{
					SHA1,
					BaseUrl,
					Filename,
					Filesize,
					stagingPercentageAsString(StagingPercentage.Value)
				});
			}
			return string.Format("{0} {1}{2} {3}", new object[4] { SHA1, BaseUrl, Filename, Filesize });
		}
	}

	[IgnoreDataMember]
	public SemanticVersion Version => Filename.ToSemanticVersion();

	[IgnoreDataMember]
	public string PackageName
	{
		get
		{
			Match match = packageNameRegex.Match(Filename);
			if (!match.Success)
			{
				return Filename.Substring(0, Filename.IndexOfAny(new char[2] { '-', '.' }));
			}
			return match.Groups[1].Value;
		}
	}

	protected ReleaseEntry(string sha1, string filename, long filesize, bool isDelta, string baseUrl = null, string query = null, float? stagingPercentage = null)
	{
		SHA1 = sha1;
		BaseUrl = baseUrl;
		Filename = filename;
		Query = query;
		Filesize = filesize;
		IsDelta = isDelta;
		StagingPercentage = stagingPercentage;
	}

	public string GetReleaseNotes(string packageDirectory)
	{
		ZipPackage zipPackage = new ZipPackage(Path.Combine(packageDirectory, Filename));
		_ = zipPackage.Id;
		if (string.IsNullOrWhiteSpace(zipPackage.ReleaseNotes))
		{
			throw new Exception($"Invalid 'ReleaseNotes' value in nuspec file at '{Path.Combine(packageDirectory, Filename)}'");
		}
		return zipPackage.ReleaseNotes;
	}

	public Uri GetIconUrl(string packageDirectory)
	{
		return new ZipPackage(Path.Combine(packageDirectory, Filename)).IconUrl;
	}

	public static ReleaseEntry ParseReleaseEntry(string entry)
	{
		float? stagingPercentage = null;
		Match match = stagingRegex.Match(entry);
		if (match != null && match.Success)
		{
			stagingPercentage = float.Parse(match.Groups[1].Value) / 100f;
		}
		entry = commentRegex.Replace(entry, "");
		if (string.IsNullOrWhiteSpace(entry))
		{
			return null;
		}
		match = entryRegex.Match(entry);
		if (!match.Success)
		{
			throw new Exception("Invalid release entry: " + entry);
		}
		if (match.Groups.Count != 4)
		{
			throw new Exception("Invalid release entry: " + entry);
		}
		string text = match.Groups[2].Value;
		string baseUrl = null;
		string query = null;
		if (Utility.IsHttpUrl(text))
		{
			Uri uri = new Uri(text);
			string localPath = uri.LocalPath;
			string leftPart = uri.GetLeftPart(UriPartial.Authority);
			if (string.IsNullOrEmpty(localPath) || string.IsNullOrEmpty(leftPart))
			{
				throw new Exception("Invalid URL");
			}
			int num = localPath.LastIndexOf("/") + 1;
			baseUrl = leftPart + localPath.Substring(0, num);
			text = localPath.Substring(num);
			if (!string.IsNullOrEmpty(uri.Query))
			{
				query = uri.Query;
			}
		}
		if (text.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
		{
			throw new Exception("Filename can either be an absolute HTTP[s] URL, *or* a file name");
		}
		long filesize = long.Parse(match.Groups[3].Value);
		bool isDelta = filenameIsDeltaFile(text);
		return new ReleaseEntry(match.Groups[1].Value, text, filesize, isDelta, baseUrl, query, stagingPercentage);
	}

	public bool IsStagingMatch(Guid? userId)
	{
		if (!StagingPercentage.HasValue)
		{
			return true;
		}
		if (!userId.HasValue)
		{
			return false;
		}
		return (double)BitConverter.ToUInt32(userId.Value.ToByteArray(), 12) / 4294967295.0 < (double)StagingPercentage.Value;
	}

	public static IEnumerable<ReleaseEntry> ParseReleaseFile(string fileContents)
	{
		if (string.IsNullOrEmpty(fileContents))
		{
			return new ReleaseEntry[0];
		}
		fileContents = Utility.RemoveByteOrderMarkerIfPresent(fileContents);
		ReleaseEntry[] array = (from x in (from x in fileContents.Split(new char[1] { '\n' })
				where !string.IsNullOrWhiteSpace(x)
				select x).Select(ParseReleaseEntry)
			where x != null
			select x).ToArray();
		if (!array.Any((ReleaseEntry x) => x == null))
		{
			return array;
		}
		return null;
	}

	public static IEnumerable<ReleaseEntry> ParseReleaseFileAndApplyStaging(string fileContents, Guid? userToken)
	{
		if (string.IsNullOrEmpty(fileContents))
		{
			return new ReleaseEntry[0];
		}
		fileContents = Utility.RemoveByteOrderMarkerIfPresent(fileContents);
		ReleaseEntry[] array = (from x in (from x in fileContents.Split(new char[1] { '\n' })
				where !string.IsNullOrWhiteSpace(x)
				select x).Select(ParseReleaseEntry)
			where x?.IsStagingMatch(userToken) ?? false
			select x).ToArray();
		if (!array.Any((ReleaseEntry x) => x == null))
		{
			return array;
		}
		return null;
	}

	public static void WriteReleaseFile(IEnumerable<ReleaseEntry> releaseEntries, Stream stream)
	{
		using StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
		streamWriter.Write(string.Join("\n", from x in releaseEntries
			orderby x.Version, x.IsDelta descending
			select x.EntryAsString));
	}

	public static void WriteReleaseFile(IEnumerable<ReleaseEntry> releaseEntries, string path)
	{
		using FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
		WriteReleaseFile(releaseEntries, stream);
	}

	public static ReleaseEntry GenerateFromFile(Stream file, string filename, string baseUrl = null)
	{
		return new ReleaseEntry(Utility.CalculateStreamSHA1(file), filename, file.Length, filenameIsDeltaFile(filename), baseUrl);
	}

	public static ReleaseEntry GenerateFromFile(string path, string baseUrl = null)
	{
		using FileStream file = File.OpenRead(path);
		return GenerateFromFile(file, Path.GetFileName(path), baseUrl);
	}

	public static List<ReleaseEntry> BuildReleasesFile(string releasePackagesDir)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(releasePackagesDir);
		ConcurrentQueue<ReleaseEntry> entriesQueue = new ConcurrentQueue<ReleaseEntry>();
		Parallel.ForEach(directoryInfo.GetFiles("*.nupkg"), delegate(FileInfo x)
		{
			using FileStream file = x.OpenRead();
			entriesQueue.Enqueue(GenerateFromFile(file, x.Name));
		});
		List<ReleaseEntry> list = entriesQueue.ToList();
		string path = null;
		Utility.WithTempFile(out path, releasePackagesDir);
		try
		{
			using (FileStream stream = File.OpenWrite(path))
			{
				if (list.Count > 0)
				{
					WriteReleaseFile(list, stream);
				}
			}
			string text = Path.Combine(directoryInfo.FullName, "RELEASES");
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.Move(path, text);
			return list;
		}
		finally
		{
			if (File.Exists(path))
			{
				Utility.DeleteFileHarder(path, ignoreIfFails: true);
			}
		}
	}

	private static string stagingPercentageAsString(float percentage)
	{
		return $"{(double)percentage * 100.0:F0}%";
	}

	private static bool filenameIsDeltaFile(string filename)
	{
		return filename.EndsWith("-delta.nupkg", StringComparison.InvariantCultureIgnoreCase);
	}

	public static ReleasePackage GetPreviousRelease(IEnumerable<ReleaseEntry> releaseEntries, IReleasePackage package, string targetDir)
	{
		if (releaseEntries == null || !releaseEntries.Any())
		{
			return null;
		}
		return (from x in releaseEntries
			where !x.IsDelta
			where x.Version < package.ToSemanticVersion()
			orderby x.Version descending
			select new ReleasePackage(Path.Combine(targetDir, x.Filename), isReleasePackage: true)).FirstOrDefault();
	}
}
