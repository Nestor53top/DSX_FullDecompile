using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using WpfAnimatedGif;

namespace Squirrel.Update;

public class AnimatedGifWindow : Window
{
	public AnimatedGifWindow()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		Image val = new Image();
		BitmapImage val2 = null;
		string? directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		string path = Path.Combine(directoryName, "background.gif");
		if (File.Exists(path))
		{
			val2 = new BitmapImage();
			val2.BeginInit();
			val2.StreamSource = File.OpenRead(path);
			val2.EndInit();
			ImageBehavior.SetAnimatedSource(val, (ImageSource)(object)val2);
			((ContentControl)this).Content = val;
			((FrameworkElement)this).Width = ((ImageSource)val2).Width;
			((FrameworkElement)this).Height = ((ImageSource)val2).Height;
		}
		string text = Path.Combine(directoryName, "setupIcon.ico");
		if (File.Exists(text))
		{
			((Window)this).Icon = (ImageSource)(object)BitmapFrame.Create(new Uri(text, UriKind.Relative));
		}
		((Window)this).AllowsTransparency = true;
		((Window)this).WindowStyle = (WindowStyle)0;
		((Window)this).WindowStartupLocation = (WindowStartupLocation)1;
		((Window)this).ShowInTaskbar = true;
		((Window)this).Topmost = true;
		((Window)this).TaskbarItemInfo = new TaskbarItemInfo
		{
			ProgressState = (TaskbarItemProgressState)2
		};
		((Window)this).Title = "Installing...";
		((Control)this).Background = (Brush)new SolidColorBrush(Color.FromArgb((byte)0, (byte)0, (byte)0, (byte)0));
	}

	public static void ShowWindow(TimeSpan initialDelay, CancellationToken token, ProgressSource progressSource)
	{
		AnimatedGifWindow wnd = null;
		Thread thread = new Thread((ThreadStart)delegate
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			if (token.IsCancellationRequested)
			{
				return;
			}
			try
			{
				Task.Delay(initialDelay, token).ContinueWith((Task t) => true).Wait();
			}
			catch (Exception)
			{
				return;
			}
			wnd = new AnimatedGifWindow();
			((Window)wnd).Show();
			Task.Delay(TimeSpan.FromSeconds(5.0), token).ContinueWith(delegate(Task t)
			{
				if (!t.IsCanceled)
				{
					((DispatcherObject)wnd).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						((Window)wnd).Topmost = false;
					}, new object[0]);
				}
			});
			token.Register(delegate
			{
				((DispatcherObject)wnd).Dispatcher.BeginInvoke((Delegate)new Action(((Window)wnd).Close), new object[0]);
			});
			EventHandler<int> value = delegate(object sender, int p)
			{
				((DispatcherObject)wnd).Dispatcher.BeginInvoke((Delegate)(Action)delegate
				{
					((Window)wnd).TaskbarItemInfo.ProgressValue = (double)p / 100.0;
				}, new object[0]);
			};
			progressSource.Progress += value;
			try
			{
				new Application().Run((Window)(object)wnd);
			}
			finally
			{
				progressSource.Progress -= value;
			}
		});
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
	}

	protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
	{
		((UIElement)this).OnMouseLeftButtonDown(e);
		((Window)this).DragMove();
	}
}
