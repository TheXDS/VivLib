using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Tools.Base;

namespace TheXDS.Vivianne.Tools.Audio.Bnk;

/// <summary>
/// Includes helper functions that allow volume normalization on
/// <see cref="AsfFile"/> objects.
/// </summary>
public class AsfNormalizer : IInPlaceTransformTool<AsfFile, double>, IInPlaceTransformTool<AsfFile>
{
    /// <summary>
    /// Returns a volume-normalized copy of the audio data in the
    /// <see cref="AsfFile"/>.
    /// </summary>
    /// <param name="stream">
    /// <see cref="AsfFile"/> to read the audio data from.
    /// </param>
    /// <param name="level">
    /// Volume normalization level. Must be between <c>0.0</c> and <c>1.0</c>.
    /// </param>
    /// <returns>
    /// The raw, volume-normalized audio data from the specified
    /// <see cref="AsfFile"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the number of bits per sample is not currently supported.
    /// </exception>
    public static byte[] NormalizeVolume(AsfFile stream, double level)
    {
        return AudioNormalizer.NormalizeVolume([..stream.AudioBlocks.SelectMany(p => p)], stream.BytesPerSample * 8, level);
    }

    /// <inheritdoc/>
    public Task<bool> TransformAsync(AsfFile item, double parameters, IProgress<ProgressReport> progress, CancellationToken cancellationToken)
    {
        progress.Report(0);
        for (var j = 0; j < item.AudioBlocks.Count; j++)
        {
            item.AudioBlocks[j] = AudioNormalizer.NormalizeVolume(item.AudioBlocks[j], item.BytesPerSample * 8, parameters);
            progress.Report((j + 1) * 100.0 / item.AudioBlocks.Count);
        }
        progress.Report(100);
        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    public Task<bool> TransformAsync(AsfFile item, IProgress<ProgressReport> progress, CancellationToken cancellationToken)
    {
        return TransformAsync(item, 1.0, progress, cancellationToken);
    }
}