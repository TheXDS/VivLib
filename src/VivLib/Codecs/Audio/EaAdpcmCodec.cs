using System.Runtime.InteropServices;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Serializers.Audio;
using TheXDS.Vivianne.Serializers.Audio.Mus;

namespace TheXDS.Vivianne.Codecs.Audio;

/// <summary>
/// Implements an audio codec that can read and decode EA ADPCM audio data.
/// </summary>
public class EaAdpcmCodec : IAudioCodec
{
    private static readonly long[] EATable =
    [
        0x00000000,
        0x000000F0,
        0x000001CC,
        0x00000188,
        0x00000000,
        0x00000000,
        0xFFFFFF30,
        0xFFFFFF24,
        0x00000000,
        0x00000001,
        0x00000003,
        0x00000004,
        0x00000007,
        0x00000008,
        0x0000000A,
        0x0000000B,
        0x00000000,
        0xFFFFFFFF,
        0xFFFFFFFD,
        0xFFFFFFFC
    ];

    /// <summary>
    /// Creates a new instance of the <see cref="EaAdpcmCodec"/> class.
    /// </summary>
    /// <returns>A new <see cref="EaAdpcmCodec"/> instance.</returns>
    public static EaAdpcmCodec Create() => new();

    /// <inheritdoc/>
    public byte[] Decode(byte[] sourceBytes, PtHeader header) => header[PtAudioHeaderField.Channels].Value switch
    {
        2 => DecompressStereo(sourceBytes),
        _ => throw new NotSupportedException($"Unsupported channel count: {header[PtAudioHeaderField.Channels]}"),
    };

    /// <inheritdoc/>
    public byte[] Encode(byte[] sourceBytes, PtHeader header)
    {
        return header[PtAudioHeaderField.Channels].Value switch
        {
            2 => EncodeStereo(sourceBytes),
            _ => throw new NotSupportedException($"Unsupported channel count: {header[PtAudioHeaderField.Channels]}"),
        };
    }

    private static byte[] EncodeStereo(byte[] sourceBytes)
    {
        int totalFrames = sourceBytes.Length / 4;
        short[] pcm = new short[totalFrames * 2];
        Buffer.BlockCopy(sourceBytes, 0, pcm, 0, totalFrames * 4);
        var chunkHeader = new EaAdpcmStereoChunkHeader
        {
            OutSize = totalFrames,
            LeftChannel = new EaAdpcmInitialState { CurrentSample = 0, PreviousSample = 0 },
            RightChannel = new EaAdpcmInitialState { CurrentSample = 0, PreviousSample = 0 },
        };
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.MarshalWriteStruct(chunkHeader);

        const int SubBlockSamples = 0x1C;
        int totalSubBlocks = totalFrames / SubBlockSamples;
        int remainingSamples = totalFrames % SubBlockSamples;
        int curL = 0, prevL = 0;
        int curR = 0, prevR = 0;

        for (int block = 0; block < totalSubBlocks; block++)
        {
            int baseFrame = block * SubBlockSamples;
            EncodeSubBlock(bw, pcm, baseFrame, SubBlockSamples, ref curL, ref prevL, ref curR, ref prevR);
        }

        if (remainingSamples > 0)
        {
            int baseFrame = totalSubBlocks * SubBlockSamples;
            EncodeSubBlock(bw, pcm, baseFrame, remainingSamples, ref curL, ref prevL, ref curR, ref prevR);
        }

        return ms.ToArray();
    }

