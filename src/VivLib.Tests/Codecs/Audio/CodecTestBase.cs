using System.Runtime.InteropServices;
using TheXDS.Vivianne.Serializers.Audio;

namespace TheXDS.Vivianne.Codecs.Audio;

internal abstract class CodecTestBase<TCodec> where TCodec : notnull, IAudioCodec, new()
{
    private const double AcceptableCorrelation = 0.95;

    private static readonly PtHeader Stereo22050 = new();

    private static short[] _pcm;

    [OneTimeSetUp]
    public static void SetUp()
    {
        Stereo22050.AudioValues[PtAudioHeaderField.Channels] = 2;
        const double freqL = 440.0, freqR = 880.0;
        const double durationSec = 1.0;
        double sampleRate = Stereo22050.AudioValues[PtAudioHeaderField.SampleRate].Value;

        int totalSamples = (int)(sampleRate * durationSec);
        _pcm = new short[totalSamples * 2];
        for (int i = 0; i < totalSamples; i++)
        {
            double t = i / sampleRate;
            _pcm[i * 2 + 0] = (short)(Math.Sin(2 * Math.PI * freqL * t) * short.MaxValue * 0.6);
            _pcm[i * 2 + 1] = (short)(Math.Sin(2 * Math.PI * freqR * t) * short.MaxValue * 0.6);
        }
    }

    [Test]
    public void Codec_roundtrip_test()
    {
        var codec = new TCodec();
        byte[] encoded = codec.Encode(MemoryMarshal.AsBytes(new ReadOnlySpan<short>(_pcm)).ToArray(), Stereo22050);
        byte[] decoded = codec.Decode(encoded, Stereo22050);
        short[] decodedPcm = new short[decoded.Length / 2];
        Buffer.BlockCopy(decoded, 0, decodedPcm, 0, decoded.Length);
        Assert.That(ComparePcm(_pcm, decodedPcm), Is.InRange(AcceptableCorrelation, 1.0));
    }

    private static double ComparePcm(short[] original, short[] decoded)
    {
        int len = Math.Min(original.Length, decoded.Length);
        double sumSqErr = 0;
        double sumOrig = 0, sumDec = 0;
        double sumOrig2 = 0, sumDec2 = 0, sumCross = 0;
        int maxDiff = 0;

        for (int i = 0; i < len; i++)
        {
            int diff = original[i] - decoded[i];
            sumSqErr += diff * diff;
            if (Math.Abs(diff) > maxDiff) maxDiff = Math.Abs(diff);

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
        double corr = (sumCross / len - meanOrig * meanDec)
                    / Math.Sqrt((sumOrig2 / len - meanOrig * meanOrig) * (sumDec2 / len - meanDec * meanDec));

        return corr;
    }
}
