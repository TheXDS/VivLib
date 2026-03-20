using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Extensions;
using TheXDS.Vivianne.Models.Audio.Bnk;
using St = TheXDS.Vivianne.Resources.Strings.Info.Audio.BnkFileInfoExtractor;

namespace TheXDS.Vivianne.Info.Audio;

/// <summary>
/// Implements an information extractor for <see cref="BnkFile"/> entities.
/// </summary>
/// <param name="humanSize">
/// If set to <see langword="true"/>, the size of objects will be expressed
/// in human-readable format, otherwise the size of the entity in bytes
/// will be displayed directly.
/// </param>
public class BnkFileInfoExtractor(bool humanSize) : IEntityInfoExtractor<BnkFile>
{
    /// <inheritdoc/>
    public string[] GetInfo(BnkFile entity)
    {
        return [
            string.Format(St.BnkFileNfo_Version, entity.FileVersion),
            string.Format(St.BnkFileNfo_DeclaredStreams, entity.Streams.Count),
            string.Format(St.BnkFileNfo_StreamsWithPtHeaders, entity.Streams.NotNull().Count()),
            string.Format(St.BnkFileNfo_UsablePayload, entity.Streams.NotNull().Sum(p => p.SampleData.Length + (p.AltStream?.SampleData.Length ?? 0)).GetSize(humanSize)),
            string.Format(St.BnkFileNfo_TotalPayload, entity.PayloadSize.GetSize(humanSize)),
        ];
    }
}
