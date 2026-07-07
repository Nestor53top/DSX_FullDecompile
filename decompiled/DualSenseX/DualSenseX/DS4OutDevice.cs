using System.Collections.Generic;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace DualSenseX;

internal abstract class DS4OutDevice : OutputDevice
{
	public const string devtype = "DS4";

	public IDualShock4Controller cont;

	public Dictionary<int, DualShock4FeedbackReceivedEventHandler> forceFeedbacksDict = new Dictionary<int, DualShock4FeedbackReceivedEventHandler>();

	public DS4OutDevice(ViGEmClient client)
	{
		cont = client.CreateDualShock4Controller();
		cont.AutoSubmitReport = false;
	}

	public override void Connect()
	{
		cont.Connect();
		connected = true;
	}

	public override void Disconnect()
	{
		foreach (KeyValuePair<int, DualShock4FeedbackReceivedEventHandler> item in forceFeedbacksDict)
		{
			cont.FeedbackReceived -= item.Value;
		}
		forceFeedbacksDict.Clear();
		connected = false;
		cont.Disconnect();
		cont = null;
	}

	public override string GetDeviceType()
	{
		return "DS4";
	}

	public override void RemoveFeedbacks()
	{
		foreach (KeyValuePair<int, DualShock4FeedbackReceivedEventHandler> item in forceFeedbacksDict)
		{
			cont.FeedbackReceived -= item.Value;
		}
		forceFeedbacksDict.Clear();
	}

	public override void RemoveFeedback(int inIdx)
	{
		if (forceFeedbacksDict.TryGetValue(inIdx, out var value))
		{
			cont.FeedbackReceived -= value;
			forceFeedbacksDict.Remove(inIdx);
		}
	}
}
