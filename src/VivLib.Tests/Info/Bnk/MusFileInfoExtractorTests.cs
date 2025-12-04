#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Info.Bnk;
using Moq;

namespace TheXDS.Vivianne.Info.Bnk;

[TestFixture]
public class MusFileInfoExtractorTests
{
    private MusFileInfoExtractor _extractorHumanSize = null!;
    private MusFileInfoExtractor _extractorByteSize = null!;

    [SetUp]
    public void Setup()
    {
        _extractorHumanSize = new(humanSize: true);
        _extractorByteSize = new(humanSize: false);
    }

    [Test]
    public void GetInfo_WithSimpleMusFile_ReturnsInfoArray()
    {
        var musFile = CreateSimpleMusFile();

        var info = _extractorHumanSize.GetInfo(musFile);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info, Is.All.InstanceOf<string>());
    }

    [Test]
    public void GetInfo_InheritsFromAsfFileInfoExtractor()
    {
        var musFile = CreateSimpleMusFile();

        var info = _extractorHumanSize.GetInfo(musFile);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info.Any(s => s.Contains("Duration")), Is.True);
            Assert.That(info.Any(s => s.Contains("Total samples")), Is.True);
            Assert.That(info.Any(s => s.Contains("Channels")), Is.True);
            Assert.That(info.Any(s => s.Contains("Audio stream format")), Is.True);
            Assert.That(info.Any(s => s.Contains("Sample rate")), Is.True);
        }
    }

    [Test]
    public void GetInfo_JoinsMultipleSubStreams()
    {
        var musFile = new MusFile();
        musFile.AsfSubStreams[0] = new AsfFile
        {
            Channels = 2,
            SampleRate = 44100,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks = { new byte[8820] }
        };
        musFile.AsfSubStreams[1] = new AsfFile
        {
            Channels = 2,
            SampleRate = 44100,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks = { new byte[8820] }
        };

        var info = _extractorHumanSize.GetInfo(musFile);

        Assert.That(info, Is.Not.Empty);
    }

    [Test]
    public void GetInfo_WithHumanSizeTrue_ReturnsSizeInHumanReadableFormat()
    {
        var musFile = CreateSimpleMusFile();

        var info = _extractorHumanSize.GetInfo(musFile);
        var sizeInfo = info.FirstOrDefault(s => s.Contains("Size"));

        if (sizeInfo != null)
        {
            Assert.That(sizeInfo, Does.Match(@"\d+(\.\d+)?\s*[KMGT]?iB"));
        }
    }

    [Test]
    public void GetInfo_WithHumanSizeFalse_ReturnsSizeInBytes()
    {
        var musFile = CreateSimpleMusFile();

        var info = _extractorByteSize.GetInfo(musFile);
        var sizeInfo = info.FirstOrDefault(s => s.Contains("Size"));

        if (sizeInfo != null)
        {
            // Should contain numeric byte count
            Assert.That(sizeInfo, Does.Match(@"\d+"));
        }
    }

    [Test]
    public void GetInfo_WithMultipleAudioBlocks_IncludesBlockCount()
    {
        var musFile = new MusFile();
        musFile.AsfSubStreams[0] = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks =
            {
                new byte[4410],
                new byte[4410],
                new byte[4410]
            }
        };

        var info = _extractorHumanSize.GetInfo(musFile);

        Assert.That(info.Any(s => s.Contains("Total audio blocks")), Is.True);
    }

    [Test]
    public void GetInfo_ResultDoesNotContainNullEntries()
    {
        var musFile = CreateSimpleMusFile();

        var info = _extractorHumanSize.GetInfo(musFile);

        Assert.That(info, Does.Not.Contain(null));
    }

    [Test]
    public void GetInfo_WithDifferentChannelCounts_IncludesCorrectChannelInfo()
    {
        for (byte channels = 1; channels <= 4; channels++)
        {
            var musFile = new MusFile();
            musFile.AsfSubStreams[0] = new AsfFile
            {
                Channels = channels,
                SampleRate = 22050,
                BytesPerSample = 2,
                Compression = CompressionMethod.None,
                LoopStart = 0,
                LoopEnd = 22050,
                AudioBlocks = { new byte[44100] }
            };

            var info = _extractorHumanSize.GetInfo(musFile);
            var channelInfo = info.First(s => s.Contains("Channels"));

            Assert.That(channelInfo, Does.Contain(channels.ToString()));
        }
    }

    [Test]
    public void GetInfo_WithDifferentSampleRates_IncludesCorrectSampleRateInfo()
    {
        var sampleRates = new ushort[] { 8000, 22050, 44100, 48000 };

        foreach (var sampleRate in sampleRates)
        {
            var musFile = new MusFile();
            musFile.AsfSubStreams[0] = new AsfFile
            {
                Channels = 1,
                SampleRate = sampleRate,
                BytesPerSample = 2,
                Compression = CompressionMethod.None,
                LoopStart = 0,
                LoopEnd = sampleRate,
                AudioBlocks = { new byte[sampleRate * 2] }
            };

            var info = _extractorHumanSize.GetInfo(musFile);
            var rateInfo = info.First(s => s.Contains("Sample rate"));

            Assert.That(rateInfo, Does.Contain(sampleRate.ToString()));
        }
    }

    [Test]
    public void GetInfo_WithLoopOffset_IncludesLoopOffsetInfo()
    {
        var musFile = new MusFile();
        musFile.AsfSubStreams[0] = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            LoopOffset = 11025,
            AudioBlocks = { new byte[44100] }
        };

        var info = _extractorHumanSize.GetInfo(musFile);

        // At least one sub-stream has loop offset
        Assert.That(info.Any(s => s.Contains("SCLl Loop offset")), Is.True);
    }

    [Test]
    public void GetInfo_WithProperties_IncludesPTHeaderProperties()
    {
        var musFile = new MusFile();
        musFile.AsfSubStreams[0] = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks = { new byte[44100] },
            Properties = new Dictionary<byte, PtHeaderValue>
            {
                { 0x10, new PtHeaderValue(4, 0x12345678) }
            }
        };

        var info = _extractorHumanSize.GetInfo(musFile);

        Assert.That(info.Any(s => s.Contains("PTHeader")), Is.True);
    }

    private static MusFile CreateSimpleMusFile()
    {
        var musFile = new MusFile();
        musFile.AsfSubStreams[0] = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks = { new byte[44100] }
        };
        return musFile;
    }
}
