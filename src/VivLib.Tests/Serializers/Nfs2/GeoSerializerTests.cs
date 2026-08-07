using TheXDS.Vivianne.Models.Geo;
using TheXDS.Vivianne.Serializers.Geo;

namespace TheXDS.Vivianne.Serializers.Nfs2;

[TestFixture, Ignore("Test is being reworked alongside proper (de)serializer.")]
internal class GeoSerializerTests() : SerializerTestsBase<GeoSerializer, GeoFile>("Nfs2.test.geo", GetDefaultFile())
{
    private static GeoFile GetDefaultFile() => new()
    {
        MagicNumber = 0x0,
        Unk_0x04 = [.. Enumerable.Range(0, 32)],
        Unk_0x84 = 0x0feeddccbbaa9988,
        Parts =
        [
            new GeoPart()
            {
                Faces =
                [
                    new GeoFace()
                    {
                        TextureName = "TEST",
                        MaterialFlags = GeoMaterialFlags.Default,
                        Vertex1 = 0,
                        Vertex2 = 1,
                        Vertex3 = 2,
                        Vertex4 = 3,
                    },
                ],
                Unk_0x14 = 0x14,
                Unk_0x18 = 0x18,
                Unk_0x1C = 0x0,
                Unk_0x24 = 0x1,
                Unk_0x2C = 0x1,
                Origin = new(){ X = 1, Y = 2, Z = 3 },
                Vertices =
                [
                    new(0.5f, 0.5f, 0.5f),
                    new(0.5f, 0.5f, -0.5f),
                    new(0.5f, -0.5f, 0.5f),
                    new(0.5f, -0.5f, -0.5f),
                    new(0f, 0f, 0f),
                ]
            }
        ]
    };

    protected override void TestParsedFile(GeoFile expected, GeoFile actual)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.MagicNumber, Is.EqualTo(expected.MagicNumber));
            Assert.That(actual.Unk_0x04, Is.EquivalentTo(expected.Unk_0x04));
            Assert.That(actual.Unk_0x84, Is.EqualTo(expected.Unk_0x84));
            Assert.That(actual.Parts[0]!.Faces[0].TextureName, Is.EqualTo(expected.Parts[0]!.Faces[0].TextureName));
            Assert.That(actual.Parts[0]!.Faces[0].MaterialFlags, Is.EqualTo(expected.Parts[0]!.Faces[0].MaterialFlags));
            Assert.That(actual.Parts[0]!.Faces[0].Vertex1, Is.EqualTo(expected.Parts[0]!.Faces[0].Vertex1));
            Assert.That(actual.Parts[0]!.Faces[0].Vertex2, Is.EqualTo(expected.Parts[0]!.Faces[0].Vertex2));
            Assert.That(actual.Parts[0]!.Faces[0].Vertex3, Is.EqualTo(expected.Parts[0]!.Faces[0].Vertex3));
            Assert.That(actual.Parts[0]!.Faces[0].Vertex4, Is.EqualTo(expected.Parts[0]!.Faces[0].Vertex4));
            Assert.That(actual.Parts[0]!.Unk_0x14, Is.EqualTo(expected.Parts[0]!.Unk_0x14));
            Assert.That(actual.Parts[0]!.Unk_0x18, Is.EqualTo(expected.Parts[0]!.Unk_0x18));
            Assert.That(actual.Parts[0]!.Unk_0x1C, Is.EqualTo(expected.Parts[0]!.Unk_0x1C));
            Assert.That(actual.Parts[0]!.Unk_0x24, Is.EqualTo(expected.Parts[0]!.Unk_0x24));
            Assert.That(actual.Parts[0]!.Unk_0x2C, Is.EqualTo(expected.Parts[0]!.Unk_0x2C));
            Assert.That(actual.Parts[0]!.Origin, Is.EqualTo(expected.Parts[0]!.Origin));
            Assert.That(actual.Parts[0]!.Vertices, Is.EquivalentTo(expected.Parts[0]!.Vertices));
            Assert.That(actual.Parts[0]!.TransformedVertices, Is.EquivalentTo(expected.Parts[0]!.TransformedVertices));
        }
    }
}