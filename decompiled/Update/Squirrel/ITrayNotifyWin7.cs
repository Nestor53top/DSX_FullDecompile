using System.Runtime.InteropServices;

namespace Squirrel;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("FB852B2C-6BAD-4605-9551-F15F87830935")]
internal interface ITrayNotifyWin7
{
	void RegisterCallback([MarshalAs(UnmanagedType.Interface)] INotificationCb callback);

	void SetPreference([In] ref NOTIFYITEM_Writable notifyItem);

	void EnableAutoTray([In] bool enabled);
}
