#pragma warning disable CS1591

using System.Numerics;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Info.Fce;

namespace TheXDS.Vivianne.Info.Fce;

[TestFixture]
public class FcePartInfoExtractorTests
{
    private FcePartInfoExtractor _extractor = null!;

    [SetUp]
    public void Setup()
    {
        _extractor = new();
    }

    [Test]
    public void GetInfo_WithSimpleFcePart_ReturnsInfoArray()
    {
        var part = CreateSimpleFcePart();

        var info = _extractor.GetInfo(part);

        Assert.That(info, Is.Not.Empty);
        Assert.That(info, Is.All.InstanceOf<string>());
        Assert.That(info, Is.All.Not.Empty);
    }

    [Test]
    public void GetInfo_IncludesPartName()
    {
        var part = CreateSimpleFcePart();
        part.Name = "TestPart";

        var info = _extractor.GetInfo(part);
        var nameInfo = info.First(s => s.Contains("Name"));

        Assert.That(nameInfo, Does.Contain("TestPart"));
    }

    [Test]
    public void GetInfo_IncludesOrigin()
    {
        var part = CreateSimpleFcePart();
        part.Origin = new Vector3(10, 20, 30);

        var info = _extractor.GetInfo(part);
        var originInfo = info.First(s => s.Contains("Origin"));

        Assert.That(originInfo, Does.Contain("10"));
        Assert.That(originInfo, Does.Contain("20"));
        Assert.That(originInfo, Does.Contain("30"));
    }

    [Test]
    public void GetInfo_IncludesVertexCount()
    {
        var part = CreateSimpleFcePart();
        part.Vertices = new Vector3[5];

        var info = _extractor.GetInfo(part);
        var vertexInfo = info.First(s => s.Contains("Vertices"));

        Assert.That(vertexInfo, Does.Contain("5"));
    }

    [Test]
    public void GetInfo_IncludesTriangleCount()
    {
        var part = CreateSimpleFcePart();
        part.Triangles = new FceTriangle[10];

        var info = _extractor.GetInfo(part);
        var triangleInfo = info.First(s => s.Contains("Triangles"));

        Assert.That(triangleInfo, Does.Contain("10"));
    }

    [Test]
    public void GetInfo_WithZeroVertices_ReturnsCorrectCount()
    {
        var part = CreateSimpleFcePart();
        part.Vertices = [];

        var info = _extractor.GetInfo(part);
        var vertexInfo = info.First(s => s.Contains("Vertices"));

        Assert.That(vertexInfo, Does.Contain("0"));
    }

    [Test]
    public void GetInfo_WithZeroTriangles_ReturnsCorrectCount()
    {
        var part = CreateSimpleFcePart();
        part.Triangles = [];

        var info = _extractor.GetInfo(part);
        var triangleInfo = info.First(s => s.Contains("Triangles"));

        Assert.That(triangleInfo, Does.Contain("0"));
    }

    [Test]
    public void GetInfo_WithLargeVertexCount_ReturnsCorrectCount()
    {
        var part = CreateSimpleFcePart();
        part.Vertices = new Vector3[1000];

        var info = _extractor.GetInfo(part);
        var vertexInfo = info.First(s => s.Contains("Vertices"));

        Assert.That(vertexInfo, Does.Contain("1000"));
    }

    [Test]
    public void GetInfo_WithLargeTriangleCount_ReturnsCorrectCount()
    {
        var part = CreateSimpleFcePart();
        part.Triangles = new FceTriangle[5000];

        var info = _extractor.GetInfo(part);
        var triangleInfo = info.First(s => s.Contains("Triangles"));

        Assert.That(triangleInfo, Does.Contain("5000"));
    }

    [Test]
    public void GetInfo_WithNegativeOriginCoordinates_DisplaysCorrectly()
    {
        var part = CreateSimpleFcePart();
        part.Origin = new Vector3(-10.5f, -20.5f, -30.5f);

        var info = _extractor.GetInfo(part);
        var originInfo = info.First(s => s.Contains("Origin"));

        Assert.That(originInfo, Does.Contain("-10"));
        Assert.That(originInfo, Does.Contain("-20"));
        Assert.That(originInfo, Does.Contain("-30"));
    }

    [Test]
    public void GetInfo_WithZeroOrigin_DisplaysZeroCoordinates()
    {
        var part = CreateSimpleFcePart();
        part.Origin = Vector3.Zero;

        var info = _extractor.GetInfo(part);
        var originInfo = info.First(s => s.Contains("Origin"));

        Assert.That(originInfo, Does.Contain("0"));
    }

    [Test]
    public void GetInfo_WithDifferentNames_DisplaysCorrectName()
    {
        var names = new string[] { "Part1", "Body", "Wheel_FL", ":HB", ":TB" };

        foreach (var name in names)
        {
            var part = CreateSimpleFcePart();
            part.Name = name;

            var info = _extractor.GetInfo(part);
            var nameInfo = info.First(s => s.Contains("Name"));

            Assert.That(nameInfo, Does.Contain(name));
        }
    }

    [Test]
    public void GetInfo_ResultDoesNotContainNullEntries()
    {
        var part = CreateSimpleFcePart();

        var info = _extractor.GetInfo(part);

        Assert.That(info, Does.Not.Contain(null));
    }

    [Test]
    public void GetInfo_ReturnsExactlyFourElements()
    {
        var part = CreateSimpleFcePart();

        var info = _extractor.GetInfo(part);

        Assert.That(info, Has.Length.EqualTo(4));
    }

    [Test]
    public void GetInfo_AlwaysIncludesAllFourTypes()
    {
        var part = CreateSimpleFcePart();

        var info = _extractor.GetInfo(part);

        Assert.That(info.Any(s => s.Contains("Name")), Is.True);
        Assert.That(info.Any(s => s.Contains("Origin")), Is.True);
        Assert.That(info.Any(s => s.Contains("Vertices")), Is.True);
        Assert.That(info.Any(s => s.Contains("Triangles")), Is.True);
    }

    [Test]
    public void GetInfo_WithFloatOriginValues_DisplaysNumbers()
    {
        var part = CreateSimpleFcePart();
        part.Origin = new Vector3(1.5f, 2.7f, 3.9f);

        var info = _extractor.GetInfo(part);
        var originInfo = info.First(s => s.Contains("Origin"));

        // Should contain the x, y, z values (truncated or rounded)
        Assert.That(originInfo, Does.Match(@"\d+"));
    }

    private static FcePart CreateSimpleFcePart() => new()
    {
        Name = "DefaultPart",
        Origin = Vector3.Zero,
        Vertices = new Vector3[] { Vector3.Zero, Vector3.One },
        Normals = new Vector3[] { Vector3.UnitZ, Vector3.UnitZ },
        Triangles = [new FceTriangle()]
    };
}
