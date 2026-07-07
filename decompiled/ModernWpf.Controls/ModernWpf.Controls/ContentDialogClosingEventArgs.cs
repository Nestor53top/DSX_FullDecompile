using System;

namespace ModernWpf.Controls;

public sealed class ContentDialogClosingEventArgs : EventArgs
{
	private ContentDialogClosingDeferral _deferral;

	private int _deferralCount;

	public bool Cancel { get; set; }

	public ContentDialogResult Result { get; }

	internal ContentDialogClosingEventArgs(ContentDialogResult result)
	{
		Result = result;
	}

	public ContentDialogClosingDeferral GetDeferral()
	{
		_deferralCount++;
		return new ContentDialogClosingDeferral(delegate
		{
			DecrementDeferralCount();
		});
	}

	internal void SetDeferral(ContentDialogClosingDeferral deferral)
	{
		_deferral = deferral;
	}

	internal void DecrementDeferralCount()
	{
		_deferralCount--;
		if (_deferralCount == 0)
		{
			_deferral.Complete();
		}
	}

	internal void IncrementDeferralCount()
	{
		_deferralCount++;
	}
}
