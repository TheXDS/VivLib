using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct AsfBlockHeader
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Magic;
    public int BlockSize;
}
