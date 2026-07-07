using System.Runtime.InteropServices;

namespace Squirrel;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("D782CCBA-AFB0-43F1-94DB-FDA3779EACCB")]
internal interface INotificationCb
{
	void Notify([In] uint nEvent, [In] ref NOTIFYITEM notifyItem);
}