    private static void EncodeSubBlock(BinaryWriter bw, short[] pcm, int baseFrame, int count,
        ref int curL, ref int prevL, ref int curR, ref int prevR)
    {
        (int predL, int dL, int newCurL, int newPrevL) = FindBestParams(pcm, baseFrame, count, true, curL, prevL);
        (int predR, int dR, int newCurR, int newPrevR) = FindBestParams(pcm, baseFrame, count, false, curR, prevR);
        byte predictors = (byte)((predL << 4) | (predR & 0x0F));
        byte shifts = (byte)((((dL - 8) & 0x0F) << 4) | ((dR - 8) & 0x0F));
        bw.Write(predictors);
        bw.Write(shifts);
        int c1L = (int)EATable[predL];
        int c2L = (int)EATable[predL + 4];
        int c1R = (int)EATable[predR];
        int c2R = (int)EATable[predR + 4];
        int stepL = 1 << (28 - dL);
        int stepR = 1 << (28 - dR);
        for (int i = 0; i < count; i++)
        {
            int idx = (baseFrame + i) * 2;
            int xL = pcm[idx + 0];
            int xR = pcm[idx + 1];
            long residualL = ((long)xL << 8) - ((long)curL * c1L + (long)prevL * c2L);
            long residualR = ((long)xR << 8) - ((long)curR * c1R + (long)prevR * c2R);
            int nL = QuantizeNibble(residualL, stepL);
            int nR = QuantizeNibble(residualR, stepR);
            byte packed = (byte)(((nL & 0x0F) << 4) | (nR & 0x0F));
            bw.Write(packed);
            long dequantL = (int)((nL << 28)) >> dL;
            long dequantR = (int)((nR << 28)) >> dR;
            long reconL = (dequantL + (long)curL * c1L + (long)prevL * c2L + 0x80L) >> 8;
            long reconR = (dequantR + (long)curR * c1R + (long)prevR * c2R + 0x80L) >> 8;
            short yL = Clip16BitSample(reconL);
            short yR = Clip16BitSample(reconR);
            prevL = curL;
            curL = yL;
            prevR = curR;
            curR = yR;
        }
    }

    private static (int pred, int d, int curSample, int prevSample) FindBestParams(short[] pcm, int baseFrame, int count, bool leftChannel, int initialCur, int initialPrev)
    {
        long bestErr = long.MaxValue;
        int bestPred = 0;
        int bestD = 23;
        int bestCur = initialCur;
        int bestPrev = initialPrev;
        for (int pred = 0; pred <= 3; pred++)
        {
            int c1 = (int)EATable[pred];
            int c2 = (int)EATable[pred + 4];
            for (int d = 8; d <= 23; d++)
            {
                long err = 0;
                int sPrev = initialPrev;
                int sCur = initialCur;
                bool earlyBreak = false;
                for (int i = 0; i < count; i++)
                {
                    int idx = (baseFrame + i) * 2 + (leftChannel ? 0 : 1);
                    int x = pcm[idx];
                    long residual = ((long)x << 8) - ((long)sCur * c1 + (long)sPrev * c2);
                    int step = 1 << (28 - d);
                    int n = QuantizeNibble(residual, step);
                    long dequant = (int)(n << 28) >> d;
                    long recon = (dequant + (long)sCur * c1 + (long)sPrev * c2 + 0x80L) >> 8;
                    short y = Clip16BitSample(recon);
                    long e = (long)x - y;
                    err += e * e;
                    sPrev = sCur;
                    sCur = y;
                    if (err >= bestErr)
                    {
                        earlyBreak = true;
                        break;
                    }
                }
                if (!earlyBreak && err < bestErr)
                {
                    bestErr = err;
                    bestPred = pred;
                    bestD = d;
                    bestCur = sCur;
                    bestPrev = sPrev;
                }
            }
        }
        return (bestPred, bestD, bestCur, bestPrev);
    }

    private static int QuantizeNibble(long target, int step)
    {
        long signedNibble = target >= 0
            ? (target + (step >> 1)) / step
            : -((-target + (step >> 1)) / step);
        if (signedNibble < -8) signedNibble = -8;
        if (signedNibble > 7)  signedNibble = 7;
        return (int)(signedNibble & 0x0F);
    }

    private static int HINIBBLE(byte byteValue)
    {
        return byteValue >> 4;
    }

    private static int LONIBBLE(byte byteValue)
    {
        return byteValue & 0x0F;
    }

    private static short Clip16BitSample(long sample)
    {
        return (short)sample.Clamp(short.MinValue, short.MaxValue);
    }

    private static byte[] DecompressStereo(byte[] blockData)
    {
        using var br = new BinaryReader(new MemoryStream(blockData));
        var header = br.MarshalReadStruct<EaAdpcmStereoChunkHeader>();
        var compressedData = br.ReadBytes((int)(blockData.Length - br.BaseStream.Position));
        return DecompressAdpcm(compressedData, header).ToArray();
    }

