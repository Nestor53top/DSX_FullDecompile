using System.Threading.Tasks;

namespace Polly.Utilities;

public static class TaskHelper
{
	public static Task EmptyTask = Task.FromResult(result: true);
}
