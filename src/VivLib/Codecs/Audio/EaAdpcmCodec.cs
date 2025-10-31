using System.Runtime.InteropServices;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Misc;
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

                // Apply shift
                left = left << 0x1C >> dleft;
                right = right << 0x1C >> dright;

                // Calculate new samples with predictor coefficients
                long leftSample = (left + (lCurSampleLeft * c1left) + (lPrevSampleLeft * c2left) + 0x80L) >> 8;
                long rightSample = (right + (lCurSampleRight * c1right) + (lPrevSampleRight * c2right) + 0x80L) >> 8;

                leftSample = Clip16BitSample(leftSample);
                rightSample = Clip16BitSample(rightSample);

                // Update previous and current samples
                lPrevSampleLeft = lCurSampleLeft;
                lCurSampleLeft = (int)leftSample;

                lPrevSampleRight = lCurSampleRight;
                lCurSampleRight = (int)rightSample;

                // Add the stereo sample to the output
                outputList.Add((short)lCurSampleLeft);
                outputList.Add((short)lCurSampleRight);
            }
        }
        return MemoryMarshal.AsBytes(new ReadOnlySpan<short>([.. outputList]));
    }

    private static byte[] Encode(short[] pcmSamples, int samplesPerBlock = 28)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        for (int i = 0; i < pcmSamples.Length; i += samplesPerBlock * 2)
        {
            var blockPcm = pcmSamples.Skip(i).Take(samplesPerBlock * 2).ToArray();
            var (header, compressed) = EncodeBlock(blockPcm);
            bw.MarshalWriteStruct(header);
            bw.Write(compressed);
        }

        return ms.ToArray();
    }

    private static (EaAdpcmStereoChunkHeader, byte[]) EncodeBlock(short[] pcm)
    {
        const int SubBlockSamples = 0x1C;
        int numSamples = pcm.Length / 2;
        short[] left = new short[numSamples];
        short[] right = new short[numSamples];
        for (int i = 0; i < numSamples; i++)
        {
            left[i] = pcm[i * 2];
            right[i] = pcm[i * 2 + 1];
        }
        var compressed = new List<byte>();
        var initialLeftState = new EaAdpcmInitialState
        {
            PreviousSample = left.Length > 0 ? left[0] : (short)0,
            CurrentSample = left.Length > 1 ? left[1] : (short)0
        };
        var initialRightState = new EaAdpcmInitialState
        {
            PreviousSample = right.Length > 0 ? right[0] : (short)0,
            CurrentSample = right.Length > 1 ? right[1] : (short)0
        };
        var leftState = new EaAdpcmInitialState
        {
            PreviousSample = initialLeftState.PreviousSample,
            CurrentSample = initialLeftState.CurrentSample
        };
        var rightState = new EaAdpcmInitialState
        {
            PreviousSample = initialRightState.PreviousSample,
            CurrentSample = initialRightState.CurrentSample
        };
        for (int offset = 0; offset < numSamples; offset += SubBlockSamples)
        {
            int remaining = Math.Min(SubBlockSamples, numSamples - offset);
            short[] leftSubPcm = new short[2 + SubBlockSamples];
            short[] rightSubPcm = new short[2 + SubBlockSamples];
            leftSubPcm[0] = leftState.PreviousSample;
            leftSubPcm[1] = leftState.CurrentSample;
            rightSubPcm[0] = rightState.PreviousSample;
            rightSubPcm[1] = rightState.CurrentSample;
            for (int i = 0; i < remaining; i++)
            {
                leftSubPcm[2 + i] = left[offset + i];
                rightSubPcm[2 + i] = right[offset + i];
            }
            if (remaining < SubBlockSamples)
            {
                short lastL = left[offset + remaining - 1];
                short lastR = right[offset + remaining - 1];
                for (int i = remaining; i < SubBlockSamples; i++)
                {
                    leftSubPcm[2 + i] = lastL;
                    rightSubPcm[2 + i] = lastR;
                }
            }
            var (lPredictor, lShift, lNibbles, lNewState) = EncodeChannel(leftSubPcm);
            var (rPredictor, rShift, rNibbles, rNewState) = EncodeChannel(rightSubPcm);
            compressed.Add((byte)((lPredictor << 4) | (rPredictor & 0x0F)));
            compressed.Add((byte)(((lShift - 8) << 4) | ((rShift - 8) & 0x0F)));
            int nibCount = Math.Min(SubBlockSamples, Math.Min(lNibbles.Length, rNibbles.Length));
            for (int i = 0; i < nibCount; i++)
                compressed.Add((byte)((lNibbles[i] << 4) | (rNibbles[i] & 0x0F)));
            leftState = lNewState;
            rightState = rNewState;
        }
        var header = new EaAdpcmStereoChunkHeader
        {
            OutSize = (ushort)numSamples,
            LeftChannel = initialLeftState,
            RightChannel = initialRightState
        };
        return (header, compressed.ToArray());
    }

    private static (int predictor, int shift, byte[] nibbles, EaAdpcmInitialState state) EncodeChannel(short[] pcm)
    {
        int bestPredictor = 0;
        int bestShift = 0;
        long bestError = long.MaxValue;
        byte[] bestNibbles = [];
        EaAdpcmInitialState bestState = default;

        for (int predictor = 0; predictor < 16; predictor++)
        {
            int c1 = (int)EATable[predictor];
            int c2 = (int)EATable[predictor + 4];

            for (int dshift = 8; dshift <= 15; dshift++)
            {
                var nibbles = new byte[Math.Max(0, pcm.Length - 2)];
                int prev = pcm[0];
                int curr = pcm[1];
                long err = 0;
                for (int i = 2; i < pcm.Length; i++)
                {
                    int delta = (pcm[i] << 8) - ((c1 * curr) + (c2 * prev));
                    int quantized = (int)(((long)delta << dshift) >> 28);
                    quantized = Math.Clamp(quantized, -8, 7);
                    int reconstructed = (int)((((long)quantized << 28) >> dshift) + ((c1 * curr) + (c2 * prev)) + 0x80) >> 8;
                    err += Math.Abs(pcm[i] - reconstructed);
                    prev = curr;
                    curr = reconstructed;
                    nibbles[i - 2] = (byte)(quantized & 0xF);
                }
                if (err < bestError)
                {
                    bestError = err;
                    bestPredictor = predictor;
                    bestShift = dshift;
                    bestNibbles = nibbles;
                    bestState = new EaAdpcmInitialState
                    {
                        PreviousSample = (short)prev,
                        CurrentSample = (short)curr
                    };
                }
            }
        }

        return (bestPredictor, bestShift, bestNibbles, bestState);
    }

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
        return Encode(CommonHelpers.MapToInt16(sourceBytes));
    }
}