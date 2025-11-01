using System.Runtime.InteropServices;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fe.Nfs2;

namespace TheXDS.Vivianne.Serializers.Fe.Nfs2;

/// <summary>
/// Implements a serializer that can read and write language files for Need For Speed II/II SE
/// </summary>
public partial class LangFileSerializer : ISerializer<LangFile>
{
    /// <inheritdoc/>
    public LangFile Deserialize(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        List<LangFileEntryHeader> entries = [];
        do
        {
            entries.Add(reader.MarshalReadStruct<LangFileEntryHeader>());
        } while (stream.Position < entries[0].Offset);
        return [..entries.Select(p => Map(p, reader))];
    }

    /// <inheritdoc/>
    public void SerializeTo(LangFile entity, Stream stream)
    {
        using var poolMs = new MemoryStream();
        using var poolWriter = new BinaryWriter(poolMs);
        using var writer = new BinaryWriter(stream);
        List<uint> offsets = [];
        var calculatedHeaderSize = Marshal.SizeOf<LangFileEntryHeader>() * entity.Count;
        foreach (var j in entity)
        {
            offsets.Add((uint)(poolMs.Position + calculatedHeaderSize));
            poolWriter.WriteNullTerminatedString(j.Text ?? string.Empty);
        }
        poolWriter.Flush();
        writer.MarshalWriteStructArray([..entity.Zip(offsets).Select(p => Map(p.First, p.Second))]);
        writer.Write(poolMs.ToArray());
    }

    private static LangFileEntry Map(LangFileEntryHeader entry, BinaryReader reader)
    {
        reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
        return new LangFileEntry()
        {
            FontSize = entry.FontSize,
            Unk_0x02 = entry.Unk_0x02,
            Unk_0x04 = entry.Unk_0x04,
            Unk_0x06 = entry.Unk_0x06,
            Text = reader.ReadNullTerminatedString()
        };
    }

    private static LangFileEntryHeader Map(LangFileEntry entry, uint offset) => new()
    {
        FontSize = entry.FontSize,
        Unk_0x02 = entry.Unk_0x02,
        Unk_0x04 = entry.Unk_0x04,
        Unk_0x06 = entry.Unk_0x06,
        Offset = offset
    };
}
