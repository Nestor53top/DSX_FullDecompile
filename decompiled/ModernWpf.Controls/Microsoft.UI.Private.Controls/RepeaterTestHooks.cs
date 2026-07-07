using ModernWpf;
using ModernWpf.Controls;

namespace Microsoft.UI.Private.Controls;

internal class RepeaterTestHooks
{
	private static RepeaterTestHooks s_testHooks;

	private event TypedEventHandler<object, object> m_buildTreeCompleted;

	public static event TypedEventHandler<object, object> BuildTreeCompleted
	{
		add
		{
			EnsureHooks();
			s_testHooks.m_buildTreeCompleted += value;
		}
		remove
		{
			if (s_testHooks != null)
			{
				s_testHooks.m_buildTreeCompleted -= value;
			}
		}
	}

	internal void NotifyBuildTreeCompletedImpl()
	{
		this.m_buildTreeCompleted?.Invoke(null, null);
	}

	public static int GetElementFactoryElementIndex(object getArgs)
	{
		return ((ElementFactoryGetArgs)getArgs).Index;
	}

	public static object CreateRepeaterElementFactoryGetArgs()
	{
		return new ElementFactoryGetArgs();
	}

	public static object CreateRepeaterElementFactoryRecycleArgs()
	{
		return new ElementFactoryRecycleArgs();
	}

	public static string GetLayoutId(object layout)
	{
		if (layout is Layout layout2)
		{
			return layout2.LayoutId;
		}
		return string.Empty;
	}

	public static void SetLayoutId(object layout, string id)
	{
		if (layout is Layout layout2)
		{
			layout2.LayoutId = id;
		}
	}

	private static void EnsureHooks()
	{
		if (s_testHooks == null)
		{
			s_testHooks = new RepeaterTestHooks();
		}
	}

	private static void NotifyBuildTreeCompleted()
	{
		if (s_testHooks != null)
		{
			s_testHooks.NotifyBuildTreeCompletedImpl();
		}
	}
}
