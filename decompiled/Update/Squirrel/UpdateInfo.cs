using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Squirrel.SimpleSplat;

namespace Squirrel;

[DataContract]
internal class UpdateInfo : IEnableLogger
{
	[DataMember]
	public ReleaseEntry CurrentlyInstalledVersion { get; protected set; }

	[DataMember]
	public ReleaseEntry FutureReleaseEntry { get; protected set; }

	[DataMember]
	public List<ReleaseEntry> ReleasesToApply { get; protected set; }

	[IgnoreDataMember]
	public bool IsBootstrapping => CurrentlyInstalledVersion == null;

	[IgnoreDataMember]
	public string PackageDirectory { get; protected set; }

	protected UpdateInfo(ReleaseEntry currentlyInstalledVersion, IEnumerable<ReleaseEntry> releasesToApply, string packageDirectory)
	{
		CurrentlyInstalledVersion = currentlyInstalledVersion;
		ReleasesToApply = (releasesToApply ?? Enumerable.Empty<ReleaseEntry>()).ToList();
		FutureReleaseEntry = (ReleasesToApply.Any() ? ReleasesToApply.MaxBy((ReleaseEntry x) => x.Version).FirstOrDefault() : CurrentlyInstalledVersion);
		PackageDirectory = packageDirectory;
	}

	public Dictionary<ReleaseEntry, string> FetchReleaseNotes()
	{
		return ReleasesToApply.SelectMany(delegate(ReleaseEntry x)
		{
			try
			{
				string releaseNotes = x.GetReleaseNotes(PackageDirectory);
				return EnumerableExtensions.Return(Tuple.Create(x, releaseNotes));
			}
			catch (Exception exception)
			{
				this.Log().WarnException("Couldn't get release notes for:" + x.Filename, exception);
				return Enumerable.Empty<Tuple<ReleaseEntry, string>>();
			}
		}).ToDictionary((Tuple<ReleaseEntry, string> k) => k.Item1, (Tuple<ReleaseEntry, string> v) => v.Item2);
	}

	public static UpdateInfo Create(ReleaseEntry currentVersion, IEnumerable<ReleaseEntry> availableReleases, string packageDirectory)
	{
		ReleaseEntry releaseEntry = availableReleases.MaxBy((ReleaseEntry x) => x.Version).FirstOrDefault((ReleaseEntry x) => !x.IsDelta);
		if (releaseEntry == null)
		{
			throw new Exception("There should always be at least one full release");
		}
		if (currentVersion == null)
		{
			return new UpdateInfo(null, new ReleaseEntry[1] { releaseEntry }, packageDirectory);
		}
		if (currentVersion.Version >= releaseEntry.Version)
		{
			return new UpdateInfo(currentVersion, Enumerable.Empty<ReleaseEntry>(), packageDirectory);
		}
		IOrderedEnumerable<ReleaseEntry> source = from v in availableReleases
			where v.Version > currentVersion.Version
			orderby v.Version
			select v;
		long num = source.Where((ReleaseEntry x) => x.IsDelta).Sum((ReleaseEntry x) => x.Filesize);
		if (num >= releaseEntry.Filesize || num <= 0)
		{
			return new UpdateInfo(currentVersion, new ReleaseEntry[1] { releaseEntry }, packageDirectory);
		}
		return new UpdateInfo(currentVersion, source.Where((ReleaseEntry x) => x.IsDelta).ToArray(), packageDirectory);
	}
}
