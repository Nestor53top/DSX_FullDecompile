using System;
using System.Net.NetworkInformation;

namespace Microsoft.AppCenter.Ingestion.Http;

public class NetworkStateAdapter : INetworkStateAdapter
{
	public bool IsConnected
	{
		get
		{
			try
			{
				return NetworkInterface.GetIsNetworkAvailable();
			}
			catch (Exception exception)
			{
				AppCenterLog.Error(AppCenterLog.LogTag, "An error occurred while checking network state.", exception);
				return false;
			}
		}
	}

	public event EventHandler NetworkStatusChanged;

	public NetworkStateAdapter()
	{
		NetworkChange.NetworkAddressChanged += delegate(object sender, EventArgs args)
		{
			this.NetworkStatusChanged?.Invoke(sender, args);
		};
	}
}
