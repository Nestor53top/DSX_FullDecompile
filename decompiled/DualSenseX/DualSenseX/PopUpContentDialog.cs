using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ModernWpf.Controls;

namespace DualSenseX;

public class PopUpContentDialog : ContentDialog, IComponentConnector
{
	internal ComboBox ModeComboBox;

	internal Slider Force1Slider;

	internal Slider Force2Slider;

	internal Slider Force3Slider;

	internal Slider Force4Slider;

	internal Slider Force5Slider;

	internal Slider Force6Slider;

	internal Slider Force7Slider;

	private bool _contentLoaded;

	public PopUpContentDialog()
	{
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/DualSenseX;component/popupcontentdialog.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		switch (connectionId)
		{
		case 1:
			ModeComboBox = (ComboBox)target;
			break;
		case 2:
			Force1Slider = (Slider)target;
			break;
		case 3:
			Force2Slider = (Slider)target;
			break;
		case 4:
			Force3Slider = (Slider)target;
			break;
		case 5:
			Force4Slider = (Slider)target;
			break;
		case 6:
			Force5Slider = (Slider)target;
			break;
		case 7:
			Force6Slider = (Slider)target;
			break;
		case 8:
			Force7Slider = (Slider)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
