namespace TheXDS.Vivianne.Codecs.Textures;

/// <summary>
/// Implements a codec for images that use RefPack compression on raw pixel
/// data.
/// </summary>
public class RefPackImageCodec : IImageCodec
{
    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, int width, int height)
    {
        return RefPackCodec.Decompress(sourceBytes);
    }

    /// <inheritdoc/>
    public byte[] Encode(byte[] sourceBytes, int width, int height)
    {
        return RefPackCodec.Compress(sourceBytes);
    }
}
