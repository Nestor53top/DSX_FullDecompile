using System;
using System.Text;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes;

[JsonObject("errorAttachment")]
public class ErrorAttachmentLog : Log
{
	internal const string JsonIdentifier = "errorAttachment";

	private const string ContentTypePlainText = "text/plain";

	[JsonProperty(PropertyName = "id")]
	public Guid Id { get; set; }

	[JsonProperty(PropertyName = "errorId")]
	public Guid ErrorId { get; set; }

	[JsonProperty(PropertyName = "contentType")]
	public string ContentType { get; set; }

	[JsonProperty(PropertyName = "fileName")]
	public string FileName { get; set; }

	[JsonProperty(PropertyName = "data")]
	public byte[] Data { get; set; }

	public static ErrorAttachmentLog AttachmentWithText(string text, string fileName)
	{
		return PlatformAttachmentWithText(text, fileName);
	}

	public static ErrorAttachmentLog AttachmentWithBinary(byte[] data, string fileName, string contentType)
	{
		return PlatformAttachmentWithBinary(data, fileName, contentType);
	}

	private static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
	{
		if (text == null)
		{
			return null;
		}
		return PlatformAttachmentWithBinary(Encoding.UTF8.GetBytes(text), fileName, "text/plain");
	}

	private static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string fileName, string contentType)
	{
		if (data == null)
		{
			return null;
		}
		return new ErrorAttachmentLog
		{
			Data = data,
			FileName = fileName,
			ContentType = contentType
		};
	}

	public override void Validate()
	{
		base.Validate();
		if (ContentType == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "ContentType");
		}
		if (Data == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Data");
		}
	}

	public bool ValidatePropertiesForAttachment()
	{
		bool num = ErrorId != Guid.Empty;
		bool flag = Id != Guid.Empty;
		bool flag2 = Data != null && Data.Length != 0;
		bool flag3 = !string.IsNullOrEmpty(ContentType);
		return num && flag && flag2 && flag3;
	}
}
