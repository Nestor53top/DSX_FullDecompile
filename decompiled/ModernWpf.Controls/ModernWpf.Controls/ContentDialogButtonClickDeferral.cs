using System;

namespace ModernWpf.Controls;

public sealed class ContentDialogButtonClickDeferral
{
	private readonly Action _handler;

	internal ContentDialogButtonClickDeferral(Action handler)
	{
		_handler = handler ?? throw new ArgumentNullException("handler");
	}

	public void Complete()
	{
		_handler();
	}
}
