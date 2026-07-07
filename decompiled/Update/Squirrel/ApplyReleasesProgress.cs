using System;

namespace Squirrel;

internal class ApplyReleasesProgress : Progress<int>
{
	private readonly int _releasesToApply;

	private int _appliedReleases;

	private int _currentReleaseProgress;

	public ApplyReleasesProgress(int releasesToApply, Action<int> handler)
		: base(handler)
	{
		_releasesToApply = releasesToApply;
	}

	public void ReportReleaseProgress(int progressOfCurrentRelease)
	{
		_currentReleaseProgress = progressOfCurrentRelease;
		CalculateProgress();
	}

	public void FinishRelease()
	{
		_appliedReleases++;
		_currentReleaseProgress = 0;
		CalculateProgress();
	}

	private void CalculateProgress()
	{
		int num = 100 / _releasesToApply;
		int num2 = Math.Min(_appliedReleases, _releasesToApply) * num;
		double num3 = (double)num / 100.0 * (double)_currentReleaseProgress;
		double num4 = (double)num2 + num3;
		OnReport((int)num4);
	}
}
