using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using ModernWpf.Controls;

namespace DualSenseX;

public class ContentDialogExample : ContentDialog, IComponentConnector
{
	internal TextBlock BodyText;

	internal Rectangle Rectangle1;

	internal Rectangle Rectangle2;

	internal Rectangle Rectangle3;

	internal TextBlock whatsnewTextblock;

	internal ScrollViewer ScrollViewer;

	internal TextBlock BodyText2;

	internal Label installNowTextblock;

	private bool _contentLoaded;

	public ContentDialogExample(string Title, string BodyText, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText)
	{
		InitializeComponent();
		base.Title = Title;
		this.BodyText.Text = BodyText;
		base.PrimaryButtonText = PrimaryButtonText;
		base.SecondaryButtonText = SecondaryButtonText;
		base.CloseButtonText = CloseButtonText;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri uri = new Uri("/DualSenseX;component/contentdialogexample.xaml", UriKind.Relative);
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
			BodyText = (TextBlock)target;
			break;
		case 2:
			Rectangle1 = (Rectangle)target;
			break;
		case 3:
			Rectangle2 = (Rectangle)target;
			break;
		case 4:
			Rectangle3 = (Rectangle)target;
			break;
		case 5:
			whatsnewTextblock = (TextBlock)target;
			break;
		case 6:
			ScrollViewer = (ScrollViewer)target;
			break;
		case 7:
			BodyText2 = (TextBlock)target;
			break;
		case 8:
			installNowTextblock = (Label)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
