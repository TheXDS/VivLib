namespace TheXDS.Vivianne.Codecs.Textures;

internal class RefPackImageCodecTests
{
    private static readonly RefPackImageCodec Codec = new();

    [Test]
    public void Decode_delegates_to_RefPack_Decompress()
    {
        byte[] source = [.. Enumerable.Range(0, 1024).Select(i => (byte)(i % 256))];
        var compressed = RefPackCodec.Compress(source);
        var result = Codec.Decode(compressed, 32, 32);
        Assert.That(result.SequenceEqual(source));
    }

    [Test]
    public void Encode_delegates_to_RefPack_Compress()
    {
        byte[] source = [.. Enumerable.Range(0, 1024).Select(i => (byte)(i % 256))];
        var result = Codec.Encode(source, 32, 32);
        Assert.That(result, Is.Not.EqualTo(source));
        Assert.That(RefPackCodec.IsCompressed(result), Is.True);
    }

    [TestCaseSource(nameof(GetTestCases))]
    public void Codec_roundtrip_test(byte[] testBytes)
    {
        var compressed = Codec.Encode(testBytes, 32, 32);
        var roundtrip = Codec.Decode(compressed, 32, 32);
        Assert.That(roundtrip.SequenceEqual(testBytes));
    }

    private static IEnumerable<byte[]> GetTestCases()
    {
        yield return GetRunOfZeros(4096);
        yield return GetRunOfZeros(65536);
        yield return GetBlockPattern(4096);
        yield return GetBlockPattern(65536);
    }

    private static byte[] GetBlockPattern(int size)
    {
        var arr = new byte[size];
        int half = size / 2;
        Array.Fill(arr, (byte)0, 0, half);
        Array.Fill(arr, (byte)0xFF, half, half);
        return arr;
    }

    private static byte[] GetRunOfZeros(int length)
    {
        return [.. Enumerable.Repeat((byte)0, length)];
    }

    [Test]
    public void Decode_returns_correct_size_for_various_dimensions()
    {
        byte[] source = [.. Enumerable.Range(0, 4096).Select(i => (byte)(i % 256))];
        var compressed = Codec.Encode(source, 64, 64);
        var decoded = Codec.Decode(compressed, 64, 64);
        Assert.That(decoded, Has.Length.EqualTo(source.Length));
    }
}
