using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models;

public class StackFrame
{
	[JsonProperty(PropertyName = "address")]
	public string Address { get; set; }

	[JsonProperty(PropertyName = "code")]
	public string Code { get; set; }

	[JsonProperty(PropertyName = "className")]
	public string ClassName { get; set; }

	[JsonProperty(PropertyName = "methodName")]
	public string MethodName { get; set; }

	[JsonProperty(PropertyName = "lineNumber")]
	public int? LineNumber { get; set; }

	[JsonProperty(PropertyName = "fileName")]
	public string FileName { get; set; }

	public StackFrame()
	{
	}

	public StackFrame(string address = null, string code = null, string className = null, string methodName = null, int? lineNumber = null, string fileName = null)
	{
		Address = address;
		Code = code;
		ClassName = className;
		MethodName = methodName;
		LineNumber = lineNumber;
		FileName = fileName;
	}
}
