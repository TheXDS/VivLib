using System.Text;
using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Audio.Mus;
using St = TheXDS.Vivianne.Resources.Strings.Serializers.Audio.Mus.MusSerializer;

namespace TheXDS.Vivianne.Serializers.Audio.Mus;

/// <summary>
/// Implements a serializer for MUS/ASF files.
/// </summary>
/// <remarks>
/// Technically, a MUS file with a single ASF sub-stream is essentially an ASF
/// file. Therefore, this serializer can be used to read an ASF file and return
/// it as a <see cref="MusFile"/> instance. However, a MUS file with multiple
/// sub-streams is not a valid .ASF file, and therefore this serializer can
/// only implement deserialization of <see cref="AsfFile"/> instances, even if
/// the formats are otherwise equivalent.
/// </remarks>
public partial class MusSerializer : ISerializer<MusFile>, ISerializer<AsfFile>
{
    /// <inheritdoc/>
    public MusFile Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream, Encoding.Latin1, leaveOpen: true);
        var mus = new MusFile();
        do
        {
            var currentPosition = stream.Position;
            if (ReadAsfFile(br) is { } asf) mus.AsfSubStreams.Add((int)currentPosition, asf);
        } while ((stream.Position + Marshal.SizeOf<AsfBlockHeader>()) < stream.Length);
        return mus;
    }

    /// <inheritdoc/>
    public void SerializeTo(MusFile entity, Stream stream)
    {
        using BinaryWriter bw = new(stream, Encoding.Latin1, leaveOpen: true);
        foreach (AsfFile asf in entity.AsfSubStreams.Values)
        {
            WriteAsf(asf, bw);
        }
    }

    /// <inheritdoc/>
    public void SerializeTo(AsfFile entity, Stream stream)
    {
        using BinaryWriter bw = new(stream, Encoding.Latin1, leaveOpen: true);
        WriteAsf(entity, bw);
    }

    AsfFile IOutSerializer<AsfFile>.Deserialize(Stream stream)
    {
        using BinaryReader br = new(stream, Encoding.Latin1, leaveOpen: true);
        return ReadAsfFile(br) ?? throw new InvalidDataException(St.InvalidAsfStream);
    }
}
