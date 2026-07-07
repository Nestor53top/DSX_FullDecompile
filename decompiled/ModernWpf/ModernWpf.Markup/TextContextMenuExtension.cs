using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
using ModernWpf.Controls;

namespace ModernWpf.Markup;

[EditorBrowsable(EditorBrowsableState.Never)]
[MarkupExtensionReturnType(typeof(ContextMenu))]
public class TextContextMenuExtension : MarkupExtension
{
	private static readonly ThreadLocal<TextContextMenu> DefaultContextMenu = new ThreadLocal<TextContextMenu>(() => new TextContextMenu());

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		return DefaultContextMenu.Value;
	}
}
