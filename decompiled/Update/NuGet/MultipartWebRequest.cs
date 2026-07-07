using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace NuGet;

internal class MultipartWebRequest
{
	private sealed class PostFileData
	{
		public Func<Stream> FileFactory { get; set; }

		public string ContentType { get; set; }

		public string FieldName { get; set; }

		public long ContentLength { get; set; }
	}

	private const string FormDataTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n";

	private const string FileTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n";

	private readonly Dictionary<string, string> _formData;

	private readonly List<PostFileData> _files;

	public MultipartWebRequest()
		: this(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
	{
	}

	public MultipartWebRequest(Dictionary<string, string> formData)
	{
		_formData = formData;
		_files = new List<PostFileData>();
	}

	public void AddFormData(string key, string value)
	{
		_formData.Add(key, value);
	}

	public void AddFile(Func<Stream> fileFactory, string fieldName, long length, string contentType = "application/octet-stream")
	{
		_files.Add(new PostFileData
		{
			FileFactory = fileFactory,
			FieldName = fieldName,
			ContentType = contentType,
			ContentLength = length
		});
	}

	public void CreateMultipartRequest(WebRequest request)
	{
		string text = "---------------------------" + DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture);
		request.ContentType = "multipart/form-data; boundary=" + text;
		request.ContentLength = CalculateContentLength(text);
		using Stream stream = request.GetRequestStream();
		foreach (KeyValuePair<string, string> formDatum in _formData)
		{
			string s = string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", new object[3] { text, formDatum.Key, formDatum.Value });
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			stream.Write(bytes, 0, bytes.Length);
		}
		byte[] bytes2 = Encoding.UTF8.GetBytes(Environment.NewLine);
		foreach (PostFileData file in _files)
		{
			string s2 = string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n", new object[4] { text, file.FieldName, file.FieldName, file.ContentType });
			byte[] bytes3 = Encoding.UTF8.GetBytes(s2);
			stream.Write(bytes3, 0, bytes3.Length);
			Stream stream2 = file.FileFactory();
			stream2.CopyTo(stream, 4096);
			stream2.Close();
			stream.Write(bytes2, 0, bytes2.Length);
		}
		string s3 = string.Format(CultureInfo.InvariantCulture, "--{0}--", new object[1] { text });
		byte[] bytes4 = Encoding.UTF8.GetBytes(s3);
		stream.Write(bytes4, 0, bytes4.Length);
	}

	private long CalculateContentLength(string boundary)
	{
		long num = 0L;
		foreach (KeyValuePair<string, string> formDatum in _formData)
		{
			string s = string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", new object[3] { boundary, formDatum.Key, formDatum.Value });
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			num += bytes.Length;
		}
		byte[] bytes2 = Encoding.UTF8.GetBytes(Environment.NewLine);
		foreach (PostFileData file in _files)
		{
			string s2 = string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n", new object[4] { boundary, file.FieldName, file.FieldName, file.ContentType });
			byte[] bytes3 = Encoding.UTF8.GetBytes(s2);
			num += bytes3.Length;
			num += file.ContentLength;
			num += bytes2.Length;
		}
		string s3 = string.Format(CultureInfo.InvariantCulture, "--{0}--", new object[1] { boundary });
		byte[] bytes4 = Encoding.UTF8.GetBytes(s3);
		return num + bytes4.Length;
	}
}
