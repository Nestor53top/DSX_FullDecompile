using System;
using System.Windows;

namespace NuGet;

internal class SendingRequestEventManager : WeakEventManager
{
	private static readonly object _managerLock = new object();

	private static SendingRequestEventManager CurrentManager
	{
		get
		{
			Type typeFromHandle = typeof(SendingRequestEventManager);
			lock (_managerLock)
			{
				SendingRequestEventManager sendingRequestEventManager = (SendingRequestEventManager)(object)WeakEventManager.GetCurrentManager(typeFromHandle);
				if (sendingRequestEventManager == null)
				{
					sendingRequestEventManager = new SendingRequestEventManager();
					WeakEventManager.SetCurrentManager(typeFromHandle, (WeakEventManager)(object)sendingRequestEventManager);
				}
				return sendingRequestEventManager;
			}
		}
	}

	public static void AddListener(IHttpClientEvents source, IWeakEventListener listener)
	{
		((WeakEventManager)CurrentManager).ProtectedAddListener((object)source, listener);
	}

	public static void RemoveListener(IHttpClientEvents source, IWeakEventListener listener)
	{
		((WeakEventManager)CurrentManager).ProtectedRemoveListener((object)source, listener);
	}

	protected override void StartListening(object source)
	{
		((IHttpClientEvents)source).SendingRequest += OnSendingRequest;
	}

	protected override void StopListening(object source)
	{
		((IHttpClientEvents)source).SendingRequest -= OnSendingRequest;
	}

	private void OnSendingRequest(object sender, WebRequestEventArgs e)
	{
		((WeakEventManager)this).DeliverEvent(sender, (EventArgs)e);
	}
}
