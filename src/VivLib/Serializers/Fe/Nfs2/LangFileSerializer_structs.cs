using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Fe.Nfs2;

public partial class LangFileSerializer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct LangFileEntryHeader
    {
        public ushort FontSize;
        public ushort Unk_0x02;
        public ushort Unk_0x04;
        public ushort Unk_0x06;
        public uint Offset;
    }
}
