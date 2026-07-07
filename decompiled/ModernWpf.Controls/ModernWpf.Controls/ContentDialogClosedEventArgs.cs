using System;

namespace ModernWpf.Controls;

public class ContentDialogClosedEventArgs : EventArgs
{
	public ContentDialogResult Result { get; }

	internal ContentDialogClosedEventArgs(ContentDialogResult result)
	{
		Result = result;
	}
}
