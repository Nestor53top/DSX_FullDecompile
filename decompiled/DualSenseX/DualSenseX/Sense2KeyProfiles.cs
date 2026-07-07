using System.Collections.Generic;

namespace DualSenseX;

public class Sense2KeyProfiles
{
	public List<Sense2Key_buttons> Sense2Key_buttons_List = new List<Sense2Key_buttons>();

	public string ProfileName { get; set; }

	public int DelayNumber { get; set; }

	public override string ToString()
	{
		return ProfileName;
	}
}
