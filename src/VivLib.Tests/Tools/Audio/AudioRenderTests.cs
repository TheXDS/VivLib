// src/VivLib/Tools/Audio/AudioRenderTests.cs
using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Tools.Audio;

[TestFixture]
internal class AudioRenderTests
{
    #region Helpers

    /// <summary>
    /// Builds a minimal 16‑bit PCM WAV byte array for the given raw data.
    /// </summary>
    private static byte[] CreateWav(byte[] pcmData, byte channels = 1, ushort sampleRate = 44100)
    {
        return AudioRender.RenderData(
            new BnkStream
            {
                SampleData = pcmData,
                BytesPerSample = 2,
                Channels = channels,
                SampleRate = sampleRate,
                Compression = CompressionMethod.None,
                Interleaved = false
            },
            pcmData);
    }

    /// <summary>
    /// Extracts the data chunk from a WAV file.
    /// </summary>
    private static byte[] ExtractDataChunk(byte[] wav)
    {
        using var br = new BinaryReader(new MemoryStream(wav));
        br.BaseStream.Seek(12, SeekOrigin.Begin); // Skip RIFF header
        while (br.BaseStream.Position < br.BaseStream.Length)
        {
            var chunkId = Encoding.ASCII.GetString(br.ReadBytes(4));
            var chunkSize = br.ReadInt32();
            if (chunkId == "data") return br.ReadBytes(chunkSize);
            br.BaseStream.Seek(chunkSize, SeekOrigin.Current);
        }
        return Array.Empty<byte>();
    }

    #endregion

    #region RenderBnk / RenderBnkLoop

    [Test]
    public void RenderBnk_ShouldProduceValidWaveHeader()
    {
        // Arrange
        var rawSamples = new byte[] { 0x01, 0x00, 0x02, 0x00 }; // 2 samples, 16‑bit mono
        var blob = new BnkStream
        {
            SampleData = rawSamples,
            BytesPerSample = 2,
            Channels = 1,
            SampleRate = 44100,
            Compression = CompressionMethod.None,
            Interleaved = false
        };

        // Act
        var wav = AudioRender.RenderBnk(blob);

        // Assert
        // RIFF header
        Assert.That(Encoding.ASCII.GetString(wav, 0, 4), Is.EqualTo("RIFF"));
        // WAVE format
        Assert.That(Encoding.ASCII.GetString(wav, 8, 4), Is.EqualTo("WAVE"));
        // fmt  sub‑chunk
        Assert.That(Encoding.ASCII.GetString(wav, 12, 4), Is.EqualTo("fmt "));
        // data sub‑chunk
        Assert.That(Encoding.ASCII.GetString(wav, 36, 4), Is.EqualTo("data"));

        var dataChunk = ExtractDataChunk(wav);
        Assert.That(dataChunk, Is.EqualTo(rawSamples));
    }

    [Test]
    public void RenderBnkLoop_ShouldRenderOnlyLoopSegment()
    {
        // Arrange: 4 samples (8 bytes)
        var rawSamples = new byte[] { 0x01, 0x00, 0x02, 0x00, 0x03, 0x00, 0x04, 0x00 };
        var blob = new BnkStream
        {
            SampleData = rawSamples,
            BytesPerSample = 2,
            Channels = 1,
            SampleRate = 44100,
            Compression = CompressionMethod.None,
            Interleaved = false,
            LoopStart = 1,   // start at second sample
            LoopEnd   = 3    // end before fourth sample
        };

        // Act
        var wav = AudioRender.RenderBnkLoop(blob);

        // Assert: only samples 2 and 3 should be present
        var data = ExtractDataChunk(wav);
        var expected = new byte[] { 0x02, 0x00, 0x03, 0x00 };
        Assert.That(data, Is.EqualTo(expected));
    }

    #endregion

    #region BnkFromWav & AsfFromWav

    [Test]
    public void BnkFromWav_ShouldRestoreSampleData()
    {
        // Arrange
        var rawSamples = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };
        var wav = CreateWav(rawSamples);

        // Act
        var bnk = AudioRender.BnkFromWav(wav);

