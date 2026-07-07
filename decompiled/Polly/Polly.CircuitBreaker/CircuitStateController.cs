using System;
using System.Threading;
using Polly.Utilities;

namespace Polly.CircuitBreaker;

internal abstract class CircuitStateController<TResult> : ICircuitController<TResult>
{
	protected readonly TimeSpan _durationOfBreak;

	protected long _blockedTill;

	protected CircuitState _circuitState;

	protected DelegateResult<TResult> _lastOutcome;

	protected readonly Action<DelegateResult<TResult>, CircuitState, TimeSpan, Context> _onBreak;

	protected readonly Action<Context> _onReset;

	protected readonly Action _onHalfOpen;

	protected readonly object _lock = new object();

	public CircuitState CircuitState
	{
		get
		{
			if (_circuitState != CircuitState.Open)
			{
				return _circuitState;
			}
			using (TimedLock.Lock(_lock))
			{
				if (_circuitState == CircuitState.Open && !IsInAutomatedBreak_NeedsLock)
				{
					_circuitState = CircuitState.HalfOpen;
					_onHalfOpen();
				}
				return _circuitState;
			}
		}
	}

	public Exception LastException
	{
		get
		{
			using (TimedLock.Lock(_lock))
			{
				return _lastOutcome?.Exception;
			}
		}
	}

	public TResult LastHandledResult
	{
		get
		{
			using (TimedLock.Lock(_lock))
			{
				return (_lastOutcome != null) ? _lastOutcome.Result : default(TResult);
			}
		}
	}

	protected bool IsInAutomatedBreak_NeedsLock => SystemClock.UtcNow().Ticks < _blockedTill;

	protected CircuitStateController(TimeSpan durationOfBreak, Action<DelegateResult<TResult>, CircuitState, TimeSpan, Context> onBreak, Action<Context> onReset, Action onHalfOpen)
	{
		_durationOfBreak = durationOfBreak;
		_onBreak = onBreak;
		_onReset = onReset;
		_onHalfOpen = onHalfOpen;
		_circuitState = CircuitState.Closed;
		Reset();
	}

	public void Isolate()
	{
		using (TimedLock.Lock(_lock))
		{
			_lastOutcome = new DelegateResult<TResult>(new IsolatedCircuitException("The circuit is manually held open and is not allowing calls."));
			BreakFor_NeedsLock(TimeSpan.MaxValue, Context.None());
			_circuitState = CircuitState.Isolated;
		}
	}

	protected void Break_NeedsLock(Context context)
	{
		BreakFor_NeedsLock(_durationOfBreak, context);
	}

	private void BreakFor_NeedsLock(TimeSpan durationOfBreak, Context context)
	{
		long ticks;
		if (!(durationOfBreak > DateTime.MaxValue - SystemClock.UtcNow()))
		{
			ticks = (SystemClock.UtcNow() + durationOfBreak).Ticks;
		}
		else
		{
			DateTime maxValue = DateTime.MaxValue;
			ticks = maxValue.Ticks;
		}
		_blockedTill = ticks;
		CircuitState circuitState = _circuitState;
		_circuitState = CircuitState.Open;
		_onBreak(_lastOutcome, circuitState, durationOfBreak, context);
	}

	public void Reset()
	{
		OnCircuitReset(Context.None());
	}

	protected void ResetInternal_NeedsLock(Context context)
	{
		DateTime minValue = DateTime.MinValue;
		_blockedTill = minValue.Ticks;
		_lastOutcome = null;
		CircuitState circuitState = _circuitState;
		_circuitState = CircuitState.Closed;
		if (circuitState != CircuitState.Closed)
		{
			_onReset(context);
		}
	}

	protected bool PermitHalfOpenCircuitTest()
	{
		long blockedTill = _blockedTill;
		if (SystemClock.UtcNow().Ticks >= blockedTill)
		{
			ref long blockedTill2 = ref _blockedTill;
			long ticks = SystemClock.UtcNow().Ticks;
			TimeSpan durationOfBreak = _durationOfBreak;
			return Interlocked.CompareExchange(ref blockedTill2, ticks + durationOfBreak.Ticks, blockedTill) == blockedTill;
		}
		return false;
	}

	private BrokenCircuitException GetBreakingException()
	{
		DelegateResult<TResult> lastOutcome = _lastOutcome;
		if (lastOutcome == null)
		{
			return new BrokenCircuitException("The circuit is now open and is not allowing calls.");
		}
		if (lastOutcome.Exception != null)
		{
			return new BrokenCircuitException("The circuit is now open and is not allowing calls.", lastOutcome.Exception);
		}
		return new BrokenCircuitException<TResult>("The circuit is now open and is not allowing calls.", lastOutcome.Result);
	}

	public void OnActionPreExecute()
	{
		switch (CircuitState)
		{
		case CircuitState.HalfOpen:
			if (!PermitHalfOpenCircuitTest())
			{
				throw GetBreakingException();
			}
			break;
		case CircuitState.Open:
			throw GetBreakingException();
		case CircuitState.Isolated:
			throw new IsolatedCircuitException("The circuit is manually held open and is not allowing calls.");
		default:
			throw new InvalidOperationException("Unhandled CircuitState.");
		case CircuitState.Closed:
			break;
		}
	}

	public abstract void OnActionSuccess(Context context);

	public abstract void OnActionFailure(DelegateResult<TResult> outcome, Context context);

	public abstract void OnCircuitReset(Context context);
}
