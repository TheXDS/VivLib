using System.Runtime.InteropServices;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Resources.Strings.Serializers.Audio.Mus;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Audio;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Codecs.Audio;

internal abstract class CodecTestBase<TCodec> where TCodec : notnull, IAudioCodec, new()
{
    private const double AcceptableCorrelation = 0.9999;

    private static readonly PtHeader Stereo22050 = new();

    private static short[] _pcm;

    private static readonly double[][] DtmfTones =
    [
        [941.0, 1336.0],
        [0],
        [697.0, 1209.0],
        [0],
        [697.0, 1336.0],
        [0],
        [697.0, 1477.0],
        [0],
        [770.0, 1209.0],
        [0],
        [770.0, 1336.0],
        [0],
        [770.0, 1477.0],
        [0],
        [852.0, 1209.0],
        [0],
        [852.0, 1336.0],
        [0],
        [852.0, 1477.0],
        [0],
        [941.0],
        [0],
        [697.0],
        [0],
        [697.0],
        [0],
        [697.0],
        [0],
        [770.0],
        [0],
        [770.0],
        [0],
        [770.0],
        [0],
        [852.0],
        [0],
        [852.0],
        [0],
        [852.0],
        [0],
        [1336.0],
        [0],
        [1209.0],
        [0],
        [1336.0],
        [0],
        [1477.0],
        [0],
        [1209.0],
        [0],
        [1336.0],
        [0],
        [1477.0],
        [0],
        [1209.0],
        [0],
        [1336.0],
        [0],
        [1477.0],
        [0],
    ];

    private static short MakeTone(double[] frequency, double time, double amplitude = 0.6)
    {
        return (short)(Math.Sin(2 * Math.PI * frequency.Average() * time) * short.MaxValue * amplitude);
    }

    private static void MakeDtmfTone(int i, int digit, double time, double amplitude = 0.6)
    {
        _pcm[i * 2 + 0] = MakeTone(DtmfTones[digit], time, amplitude);
        _pcm[i * 2 + 1] = MakeTone(DtmfTones[59 - digit], time, amplitude);
    }

    [OneTimeSetUp]
    public static void SetUp()
    {
        Stereo22050.AudioValues[PtAudioHeaderField.Channels] = 2;
        const double durationSec = 60.0;
        double sampleRate = Stereo22050.AudioValues[PtAudioHeaderField.SampleRate].Value;

        int totalSamples = (int)(sampleRate * durationSec);
        _pcm = new short[totalSamples * 2];
        for (int i = 0; i < totalSamples; i++)
        {
            double t = i / sampleRate;
            MakeDtmfTone(i, (int)double.Floor(t), t);
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
        var corr = ComparePcm(_pcm, decodedPcm);
        Assert.That(corr, Is.InRange(AcceptableCorrelation, 1.0));
        Assert.Pass($"Correlation: {corr:P} quality of original PCM data preserved.");
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