        // Assert
        Assert.That(bnk.SampleData, Is.EqualTo(rawSamples));
        Assert.That(bnk.Channels, Is.EqualTo(1));
        Assert.That(bnk.SampleRate, Is.EqualTo(44100));
        Assert.That(bnk.BytesPerSample, Is.EqualTo(2));
    }

    [Test]
    public void AsfFromWav_ShouldStoreAudioBlock()
    {
        // Arrange
        var rawSamples = new byte[] { 0x11, 0x22, 0x33, 0x44 };
        var wav = CreateWav(rawSamples);

        // Act
        var asf = AudioRender.AsfFromWav(wav);

        // Assert
        Assert.That(asf.AudioBlocks, Has.Exactly(1).Items);
        Assert.That(asf.AudioBlocks[0], Is.EqualTo(rawSamples));
    }

    #endregion

    #region RenderData (interleaving)

    [Test]
    public void RenderData_Interleaved_ShouldCombineChannelsCorrectly()
    {
        // Arrange: stereo data – 2 samples per channel (4 samples total)
        var channel1 = new byte[] { 0x01, 0x00, 0x02, 0x00 }; // 16‑bit
        var channel2 = new byte[] { 0x03, 0x00, 0x04, 0x00 };
        var interleaved = AudioRender.RenderData(
            new BnkStream
            {
                SampleData = [.. channel1, .. channel2],
                BytesPerSample = 2,
                Channels = 2,
                SampleRate = 48000,
                Compression = CompressionMethod.None,
                Interleaved = true
            },
            [.. channel1, .. channel2]);

        // Act: extract data chunk
        var data = ExtractDataChunk(interleaved);

        // Expected interleaved order: C1S1, C2S1, C1S2, C2S2
        var expected = new byte[]
        {
            0x01, 0x00, 0x03, 0x00,  // first samples of each channel
            0x02, 0x00, 0x04, 0x00   // second samples of each channel
        };

        // Assert
        Assert.That(data, Is.EqualTo(expected));
    }

    #endregion

    #region Stream Concatenation

    [Test]
    public void JoinStreams_ShouldConcatenateAudioBlocksAndProperties()
    {
        // Arrange
        var asf1 = new AsfFile
        {
            AudioBlocks = 
            {
                new byte[] { 0x10, 0x20 },
                new byte[] { 0x30, 0x40 }
            },
            Properties = new Dictionary<byte, PtHeaderValue>
            {
                { 0x01, new PtHeaderValue { Value = 0x01 } }
            },
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None
        };
        var asf2 = new AsfFile
        {
            AudioBlocks = 
            {
                new byte[] { 0x50, 0x60 }
            },
            Properties = new Dictionary<byte, PtHeaderValue>
            {
                { 0x02, new PtHeaderValue { Value = 0x02 } }
            },
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None
        };

        // Act
        var result = AudioRender.JoinStreams([asf1, asf2]);

        // Assert
        var allData = result.AudioBlocks.SelectMany(b => b).ToArray();
        var expectedData = asf1.AudioBlocks.SelectMany(b => b).Concat(asf2.AudioBlocks.SelectMany(b => b)).ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(allData, Is.EqualTo(expectedData));
            Assert.That(result.Properties.ContainsKey(0x01), Is.True);
            Assert.That(result.Properties.ContainsKey(0x02), Is.True);
            Assert.That(result.Channels, Is.EqualTo(1));
            Assert.That(result.SampleRate, Is.EqualTo(22050));
        }
    }

    [Test]
    public void GetJointStreamHeader_ShouldReturnCommonProperties()
    {
        // Arrange
        var streams = new[]
        {
            new AsfFile { Channels = 2, SampleRate = 48000, BytesPerSample = 2, Compression = CompressionMethod.None },
            new AsfFile { Channels = 2, SampleRate = 48000, BytesPerSample = 2, Compression = CompressionMethod.None }
        };

        // Act
        var header = AudioRender.GetJointStreamHeader(streams);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(header.Channels, Is.EqualTo(2));
            Assert.That(header.SampleRate, Is.EqualTo(48000));
            Assert.That(header.BytesPerSample, Is.EqualTo(2));
            Assert.That(header.Compression, Is.EqualTo(CompressionMethod.None));
        }
    }

    [Test]
    public void ReSliceAsf_ShouldDivideIntoEqualSlices()
    {
        // Arrange
        var asf = new AsfFile
        {
            AudioBlocks =
            {
                new byte[4] { 0x01, 0x02, 0x03, 0x04 },
                new byte[4] { 0x05, 0x06, 0x07, 0x08 }
            }
        };

        // Act
        AudioRender.ReSliceAsf(asf, 2);

        // Assert: two blocks each containing 4 bytes
        Assert.That(asf.AudioBlocks, Has.Exactly(2).Items);
        foreach (var block in asf.AudioBlocks)
        {
            Assert.That(block, Has.Length.EqualTo(4));
        }

        var expected = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        Assert.That(asf.AudioBlocks[0].Concat(asf.AudioBlocks[1]), Is.EqualTo(expected));
    }

    #endregion
}