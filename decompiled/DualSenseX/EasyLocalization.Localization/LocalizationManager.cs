using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using EasyLocalization.Annotations;
using EasyLocalization.Readers;

namespace EasyLocalization.Localization;

public class LocalizationManager : INotifyPropertyChanged
{
	private static LocalizationManager _instance;

	private readonly Dictionary<CultureInfo, Dictionary<string, LocalizationEntry>> _languageEntries = new Dictionary<CultureInfo, Dictionary<string, LocalizationEntry>>();

	private CultureInfo _currentCulture;

	public static LocalizationManager Instance => _instance ?? (_instance = new LocalizationManager());

	public CultureInfo CurrentCulture
	{
		get
		{
			return _currentCulture;
		}
		set
		{
			CultureInfo currentCulture = _currentCulture;
			if ((currentCulture == null || !currentCulture.Equals(value)) && _languageEntries.ContainsKey(value))
			{
				_currentCulture = value;
				OnPropertyChanged("CurrentCulture");
			}
		}
	}

	public List<CultureInfo> AvailableCultures => _languageEntries.Keys.ToList();

	public event PropertyChangedEventHandler PropertyChanged;

	[NotifyPropertyChangedInvocator]
	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void AddCulture(CultureInfo culture, FileReader reader, bool choose = false)
	{
		if (_languageEntries.ContainsKey(culture))
		{
			_languageEntries[culture] = reader.GetEntries();
		}
		else
		{
			_languageEntries.Add(culture, reader.GetEntries());
		}
		if (choose)
		{
			CurrentCulture = culture;
		}
	}

	public string GetValue(string key, bool nullWhenUnfound = true)
	{
		if (_languageEntries == null || CurrentCulture == null)
		{
			return key;
		}
		Dictionary<string, LocalizationEntry> dictionary = _languageEntries[CurrentCulture];
		if (key == null || !dictionary.ContainsKey(key))
		{
			using (List<CultureInfo>.Enumerator enumerator = Instance.AvailableCultures.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					CultureInfo current = enumerator.Current;
					if (key != null && current.EnglishName == "English (United States)")
					{
						Dictionary<string, LocalizationEntry> dictionary2 = _languageEntries[current];
						if (dictionary2 != null && dictionary2[key].Value != null)
						{
							return dictionary2[key].Value;
						}
					}
					return nullWhenUnfound ? "No Translation Found" : key;
				}
			}
			if (!nullWhenUnfound)
			{
				return key;
			}
			return "No Translation Found";
		}
		return dictionary[key].Value;
	}

	public string GetValue(string key, int count, bool nullWhenUnfound = true)
	{
		if (_languageEntries == null || CurrentCulture == null)
		{
			return key;
		}
		Dictionary<string, LocalizationEntry> dictionary = _languageEntries[CurrentCulture];
		if (key == null || !dictionary.ContainsKey(key))
		{
			if (!nullWhenUnfound)
			{
				return key;
			}
			return null;
		}
		LocalizationEntry localizationEntry = dictionary[key];
		return count switch
		{
			1 => localizationEntry.Value, 
			0 => localizationEntry.ZeroValue, 
			_ => string.Format(localizationEntry.PluralValue, count), 
		};
	}

	public void Refresh()
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentCulture"));
	}
}
