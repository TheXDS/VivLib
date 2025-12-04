#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Info.Bnk;

namespace TheXDS.Vivianne.Info.Bnk;

[TestFixture]
public class AsfFileInfoExtractorTests
{
    private AsfFileInfoExtractor _extractorHumanSize = null!;
    private AsfFileInfoExtractor _extractorByteSize = null!;

    [SetUp]
    public void Setup()
    {
        _extractorHumanSize = new(humanSize: true);
        _extractorByteSize = new(humanSize: false);
    }

    [Test]
    public void GetInfo_WithSimpleAsfFile_ReturnsInfoArray()
    {
        var asfFile = new AsfFile
        {
            Channels = 2,
            SampleRate = 44100,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 44100,
            AudioBlocks =
            {
                new byte[8820], // 0.1 seconds at 44100 Hz, 2 channels, 2 bytes per sample
                new byte[8820]
            }
        };

        var info = _extractorHumanSize.GetInfo(asfFile);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info, Is.All.InstanceOf<string>());
        Assert.That(info.Any(s => s.Contains("Duration")), Is.True);
        Assert.That(info.Any(s => s.Contains("Total samples")), Is.True);
        Assert.That(info.Any(s => s.Contains("Channels")), Is.True);
        Assert.That(info.Any(s => s.Contains("Audio stream format")), Is.True);
        Assert.That(info.Any(s => s.Contains("Sample rate")), Is.True);
    }

    [Test]
    public void GetInfo_WithHumanSizeTrue_ReturnsSizeInHumanReadableFormat()
    {
        var asfFile = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks = { new byte[44100] }
        };

        var info = _extractorHumanSize.GetInfo(asfFile);
        var sizeInfo = info.First(s => s.Contains("Size"));

        Assert.That(sizeInfo, Does.Match(@"\d+(\.\d+)?\s*[KMGT]?iB"));
    }

    [Test]
    public void GetInfo_WithHumanSizeFalse_ReturnsSizeInBytes()
    {
        var asfFile = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks = { new byte[44100] }
        };

        var info = _extractorByteSize.GetInfo(asfFile);
        var sizeInfo = info.First(s => s.Contains("Size"));

        Assert.That(sizeInfo, Does.Contain("44100"));
    }

    [Test]
    public void GetInfo_WithMultipleAudioBlocks_IncludesBlockCount()
    {
        var asfFile = new AsfFile
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

        var info = _extractorHumanSize.GetInfo(asfFile);

        Assert.That(info.Any(s => s.Contains("Total audio blocks: 3")), Is.True);
    }

    [Test]
    public void GetInfo_WithLoopOffset_IncludesLoopOffsetInfo()
    {
        var asfFile = new AsfFile
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

        var info = _extractorHumanSize.GetInfo(asfFile);

        Assert.That(info.Any(s => s.Contains("SCLl Loop offset")), Is.True);
    }

    [Test]
    public void GetInfo_WithoutLoopOffset_DoesNotIncludeLoopOffsetInfo()
    {
        var asfFile = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            LoopOffset = null,
            AudioBlocks = { new byte[44100] }
        };

        var info = _extractorHumanSize.GetInfo(asfFile);

        Assert.That(info.Any(s => s.Contains("SCLl Loop offset")), Is.False);
    }

    [Test]
    public void GetInfo_AlwaysIncludesLoopStartAndEnd()
    {
        var asfFile = new AsfFile
        {
            Channels = 2,
            SampleRate = 44100,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 1000,
            LoopEnd = 5000,
            AudioBlocks = { new byte[8820] }
        };

        var info = _extractorHumanSize.GetInfo(asfFile);

        Assert.That(info.Any(s => s.Contains("PT Loop start")), Is.True);
        Assert.That(info.Any(s => s.Contains("PT Loop end")), Is.True);
    }

    [Test]
    public void GetInfo_WithProperties_IncludesPTHeaderProperties()
    {
        var asfFile = new AsfFile
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
                { 0x10, new PtHeaderValue(4, 0x12345678) },
                { 0x20, new PtHeaderValue(4, unchecked((int)0xABCDEF00)) }
            }
        };

        var info = _extractorHumanSize.GetInfo(asfFile);

        Assert.That(info.Any(s => s.Contains("PTHeader")), Is.True);
        Assert.That(info.Any(s => s.Contains("0x12345678")), Is.True);
        Assert.That(info.Any(s => s.Contains("0xABCDEF00")), Is.True);
    }

    [Test]
    public void GetInfo_WithDifferentChannelCounts_IncludesCorrectChannelInfo()
    {
        for (byte channels = 1; channels <= 6; channels++)
        {
            var asfFile = new AsfFile
            {
                Channels = channels,
                SampleRate = 22050,
                BytesPerSample = 2,
                Compression = CompressionMethod.None,
                LoopStart = 0,
                LoopEnd = 22050,
                AudioBlocks = { new byte[44100] }
            };

            var info = _extractorHumanSize.GetInfo(asfFile);
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
            var asfFile = new AsfFile
            {
                Channels = 1,
                SampleRate = sampleRate,
                BytesPerSample = 2,
                Compression = CompressionMethod.None,
                LoopStart = 0,
                LoopEnd = sampleRate,
                AudioBlocks = { new byte[sampleRate * 2] }
            };

            var info = _extractorHumanSize.GetInfo(asfFile);
            var rateInfo = info.First(s => s.Contains("Sample rate"));

            Assert.That(rateInfo, Does.Contain(sampleRate.ToString()));
        }
    }

    [Test]
    public void GetInfo_WithDifferentBitDepths_IncludesCorrectFormatInfo()
    {
        var bitDepths = new byte[] { 1, 2, 4 };

        foreach (var bytesPerSample in bitDepths)
        {
            var asfFile = new AsfFile
            {
                Channels = 1,
                SampleRate = 22050,
                BytesPerSample = bytesPerSample,
                Compression = CompressionMethod.None,
                LoopStart = 0,
                LoopEnd = 22050,
                AudioBlocks = { new byte[44100] }
            };

            var info = _extractorHumanSize.GetInfo(asfFile);
            var formatInfo = info.First(s => s.Contains("Audio stream format"));
            var expectedBits = bytesPerSample * 8;

            Assert.That(formatInfo, Does.Contain(expectedBits.ToString()));
        }
    }

    [Test]
    public void GetInfo_WithEmptyAudioBlocks_ReturnsInfoWithoutError()
    {
        var asfFile = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks = { }
        };

        var info = _extractorHumanSize.GetInfo(asfFile);

        Assert.That(info, Is.Not.Empty);
    }

    [Test]
    public void GetInfo_ResultDoesNotContainNullEntries()
    {
        var asfFile = new AsfFile
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            LoopStart = 0,
            LoopEnd = 22050,
            AudioBlocks = { new byte[44100] }
        };

        var info = _extractorHumanSize.GetInfo(asfFile);

        Assert.That(info, Does.Not.Contain(null));
    }
}
