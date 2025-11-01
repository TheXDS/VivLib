namespace TheXDS.Vivianne.Models.Fe.Nfs2;

public class LangFileEntry
{
    public ushort FontSize { get; set; }

    public ushort Unk_0x02 { get; set; }

    public ushort Unk_0x04 { get; set; }

    public ushort Unk_0x06 { get; set; }

    public string Text { get; set; }
}

public class LangFile : List<LangFileEntry>;