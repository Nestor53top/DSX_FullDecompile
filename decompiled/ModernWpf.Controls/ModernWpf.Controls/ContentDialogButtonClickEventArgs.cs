using System;

namespace ModernWpf.Controls;

public class ContentDialogButtonClickEventArgs : EventArgs
{
	private ContentDialogButtonClickDeferral _deferral;

	private int _deferralCount;

	public bool Cancel { get; set; }

	internal ContentDialogButtonClickEventArgs()
	{
	}

	public ContentDialogButtonClickDeferral GetDeferral()
	{
		_deferralCount++;
		return new ContentDialogButtonClickDeferral(delegate
		{
			DecrementDeferralCount();
		});
	}

	internal void SetDeferral(ContentDialogButtonClickDeferral deferral)
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
