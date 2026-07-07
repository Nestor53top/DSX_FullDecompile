using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Navigation;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class HyperlinkButton : ButtonBase
{
	public static readonly DependencyProperty NavigateUriProperty;

	public static readonly DependencyProperty TargetNameProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty FocusVisualMarginProperty;

	private readonly Hyperlink m_hyperlink;

	public Uri NavigateUri
	{
		get
		{
			return (Uri)((DependencyObject)this).GetValue(NavigateUriProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(NavigateUriProperty, (object)value);
		}
	}

	public string TargetName
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(TargetNameProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TargetNameProperty, (object)value);
		}
	}

	public bool UseSystemFocusVisuals
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(UseSystemFocusVisualsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UseSystemFocusVisualsProperty, (object)value);
		}
	}

	public Thickness FocusVisualMargin
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Thickness)((DependencyObject)this).GetValue(FocusVisualMarginProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FocusVisualMarginProperty, (object)value);
		}
	}

	static HyperlinkButton()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		NavigateUriProperty = Hyperlink.NavigateUriProperty.AddOwner(typeof(HyperlinkButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnNavigateUriChanged)));
		TargetNameProperty = Hyperlink.TargetNameProperty.AddOwner(typeof(HyperlinkButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTargetNameChanged)));
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(HyperlinkButton));
		FocusVisualMarginProperty = FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(HyperlinkButton));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(HyperlinkButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(HyperlinkButton)));
		Control.HorizontalContentAlignmentProperty.OverrideMetadata(typeof(HyperlinkButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)(HorizontalAlignment)1));
		Control.VerticalContentAlignmentProperty.OverrideMetadata(typeof(HyperlinkButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)(VerticalAlignment)1));
	}

	public HyperlinkButton()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		m_hyperlink = new Hyperlink
		{
			NavigateUri = NavigateUri,
			TargetName = TargetName
		};
		m_hyperlink.RequestNavigate += new RequestNavigateEventHandler(OnRequestNavigate);
		((FrameworkElement)this).AddLogicalChild((object)m_hyperlink);
	}

	private static void OnNavigateUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((HyperlinkButton)(object)d).m_hyperlink.NavigateUri = (Uri)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
	}

	private static void OnTargetNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((HyperlinkButton)(object)d).m_hyperlink.TargetName = (string)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new HyperlinkButtonAutomationPeer(this);
	}

	protected override void OnClick()
	{
		if (AutomationPeer.ListenerExists((AutomationEvents)5))
		{
			AutomationPeer val = UIElementAutomationPeer.CreatePeerForElement((UIElement)(object)this);
			if (val != null)
			{
				val.RaiseAutomationEvent((AutomationEvents)5);
			}
		}
		m_hyperlink.DoClick();
		((ButtonBase)this).OnClick();
	}

	internal void AutomationButtonBaseClick()
	{
		((ButtonBase)this).OnClick();
	}

	private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Uri uri = e.Uri;
		if (uri.IsAbsoluteUri && uri.Scheme.IndexOf("http", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			Process.Start(new ProcessStartInfo(uri.ToString())
			{
				UseShellExecute = true
			});
			((RoutedEventArgs)e).Handled = true;
		}
	}
}
