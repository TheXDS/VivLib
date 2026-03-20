#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Info.Audio;

namespace TheXDS.Vivianne.Info.Bnk;

[TestFixture]
public class BnkFileInfoExtractorTests
{
    private BnkFileInfoExtractor _extractorHumanSize = null!;
    private BnkFileInfoExtractor _extractorByteSize = null!;

    [SetUp]
    public void Setup()
    {
        _extractorHumanSize = new(humanSize: true);
        _extractorByteSize = new(humanSize: false);
    }

    [Test]
    public void GetInfo_WithSimpleBnkFile_ReturnsInfoArray()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 8820,
            Streams =
            {
                new BnkStream
                {
                    Channels = 2,
                    SampleRate = 44100,
                    BytesPerSample = 2,
                    Compression = CompressionMethod.None,
                    SampleData = new byte[8820],
                    PostAudioStreamData = []
                }
            }
        };

        var info = _extractorHumanSize.GetInfo(bnkFile);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info, Is.All.InstanceOf<string>());
        Assert.That(info.Any(s => s.Contains("BNK format version")), Is.True);
        Assert.That(info.Any(s => s.Contains("Declared streams")), Is.True);
    }

    [Test]
    public void GetInfo_IncludesFileVersion()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 4,
            PayloadSize = 1000,
            Streams = { }
        };

        var info = _extractorHumanSize.GetInfo(bnkFile);
        var versionInfo = info.First(s => s.Contains("BNK format version"));

        Assert.That(versionInfo, Does.Contain("4"));
    }

    [Test]
    public void GetInfo_IncludesDeclaredStreamCount()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 1000,
            Streams =
            {
                null,
                new BnkStream { Channels = 1, SampleRate = 22050, BytesPerSample = 2, Compression = CompressionMethod.None, SampleData = [], PostAudioStreamData = [] },
                null,
                new BnkStream { Channels = 1, SampleRate = 22050, BytesPerSample = 2, Compression = CompressionMethod.None, SampleData = [], PostAudioStreamData = [] }
            }
        };

        var info = _extractorHumanSize.GetInfo(bnkFile);
        var streamInfo = info.First(s => s.Contains("Declared streams"));

        Assert.That(streamInfo, Does.Contain("4"));
    }

    [Test]
    public void GetInfo_IncludesStreamsWithPTHeaders()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 8820,
            Streams =
            {
                new BnkStream { Channels = 1, SampleRate = 22050, BytesPerSample = 2, Compression = CompressionMethod.None, SampleData = [], PostAudioStreamData = [] },
                null,
                new BnkStream { Channels = 1, SampleRate = 22050, BytesPerSample = 2, Compression = CompressionMethod.None, SampleData = [], PostAudioStreamData = [] }
            }
        };

        var info = _extractorHumanSize.GetInfo(bnkFile);
        var ptHeaderInfo = info.First(s => s.Contains("Streams with PT headers"));

        Assert.That(ptHeaderInfo, Does.Contain("2"));
    }

    [Test]
    public void GetInfo_IncludesUsableAudioPayload()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 20000,
            Streams =
            {
                new BnkStream
                {
                    Channels = 1,
                    SampleRate = 22050,
                    BytesPerSample = 2,
                    Compression = CompressionMethod.None,
                    SampleData = new byte[8820],
                    PostAudioStreamData = [],
                    AltStream = null
                },
                new BnkStream
                {
                    Channels = 1,
                    SampleRate = 22050,
                    BytesPerSample = 2,
                    Compression = CompressionMethod.None,
                    SampleData = new byte[4410],
                    PostAudioStreamData = [],
                    AltStream = null
                }
            }
        };

        var info = _extractorByteSize.GetInfo(bnkFile);
        var usableInfo = info.First(s => s.Contains("Usable audio payload"));

        Assert.That(usableInfo, Does.Contain("13230"));
    }

    [Test]
    public void GetInfo_IncludesTotalPayloadSize()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 15000,
            Streams = { }
        };

        var info = _extractorByteSize.GetInfo(bnkFile);
        var totalInfo = info.First(s => s.Contains("Total payload size"));

        Assert.That(totalInfo, Does.Contain("15000"));
    }

    [Test]
    public void GetInfo_WithHumanSizeTrue_ReturnsSizeInHumanReadableFormat()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 1048576, // 1 MB
            Streams = { }
        };

        var info = _extractorHumanSize.GetInfo(bnkFile);
        var totalInfo = info.First(s => s.Contains("Total payload size"));

        Assert.That(totalInfo, Does.Match(@"\d+(\.\d+)?\s*[KMGT]?iB"));
    }

    [Test]
    public void GetInfo_WithHumanSizeFalse_ReturnsSizeInBytes()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 5000,
            Streams = { }
        };

        var info = _extractorByteSize.GetInfo(bnkFile);
        var totalInfo = info.First(s => s.Contains("Total payload size"));

        Assert.That(totalInfo, Does.Contain("5000"));
    }

    [Test]
    public void GetInfo_WithAltStreams_IncludesAltStreamDataInPayload()
    {
        var mainStream = new BnkStream
        {
            Channels = 1,
            SampleRate = 22050,
            BytesPerSample = 2,
            Compression = CompressionMethod.None,
            SampleData = new byte[8820],
            PostAudioStreamData = [],
            AltStream = new BnkStream
            {
                Channels = 1,
                SampleRate = 22050,
                BytesPerSample = 2,
                Compression = CompressionMethod.None,
                SampleData = new byte[4410],
                PostAudioStreamData = [],
                IsAltStream = true
            }
        };

        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 20000,
            Streams = { mainStream }
        };

        var info = _extractorByteSize.GetInfo(bnkFile);
        var usableInfo = info.First(s => s.Contains("Usable audio payload"));

        // Should include both main (8820) and alt (4410) stream data
        Assert.That(usableInfo, Does.Contain("13230"));
    }

    [Test]
    public void GetInfo_WithNullStreams_SkipsNullsInCalculations()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 10000,
            Streams =
            {
                new BnkStream { Channels = 1, SampleRate = 22050, BytesPerSample = 2, Compression = CompressionMethod.None, SampleData = new byte[5000], PostAudioStreamData = [] },
                null,
                new BnkStream { Channels = 1, SampleRate = 22050, BytesPerSample = 2, Compression = CompressionMethod.None, SampleData = new byte[5000], PostAudioStreamData = [] },
                null
            }
        };

        var info = _extractorHumanSize.GetInfo(bnkFile);

        // Should not throw and should calculate correctly
        Assert.That(info, Is.Not.Empty);
        Assert.That(info.Any(s => s.Contains("Streams with PT headers: 2")), Is.True);
    }

    [Test]
    public void GetInfo_WithMultipleDifferentVersions_ReturnsCorrectVersion()
    {
        var versions = new short[] { 2, 4 };

        foreach (var version in versions)
        {
            var bnkFile = new BnkFile
            {
                FileVersion = version,
                PayloadSize = 1000,
                Streams = { }
            };

            var info = _extractorHumanSize.GetInfo(bnkFile);
            var versionInfo = info.First(s => s.Contains("BNK format version"));

            Assert.That(versionInfo, Does.Contain(version.ToString()));
        }
    }

    [Test]
    public void GetInfo_WithLargePayloadSize_HandlesCorrectly()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = int.MaxValue,
            Streams = { }
        };

        var info = _extractorHumanSize.GetInfo(bnkFile);

        Assert.That(info, Is.Not.Empty);
    }

    [Test]
    public void GetInfo_ResultDoesNotContainNullEntries()
    {
        var bnkFile = new BnkFile
        {
            FileVersion = 2,
            PayloadSize = 1000,
            Streams = { }
        };

        var info = _extractorHumanSize.GetInfo(bnkFile);

        Assert.That(info, Does.Not.Contain(null));
    }
}
