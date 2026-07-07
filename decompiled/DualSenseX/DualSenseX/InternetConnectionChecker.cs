using System;
using System.Runtime.InteropServices;
using NETWORKLIST;

namespace DualSenseX;

public class InternetConnectionChecker
{
	private readonly INetworkListManager _networkListManager;

	public InternetConnectionChecker()
	{
		_networkListManager = (NetworkListManager)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("DCB00C01-570F-4A9B-8D69-199FDBA5723B")));
	}

	public bool IsConnected()
	{
		return _networkListManager.IsConnectedToInternet;
	}

	[DllImport("wininet.dll")]
	private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

	public bool IsConnectedToInternet()
	{
		int Description;
		return InternetGetConnectedState(out Description, 0);
	}
}
