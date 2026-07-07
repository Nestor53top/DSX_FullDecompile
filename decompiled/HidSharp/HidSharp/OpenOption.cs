using System;
using System.Collections.Generic;

namespace HidSharp;

public sealed class OpenOption
{
	private static Dictionary<Guid, OpenOption> _options;

	private OpenOptionDeserializeCallback _deserializeCallback;

	private OpenOptionSerializeCallback _serializeCallback;

	public static OpenOption Exclusive { get; private set; }

	public static OpenOption Interruptible { get; private set; }

	public static OpenOption Priority { get; private set; }

	public static OpenOption TimeoutIfInterruptible { get; private set; }

	public static OpenOption TimeoutIfTransient { get; private set; }

	public static OpenOption Transient { get; private set; }

	internal static OpenOption BleService { get; private set; }

	public object DefaultValue { get; private set; }

	public string FriendlyName { get; private set; }

	public Guid Guid { get; private set; }

	static OpenOption()
	{
		_options = new Dictionary<Guid, OpenOption>();
		OpenOptionDeserializeCallback deserializeCallback = DeserializeBoolean;
		OpenOptionSerializeCallback serializeCallback = SerializeBoolean;
		object defaultValue = false;
		string friendlyName = "Exclusive";
		Exclusive = New(new Guid("{49DB23CD-727E-4788-BBAD-7D67ACCBC469}"), deserializeCallback, serializeCallback, defaultValue, friendlyName);
		OpenOptionDeserializeCallback deserializeCallback2 = DeserializeBoolean;
		OpenOptionSerializeCallback serializeCallback2 = SerializeBoolean;
		object defaultValue2 = false;
		string friendlyName2 = "Interruptible";
		Interruptible = New(new Guid("{55C9673C-A49C-4190-B0BC-294020EAAE54}"), deserializeCallback2, serializeCallback2, defaultValue2, friendlyName2);
		OpenOptionDeserializeCallback deserializeCallback3 = (byte[] buffer) => (buffer.Length < 1) ? null : ((object)(OpenPriority)buffer[0]);
		OpenOptionSerializeCallback serializeCallback3 = (object value) => new byte[1] { (byte)(OpenPriority)value };
		object defaultValue3 = OpenPriority.Normal;
		string friendlyName3 = "Priority";
		Priority = New(new Guid("{3C065A90-A685-44BD-BE06-50EDACF51F11}"), deserializeCallback3, serializeCallback3, defaultValue3, friendlyName3);
		OpenOptionDeserializeCallback deserializeCallback4 = DeserializeInt32;
		OpenOptionSerializeCallback serializeCallback4 = SerializeInt32;
		object defaultValue4 = 3000;
		string friendlyName4 = "TimeoutIfInterruptible";
		TimeoutIfInterruptible = New(new Guid("{C8F9B70B-302F-4326-B28D-E823C4E6131E}"), deserializeCallback4, serializeCallback4, defaultValue4, friendlyName4);
		OpenOptionDeserializeCallback deserializeCallback5 = DeserializeInt32;
		OpenOptionSerializeCallback serializeCallback5 = SerializeInt32;
		object defaultValue5 = 30000;
		string friendlyName5 = "TimeoutIfTransient";
		TimeoutIfTransient = New(new Guid("{0A918B9F-6FF5-4A14-A945-78685B37BF40}"), deserializeCallback5, serializeCallback5, defaultValue5, friendlyName5);
		OpenOptionDeserializeCallback deserializeCallback6 = DeserializeBoolean;
		OpenOptionSerializeCallback serializeCallback6 = SerializeBoolean;
		object defaultValue6 = false;
		string friendlyName6 = "Transient";
		Transient = New(new Guid("{C564DE4B-A9A8-4F5F-A7E4-1A14AF9BEFEC}"), deserializeCallback6, serializeCallback6, defaultValue6, friendlyName6);
		OpenOptionDeserializeCallback deserializeCallback7 = delegate
		{
			throw new NotImplementedException();
		};
		OpenOptionSerializeCallback serializeCallback7 = delegate
		{
			throw new NotImplementedException();
		};
		object defaultValue7 = null;
		string friendlyName7 = "BLE Service";
		BleService = New(new Guid("{A0E7B2C1-656D-40FB-9C29-3CD28F54D45D}"), deserializeCallback7, serializeCallback7, defaultValue7, friendlyName7);
	}

	private OpenOption()
	{
	}

	public override bool Equals(object obj)
	{
		return obj == this;
	}

	public override int GetHashCode()
	{
		return Guid.GetHashCode();
	}

	public override string ToString()
	{
		return FriendlyName;
	}

	public static OpenOption FromGuid(Guid guid)
	{
		lock (_options)
		{
			OpenOption value;
			return _options.TryGetValue(guid, out value) ? value : null;
		}
	}

	public static OpenOption New(Guid guid, OpenOptionDeserializeCallback deserializeCallback, OpenOptionSerializeCallback serializeCallback, object defaultValue = null, string friendlyName = null)
	{
		Throw.If.Null(deserializeCallback, "deserializeCallback");
		Throw.If.Null(serializeCallback, "serializeCallback");
		lock (_options)
		{
			if (_options.ContainsKey(guid))
			{
				throw new ArgumentException();
			}
			OpenOption openOption = new OpenOption();
			openOption.Guid = guid;
			openOption.DefaultValue = defaultValue;
			openOption.FriendlyName = friendlyName ?? guid.ToString("B");
			openOption._deserializeCallback = deserializeCallback;
			openOption._serializeCallback = serializeCallback;
			OpenOption openOption2 = openOption;
			_options.Add(guid, openOption2);
			return openOption2;
		}
	}

	private static object DeserializeBoolean(byte[] buffer)
	{
		if (buffer.Length < 1)
		{
			return null;
		}
		return (buffer[0] & 1) != 0;
	}

	private static byte[] SerializeBoolean(object value)
	{
		return new byte[1] { (byte)(((bool)value) ? 1u : 0u) };
	}

	private static object DeserializeInt32(byte[] buffer)
	{
		if (buffer.Length < 4)
		{
			return null;
		}
		return BitConverter.ToInt32(buffer, 0);
	}

	private static byte[] SerializeInt32(object value)
	{
		return BitConverter.GetBytes((int)value);
	}

	public object Deserialize(byte[] buffer)
	{
		return _deserializeCallback(buffer);
	}

	public byte[] Serialize(object value)
	{
		return _serializeCallback(value);
	}
}
