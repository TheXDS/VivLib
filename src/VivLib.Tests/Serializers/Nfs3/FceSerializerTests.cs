#pragma warning disable CS1591

using System.Numerics;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Models.Fce.Nfs3;
using TheXDS.Vivianne.Serializers.Fce.Nfs3;

namespace TheXDS.Vivianne.Serializers.Nfs3;

[TestFixture]
public class FceSerializerTests() : SerializerTestsBase<FceSerializer, FceFile>("Nfs3.test.fce", GetDefaultFile())
{
    private static FceFile GetDefaultFile() => new()
    {
        Magic = 0,
        Arts = 1,
        XHalfSize = 0.5f,
        YHalfSize = 0.5f,
        ZHalfSize = 0.5f,
        RsvdTable1 = new byte[256],
        RsvdTable2 = new byte[96],
        RsvdTable3 = new byte[96],
        PrimaryColors = {
            new HsbColor(0, 255, 255, 255),
            new HsbColor(64, 255, 255, 255),
            new HsbColor(128, 255, 255, 255),
            new HsbColor(192, 255, 255, 255)
        },
        SecondaryColors = {
            new HsbColor(192, 255, 255, 255),
            new HsbColor(128, 255, 255, 255),
            new HsbColor(64, 255, 255, 255),
            new HsbColor(0, 255, 255, 255)
        },
        Parts = {
            new FcePart()
            {
                Name = ":HB",
                Origin = new Vector3(0f, 0f, 0f),
                Vertices =
                [
                    new(0.5f, 0.5f, 0.5f),
                    new(0.5f, 0.5f, -0.5f),
                    new(0.5f, -0.5f, 0.5f),
                    new(0.5f, -0.5f, -0.5f),
                    new(-0.5f, 0.5f, 0.5f),
                    new(-0.5f, 0.5f, -0.5f),
                    new(-0.5f, -0.5f, 0.5f),
                    new(-0.5f, -0.5f, -0.5f)
                ],
                Normals =
                [
                    new(1.5f, 1.5f, 1.5f),
                    new(1.5f, 1.5f, -1.5f),
                    new(1.5f, -1.5f, 1.5f),
                    new(1.5f, -1.5f, -1.5f),
                    new(-1.5f, 1.5f, 1.5f),
                    new(-1.5f, 1.5f, -1.5f),
                    new(-1.5f, -1.5f, 1.5f),
                    new(-1.5f, -1.5f, -1.5f)
                ],
                Triangles =
                [
                    new()
                    {
                        TexturePage = 1,
                        I1 = 0,
                        I2 = 1,
                        I3 = 2,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 2,
                        I2 = 1,
                        I3 = 3,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 5,
                        I2 = 4,
                        I3 = 6,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 5,
                        I2 = 6,
                        I3 = 7,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 2,
                        I2 = 6,
                        I3 = 4,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 0,
                        I2 = 2,
                        I3 = 4,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 0,
                        I2 = 4,
                        I3 = 5,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 0,
                        I2 = 5,
                        I3 = 1,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 2,
                        I2 = 7,
                        I3 = 6,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 2,
                        I2 = 3,
                        I3 = 7,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 3,
                        I2 = 5,
                        I3 = 7,
                        Unk_0x10 = new byte[12],
                        U1 = 0f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 0f,
                        V2 = 1f,
                        V3 = 0f
                    },
                    new()
                    {
                        TexturePage = 1,
                        I1 = 1,
                        I2 = 5,
                        I3 = 3,
                        Unk_0x10 = new byte[12],
                        U1 = 1f,
                        U2 = 0f,
                        U3 = 1f,
                        V1 = 1f,
                        V2 = 1f,
                        V3 = 0f
                    }
                ]
            }
        },
        Dummies =
        {
            new FceDummy()
            {
                Name = "HLFO",
                Position = new Vector3(0.5f, 0f, 0f)
            },
            new FceDummy()
            {
                Name = "HFRE",
                Position = new Vector3(-0.5f, 0f, 0f)
            },
                        new FceDummy()
            {
                Name = "TRLN",
                Position = new Vector3(0f, 0f, 0.5f)
            },
            new FceDummy()
            {
                Name = "TRRN",
                Position = new Vector3(0f, 0f, -0.5f)
            }

        },
        Unk_0x1e04 = new byte[256]
    };

