using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using ModernWpf.Controls;

namespace DualSenseX;

public class ProgressBarDownloadDialog : ContentDialog, IComponentConnector
{
	internal ProgressBar ContinuousProgressBar;

	internal Label PleaseWaitLabel;

	internal TextBlock BodyText;

	internal Label ProgressValueLabel;

	internal ProgressBar ProgressBar2;

	private bool _contentLoaded;

	public ProgressBarDownloadDialog(string Title, string BodyText, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText, int Value)
	{
		InitializeComponent();
		((RangeBase)ProgressBar2).Value = Value;
		base.Title = Title;
		this.BodyText.Text = BodyText;
		base.PrimaryButtonText = PrimaryButtonText;
		base.SecondaryButtonText = SecondaryButtonText;
		base.CloseButtonText = CloseButtonText;
	}

	public void CloseDialog()
	{
		Hide();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/DualSenseX;component/progressbardownloaddialog.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			ContinuousProgressBar = (ProgressBar)target;
			break;
		case 2:
			PleaseWaitLabel = (Label)target;
			break;
		case 3:
			BodyText = (TextBlock)target;
			break;
		case 4:
			ProgressValueLabel = (Label)target;
			break;
		case 5:
			ProgressBar2 = (ProgressBar)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
