using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ModernWpf.Automation.Peers;

namespace ModernWpf.Controls;

public class PersonPicture : Control
{
	private static readonly ResourceAccessor ResourceAccessor;

	private TextBlock m_initialsTextBlock;

	private TextBlock m_badgeNumberTextBlock;

	private FontIcon m_badgeGlyphIcon;

	private ImageBrush m_badgeImageBrush;

	private Ellipse m_badgingEllipse;

	private Ellipse m_badgingBackgroundEllipse;

	private string m_displayNameInitials;

	private string m_contactDisplayNameInitials;

	private ImageSource m_contactImageSource;

	public static readonly DependencyProperty BadgeGlyphProperty;

	public static readonly DependencyProperty BadgeImageSourceProperty;

	public static readonly DependencyProperty BadgeNumberProperty;

	public static readonly DependencyProperty BadgeTextProperty;

	public static readonly DependencyProperty DisplayNameProperty;

	public static readonly DependencyProperty InitialsProperty;

	public static readonly DependencyProperty IsGroupProperty;

	public static readonly DependencyProperty ProfilePictureProperty;

	private static readonly DependencyPropertyKey TemplateSettingsPropertyKey;

	public static readonly DependencyProperty TemplateSettingsProperty;

	public string BadgeGlyph
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(BadgeGlyphProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BadgeGlyphProperty, (object)value);
		}
	}

	public ImageSource BadgeImageSource
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(BadgeImageSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BadgeImageSourceProperty, (object)value);
		}
	}

	public int BadgeNumber
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(BadgeNumberProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BadgeNumberProperty, (object)value);
		}
	}

	public string BadgeText
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(BadgeTextProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(BadgeTextProperty, (object)value);
		}
	}

	public string DisplayName
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(DisplayNameProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DisplayNameProperty, (object)value);
		}
	}

	public string Initials
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(InitialsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InitialsProperty, (object)value);
		}
	}

	public bool IsGroup
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsGroupProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsGroupProperty, (object)value);
		}
	}

	public ImageSource ProfilePicture
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(ProfilePictureProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ProfilePictureProperty, (object)value);
		}
	}

	public PersonPictureTemplateSettings TemplateSettings
	{
		get
		{
			return (PersonPictureTemplateSettings)((DependencyObject)this).GetValue(TemplateSettingsProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(TemplateSettingsPropertyKey, (object)value);
		}
	}

	static PersonPicture()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_004f: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0103: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_0148: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_018d: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Expected O, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Expected O, but got Unknown
		ResourceAccessor = new ResourceAccessor(typeof(PersonPicture));
		BadgeGlyphProperty = DependencyProperty.Register("BadgeGlyph", typeof(string), typeof(PersonPicture), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnBadgeGlyphPropertyChanged), new CoerceValueCallback(CoerceStringProperty)));
		BadgeImageSourceProperty = DependencyProperty.Register("BadgeImageSource", typeof(ImageSource), typeof(PersonPicture), new PropertyMetadata((object)null, new PropertyChangedCallback(OnBadgeImageSourcePropertyChanged)));
		BadgeNumberProperty = DependencyProperty.Register("BadgeNumber", typeof(int), typeof(PersonPicture), new PropertyMetadata((object)0, new PropertyChangedCallback(OnBadgeNumberPropertyChanged)));
		BadgeTextProperty = DependencyProperty.Register("BadgeText", typeof(string), typeof(PersonPicture), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnBadgeTextPropertyChanged), new CoerceValueCallback(CoerceStringProperty)));
		DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(PersonPicture), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnDisplayNamePropertyChanged), new CoerceValueCallback(CoerceStringProperty)));
		InitialsProperty = DependencyProperty.Register("Initials", typeof(string), typeof(PersonPicture), new PropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnInitialsPropertyChanged), new CoerceValueCallback(CoerceStringProperty)));
		IsGroupProperty = DependencyProperty.Register("IsGroup", typeof(bool), typeof(PersonPicture), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsGroupPropertyChanged)));
		ProfilePictureProperty = DependencyProperty.Register("ProfilePicture", typeof(ImageSource), typeof(PersonPicture), new PropertyMetadata((object)null, new PropertyChangedCallback(OnProfilePicturePropertyChanged)));
		TemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("TemplateSettings", typeof(PersonPictureTemplateSettings), typeof(PersonPicture), new PropertyMetadata((object)null, new PropertyChangedCallback(OnTemplateSettingsPropertyChanged)));
		TemplateSettingsProperty = TemplateSettingsPropertyKey.DependencyProperty;
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PersonPicture), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(PersonPicture)));
	}

	public PersonPicture()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		TemplateSettings = new PersonPictureTemplateSettings();
		((FrameworkElement)this).Unloaded += new RoutedEventHandler(OnUnloaded);
		((FrameworkElement)this).SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new PersonPictureAutomationPeer(this);
	}

	public override void OnApplyTemplate()
	{
		((FrameworkElement)this).OnApplyTemplate();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("InitialsTextBlock");
		m_initialsTextBlock = (TextBlock)(object)((templateChild is TextBlock) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("BadgeNumberTextBlock");
		m_badgeNumberTextBlock = (TextBlock)(object)((templateChild2 is TextBlock) ? templateChild2 : null);
		m_badgeGlyphIcon = ((FrameworkElement)this).GetTemplateChild("BadgeGlyphIcon") as FontIcon;
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("BadgingEllipse");
		m_badgingEllipse = (Ellipse)(object)((templateChild3 is Ellipse) ? templateChild3 : null);
		DependencyObject templateChild4 = ((FrameworkElement)this).GetTemplateChild("BadgingBackgroundEllipse");
		m_badgingBackgroundEllipse = (Ellipse)(object)((templateChild4 is Ellipse) ? templateChild4 : null);
		UpdateBadge();
		UpdateIfReady();
	}

	private string GetInitials()
	{
		if (!string.IsNullOrEmpty(Initials))
		{
			return Initials;
		}
		if (!string.IsNullOrEmpty(m_displayNameInitials))
		{
			return m_displayNameInitials;
		}
		return m_contactDisplayNameInitials;
	}

	private ImageSource GetImageSource()
	{
		if (ProfilePicture != null)
		{
			return ProfilePicture;
		}
		return m_contactImageSource;
	}

	private void UpdateIfReady()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		string initials = GetInitials();
		ImageSource imageSource = GetImageSource();
		PersonPictureTemplateSettings templateSettings = TemplateSettings;
		templateSettings.ActualInitials = initials;
		if (imageSource != null)
		{
			ImageBrush val = templateSettings.ActualImageBrush;
			if (val == null)
			{
				val = new ImageBrush();
				((TileBrush)val).Stretch = (Stretch)3;
				templateSettings.ActualImageBrush = val;
			}
			val.ImageSource = imageSource;
		}
		else
		{
			templateSettings.ActualImageBrush = null;
		}
		if (IsGroup)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Group", false);
		}
		else if (imageSource != null)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Photo", false);
		}
		else if (!string.IsNullOrEmpty(initials))
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Initials", false);
		}
		else
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "NoPhotoOrInitials", false);
		}
		UpdateAutomationName();
	}

	private void UpdateBadge()
	{
		if (BadgeImageSource != null)
		{
			UpdateBadgeImageSource();
		}
		else if (BadgeNumber != 0)
		{
			UpdateBadgeNumber();
		}
		else if (!string.IsNullOrEmpty(BadgeGlyph))
		{
			UpdateBadgeGlyph();
		}
		else
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "NoBadge", false);
			TextBlock badgeNumberTextBlock = m_badgeNumberTextBlock;
			if (badgeNumberTextBlock != null)
			{
				badgeNumberTextBlock.Text = "";
			}
			FontIcon badgeGlyphIcon = m_badgeGlyphIcon;
			if (badgeGlyphIcon != null)
			{
				badgeGlyphIcon.Glyph = "";
			}
		}
		UpdateAutomationName();
	}

	private void UpdateBadgeNumber()
	{
		if (m_badgingEllipse == null || m_badgeNumberTextBlock == null)
		{
			return;
		}
		int badgeNumber = BadgeNumber;
		if (badgeNumber <= 0)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "NoBadge", false);
			m_badgeNumberTextBlock.Text = "";
			return;
		}
		VisualStateManager.GoToState((FrameworkElement)(object)this, "BadgeWithoutImageSource", false);
		if (badgeNumber <= 99)
		{
			m_badgeNumberTextBlock.Text = badgeNumber.ToString();
		}
		else
		{
			m_badgeNumberTextBlock.Text = "99+";
		}
	}

	private void UpdateBadgeGlyph()
	{
		if (m_badgingEllipse != null && m_badgeGlyphIcon != null)
		{
			string badgeGlyph = BadgeGlyph;
			if (string.IsNullOrEmpty(badgeGlyph))
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "NoBadge", false);
				m_badgeGlyphIcon.Glyph = "";
			}
			else
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "BadgeWithoutImageSource", false);
				m_badgeGlyphIcon.Glyph = badgeGlyph;
			}
		}
	}

	private void UpdateBadgeImageSource()
	{
		if (m_badgeImageBrush == null)
		{
			DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("BadgeImageBrush");
			m_badgeImageBrush = (ImageBrush)(object)((templateChild is ImageBrush) ? templateChild : null);
		}
		if (m_badgingEllipse != null && m_badgeImageBrush != null)
		{
			m_badgeImageBrush.ImageSource = BadgeImageSource;
			if (BadgeImageSource != null)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "BadgeWithImageSource", false);
			}
			else
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "NoBadge", false);
			}
		}
	}

	private void UpdateAutomationName()
	{
		string text = (IsGroup ? ResourceAccessor.GetLocalizedStringResource("GroupName") : ((!string.IsNullOrEmpty(DisplayName)) ? DisplayName : (string.IsNullOrEmpty(Initials) ? ResourceAccessor.GetLocalizedStringResource("PersonName") : Initials)));
		string text2 = ((BadgeNumber > 0) ? (string.IsNullOrEmpty(BadgeText) ? string.Format(GetLocalizedPluralBadgeItemStringResource(BadgeNumber), text, BadgeNumber) : string.Format(ResourceAccessor.GetLocalizedStringResource("BadgeItemTextOverride"), text, BadgeNumber, BadgeText)) : ((string.IsNullOrEmpty(BadgeGlyph) && BadgeImageSource == null) ? text : (string.IsNullOrEmpty(BadgeText) ? string.Format(ResourceAccessor.GetLocalizedStringResource("BadgeIcon"), text) : string.Format(ResourceAccessor.GetLocalizedStringResource("BadgeIconTextOverride"), text, BadgeText))));
		AutomationProperties.SetName((DependencyObject)(object)this, text2);
	}

	private string GetLocalizedPluralBadgeItemStringResource(int numericValue)
	{
		int num = numericValue % 10;
		switch (numericValue)
		{
		case 1:
			return ResourceAccessor.GetLocalizedStringResource("BadgeItemSingular");
		case 2:
			return ResourceAccessor.GetLocalizedStringResource("BadgeItemPlural7");
		case 3:
		case 4:
			return ResourceAccessor.GetLocalizedStringResource("BadgeItemPlural2");
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
			return ResourceAccessor.GetLocalizedStringResource("BadgeItemPlural5");
		default:
			if (numericValue >= 11 && numericValue <= 19)
			{
				return ResourceAccessor.GetLocalizedStringResource("BadgeItemPlural6");
			}
			switch (num)
			{
			case 1:
				return ResourceAccessor.GetLocalizedStringResource("BadgeItemPlural1");
			case 2:
			case 3:
			case 4:
				return ResourceAccessor.GetLocalizedStringResource("BadgeItemPlural3");
			default:
				return ResourceAccessor.GetLocalizedStringResource("BadgeItemPlural4");
			}
		}
	}

	private void PrivateOnPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		DependencyProperty property = ((DependencyPropertyChangedEventArgs)(ref args)).Property;
		if (property == BadgeNumberProperty || property == BadgeGlyphProperty || property == BadgeImageSourceProperty)
		{
			UpdateBadge();
		}
		else if (property == BadgeTextProperty)
		{
			UpdateAutomationName();
		}
		else if (property == DisplayNameProperty)
		{
			OnDisplayNameChanged(args);
		}
		else if (property == ProfilePictureProperty || property == InitialsProperty || property == IsGroupProperty)
		{
			UpdateIfReady();
		}
	}

	private void OnDisplayNameChanged(DependencyPropertyChangedEventArgs args)
	{
		m_displayNameInitials = InitialsGenerator.InitialsFromDisplayName(DisplayName);
		UpdateIfReady();
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		Size val = args.NewSize;
		double width = ((Size)(ref val)).Width;
		val = args.PreviousSize;
		bool flag = width != ((Size)(ref val)).Width;
		val = args.NewSize;
		double height = ((Size)(ref val)).Height;
		val = args.PreviousSize;
		bool flag2 = height != ((Size)(ref val)).Height;
		double num2;
		if (flag && flag2)
		{
			val = args.NewSize;
			double width2 = ((Size)(ref val)).Width;
			val = args.NewSize;
			double num;
			if (!(width2 < ((Size)(ref val)).Height))
			{
				val = args.NewSize;
				num = ((Size)(ref val)).Height;
			}
			else
			{
				val = args.NewSize;
				num = ((Size)(ref val)).Width;
			}
			num2 = num;
		}
		else if (flag)
		{
			val = args.NewSize;
			num2 = ((Size)(ref val)).Width;
		}
		else
		{
			if (!flag2)
			{
				return;
			}
			val = args.NewSize;
			num2 = ((Size)(ref val)).Height;
		}
		((FrameworkElement)this).Height = num2;
		((FrameworkElement)this).Width = num2;
		double fontSize = Math.Max(1.0, ((FrameworkElement)this).Width * 0.42);
		TextBlock initialsTextBlock = m_initialsTextBlock;
		if (initialsTextBlock != null)
		{
			initialsTextBlock.FontSize = fontSize;
		}
		if (m_badgingEllipse != null && m_badgingBackgroundEllipse != null && m_badgeNumberTextBlock != null && m_badgeGlyphIcon != null)
		{
			val = args.NewSize;
			double width3 = ((Size)(ref val)).Width;
			val = args.NewSize;
			double num3;
			if (!(width3 < ((Size)(ref val)).Height))
			{
				val = args.NewSize;
				num3 = ((Size)(ref val)).Height;
			}
			else
			{
				val = args.NewSize;
				num3 = ((Size)(ref val)).Width;
			}
			double num4 = num3;
			((FrameworkElement)m_badgingEllipse).Height = num4 * 0.5;
			((FrameworkElement)m_badgingEllipse).Width = num4 * 0.5;
			((FrameworkElement)m_badgingBackgroundEllipse).Height = num4 * 0.5;
			((FrameworkElement)m_badgingBackgroundEllipse).Width = num4 * 0.5;
			m_badgeNumberTextBlock.FontSize = Math.Max(1.0, ((FrameworkElement)m_badgingEllipse).Height * 0.6);
			m_badgeGlyphIcon.FontSize = Math.Max(1.0, ((FrameworkElement)m_badgingEllipse).Height * 0.6);
		}
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
	}

	private static void OnBadgeGlyphPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnBadgeImageSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnBadgeNumberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnBadgeTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnDisplayNamePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnInitialsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnIsGroupPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnProfilePicturePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static void OnTemplateSettingsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((PersonPicture)(object)sender).PrivateOnPropertyChanged(args);
	}

	private static object CoerceStringProperty(DependencyObject d, object baseValue)
	{
		return baseValue ?? string.Empty;
	}
}
