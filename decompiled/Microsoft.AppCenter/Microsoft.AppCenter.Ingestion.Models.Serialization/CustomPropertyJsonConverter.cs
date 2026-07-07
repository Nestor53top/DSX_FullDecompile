using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AppCenter.Ingestion.Models.Serialization;

public class CustomPropertyJsonConverter : JsonConverter
{
	private readonly Dictionary<string, Type> _customPropertyTypes = new Dictionary<string, Type>
	{
		{
			"boolean",
			typeof(BooleanProperty)
		},
		{
			"clear",
			typeof(ClearProperty)
		},
		{
			"dateTime",
			typeof(DateTimeProperty)
		},
		{
			"number",
			typeof(NumberProperty)
		},
		{
			"string",
			typeof(StringProperty)
		}
	};

	private readonly object _jsonConverterLock = new object();

	private static readonly JsonSerializerSettings SerializationSettings;

	internal const string TypeIdKey = "type";

	public CustomPropertyJsonConverter()
	{
		_customPropertyTypes["boolean"] = typeof(BooleanProperty);
		_customPropertyTypes["clear"] = typeof(ClearProperty);
		_customPropertyTypes["clear"] = typeof(ClearProperty);
		_customPropertyTypes["clear"] = typeof(ClearProperty);
	}

	public override bool CanConvert(Type objectType)
	{
		return typeof(CustomProperty).IsAssignableFrom(objectType);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JObject jObject = JObject.Load(reader);
		string text = jObject.GetValue("type")?.ToString();
		Type objectType2;
		lock (_jsonConverterLock)
		{
			if (text == null || !_customPropertyTypes.ContainsKey(text))
			{
				throw new JsonReaderException("Could not identify type of log");
			}
			objectType2 = _customPropertyTypes[text];
		}
		jObject.Remove("type");
		return jObject.ToObject(objectType2);
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (!(value.GetType().GetTypeInfo().GetCustomAttribute(typeof(JsonObjectAttribute)) is JsonObjectAttribute jsonObjectAttribute))
		{
			throw new JsonWriterException("Cannot serialize property; Log type is missing JsonObjectAttribute");
		}
		JObject jObject = JObject.Parse(JsonConvert.SerializeObject(value, SerializationSettings));
		jObject.Add("type", JToken.FromObject(jsonObjectAttribute.Id));
		writer.WriteRawValue(jObject.ToString());
	}
}
