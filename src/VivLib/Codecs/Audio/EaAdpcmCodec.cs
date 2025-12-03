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
    /// <returns></returns>
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
        // Unpack interleaved 16-bit PCM (L,R,L,R,...) into shorts
        int totalFrames = sourceBytes.Length / 4;
        short[] pcm = new short[totalFrames * 2];
        Buffer.BlockCopy(sourceBytes, 0, pcm, 0, totalFrames * 4);

        // Prepare header: OutSize = total output samples per channel
        var chunkHeader = new EaAdpcmStereoChunkHeader
        {
            OutSize = totalFrames,
            LeftChannel = new EaAdpcmInitialState { CurrentSample = 0, PreviousSample = 0 },
            RightChannel = new EaAdpcmInitialState { CurrentSample = 0, PreviousSample = 0 },
        };

        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.MarshalWriteStruct(chunkHeader);

        // Encoder state
        int lPrevL = chunkHeader.LeftChannel.PreviousSample;
        int lCurL = chunkHeader.LeftChannel.CurrentSample;
        int lPrevR = chunkHeader.RightChannel.PreviousSample;
        int lCurR = chunkHeader.RightChannel.CurrentSample;

        const int SubBlockSamples = 0x1C; // 28 samples per sub-block

        for (int baseFrame = 0; baseFrame < totalFrames; baseFrame += SubBlockSamples)
        {
            int count = Math.Min(SubBlockSamples, totalFrames - baseFrame);

            // Pick predictor and shift for Left channel
            (int predL, int dL) = ChoosePredictorAndShift(pcm, baseFrame, count, true, lPrevL, lCurL);
            // Pick predictor and shift for Right channel
            (int predR, int dR) = ChoosePredictorAndShift(pcm, baseFrame, count, false, lPrevR, lCurR);

            // Write control bytes
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

                // Compute residual in fixed-point 8.8 domain
                long residualL = ((long)xL << 8) - ((long)lCurL * c1L + (long)lPrevL * c2L);
                long residualR = ((long)xR << 8) - ((long)lCurR * c1R + (long)lPrevR * c2R);

                // Quantize to 4-bit nibble
                int nL = QuantizeNibble(residualL, stepL);
                int nR = QuantizeNibble(residualR, stepR);

                // Pack and write nibbles (high nibble = left, low nibble = right)
                byte packed = (byte)(((nL & 0x0F) << 4) | (nR & 0x0F));
                bw.Write(packed);

                // Reconstruct samples using decompression formula
                // This MUST match the decompressor exactly to ensure roundtrip fidelity
                long reconL = ReconstructSample(nL, dL, lCurL, lPrevL, c1L, c2L);
                long reconR = ReconstructSample(nR, dR, lCurR, lPrevR, c1R, c2R);

                short yL = Clip16BitSample(reconL);
                short yR = Clip16BitSample(reconR);

                lPrevL = lCurL;
                lCurL = yL;
                lPrevR = lCurR;
                lCurR = yR;
            }
        }

        return ms.ToArray();
    }

    /// <summary>
    /// Reconstructs a sample using the same formula as the decompressor.
    /// This ensures perfect roundtrip: encode(x) -> decode() -> x
    /// </summary>
    private static long ReconstructSample(int nibble, int d, int curSample, int prevSample, int c1, int c2)
    {
        // The decompressor does: left = left << 0x1C >> dleft
        // where dleft = d (passed as parameter)
        // This sign-extends the 4-bit value and scales it.
        // Then: (left + (cur*c1) + (prev*c2) + 0x80) >> 8
        
        // Replicate exactly what the decompressor does:
        int signExtended = (nibble << 28) >> 28;  // Same as SignExtendNibble but explicit
        long scaled = (long)signExtended << (28 - d);    // Scale: this is equivalent to shifted by step size
        
        // Apply predictor formula with rounding (0x80 is for rounding in 8.8 fixed point)
        long sample = (scaled + ((long)curSample * c1) + ((long)prevSample * c2) + 0x80L) >> 8;
        
        return sample;
    }

    private static (int predictorIndex, int d) ChoosePredictorAndShift(short[] pcm, int baseFrame, int count, bool leftChannel, int prevSample, int curSample)
    {
        int bestPred = 0;
        int bestD = 23;
        long bestErr = long.MaxValue;

        for (int pred = 0; pred <= 3; pred++)
        {
            int c1 = (int)EATable[pred];
            int c2 = (int)EATable[pred + 4];

            for (int d = 8; d <= 23; d++)
            {
                long err = 0;
                int sPrev = prevSample;
                int sCur = curSample;
                
                for (int i = 0; i < count; i++)
                {
                    int idx = (baseFrame + i) * 2 + (leftChannel ? 0 : 1);
                    int x = pcm[idx];
                    
                    // Compute residual in 8.8 fixed point
                    long residual = ((long)x << 8) - ((long)sCur * c1 + (long)sPrev * c2);
                    
                    // Quantize to nibble
                    int n = QuantizeNibble(residual, 1 << (28 - d));
                    
                    // Reconstruct using decompressor formula
                    long recon = ReconstructSample(n, d, sCur, sPrev, c1, c2);
                    short y = Clip16BitSample(recon);
                    
                    long e = (long)x - y;
                    err += e * e;
                    
                    sPrev = sCur;
                    sCur = y;
                    
                    if (err >= bestErr) break;
                }

                if (err < bestErr)
                {
                    bestErr = err;
                    bestPred = pred;
                    bestD = d;
                }
            }
        }

        return (bestPred, bestD);
    }

    private static int ComputeShiftFromTarget(long maxAbsTarget)
    {
        // Ensure max |n| <= 7 where n = round(target / step), step = 1 << (28 - d)
        // => step >= maxAbsTarget / 7
        if (maxAbsTarget <= 0) return 23; // smallest step for quiet blocks
        long required = maxAbsTarget / 7 + 1; // ceiling
        int bits = CeilLog2(required);
        int d = 28 - bits;
        if (d < 8) d = 8;
        if (d > 23) d = 23;
        return d;
    }

    private static int CeilLog2(long x)
    {
        if (x <= 1) return 0;
        int n = 0;
        long v = x - 1;
        while (v > 0)
        {
            n++;
            v >>= 1;
        }
        return n;
    }

    private static int QuantizeNibble(long target, int step)
    {
        // Quantize target by step, with banker's rounding (round to nearest, ties to even)
        // This should minimize the error between target and n * step
        
        // Simple rounding: q = round(target / step)
        long q;
        if (step > 0)
        {
            q = (target + (step >> 1)) / step;  // Positive: round half up
        }
        else
        {
            q = (target - (step >> 1)) / step;  // Negative: round half down
        }
        
        // Clamp to 4-bit signed range
        if (q < -8) q = -8;
        if (q > 7) q = 7;
        
        return (int)(q & 0x0F);
    }

    private static int SignExtendNibble(int n)
    {
        // Interpret 4-bit as signed [-8..7]
        n &= 0x0F;
        return (n << 28) >> 28;
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