namespace TheXDS.Vivianne.Models.Audio.Base;

/// <summary>
/// Represents only the header information of an audio stream, without the
/// actual audio data.
/// </summary>
public class AudioStreamHeader : AudioStreamBase
{
    /// <summary>
    /// Gets or sets the size of the audio stream data in bytes.
    /// </summary>
    public int DataSize { get; set; }

    /// <inheritdoc/>
    public override int TotalSamples => DataSize / (Channels * BytesPerSample);
}
