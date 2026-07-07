using System;
using System.Runtime.InteropServices;

namespace Squirrel.Update;

internal struct WINTRUST_DATA : IDisposable
{
	public uint cbStruct = (uint)Marshal.SizeOf(typeof(WINTRUST_DATA));

	public IntPtr pPolicyCallbackData;

	public IntPtr pSIPCallbackData;

	public UiChoice dwUIChoice;

	public RevocationCheckFlags fdwRevocationChecks;

	public UnionChoice dwUnionChoice;

	public IntPtr pInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)));

	public StateAction dwStateAction;

	public IntPtr hWVTStateData;

	public TrustProviderFlags dwProvFlags;

	public UIContext dwUIContext;

	private IntPtr pwszURLReference;

	public WINTRUST_DATA(WINTRUST_FILE_INFO fileInfo)
	{
		Marshal.StructureToPtr((object)fileInfo, pInfoStruct, false);
		dwUnionChoice = UnionChoice.File;
		pPolicyCallbackData = IntPtr.Zero;
		pSIPCallbackData = IntPtr.Zero;
		dwUIChoice = UiChoice.NoUI;
		fdwRevocationChecks = RevocationCheckFlags.None;
		dwStateAction = StateAction.Ignore;
		hWVTStateData = IntPtr.Zero;
		pwszURLReference = IntPtr.Zero;
		dwProvFlags = TrustProviderFlags.Safer;
		dwUIContext = UIContext.Execute;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void Dispose(bool disposing)
	{
		if (dwUnionChoice == UnionChoice.File)
		{
			WINTRUST_FILE_INFO wINTRUST_FILE_INFO = default(WINTRUST_FILE_INFO);
			Marshal.PtrToStructure(pInfoStruct, (object)wINTRUST_FILE_INFO);
			wINTRUST_FILE_INFO.Dispose();
			Marshal.DestroyStructure(pInfoStruct, typeof(WINTRUST_FILE_INFO));
		}
		Marshal.FreeHGlobal(pInfoStruct);
	}
}
