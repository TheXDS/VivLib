using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Tools.Base;

namespace TheXDS.Vivianne.Tools.Audio.Bnk;

/// <summary>
/// Includes helper functions that allow volume normalization on
/// <see cref="BnkStream"/> objects.
/// </summary>
public class BnkNormalizer : IInPlaceTransformTool<BnkStream, double>, IInPlaceTransformTool<BnkStream>
{
    /// <summary>
    /// Returns a volume-normalized copy of the audio data in the
    /// <see cref="BnkStream"/>.
    /// </summary>
    /// <param name="stream">
    /// <see cref="BnkStream"/> to read the audio data from.
    /// </param>
    /// <param name="level">
    /// Volume normalization level. Must be between <c>0.0</c> and <c>1.0</c>.
    /// </param>
    /// <returns>
    /// The raw, volume-normalized audio data from the specified
    /// <see cref="BnkStream"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the number of bits per sample is not currently supported.
    /// </exception>
    public static byte[] NormalizeVolume(BnkStream stream, double level)
    {
        return AudioNormalizer.NormalizeVolume(stream.SampleData, stream.BytesPerSample * 8, level);
    }

    /// <inheritdoc/>
    public Task<bool> TransformAsync(BnkStream item, double parameters, IProgress<ProgressReport> progress, CancellationToken cancellationToken)
    {
        item.SampleData = AudioNormalizer.NormalizeVolume(item.SampleData, item.BytesPerSample * 8, parameters);
        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    public Task<bool> TransformAsync(BnkStream item, IProgress<ProgressReport> progress, CancellationToken cancellationToken)
    {
        return TransformAsync(item, 1.0, progress, cancellationToken);
    }
}
