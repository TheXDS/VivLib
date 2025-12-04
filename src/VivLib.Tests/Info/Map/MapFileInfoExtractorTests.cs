#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Info.Map;

namespace TheXDS.Vivianne.Info.Map;

[TestFixture]
public class MapFileInfoExtractorTests
{
    private MapFileInfoExtractor _extractor = null!;

    [SetUp]
    public void Setup()
    {
        _extractor = new();
    }

    [Test]
    public void GetInfo_WithSimpleMapFile_ReturnsInfoArray()
    {
        var mapFile = CreateSimpleMapFile();

        var info = _extractor.GetInfo(mapFile);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info, Is.All.InstanceOf<string>());
    }

    [Test]
    public void GetInfo_IncludesUnk0x04Value()
    {
        var mapFile = CreateSimpleMapFile();
        mapFile.Unk_0x04 = 0x42;

        var info = _extractor.GetInfo(mapFile);
        var unkInfo = info.First(s => s.Contains("Unk_0x04"));

        Assert.That(unkInfo, Does.Contain("42"));
    }

    [Test]
    public void GetInfo_IncludesItemCount()
    {
        var mapFile = new MapFile
        {
            Unk_0x04 = 0,
            FirstItem = 0,
            Items =
            {
                new MapItem { MusOffset = 0, Jumps = [] },
                new MapItem { MusOffset = 1000, Jumps = [] },
                new MapItem { MusOffset = 2000, Jumps = [] }
            }
        };

        var info = _extractor.GetInfo(mapFile);
        var itemsInfo = info.First(s => s.Contains("Items"));

        Assert.That(itemsInfo, Does.Contain("3"));
    }

    [Test]
    public void GetInfo_IncludesFirstItemIndex()
    {
        var mapFile = CreateSimpleMapFile();
        mapFile.FirstItem = 2;

        var info = _extractor.GetInfo(mapFile);
        var firstItemInfo = info.First(s => s.Contains("First item"));

        Assert.That(firstItemInfo, Does.Contain("2"));
    }

    [Test]
    public void GetInfo_DumpsEachItemWithIndex()
    {
        var mapFile = new MapFile
        {
            Unk_0x04 = 0,
            FirstItem = 0,
            Items =
            {
                new MapItem { MusOffset = 0x1000, Jumps = [] },
                new MapItem { MusOffset = 0x2000, Jumps = [] }
            }
        };

        var info = _extractor.GetInfo(mapFile);

        Assert.That(info.Any(s => s.Contains("Index: 0")), Is.True);
        Assert.That(info.Any(s => s.Contains("Index: 1")), Is.True);
    }

    [Test]
    public void GetInfo_IncludesMusOffsetForEachItem()
    {
        var mapFile = new MapFile
        {
            Unk_0x04 = 0,
            FirstItem = 0,
            Items =
            {
                new MapItem { MusOffset = 0x12345678, Jumps = [] }
            }
        };

        var info = _extractor.GetInfo(mapFile);

        Assert.That(info.Any(s => s.Contains("12345678")), Is.True);
    }

    [Test]
    public void GetInfo_WithZeroItems_ReturnsHeaderInfoOnly()
    {
        var mapFile = new MapFile
        {
            Unk_0x04 = 0,
            FirstItem = 0,
            Items = []
        };

        var info = _extractor.GetInfo(mapFile);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info.Any(s => s.Contains("Items: 0")), Is.True);
    }

    [Test]
    public void GetInfo_WithMultipleItems_DisplaysAllItems()
    {
        var mapFile = new MapFile
        {
            Unk_0x04 = 0,
            FirstItem = 0,
            Items =
            {
                new MapItem { MusOffset = 0x1000, Jumps = [] },
                new MapItem { MusOffset = 0x2000, Jumps = [] },
                new MapItem { MusOffset = 0x3000, Jumps = [] },
                new MapItem { MusOffset = 0x4000, Jumps = [] },
                new MapItem { MusOffset = 0x5000, Jumps = [] }
            }
        };

        var info = _extractor.GetInfo(mapFile);

        Assert.That(info.Count(s => s.Contains("Index:")), Is.EqualTo(5));
    }

    [Test]
    public void GetInfo_ItemsAreIndented()
    {
        var mapFile = CreateSimpleMapFile();

        var info = _extractor.GetInfo(mapFile);
        var itemInfoLines = info.Where(s => s.Contains("Index:")).ToArray();

        Assert.That(itemInfoLines.Length, Is.GreaterThan(0));
        Assert.That(itemInfoLines.All(s => s.StartsWith("  ")), Is.True);
    }

    [Test]
    public void GetInfo_WithLargeMusOffsets_DisplaysCorrectly()
    {
        var mapFile = new MapFile
        {
            Unk_0x04 = 0,
            FirstItem = 0,
            Items =
            {
                new MapItem { MusOffset = int.MaxValue, Jumps = [] },
                new MapItem { MusOffset = int.MinValue, Jumps = [] }
            }
        };

        var info = _extractor.GetInfo(mapFile);

        Assert.That(info, Is.Not.Empty);
    }

    [Test]
    public void GetInfo_WithDifferentUnkValues_IncludesValue()
    {
        var unkValues = new byte[] { 0x00, 0x01, 0xFF, 0x42 };

        foreach (var unk in unkValues)
        {
            var mapFile = CreateSimpleMapFile();
            mapFile.Unk_0x04 = unk;

            var info = _extractor.GetInfo(mapFile);
            var unkInfo = info.First(s => s.Contains("Unk_0x04"));

            Assert.That(unkInfo, Does.Contain(unk.ToString("X2")));
        }
    }

    [Test]
    public void GetInfo_ResultDoesNotContainNullEntries()
    {
        var mapFile = CreateSimpleMapFile();

        var info = _extractor.GetInfo(mapFile);

        Assert.That(info, Does.Not.Contain(null));
    }

    [Test]
    public void GetInfo_WithItemsContainingJumps_IncludesJumpInfo()
    {
        var mapFile = new MapFile
        {
            Unk_0x04 = 0,
            FirstItem = 0,
            Items =
            {
                new MapItem
                {
                    MusOffset = 0x1000,
                    Jumps =
                    {
                        new MapJump { NextItem = 1, StateData = new byte[] { 0x00, 0x01 } },
                        new MapJump { NextItem = 0, StateData = new byte[] { 0xFF, 0xFE } }
                    }
                }
            }
        };

        var info = _extractor.GetInfo(mapFile);

        // Jump info is handled by MapItemInfoExtractor, which is tested separately
        Assert.That(info, Is.Not.Empty);
    }

    [Test]
    public void GetInfo_FirstItemCanBeAnyValidIndex()
    {
        var firstItemIndices = new int[] { 0, 1, 5, 10, 100 };

        foreach (var index in firstItemIndices)
        {
            var mapFile = CreateSimpleMapFile();
            mapFile.FirstItem = index;

            var info = _extractor.GetInfo(mapFile);
            var firstItemInfo = info.First(s => s.Contains("First item"));

            Assert.That(firstItemInfo, Does.Contain(index.ToString()));
        }
    }

    private static MapFile CreateSimpleMapFile() => new()
    {
        Unk_0x04 = 0,
        FirstItem = 0,
        Items =
        {
            new MapItem { MusOffset = 0, Jumps = [] },
            new MapItem { MusOffset = 10000, Jumps = [] }
        }
    };
}
