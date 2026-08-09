using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Mus;

namespace TheXDS.Vivianne.Serializers;

[TestFixture]
internal class MusSerializer_EaAdpcmRoundtrip_Tests
{
    private const double AcceptableCorrelation = 0.9999;

    [Test]
    public void EaAdpcm_serialize_deserialize_preserves_pcm()
    {
        const int frameCount = 0x1C * 4;
        short[] pcm = new short[frameCount * 2];
        for (int i = 0; i < frameCount; i++)
        {
            short sample = (short)(Math.Sin(2 * Math.PI * 440 * i / 22050.0) * short.MaxValue * 0.6);
            pcm[i * 2 + 0] = sample;
            pcm[i * 2 + 1] = sample;
        }

        byte[] pcmBytes = MemoryMarshal.AsBytes(new ReadOnlySpan<short>(pcm)).ToArray();
        var asf = new AsfFile
        {
            Channels = 2,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.EA_ADPCM,
            LoopStart = 0,
            LoopEnd = frameCount,
        };
        asf.AudioBlocks.Add(pcmBytes);

        var serializer = new MusSerializer();
        using var ms = new MemoryStream();
        serializer.SerializeTo(asf, ms);
        ms.Position = 0;
        var roundtrip = ((IOutSerializer<AsfFile>)serializer).Deserialize(ms);

        Assert.That(roundtrip.Compression, Is.EqualTo(CompressionMethod.EA_ADPCM));
        Assert.That(roundtrip.AudioBlocks, Has.Count.EqualTo(1));
        Assert.That(roundtrip.AudioBlocks[0].Length, Is.EqualTo(pcmBytes.Length));

        short[] decoded = new short[pcm.Length];
        Buffer.BlockCopy(roundtrip.AudioBlocks[0], 0, decoded, 0, roundtrip.AudioBlocks[0].Length);
        Assert.That(ComparePcm(pcm, decoded), Is.GreaterThan(AcceptableCorrelation));
    }

    private static double ComparePcm(short[] original, short[] decoded)
    {
        int len = Math.Min(original.Length, decoded.Length);
        double sumOrig = 0, sumDec = 0;
        double sumOrig2 = 0, sumDec2 = 0, sumCross = 0;

        for (int i = 0; i < len; i++)
        {
            double o = original[i];
            double d = decoded[i];
            sumOrig += o;
            sumDec += d;
            sumOrig2 += o * o;
            sumDec2 += d * d;
            sumCross += o * d;
        }

        double meanOrig = sumOrig / len;
        double meanDec = sumDec / len;
        return (sumCross / len - meanOrig * meanDec)
            / Math.Sqrt((sumOrig2 / len - meanOrig * meanOrig) * (sumDec2 / len - meanDec * meanDec));
    }
}
