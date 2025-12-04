#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Info.Map;

namespace TheXDS.Vivianne.Info.Map;

[TestFixture]
public class MapItemInfoExtractorTests
{
    private MapItemInfoExtractor _extractor = null!;

    [SetUp]
    public void Setup()
    {
        _extractor = new();
    }

    [Test]
    public void GetInfo_WithSimpleMapItem_ReturnsInfoArray()
    {
        var item = CreateSimpleMapItem();

        var info = _extractor.GetInfo(item);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info, Is.All.InstanceOf<string>());
    }

    [Test]
    public void GetInfo_IncludesMusStreamOffset()
    {
        var item = CreateSimpleMapItem();
        item.MusOffset = 0x12345678;

        var info = _extractor.GetInfo(item);
        var offsetInfo = info.First(s => s.Contains("MUS stream offset"));

        Assert.That(offsetInfo, Does.Contain("12345678"));
    }

    [Test]
    public void GetInfo_IncludesJumpCount()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 1, StateData = [] },
                new MapJump { NextItem = 2, StateData = [] },
                new MapJump { NextItem = 0, StateData = [] }
            }
        };

        var info = _extractor.GetInfo(item);
        var jumpInfo = info.First(s => s.Contains("Jumps"));

        Assert.That(jumpInfo, Does.Contain("3"));
    }

    [Test]
    public void GetInfo_WithNoJumps_ShowsZeroJumps()
    {
        var item = new MapItem
        {
            MusOffset = 0x1000,
            Jumps = []
        };

        var info = _extractor.GetInfo(item);
        var jumpInfo = info.First(s => s.Contains("Jumps"));

        Assert.That(jumpInfo, Does.Contain("0"));
    }

    [Test]
    public void GetInfo_DumpsEachJump()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 1, StateData = new byte[] { 0x01, 0x02 } },
                new MapJump { NextItem = 2, StateData = new byte[] { 0x03, 0x04 } }
            }
        };

        var info = _extractor.GetInfo(item);

        Assert.That(info.Any(s => s.Contains("Jump to 1")), Is.True);
        Assert.That(info.Any(s => s.Contains("Jump to 2")), Is.True);
    }

    [Test]
    public void GetInfo_IncludesJumpStateData()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 1, StateData = new byte[] { 0xAB, 0xCD, 0xEF } }
            }
        };

        var info = _extractor.GetInfo(item);

        Assert.That(info.Any(s => s.Contains("AB") || s.Contains("ab")), Is.True);
        Assert.That(info.Any(s => s.Contains("CD") || s.Contains("cd")), Is.True);
        Assert.That(info.Any(s => s.Contains("EF") || s.Contains("ef")), Is.True);
    }

    [Test]
    public void GetInfo_JumpsAreIndented()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 1, StateData = [] },
                new MapJump { NextItem = 2, StateData = [] }
            }
        };

        var info = _extractor.GetInfo(item);
        var jumpLines = info.Where(s => s.Contains("Jump to")).ToArray();

        Assert.That(jumpLines.Length, Is.GreaterThan(0));
        Assert.That(jumpLines.All(s => s.StartsWith("  ")), Is.True);
    }

    [Test]
    public void GetInfo_JumpDataIsFormatted()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 1, StateData = new byte[] { 0xFF, 0x00 } }
            }
        };

        var info = _extractor.GetInfo(item);
        var dataLines = info.Where(s => s.Contains("Data:")).ToArray();

        Assert.That(dataLines.Length, Is.GreaterThan(0));
    }

    [Test]
    public void GetInfo_WithLargeMusOffset_DisplaysCorrectly()
    {
        var item = new MapItem
        {
            MusOffset = int.MaxValue,
            Jumps = []
        };

        var info = _extractor.GetInfo(item);
        var offsetInfo = info.First(s => s.Contains("MUS stream offset"));

        Assert.That(offsetInfo, Does.Contain(int.MaxValue.ToString("X8")));
    }

    [Test]
    public void GetInfo_WithNegativeMusOffset_DisplaysCorrectly()
    {
        var item = new MapItem
        {
            MusOffset = -1,
            Jumps = []
        };

        var info = _extractor.GetInfo(item);

        Assert.That(info, Is.Not.Empty);
    }

    [Test]
    public void GetInfo_JumpWithZeroStateData_DisplaysCorrectly()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 1, StateData = [] }
            }
        };

        var info = _extractor.GetInfo(item);

        Assert.That(info.Any(s => s.Contains("Jump to 1")), Is.True);
    }

    [Test]
    public void GetInfo_WithMultipleJumpsToSameDestination_DisplaysAll()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 1, StateData = new byte[] { 0x00, 0x01 } },
                new MapJump { NextItem = 1, StateData = new byte[] { 0x02, 0x03 } }
            }
        };

        var info = _extractor.GetInfo(item);

        Assert.That(info.Count(s => s.Contains("Jump to 1")), Is.EqualTo(2));
    }

    [Test]
    public void GetInfo_ResultDoesNotContainNullEntries()
    {
        var item = CreateSimpleMapItem();

        var info = _extractor.GetInfo(item);

        Assert.That(info, Does.Not.Contain(null));
    }

    [Test]
    public void GetInfo_SeparatorLinesBetweenJumps()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 1, StateData = [] },
                new MapJump { NextItem = 2, StateData = [] }
            }
        };

        var info = _extractor.GetInfo(item);
        var separatorLines = info.Where(s => s.Contains("---")).ToArray();

        Assert.That(separatorLines.Length, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void GetInfo_MusOffsetFormatted()
    {
        var item = new MapItem
        {
            MusOffset = 0x12345678,
            Jumps = []
        };

        var info = _extractor.GetInfo(item);
        var offsetInfo = info.First(s => s.Contains("MUS stream offset"));

        Assert.That(offsetInfo, Does.Match(@"0x[0-9A-Fa-f]{8}"));
    }

    [Test]
    public void GetInfo_StateDataFormatted()
    {
        var item = new MapItem
        {
            MusOffset = 0,
            Jumps =
            {
                new MapJump { NextItem = 5, StateData = new byte[] { 0x12, 0x34, 0x56 } }
            }
        };

        var info = _extractor.GetInfo(item);
        var dataLine = info.First(s => s.Contains("Data:"));

        Assert.That(dataLine, Does.Match(@"[0-9A-Fa-f]{2}"));
    }

    [Test]
    public void GetInfo_WithManyJumps_DisplaysAllCorrectly()
    {
        var jumps = new List<MapJump>();
        for (int i = 0; i < 10; i++)
        {
            jumps.Add(new MapJump { NextItem = i, StateData = new byte[] { (byte)i } });
        }

        var item = new MapItem
        {
            MusOffset = 0,
            Jumps = jumps
        };

        var info = _extractor.GetInfo(item);

        Assert.That(info.Count(s => s.Contains("Jump to")), Is.EqualTo(10));
    }

    private static MapItem CreateSimpleMapItem() => new()
    {
        MusOffset = 0x1000,
        Jumps =
        {
            new MapJump { NextItem = 1, StateData = new byte[] { 0x00, 0x01 } }
        }
    };
}
