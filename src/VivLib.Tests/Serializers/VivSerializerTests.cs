#pragma warning disable CS1591
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers.Viv;

namespace TheXDS.Vivianne.Serializers;

[TestFixture]
public class VivSerializerTests() : SerializerTestsBase<VivSerializer, VivFile>("test.viv", GetDefaultFile())
{
    private static VivFile GetDefaultFile() => new()
    {
        Directory = { { "test.txt", "TEST"u8.ToArray() } }
    };

    protected override void TestParsedFile(VivFile expected, VivFile actual)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.ContainsKey("test.txt"));
            Assert.That(actual["test.txt"], Is.EquivalentTo("TEST"u8.ToArray()));
        }
    }

    [Test]
    public void Deserialize_throws_on_invalid_header()
    {
        byte[] invalidViv = [0x56, 0x49, 0x58, 0x00];
        Assert.Throws<InvalidDataException>(() => ((ISerializer<VivFile>)serializer).Deserialize(invalidViv));
    }

    [Test]
    public void Deserialize_reads_files_with_same_name()
    {
        byte[] rawViv = [
            0x42, 0x49, 0x47, 0x46, 0x00, 0x00, 0x00, 0x3A, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x32,
            0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x00, 0x04, 0x74, 0x65, 0x73, 0x74, 0x2E, 0x74, 0x78, 0x74,
            0x00, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x04, 0x74, 0x65, 0x73, 0x74, 0x2E, 0x74, 0x78,
            0x74, 0x00, 0x44, 0x41, 0x54, 0x41, 0x4D, 0x4F, 0x52, 0x45
        ];
        var viv = ((ISerializer<VivFile>)serializer).Deserialize(rawViv);
        Assert.That(viv, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viv.ContainsKey("test.txt"));
            Assert.That(viv.ContainsKey("test (1).txt"));
            Assert.That(viv["test.txt"], Is.EquivalentTo("DATA"u8.ToArray()));
            Assert.That(viv["test (1).txt"], Is.EquivalentTo("MORE"u8.ToArray()));
        }
    }
}
