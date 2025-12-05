using System.Globalization;
using System.Numerics;
using TheXDS.MCART.Helpers;
using TheXDS.MCART.Resources;
using TheXDS.Vivianne.Misc;
using TheXDS.Vivianne.Models.Audio.Bnk;

namespace TheXDS.Vivianne.Tools.Audio.Bnk;

/// <summary>
/// Includes helper functions that allow volume normalization on
/// <see cref="BnkStream"/> objects.
/// </summary>
public static class BnkNormalizer
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
        return NormalizeVolume(stream.SampleData, stream.BytesPerSample * 8, level);
    }

    /// <summary>
    /// Returns a volume-normalized copy of the provided raw audio data.
    /// </summary>
    /// <param name="data">Raw audio data to be normalized.</param>
    /// <param name="bits">
    /// Number of bits per sample. As of right now, only <c>8</c>, <c>16</c>
    /// and <c>32</c> bits samples are supported.
    /// </param>
    /// <param name="level">
    /// Volume normalization level. Must be between <c>0.0</c> and <c>1.0</c>.
    /// </param>
    /// <returns>
    /// The raw, volume-normalized audio data from the specified audio data.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the number of bits per sample is not currently supported.
    /// </exception>
    public static byte[] NormalizeVolume(byte[] data, int bits, double level)
    {
        if (data is null || data.Length == 0) return [];
        if (!level.IsBetween(0.0, 1.0)) throw Errors.ValueOutOfRange(nameof(level), 0.0, 1.0);
        return bits switch
        {
            8 => CommonHelpers.MapToByte(Normalize(CommonHelpers.MaptoSByte(data), level)),
            16 => CommonHelpers.MapToByte(Normalize(CommonHelpers.MapToInt16(data), level)),
            32 => CommonHelpers.MapToByte(Normalize(CommonHelpers.MapToInt32(data), level)),
            _ => throw new InvalidOperationException()
        };
    }

    private static T[] Normalize<T>(T[] data, double level)
        where T : notnull,
        ISignedNumber<T>,
        IMinMaxValue<T>,
        IConvertible
    {
        var maxSample = data.Select(p => p.ToInt64(null)).Select(Math.Abs).Max()!;
        var max = T.MaxValue.ToDouble(null) * level;
        var multiplier = max / maxSample;
        return [.. data.Select(p => ((IConvertible)(p.ToDouble(null) * multiplier)).ToType(typeof(T), CultureInfo.InvariantCulture)).Cast<T>()];
    }
}