    protected override void TestParsedFile(FceFile expected, FceFile actual)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Magic, Is.EqualTo(expected.Magic));
            Assert.That(actual.Arts, Is.EqualTo(expected.Arts));
            Assert.That(actual.XHalfSize, Is.EqualTo(expected.XHalfSize));
            Assert.That(actual.YHalfSize, Is.EqualTo(expected.YHalfSize));
            Assert.That(actual.ZHalfSize, Is.EqualTo(expected.ZHalfSize));
            Assert.That(actual.RsvdTable1, Is.EquivalentTo(expected.RsvdTable1));
            Assert.That(actual.RsvdTable2, Is.EquivalentTo(expected.RsvdTable2));
            Assert.That(actual.RsvdTable3, Is.EquivalentTo(expected.RsvdTable3));
            Assert.That(actual.PrimaryColors, Is.EquivalentTo(expected.PrimaryColors));
            Assert.That(actual.SecondaryColors, Is.EquivalentTo(expected.SecondaryColors));
            Assert.That(actual.Unk_0x1e04, Is.EqualTo(expected.Unk_0x1e04));
        }
        var expectedPart = expected.GetPart(":HB")!;
        var actualPart = actual.GetPart(":HB")!;
        Assert.That(actualPart, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actualPart.Name, Is.EqualTo(expectedPart.Name));
            Assert.That(actualPart.Origin, Is.EqualTo(expectedPart.Origin));
            Assert.That(actualPart.Vertices, Is.EquivalentTo(expectedPart.Vertices));
            Assert.That(actualPart.Normals, Is.EquivalentTo(expectedPart.Normals));
        }
        foreach ((var tActual, var tExpected) in actualPart.Triangles.Zip(expectedPart.Triangles))
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(tActual.TexturePage, Is.EqualTo(tExpected.TexturePage));
                Assert.That(tActual.I1, Is.EqualTo(tExpected.I1));
                Assert.That(tActual.I2, Is.EqualTo(tExpected.I2));
                Assert.That(tActual.I3, Is.EqualTo(tExpected.I3));
                Assert.That(tActual.Unk_0x10, Is.EquivalentTo(tExpected.Unk_0x10));
                Assert.That(tActual.Flags, Is.EqualTo(tExpected.Flags));
                Assert.That(tActual.U1, Is.EqualTo(tExpected.U1));
                Assert.That(tActual.U2, Is.EqualTo(tExpected.U2));
                Assert.That(tActual.U3, Is.EqualTo(tExpected.U3));
                Assert.That(tActual.V1, Is.EqualTo(tExpected.V1));
                Assert.That(tActual.V2, Is.EqualTo(tExpected.V2));
                Assert.That(tActual.V3, Is.EqualTo(tExpected.V3));
                Assert.That(tActual.Uv1, Is.EqualTo(tExpected.Uv1));
                Assert.That(tActual.Uv2, Is.EqualTo(tExpected.Uv2));
                Assert.That(tActual.Uv3, Is.EqualTo(tExpected.Uv3));
            }
        }
        foreach (var j in (IEnumerable<string>)["HLFO", "HFRE", "TRLN", "TRRN"])
        {
            var expectedDummy = expected.GetDummy(j)!;
            var actualDummy = actual.GetDummy(j)!;
            Assert.That(actualDummy, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(actualDummy.Name, Is.EqualTo(expectedDummy.Name));
                Assert.That(actualDummy.Position, Is.EqualTo(expectedDummy.Position));
            }
        }
    }
}
