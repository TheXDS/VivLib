using TheXDS.Vivianne.Models.Fe.Nfs2;
using TheXDS.Vivianne.Serializers.Fe.Nfs2;

namespace TheXDS.Vivianne.Serializers.Nfs2;

[TestFixture]
internal class LangFileSerializerTests() : SerializerTestsBase<LangFileSerializer, LangFile>("Nfs2.text.eng", GetDefaultFile())
{
    private static LangFile GetDefaultFile()
    {
        return
        [
            new LangFileEntry()
            {
                FontSize = 0x12,
                Unk_0x02 = 0x0102,
                Unk_0x04 = 0xa8,
                Unk_0x06 = 0x7e,
                Text = "TEST"
            },
            new LangFileEntry()
            {
                FontSize = 0x24,
                Unk_0x02 = 0x0102,
                Unk_0x04 = 0xa8,
                Unk_0x06 = 0x7e,
                Text = "TST2"
            }
        ];
    }

    protected override void TestParsedFile(LangFile expected, LangFile actual)
    {
        using (Assert.EnterMultipleScope()) 
        {
            Assert.That(actual, Has.Count.EqualTo(2));
            Assert.That(actual[0].FontSize, Is.EqualTo(0x12));
            Assert.That(actual[0].Unk_0x02, Is.EqualTo(0x102));
            Assert.That(actual[0].Unk_0x04, Is.EqualTo(0xa8));
            Assert.That(actual[0].Unk_0x06, Is.EqualTo(0x7e));
            Assert.That(actual[0].Text, Is.EqualTo("TEST"));
            Assert.That(actual[1].FontSize, Is.EqualTo(0x24));
            Assert.That(actual[1].Unk_0x02, Is.EqualTo(0x102));
            Assert.That(actual[1].Unk_0x04, Is.EqualTo(0xa8));
            Assert.That(actual[1].Unk_0x06, Is.EqualTo(0x7e));
            Assert.That(actual[1].Text, Is.EqualTo("TST2"));
        }
    }
}
