using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace DualSenseX;

public class App : Application
{
	private bool _contentLoaded;

	private void Application_Startup(object sender, StartupEventArgs e)
	{
		AppCenter.SetCountryCode(RegionInfo.CurrentRegion.TwoLetterISORegionName);
		AppCenter.Start("6cecec35-5776-4ebc-a6da-580da439923d", typeof(Analytics), typeof(Crashes));
	}

	private void Application_Exit(object sender, ExitEventArgs e)
	{
		if (((IEnumerable)Application.Current.Windows).Cast<Window>().FirstOrDefault((Window window) => window is Main) is Main { App_TaskBarIcon: not null } main)
		{
			((UIElement)main.App_TaskBarIcon).Visibility = (Visibility)1;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			((Application)this).Startup += new StartupEventHandler(Application_Startup);
			((Application)this).Exit += new ExitEventHandler(Application_Exit);
			((Application)this).StartupUri = new Uri("Main.xaml", UriKind.Relative);
			Uri uri = new Uri("/DualSenseX;component/app.xaml", UriKind.Relative);
			Application.LoadComponent((object)this, uri);
		}
	}

	[STAThread]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public static void Main()
	{
		App app = new App();
		app.InitializeComponent();
		((Application)app).Run();
	}
}
