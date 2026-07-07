using System.Windows;

namespace ModernWpf;

internal static class ResourceDictionaryHelper
{
	public static void SealValues(this ResourceDictionary dictionary)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		foreach (ResourceDictionary mergedDictionary in dictionary.MergedDictionaries)
		{
			mergedDictionary.SealValues();
		}
		foreach (object value in dictionary.Values)
		{
			Freezable val = (Freezable)((value is Freezable) ? value : null);
			if (val != null)
			{
				if (!val.CanFreeze)
				{
					LocalValueEnumerator localValueEnumerator = ((DependencyObject)val).GetLocalValueEnumerator();
					while (((LocalValueEnumerator)(ref localValueEnumerator)).MoveNext())
					{
						LocalValueEntry current2 = ((LocalValueEnumerator)(ref localValueEnumerator)).Current;
						DependencyProperty property = ((LocalValueEntry)(ref current2)).Property;
						ValueSource valueSource = DependencyPropertyHelper.GetValueSource((DependencyObject)(object)val, property);
						if (((ValueSource)(ref valueSource)).IsExpression)
						{
							((DependencyObject)val).SetValue(property, ((DependencyObject)val).GetValue(property));
						}
					}
				}
				if (!val.IsFrozen)
				{
					val.Freeze();
				}
			}
			else
			{
				Style val2 = (Style)((value is Style) ? value : null);
				if (val2 != null && !val2.IsSealed)
				{
					val2.Seal();
				}
			}
		}
		if (!(dictionary is ResourceDictionaryEx resourceDictionaryEx))
		{
			return;
		}
		foreach (ResourceDictionary value2 in resourceDictionaryEx.ThemeDictionaries.Values)
		{
			value2.SealValues();
		}
	}
}
