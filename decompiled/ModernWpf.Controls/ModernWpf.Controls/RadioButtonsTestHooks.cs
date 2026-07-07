namespace ModernWpf.Controls;

internal class RadioButtonsTestHooks
{
	public static TypedEventHandler<RadioButtons, object> LayoutChanged;

	private static RadioButtonsTestHooks s_testHooks;

	public static RadioButtonsTestHooks EnsureGlobalTestHooks()
	{
		if (s_testHooks == null)
		{
			s_testHooks = new RadioButtonsTestHooks();
		}
		return s_testHooks;
	}

	public static void SetTestHooksEnabled(RadioButtons radioButtons, bool enabled)
	{
		radioButtons?.SetTestHooksEnabled(enabled);
	}

	public static void NotifyLayoutChanged(RadioButtons sender)
	{
		EnsureGlobalTestHooks();
		LayoutChanged?.Invoke(sender, null);
	}

	public static int GetRows(RadioButtons radioButtons)
	{
		return radioButtons?.GetRows() ?? (-1);
	}

	public static int GetColumns(RadioButtons radioButtons)
	{
		return radioButtons?.GetColumns() ?? (-1);
	}

	public static int GetLargerColumns(RadioButtons radioButtons)
	{
		return radioButtons?.GetLargerColumns() ?? (-1);
	}
}
