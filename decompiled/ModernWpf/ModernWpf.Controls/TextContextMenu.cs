using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class TextContextMenu : ContextMenu
{
	private static readonly ResourceAccessor ResourceAccessor;

	private static readonly CommandBinding _selectAllBinding;

	private static readonly CommandBinding _undoBinding;

	private static readonly CommandBinding _redoBinding;

	private readonly MenuItem _proofingMenuItem;

	public static readonly DependencyProperty UsingTextContextMenuProperty;

	static TextContextMenu()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		ResourceAccessor = new ResourceAccessor(typeof(TextContextMenu));
		UsingTextContextMenuProperty = DependencyProperty.RegisterAttached("UsingTextContextMenu", typeof(bool), typeof(TextContextMenu), new PropertyMetadata((object)false, new PropertyChangedCallback(OnUsingTextContextMenuChanged)));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TextContextMenu), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(TextContextMenu)));
		_selectAllBinding = new CommandBinding((ICommand)ApplicationCommands.SelectAll);
		_selectAllBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(OnSelectAllPreviewCanExecute);
		_undoBinding = new CommandBinding((ICommand)ApplicationCommands.Undo);
		_undoBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(OnUndoRedoPreviewCanExecute);
		_redoBinding = new CommandBinding((ICommand)ApplicationCommands.Redo);
		_redoBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(OnUndoRedoPreviewCanExecute);
	}

	public TextContextMenu()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		_proofingMenuItem = new MenuItem();
		((ItemsControl)this).Items.Add((object)_proofingMenuItem);
		((ItemsControl)this).Items.Add((object)new MenuItem
		{
			Command = (ICommand)ApplicationCommands.Cut,
			Icon = new SymbolIcon(Symbol.Cut)
		});
		((ItemsControl)this).Items.Add((object)new MenuItem
		{
			Command = (ICommand)ApplicationCommands.Copy,
			Icon = new SymbolIcon(Symbol.Copy)
		});
		((ItemsControl)this).Items.Add((object)new MenuItem
		{
			Command = (ICommand)ApplicationCommands.Paste,
			Icon = new SymbolIcon(Symbol.Paste)
		});
		((ItemsControl)this).Items.Add((object)new MenuItem
		{
			Command = (ICommand)ApplicationCommands.Undo,
			Icon = new SymbolIcon(Symbol.Undo)
		});
		((ItemsControl)this).Items.Add((object)new MenuItem
		{
			Command = (ICommand)ApplicationCommands.Redo,
			Icon = new SymbolIcon(Symbol.Redo)
		});
		((ItemsControl)this).Items.Add((object)new MenuItem
		{
			Command = (ICommand)ApplicationCommands.SelectAll
		});
	}

	public static bool GetUsingTextContextMenu(Control textControl)
	{
		return (bool)((DependencyObject)textControl).GetValue(UsingTextContextMenuProperty);
	}

	public static void SetUsingTextContextMenu(Control textControl, bool value)
	{
		((DependencyObject)textControl).SetValue(UsingTextContextMenuProperty, (object)value);
	}

	private static void OnUsingTextContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		Control val = (Control)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((UIElement)val).CommandBindings.Add(_selectAllBinding);
			((UIElement)val).CommandBindings.Add(_undoBinding);
			((UIElement)val).CommandBindings.Add(_redoBinding);
			((FrameworkElement)val).ContextMenuOpening += new ContextMenuEventHandler(OnContextMenuOpening);
		}
		else
		{
			((UIElement)val).CommandBindings.Remove(_selectAllBinding);
			((UIElement)val).CommandBindings.Remove(_undoBinding);
			((UIElement)val).CommandBindings.Remove(_redoBinding);
			((FrameworkElement)val).ContextMenuOpening -= new ContextMenuEventHandler(OnContextMenuOpening);
		}
	}

	protected override void OnOpened(RoutedEventArgs e)
	{
		((ContextMenu)this).OnOpened(e);
		if (((UIElement)_proofingMenuItem).IsVisible)
		{
			_proofingMenuItem.IsSubmenuOpen = true;
		}
	}

	protected override void OnClosed(RoutedEventArgs e)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		((ContextMenu)this).OnClosed(e);
		if (((ContextMenu)this).IsOpen)
		{
			return;
		}
		((ItemsControl)_proofingMenuItem).Items.Clear();
		foreach (MenuItem item in (IEnumerable)((ItemsControl)this).Items)
		{
			((DependencyObject)item).ClearValue(MenuItem.CommandTargetProperty);
		}
	}

	private static void OnSelectAllPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		TextBox val = (TextBox)((sender is TextBox) ? sender : null);
		if (val != null && string.IsNullOrEmpty(val.Text))
		{
			e.CanExecute = false;
			((RoutedEventArgs)e).Handled = true;
			return;
		}
		PasswordBox val2 = (PasswordBox)((sender is PasswordBox) ? sender : null);
		if (val2 != null && string.IsNullOrEmpty(val2.Password))
		{
			e.CanExecute = false;
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private static void OnUndoRedoPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		TextBoxBase val = (TextBoxBase)((sender is TextBoxBase) ? sender : null);
		if (val != null && val.IsReadOnly)
		{
			e.CanExecute = false;
			((RoutedEventArgs)e).Handled = true;
		}
	}

	private static void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		Control val = (Control)sender;
		if (((FrameworkElement)val).ContextMenu is TextContextMenu textContextMenu)
		{
			PasswordBox val2 = (PasswordBox)(object)((val is PasswordBox) ? val : null);
			Control target = (Control)((val2 == null || PasswordBoxHelper.GetPasswordRevealMode(val2) != PasswordRevealMode.Visible || !(((RoutedEventArgs)e).Source is TextBox)) ? ((object)val) : ((object)(Control)((RoutedEventArgs)e).Source));
			textContextMenu.UpdateItems(target);
			if (!((IEnumerable)((ItemsControl)textContextMenu).Items).OfType<MenuItem>().Any((MenuItem mi) => (int)((UIElement)mi).Visibility == 0))
			{
				((RoutedEventArgs)e).Handled = true;
			}
		}
	}

	private void UpdateProofingMenuItem(Control target)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		((HeaderedItemsControl)_proofingMenuItem).Header = ResourceAccessor.GetLocalizedStringResource("ProofingMenuItemLabel");
		((ItemsControl)_proofingMenuItem).Items.Clear();
		SpellingError val = null;
		TextBox val2 = (TextBox)(object)((target is TextBox) ? target : null);
		if (val2 != null)
		{
			val = val2.GetSpellingError(val2.CaretIndex);
		}
		else
		{
			RichTextBox val3 = (RichTextBox)(object)((target is RichTextBox) ? target : null);
			if (val3 != null)
			{
				val = val3.GetSpellingError(val3.CaretPosition);
			}
		}
		if (val != null)
		{
			foreach (string suggestion in val.Suggestions)
			{
				MenuItem val4 = new MenuItem
				{
					Header = suggestion,
					Command = (ICommand)EditingCommands.CorrectSpellingError,
					CommandParameter = suggestion,
					CommandTarget = (IInputElement)(object)target
				};
				((ItemsControl)_proofingMenuItem).Items.Add((object)val4);
			}
			if (((ItemsControl)_proofingMenuItem).HasItems)
			{
				((ItemsControl)_proofingMenuItem).Items.Add((object)new Separator());
			}
			((ItemsControl)_proofingMenuItem).Items.Add((object)new MenuItem
			{
				Header = Strings.IgnoreMenuItemLabel,
				Command = (ICommand)EditingCommands.IgnoreSpellingError,
				CommandTarget = (IInputElement)(object)target
			});
			((UIElement)_proofingMenuItem).Visibility = (Visibility)0;
		}
		else
		{
			((UIElement)_proofingMenuItem).Visibility = (Visibility)2;
		}
	}

	private void UpdateItems(Control target)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		UpdateProofingMenuItem(target);
		foreach (MenuItem item in (IEnumerable)((ItemsControl)this).Items)
		{
			MenuItem val = item;
			ICommand command = val.Command;
			RoutedUICommand val2 = (RoutedUICommand)((command is RoutedUICommand) ? command : null);
			if (val2 != null)
			{
				if (val2 == ApplicationCommands.Cut)
				{
					((HeaderedItemsControl)val).Header = ResourceAccessor.GetLocalizedStringResource("TextCommandLabelCut");
				}
				else if (val2 == ApplicationCommands.Copy)
				{
					((HeaderedItemsControl)val).Header = ResourceAccessor.GetLocalizedStringResource("TextCommandLabelCopy");
				}
				else if (val2 == ApplicationCommands.Paste)
				{
					((HeaderedItemsControl)val).Header = ResourceAccessor.GetLocalizedStringResource("TextCommandLabelPaste");
				}
				else if (val2 == ApplicationCommands.Undo)
				{
					((HeaderedItemsControl)val).Header = ResourceAccessor.GetLocalizedStringResource("TextCommandLabelUndo");
				}
				else if (val2 == ApplicationCommands.Redo)
				{
					((HeaderedItemsControl)val).Header = ResourceAccessor.GetLocalizedStringResource("TextCommandLabelRedo");
				}
				else if (val2 == ApplicationCommands.SelectAll)
				{
					((HeaderedItemsControl)val).Header = ResourceAccessor.GetLocalizedStringResource("TextCommandLabelSelectAll");
				}
				val.CommandTarget = (IInputElement)(object)target;
				((UIElement)val).Visibility = (Visibility)((!((RoutedCommand)val2).CanExecute((object)null, (IInputElement)(object)target)) ? 2 : 0);
			}
		}
	}
}
