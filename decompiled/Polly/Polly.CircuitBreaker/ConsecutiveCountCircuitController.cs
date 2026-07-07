using System;
using Polly.Utilities;

namespace Polly.CircuitBreaker;

internal class ConsecutiveCountCircuitController<TResult> : CircuitStateController<TResult>
{
	private readonly int _exceptionsAllowedBeforeBreaking;

	private int _consecutiveFailureCount;

	public ConsecutiveCountCircuitController(int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak, Action<DelegateResult<TResult>, CircuitState, TimeSpan, Context> onBreak, Action<Context> onReset, Action onHalfOpen)
		: base(durationOfBreak, onBreak, onReset, onHalfOpen)
	{
		_exceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
	}

	public override void OnCircuitReset(Context context)
	{
		using (TimedLock.Lock(_lock))
		{
			_consecutiveFailureCount = 0;
			ResetInternal_NeedsLock(context);
		}
	}

	public override void OnActionSuccess(Context context)
	{
		using (TimedLock.Lock(_lock))
		{
			switch (_circuitState)
			{
			case CircuitState.HalfOpen:
				OnCircuitReset(context);
				break;
			case CircuitState.Closed:
				_consecutiveFailureCount = 0;
				break;
			default:
				throw new InvalidOperationException("Unhandled CircuitState.");
			case CircuitState.Open:
			case CircuitState.Isolated:
				break;
			}
		}
	}

	public override void OnActionFailure(DelegateResult<TResult> outcome, Context context)
	{
		using (TimedLock.Lock(_lock))
		{
			_lastOutcome = outcome;
			switch (_circuitState)
			{
			case CircuitState.HalfOpen:
				Break_NeedsLock(context);
				break;
			case CircuitState.Closed:
				_consecutiveFailureCount++;
				if (_consecutiveFailureCount >= _exceptionsAllowedBeforeBreaking)
				{
					Break_NeedsLock(context);
				}
				break;
			default:
				throw new InvalidOperationException("Unhandled CircuitState.");
			case CircuitState.Open:
			case CircuitState.Isolated:
				break;
			}
		}
	}
}
