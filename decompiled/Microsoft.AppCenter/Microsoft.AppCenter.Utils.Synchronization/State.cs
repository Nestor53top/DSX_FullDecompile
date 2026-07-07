namespace Microsoft.AppCenter.Utils.Synchronization;

public class State
{
	private readonly int _stateNum;

	public State()
		: this(0)
	{
	}

	private State(int stateNum)
	{
		_stateNum = stateNum;
	}

	public State GetNextState()
	{
		return new State(_stateNum + 1);
	}

	public override bool Equals(object obj)
	{
		return (obj as State)?._stateNum == _stateNum;
	}

	public override int GetHashCode()
	{
		int stateNum = _stateNum;
		return stateNum.GetHashCode();
	}
}
