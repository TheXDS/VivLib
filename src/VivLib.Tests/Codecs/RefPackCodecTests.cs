using System.Security.Cryptography;
using TheXDS.Vivianne.Serializers;

namespace TheXDS.Vivianne.Codecs;

internal class RefPackCodecTests
{
    [Test]
    public void IsCompressed_returns_false_if_RefPack_signature_is_missing()
    {
        Assert.That(RefPackCodec.IsCompressed([0, 1, 2, 3, 4]), Is.False);
    }

    [Test]
    public void IsCompressed_returns_true_if_RefPack_signature_is_present()
    {
        Assert.That(RefPackCodec.IsCompressed([0x10, 0xFB]), Is.True);
    }

    [TestCaseSource(nameof(GetTestCases))]
    public void Codec_roundtrip_test(byte[] testFileContents)
    {
        var compressed = RefPackCodec.Compress(testFileContents);
        var roundtrip = RefPackCodec.Decompress(compressed);
        Assert.That(roundtrip.SequenceEqual(testFileContents));
    }

    [Test]
    public void Codec_throws_on_uncompressible_data()
    {
        Assert.That(() => RefPackCodec.Compress(GetDeterministicRndArray(1048576)), Throws.InstanceOf<InvalidDataException>());
    }

    private static IEnumerable<byte[]> GetTestCases()
    {
        yield return GetTestFsh();
        yield return GetDeterministicRndArray(65536);
        yield return [.. Enumerable.Range(0, 65536).Select(i => (byte)i)];
    }

    [Test]
    public void Codec_roundtrip_large_data_test()
    {
        byte[] testFileContents = [.. Enumerable.Range(0, 1048576).Select(i => (byte)i)];
        var compressed = RefPackCodec.Compress(testFileContents);
        var roundtrip = RefPackCodec.Decompress(compressed);
        Assert.That(roundtrip.SequenceEqual(testFileContents));
    }

    private static byte[] GetTestFsh()
    {
        using var testStream = typeof(SerializerTestsBase<,>).Assembly!.GetManifestResourceStream(@"TheXDS.Vivianne.Resources.Files.test.fsh")!;
        using var ms = new MemoryStream();
        testStream.CopyTo(ms);
        return ms.ToArray();
    }

    private static byte[] GetDeterministicRndArray(int size)
    {
        var arr = new byte[size];
        new Random(size).NextBytes(arr);
        return arr;
    }
}
