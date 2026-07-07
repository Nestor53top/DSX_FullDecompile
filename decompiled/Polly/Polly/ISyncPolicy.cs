using System;
using System.Collections.Generic;
using System.Threading;

namespace Polly;

public interface ISyncPolicy : IsPolicy
{
	ISyncPolicy WithPolicyKey(string policyKey);

	void Execute(Action action);

	void Execute(Action<Context> action, IDictionary<string, object> contextData);

	void Execute(Action<Context> action, Context context);

	void Execute(Action<CancellationToken> action, CancellationToken cancellationToken);

	void Execute(Action<Context, CancellationToken> action, IDictionary<string, object> contextData, CancellationToken cancellationToken);

	void Execute(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken);

	TResult Execute<TResult>(Func<TResult> action);

	TResult Execute<TResult>(Func<Context, TResult> action, IDictionary<string, object> contextData);

	TResult Execute<TResult>(Func<Context, TResult> action, Context context);

	TResult Execute<TResult>(Func<CancellationToken, TResult> action, CancellationToken cancellationToken);

	TResult Execute<TResult>(Func<Context, CancellationToken, TResult> action, IDictionary<string, object> contextData, CancellationToken cancellationToken);

	TResult Execute<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken);

	PolicyResult ExecuteAndCapture(Action action);

	PolicyResult ExecuteAndCapture(Action<Context> action, IDictionary<string, object> contextData);

	PolicyResult ExecuteAndCapture(Action<Context> action, Context context);

	PolicyResult ExecuteAndCapture(Action<CancellationToken> action, CancellationToken cancellationToken);

	PolicyResult ExecuteAndCapture(Action<Context, CancellationToken> action, IDictionary<string, object> contextData, CancellationToken cancellationToken);

	PolicyResult ExecuteAndCapture(Action<Context, CancellationToken> action, Context context, CancellationToken cancellationToken);

	PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<TResult> action);

	PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<Context, TResult> action, IDictionary<string, object> contextData);

	PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<Context, TResult> action, Context context);

	PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<CancellationToken, TResult> action, CancellationToken cancellationToken);

	PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<Context, CancellationToken, TResult> action, IDictionary<string, object> contextData, CancellationToken cancellationToken);

	PolicyResult<TResult> ExecuteAndCapture<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken);
}
public interface ISyncPolicy<TResult> : IsPolicy
{
	ISyncPolicy<TResult> WithPolicyKey(string policyKey);

	TResult Execute(Func<TResult> action);

	TResult Execute(Func<Context, TResult> action, IDictionary<string, object> contextData);

	TResult Execute(Func<Context, TResult> action, Context context);

	TResult Execute(Func<CancellationToken, TResult> action, CancellationToken cancellationToken);

	TResult Execute(Func<Context, CancellationToken, TResult> action, IDictionary<string, object> contextData, CancellationToken cancellationToken);

	TResult Execute(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken);

	PolicyResult<TResult> ExecuteAndCapture(Func<TResult> action);

	PolicyResult<TResult> ExecuteAndCapture(Func<Context, TResult> action, IDictionary<string, object> contextData);

	PolicyResult<TResult> ExecuteAndCapture(Func<Context, TResult> action, Context context);

	PolicyResult<TResult> ExecuteAndCapture(Func<CancellationToken, TResult> action, CancellationToken cancellationToken);

	PolicyResult<TResult> ExecuteAndCapture(Func<Context, CancellationToken, TResult> action, IDictionary<string, object> contextData, CancellationToken cancellationToken);

	PolicyResult<TResult> ExecuteAndCapture(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken);
}
