using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AppCenter.Ingestion.Models.Serialization;

public class LogJsonConverter : JsonConverter
{
	private readonly Dictionary<string, Type> _logTypes = new Dictionary<string, Type>();

	private readonly object _jsonConverterLock = new object();

	private static readonly JsonSerializerSettings SerializationSettings;

	internal const string TypeIdKey = "type";

	static LogJsonConverter()
	{
		SerializationSettings = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			DateFormatHandling = DateFormatHandling.IsoDateFormat,
			DateTimeZoneHandling = DateTimeZoneHandling.Utc,
			NullValueHandling = NullValueHandling.Ignore,
			ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
			Converters = { (JsonConverter)new CustomPropertyJsonConverter() }
		};
	}

	public void AddLogType(string typeName, Type type)
	{
		lock (_jsonConverterLock)
		{
			_logTypes[typeName] = type;
		}
	}

	public override bool CanConvert(Type objectType)
	{
		return typeof(Log).IsAssignableFrom(objectType);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JObject jObject = JObject.Load(reader);
		string text = jObject.GetValue("type")?.ToString();
		Type type;
		lock (_jsonConverterLock)
		{
			if (text == null || !_logTypes.ContainsKey(text))
			{
				throw new JsonReaderException("Could not identify type of log");
			}
			type = _logTypes[text];
			jObject.Remove("type");
			if (type == typeof(CustomPropertyLog))
			{
				return ReadCustomPropertyLog(jObject);
			}
		}
		return jObject.ToObject(type);
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (!(value.GetType().GetTypeInfo().GetCustomAttribute(typeof(JsonObjectAttribute)) is JsonObjectAttribute jsonObjectAttribute))
		{
			throw new JsonWriterException("Cannot serialize log; Log type is missing JsonObjectAttribute");
		}
		JObject jObject = JObject.Parse(JsonConvert.SerializeObject(value, SerializationSettings));
		jObject.Add("type", JToken.FromObject(jsonObjectAttribute.Id));
		writer.WriteRawValue(jObject.ToString());
	}

	public Log ReadCustomPropertyLog(JObject logObject)
	{
		string propertyName = "properties";
		JToken? value = logObject.GetValue(propertyName);
		logObject.Remove(propertyName);
		CustomPropertyLog customPropertyLog = logObject.ToObject(typeof(CustomPropertyLog)) as CustomPropertyLog;
		foreach (JToken item2 in value.Children())
		{
			CustomProperty item = JsonConvert.DeserializeObject<CustomProperty>(item2.ToString(), SerializationSettings);
			customPropertyLog.Properties.Add(item);
		}
		return customPropertyLog;
	}
}
