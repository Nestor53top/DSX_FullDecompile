using System.Runtime.InteropServices;

namespace Squirrel;

[ComImport]
[Guid("D782CCBA-AFB0-43F1-94DB-FDA3779EACCB")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface INotificationCb
{
	void Notify([In] uint nEvent, [In] ref NOTIFYITEM notifyItem);
}
