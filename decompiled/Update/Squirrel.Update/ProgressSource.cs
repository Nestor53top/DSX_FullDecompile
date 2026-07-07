using System;

namespace Squirrel.Update;

public class ProgressSource
{
	public event EventHandler<int> Progress;

	public void Raise(int i)
	{
		if (this.Progress != null)
		{
			this.Progress(this, i);
		}
	}
}
