using System.Collections.Generic;

namespace DualSenseX;

public class GamePageGamesList
{
	public List<string> GamesPage_Tags = new List<string>();

	public string GameName { get; set; }

	public string GameImageUrl { get; set; }

	public string GameGetLink { get; set; }

	public string GamesPage_TagsJoinedString { get; set; }

	public override string ToString()
	{
		return GameName;
	}
}
