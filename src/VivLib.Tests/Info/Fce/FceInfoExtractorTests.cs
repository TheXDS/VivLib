#pragma warning disable CS1591

using System.Numerics;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Info.Fce;

namespace TheXDS.Vivianne.Info.Fce;

[TestFixture]
public class FceInfoExtractorTests
{
    private FceInfoExtractor<FcePart> _extractorWithReservedHumanSize = null!;
    private FceInfoExtractor<FcePart> _extractorWithoutReservedHumanSize = null!;
    private FceInfoExtractor<FcePart> _extractorWithReservedByteSize = null!;
    private FceInfoExtractor<FcePart> _extractorWithoutReservedByteSize = null!;

    [SetUp]
    public void Setup()
    {
        _extractorWithReservedHumanSize = new(humanSize: true, showRsvdContents: true);
        _extractorWithoutReservedHumanSize = new(humanSize: true, showRsvdContents: false);
        _extractorWithReservedByteSize = new(humanSize: false, showRsvdContents: true);
        _extractorWithoutReservedByteSize = new(humanSize: false, showRsvdContents: false);
    }

    [Test]
    public void GetInfo_WithSimpleFceFile_ReturnsInfoArray()
    {
        var fceFile = CreateSimpleFceFile();

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info, Is.All.InstanceOf<string>());
    }

    [Test]
    public void GetInfo_IncludesFileSignature()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.Magic = 0x00101014;

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
        var signatureInfo = info.First(s => s.Contains("File signature"));

        Assert.That(signatureInfo, Does.Contain("0x"));
    }

    [Test]
    public void GetInfo_IncludesFileFormat()
    {
        var fceFile = CreateSimpleFceFile();

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);

        Assert.That(info.Any(s => s.Contains("File format")), Is.True);
    }

    [Test]
    public void GetInfo_IncludesNumberOfArts()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.Arts = 3;

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
        var artsInfo = info.First(s => s.Contains("Number of arts"));

        Assert.That(artsInfo, Does.Contain("3"));
    }

    [Test]
    public void GetInfo_IncludesBoundingBoxSize()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.XHalfSize = 100;
        fceFile.YHalfSize = 200;
        fceFile.ZHalfSize = 150;

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
        var bboxInfo = info.First(s => s.Contains("Bounding box size"));

        Assert.That(bboxInfo, Does.Contain("200")); // XHalfSize * 2
        Assert.That(bboxInfo, Does.Contain("400")); // YHalfSize * 2
        Assert.That(bboxInfo, Does.Contain("300")); // ZHalfSize * 2
    }

    [Test]
    public void GetInfo_IncludesReservedTableSizes_WhenShowReservedIsFalse()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.RsvdTable1 = new byte[1000];
        fceFile.RsvdTable2 = new byte[2000];
        fceFile.RsvdTable3 = new byte[1500];

        var info = _extractorWithoutReservedByteSize.GetInfo(fceFile);
        var rsvd1Info = info.First(s => s.Contains("Reserved table 1"));

        Assert.That(rsvd1Info, Does.Contain("1000"));
    }

    [Test]
    public void GetInfo_IncludesReservedTableContents_WhenShowReservedIsTrue()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.RsvdTable1 = new byte[] { 0xFF, 0xAA, 0xBB };

        var info = _extractorWithReservedByteSize.GetInfo(fceFile);

        Assert.That(info.Any(s => s.Contains("Reserved table 1 contents")), Is.True);
        Assert.That(info.Any(s => s.Contains("FF")), Is.True);
    }

    [Test]
    public void GetInfo_IncludesColorCount()
    {
        var fceFile = CreateSimpleFceFile();

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
        var colorInfo = info.First(s => s.Contains("Declared colors"));

        Assert.That(colorInfo, Does.Contain("5"));
    }

    [Test]
    public void GetInfo_IncludesPartCount()
    {
        var fceFile = CreateSimpleFceFile();

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
        var partInfo = info.First(s => s.Contains("Parts"));

        Assert.That(partInfo, Does.Contain("2"));
    }

    [Test]
    public void GetInfo_IncludesDummyCount()
    {
        var fceFile = CreateSimpleFceFile();

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
        var dummyInfo = info.First(s => s.Contains("Dummies"));

        Assert.That(dummyInfo, Does.Contain("1"));
    }

    [Test]
    public void GetInfo_WithHumanSizeTrue_ReturnsSizeInHumanReadableFormat()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.RsvdTable1 = new byte[1048576]; // 1 MB

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
        var rsvd1Info = info.First(s => s.Contains("Reserved table 1"));

        Assert.That(rsvd1Info, Does.Match(@"\d+(\.\d+)?\s*[KMGT]?iB"));
    }

    [Test]
    public void GetInfo_WithHumanSizeFalse_ReturnsSizeInBytes()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.RsvdTable1 = new byte[5000];

        var info = _extractorWithoutReservedByteSize.GetInfo(fceFile);
        var rsvd1Info = info.First(s => s.Contains("Reserved table 1"));

        Assert.That(rsvd1Info, Does.Contain("5000"));
    }

    [Test]
    public void GetInfo_WithMultipleDummies_IncludesCorrectCount()
    {
        var fceFile = new FceFile
        {
            Magic = 0x00101003,
            Arts = 1,
            XHalfSize = 100,
            YHalfSize = 100,
            ZHalfSize = 100,
            Dummies =
            {
                new FceDummy { Name = "Dummy1", Position = Vector3.Zero },
                new FceDummy { Name = "Dummy2", Position = Vector3.One },
                new FceDummy { Name = "Dummy3", Position = new Vector3(10, 20, 30) }
            },
            PrimaryColors = { new HsbColor(0, 0, 0, 255), new HsbColor(0, 0, 100, 255) },
            SecondaryColors = { new HsbColor(0, 0, 50, 255), new HsbColor(0, 0, 100, 255) },
            Parts = { CreateSimpleFcePart("Part1"), CreateSimpleFcePart("Part2") },
            RsvdTable1 = [],
            RsvdTable2 = [],
            RsvdTable3 = [],
            Unk_0x1e04 = []
        };

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
        var dummyInfo = info.First(s => s.Contains("Dummies"));

        Assert.That(dummyInfo, Does.Contain("3"));
    }

    [Test]
    public void GetInfo_WithLargeReservedTables_ReturnsInfoCorrectly()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.RsvdTable1 = new byte[10000];
        fceFile.RsvdTable2 = new byte[20000];
        fceFile.RsvdTable3 = new byte[15000];

        var info = _extractorWithoutReservedByteSize.GetInfo(fceFile);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info.Any(s => s.Contains("Reserved table 1")), Is.True);
        Assert.That(info.Any(s => s.Contains("Reserved table 2")), Is.True);
        Assert.That(info.Any(s => s.Contains("Reserved table 3")), Is.True);
    }

    [Test]
    public void GetInfo_ReservedTableContents_FormattedCorrectly()
    {
        var fceFile = CreateSimpleFceFile();
        fceFile.RsvdTable1 = Enumerable.Range(0, 100).Select(i => (byte)(i % 256)).ToArray();

        var info = _extractorWithReservedByteSize.GetInfo(fceFile);
        var hexContents = info.Where(s => s.Contains("00") || s.Contains("01") || s.Contains("FF")).ToArray();

        Assert.That(hexContents.Length, Is.GreaterThan(0));
    }

    [Test]
    public void GetInfo_ResultDoesNotContainNullEntries()
    {
        var fceFile = CreateSimpleFceFile();

        var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);

        Assert.That(info, Does.Not.Contain(null));
    }

    [Test]
    public void GetInfo_WithDifferentMagicValues_IncludesCorrectFormat()
    {
        var magicValues = new int[] { 0x00101014, 0x00101003 };

        foreach (var magic in magicValues)
        {
            var fceFile = CreateSimpleFceFile();
            fceFile.Magic = magic;

            var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
            var formatInfo = info.First(s => s.Contains("File format"));

            Assert.That(formatInfo, Is.Not.Empty);
        }
    }

    [Test]
    public void GetInfo_WithVariousArtsCount_IncludesCorrectCount()
    {
        var artCounts = new int[] { 1, 2, 5, 10 };

        foreach (var count in artCounts)
        {
            var fceFile = CreateSimpleFceFile();
            fceFile.Arts = count;

            var info = _extractorWithoutReservedHumanSize.GetInfo(fceFile);
            var artsInfo = info.First(s => s.Contains("Number of arts"));

            Assert.That(artsInfo, Does.Contain(count.ToString()));
        }
    }

    private static FceFile CreateSimpleFceFile() => new()
    {
        Magic = 0x00101003,
        Arts = 1,
        XHalfSize = 100,
        YHalfSize = 100,
        ZHalfSize = 100,
        Dummies =
        {
            new FceDummy { Name = "Dummy1", Position = Vector3.Zero }
        },
        PrimaryColors =
        {
            new HsbColor(0, 0, 0, 255),
            new HsbColor(0, 0, 50, 255),
            new HsbColor(0, 0, 100, 255),
            new HsbColor(120, 50, 50, 255),
            new HsbColor(240, 50, 50, 255)
        },
        SecondaryColors =
        {
            new HsbColor(0, 0, 25, 255),
            new HsbColor(0, 0, 75, 255),
            new HsbColor(0, 0, 100, 255),
            new HsbColor(120, 50, 25, 255),
            new HsbColor(240, 50, 75, 255)
        },
        Parts =
        {
            CreateSimpleFcePart("Part1"),
            CreateSimpleFcePart("Part2")
        },
        RsvdTable1 = [],
        RsvdTable2 = [],
        RsvdTable3 = [],
        Unk_0x1e04 = []
    };

    private static FcePart CreateSimpleFcePart(string name) => new()
    {
        Name = name,
        Origin = Vector3.Zero,
        Vertices = new Vector3[] { Vector3.Zero, Vector3.One, new Vector3(1, 0, 1) },
        Normals = new Vector3[] { Vector3.UnitZ, Vector3.UnitZ, Vector3.UnitZ },
        Triangles = [new FceTriangle { TexturePage = 0, I1 = 0, I2 = 1, I3 = 2, U1 = 0, U2 = 1, U3 = 0, V1 = 0, V2 = 0, V3 = 1 }]
    };
}
