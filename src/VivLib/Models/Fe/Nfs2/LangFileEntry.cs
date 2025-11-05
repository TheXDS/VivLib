namespace TheXDS.Vivianne.Models.Fe.Nfs2;

/// <summary>
/// Represents a single entry in a language file for Need For Speed II/II SE.
/// </summary>
public class LangFileEntry
{
    /// <summary>
    /// Gets or sets the desired font size.
    /// </summary>
    public ushort FontSize { get; set; }

    /// <summary>
    /// Gets or sets the unknown value at offset 0x02.
    /// </summary>
    public ushort Unk_0x02 { get; set; }

    /// <summary>
    /// Gets or sets the unknown value at offset 0x04.
    /// </summary>
    public ushort Unk_0x04 { get; set; }

    /// <summary>
    /// Gets or sets the unknown value at offset 0x06.
    /// </summary>
    public ushort Unk_0x06 { get; set; }

    /// <summary>
    /// Gets or sets the desired text to be displayed.
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
