using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[CompilerGenerated]
internal sealed class _003Cd3c4de51_002D9c7b_002D40bc_002D987e_002Dcad137b79099_003E_003CPrivateImplementationDetails_003E
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
	private struct __StaticArrayInitTypeSize_003D6
	{
	}

	internal static readonly __StaticArrayInitTypeSize_003D6 _2C663541DB0BD9F7E2C5100B9B1370697EE03F23852F26AB9926A5F5E73EAC5A/* Not supported: data(5C 00 2F 00 2E 00) */;

	internal static uint ComputeStringHash(string s)
	{
		uint num = default(uint);
		if (s != null)
		{
			num = 2166136261u;
			for (int i = 0; i < s.Length; i++)
			{
				num = (s[i] ^ num) * 16777619;
			}
		}
		return num;
	}
}
