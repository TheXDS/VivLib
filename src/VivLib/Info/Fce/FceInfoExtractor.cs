using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Fce.Common;
using St = TheXDS.Vivianne.Resources.Strings.Info.Fce.FceInfoExtractor;

namespace TheXDS.Vivianne.Info.Fce;

/// <summary>
/// Implements an information extractor for <see cref="FceFile"/> entities.
/// </summary>
/// <param name="humanSize">
/// If set to <see langword="true"/>, the size of objects will be expressed
/// in human-readable format, otherwise the size of the entity in bytes
/// will be displayed directly.
/// </param>
/// <param name="showRsvdContents">
/// Indicates if the contents of the reserved tables shoudl be read and dumped.
/// </param>
public class FceInfoExtractor<T>(bool humanSize, bool showRsvdContents) : IEntityInfoExtractor<IFceFile<T>>
    where T : FcePart
{
    /// <inheritdoc/>
    public string[] GetInfo(IFceFile<T> entity)
    {
        return [.. (string[])[
            string.Format(St.FceNfo_FileSignature, entity.Magic),
            string.Format(St.FceNfo_FileFormat, VersionIdentifier.FceVersion(entity.Magic)),
            string.Format(St.FceNfo_Arts, entity.Arts),
            string.Format(St.FceNfo_BoundingBox, entity.XHalfSize * 2, entity.YHalfSize * 2, entity.ZHalfSize * 2),
            DumpTable(entity.RsvdTable1, St.FceNfo_Rsvd1),
            DumpTable(entity.RsvdTable2, St.FceNfo_Rsvd2),
            DumpTable(entity.RsvdTable3, St.FceNfo_Rsvd3),
            string.Format(St.FceNfo_DeclColors, entity.Colors.Count()),
            string.Format(St.FceNfo_Parts, entity.Parts.Count),
            string.Format(St.FceNfo_Dummies, entity.Dummies.Count),
            ]];
    }

    private string DumpTable(byte[] table, string tableName)
    {
        return showRsvdContents
            ? string.Join(Environment.NewLine, ((string[])[string.Format(St.FceNfo_RsvdContents, tableName)]).Concat(ChunkUp(table, 40).Select(ToHex)))
            : string.Format(St.FceNfo_RsvdSize, tableName, table.Length.GetSize(humanSize));
    }

    private static byte[][] ChunkUp(byte[] data, int chunkSize)
    {
        int numberOfChunks = (data.Length + chunkSize - 1) / chunkSize;
        byte[][] chunks = new byte[numberOfChunks][];
        for (int i = 0; i < numberOfChunks; i++)
        {
            int currentChunkSize = Math.Min(chunkSize, data.Length - (i * chunkSize));
            chunks[i] = new byte[currentChunkSize];
            Array.Copy(data, i * chunkSize, chunks[i], 0, currentChunkSize);
        }
        return chunks;
    }

    private static string ToHex(byte[] data)
    {
        return string.Join(" ", data.Select(p => p.ToString("X2")));
    }
}
