using System;
using System.Collections.Generic;

namespace HidSharp;

public class OpenConfiguration : ICloneable
{
	private Dictionary<OpenOption, object> _options;

	public OpenConfiguration()
	{
		_options = new Dictionary<OpenOption, object>();
	}

	private OpenConfiguration(Dictionary<OpenOption, object> options)
	{
		_options = new Dictionary<OpenOption, object>(options);
	}

	public OpenConfiguration Clone()
	{
		return new OpenConfiguration(_options);
	}

	object ICloneable.Clone()
	{
		return Clone();
	}

	public object GetOption(OpenOption option)
	{
		Throw.If.Null(option, "option");
		if (!_options.TryGetValue(option, out var value))
		{
			return option.DefaultValue;
		}
		return value;
	}

	public IEnumerable<OpenOption> GetOptionsList()
	{
		return _options.Keys;
	}

	public bool IsOptionSet(OpenOption option)
	{
		Throw.If.Null(option, "option");
		return _options.ContainsKey(option);
	}

	public void SetOption(OpenOption option, object value)
	{
		Throw.If.Null(option, "option");
		if (value != null)
		{
			_options[option] = value;
		}
		else
		{
			_options.Remove(option);
		}
	}
}
