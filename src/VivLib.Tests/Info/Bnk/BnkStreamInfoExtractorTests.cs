using TheXDS.Vivianne.Info.Audio;
using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Bnk;

namespace TheXDS.Vivianne.Info.Bnk;

[TestFixture]
internal class BnkStreamInfoExtractorTests
{
    private BnkStreamInfoExtractor _extractorHumanSize = null!;
    private BnkStreamInfoExtractor _extractorByteSize = null!;

    [SetUp]
    public void Setup()
    {
        _extractorHumanSize = new(humanSize: true);
        _extractorByteSize = new(humanSize: false);
    }

    [Test]
    public void GetInfo_WithSimpleBnkStream_ReturnsInfoArray()
    {
        var stream = new BnkStream
        {
            Channels = 2,
            SampleRate = 44100,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[8820],
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info, Is.All.InstanceOf<string>());
        Assert.That(info.Any(s => s.Contains("Duration")), Is.True);
        Assert.That(info.Any(s => s.Contains("samples")), Is.True);
        Assert.That(info.Any(s => s.Contains("Channels")), Is.True);
    }

    [Test]
    public void GetInfo_IncludesDuration()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[44100], // 1 second at 22050 Hz
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);
        var durationInfo = info.First(s => s.Contains("Duration"));

        Assert.That(durationInfo, Does.Match(@"\d+:\d+:\d+"));
    }

    [Test]
    public void GetInfo_IncludesSampleCount()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[44100],
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);
        var samplesInfo = info.First(s => s.Contains("samples"));

        Assert.That(samplesInfo, Does.Contain("22050"));
    }

    [Test]
    public void GetInfo_IncludesChannelCount()
    {
        var stream = new BnkStream
        {
            Channels = 6,
            SampleRate = 44100,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[8820],
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);
        var channelInfo = info.First(s => s.Contains("Channels"));

        Assert.That(channelInfo, Does.Contain("6"));
    }

    [Test]
    public void GetInfo_IncludesSampleRate()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 48000,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[96000],
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);
        var rateInfo = info.First(s => s.Contains("Sample rate"));

        Assert.That(rateInfo, Does.Contain("48000"));
    }

    [Test]
    public void GetInfo_IncludesFormat()
    {
        var stream = new BnkStream
        {
            Channels = 2,
            SampleRate = 44100,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[8820],
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);
        var formatInfo = info.First(s => s.Contains("format"));

        Assert.That(formatInfo, Does.Contain("16"));
        Assert.That(formatInfo, Does.Contain("Unknown").Or.Contain("PCM"));
    }

    [Test]
    public void GetInfo_IncludesSampleDataSize()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[44100],
            PostAudioStreamData = []
        };

        var info = _extractorByteSize.GetInfo(stream);
        var sizeInfo = info.First(s => s.Contains("Size"));

        Assert.That(sizeInfo, Does.Contain("44100"));
    }

    [Test]
    public void GetInfo_WithHumanSizeTrue_ReturnsSizeInHumanReadableFormat()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[1048576], // 1 MB
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);
        var sizeInfo = info.First(s => s.Contains("Size"));

        Assert.That(sizeInfo, Does.Match(@"\d+(\.\d+)?\s*[KMGT]?iB"));
    }

    [Test]
    public void GetInfo_WithAltStream_IncludesAltStreamInfo()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[44100],
            PostAudioStreamData = [],
            AltStream = new BnkStream
            {
                Channels = 1,
                SampleRate = 22050,
                BytesPerSample = 2,
                Compression = CompressionMethod.None,
                SampleData = new byte[22050],
                PostAudioStreamData = [],
                IsAltStream = true
            }
        };

        var info = _extractorHumanSize.GetInfo(stream);

        Assert.That(info.Any(s => s.Contains("alt")), Is.True);
    }

    [Test]
    public void GetInfo_WithoutAltStream_DoesNotIncludeAltStreamInfo()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[44100],
            PostAudioStreamData = [],
            AltStream = null
        };

        var info = _extractorHumanSize.GetInfo(stream);

        Assert.That(info.Any(s => s.Contains("Alt")), Is.False);
    }

    [Test]
    public void GetInfo_WithDifferentBitDepths_IncludesCorrectFormat()
    {
        var bitDepths = new byte[] { 1, 2, 4 };

        foreach (var bytesPerSample in bitDepths)
        {
            var stream = new BnkStream
            {
                Channels = 1,
                SampleRate = 22050,
                BytesPerSample = bytesPerSample,
                Compression = CompressionMethod.None,
                SampleData = new byte[44100],
                PostAudioStreamData = []
            };

            var info = _extractorHumanSize.GetInfo(stream);
            var formatInfo = info.First(s => s.Contains("format"));
            var expectedBits = bytesPerSample * 8;

            Assert.That(formatInfo, Does.Contain(expectedBits.ToString()));
        }
    }

    [Test]
    public void GetInfo_WithDifferentChannelCounts_IncludesCorrectChannels()
    {
        for (byte channels = 1; channels <= 8; channels++)
        {
            var stream = new BnkStream
            {
                Channels = channels,
                SampleRate = 22050,
                BytesPerSample = 2,
                Compression = CompressionMethod.None,
                SampleData = new byte[44100],
                PostAudioStreamData = []
            };

            var info = _extractorHumanSize.GetInfo(stream);
            var channelInfo = info.First(s => s.Contains("Channels"));

            Assert.That(channelInfo, Does.Contain(channels.ToString()));
        }
    }

    [Test]
    public void GetInfo_WithZeroLengthSampleData_ReturnsInfoWithoutError()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = [],
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);

        Assert.That(info, Is.Not.Empty);
    }

    [Test]
    public void GetInfo_ResultDoesNotContainNullEntries()
    {
        var stream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[44100],
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);

        Assert.That(info, Does.Not.Contain(null));
    }

    [Test]
    public void GetInfo_CalculatesDurationCorrectly()
    {
        // 1 second of stereo 44100 Hz 16-bit audio
        var durationInSamples = 44100;
        var sampleDataLength = durationInSamples * 2 * 2; // 2 channels, 2 bytes per sample

        var stream = new BnkStream
        {
            Channels = 2,
            SampleRate = 44100,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[sampleDataLength],
            PostAudioStreamData = []
        };

        var info = _extractorHumanSize.GetInfo(stream);
        var durationInfo = info.First(s => s.Contains("Duration"));

        // Should be approximately 1 second
        Assert.That(durationInfo, Does.Contain("00:00:01"));
    }
}