    private static ReadOnlySpan<byte> DecompressAdpcm(byte[] inputBuffer, EaAdpcmStereoChunkHeader chunkHeader, int dwSubOutSize = 0x1c)
    {
        List<short> outputList = [];
        int i = 0;
        int lPrevSampleLeft = chunkHeader.LeftChannel.PreviousSample;
        int lCurSampleLeft = chunkHeader.LeftChannel.CurrentSample;
        int lPrevSampleRight = chunkHeader.RightChannel.PreviousSample;
        int lCurSampleRight = chunkHeader.RightChannel.CurrentSample;
        for (int bCount = 0; bCount < chunkHeader.OutSize / dwSubOutSize; bCount++)
        {
            if (i >= inputBuffer.Length) break;
            byte bInput = inputBuffer[i++];
            int c1left = (int)EATable[HINIBBLE(bInput)];
            int c2left = (int)EATable[HINIBBLE(bInput) + 4];
            int c1right = (int)EATable[LONIBBLE(bInput)];
            int c2right = (int)EATable[LONIBBLE(bInput) + 4];
            bInput = inputBuffer[i++];
            int dleft = HINIBBLE(bInput) + 8;
            int dright = LONIBBLE(bInput) + 8;
            for (int sCount = 0; sCount < dwSubOutSize; sCount++)
            {
                if (i >= inputBuffer.Length) break;
                bInput = inputBuffer[i++];
                int left = HINIBBLE(bInput);
                int right = LONIBBLE(bInput);
                left = left << 0x1C >> dleft;
                right = right << 0x1C >> dright;
                long leftSample = (left + (lCurSampleLeft * c1left) + (lPrevSampleLeft * c2left) + 0x80L) >> 8;
                long rightSample = (right + (lCurSampleRight * c1right) + (lPrevSampleRight * c2right) + 0x80L) >> 8;
                leftSample = Clip16BitSample(leftSample);
                rightSample = Clip16BitSample(rightSample);
                lPrevSampleLeft = lCurSampleLeft;
                lCurSampleLeft = (int)leftSample;
                lPrevSampleRight = lCurSampleRight;
                lCurSampleRight = (int)rightSample;
                outputList.Add((short)lCurSampleLeft);
                outputList.Add((short)lCurSampleRight);
            }
        }
        if (chunkHeader.OutSize % dwSubOutSize != 0 && i < inputBuffer.Length)
        {
            int remainingSamples = chunkHeader.OutSize % dwSubOutSize;
            byte bInput = inputBuffer[i++];
            int c1left = (int)EATable[HINIBBLE(bInput)];
            int c2left = (int)EATable[HINIBBLE(bInput) + 4];
            int c1right = (int)EATable[LONIBBLE(bInput)];
            int c2right = (int)EATable[LONIBBLE(bInput) + 4];

            bInput = inputBuffer[i++];
            int dleft = HINIBBLE(bInput) + 8;
            int dright = LONIBBLE(bInput) + 8;

            for (int sCount = 0; sCount < remainingSamples; sCount++)
            {
                bInput = inputBuffer[i++];
                int left = HINIBBLE(bInput);
                int right = LONIBBLE(bInput);
                left = left << 0x1C >> dleft;
                right = right << 0x1C >> dright;
                long leftSample = (left + (lCurSampleLeft * c1left) + (lPrevSampleLeft * c2left) + 0x80L) >> 8;
                long rightSample = (right + (lCurSampleRight * c1right) + (lPrevSampleRight * c2right) + 0x80L) >> 8;
                leftSample = Clip16BitSample(leftSample);
                rightSample = Clip16BitSample(rightSample);
                lPrevSampleLeft = lCurSampleLeft;
                lCurSampleLeft = (int)leftSample;
                lPrevSampleRight = lCurSampleRight;
                lCurSampleRight = (int)rightSample;
                outputList.Add((short)lCurSampleLeft);
                outputList.Add((short)lCurSampleRight);
            }
        }
        return MemoryMarshal.AsBytes(new ReadOnlySpan<short>([.. outputList]));
    }
}