using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers;

internal class MarshalSerializerTests
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct TestStruct(int number, bool flag, byte[] data, string code)
    {
        public int Number = number;
        public bool Flag = flag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Data = data;
        // 5 bytes allow for a 4-byte string, plus the null terminator.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string Code = code;
    }

    [Test]
    public void SerializeAndDeserialize_ReturnsIdenticalStruct()
    {
        var original = new TestStruct(
            number: 0x12345678,
            flag: true,
            data: [0xAA, 0xBB, 0xCC, 0xDD],
            code: "TEST");

        var serializer = new MarshalSerializer<TestStruct>();

        using var ms = new MemoryStream();
        serializer.SerializeTo(original, ms);

        ms.Position = 0;
        var result = serializer.Deserialize(ms);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Number, Is.EqualTo(original.Number));
            Assert.That(result.Flag, Is.EqualTo(original.Flag));
            Assert.That(result.Data, Is.EqualTo(original.Data));
            Assert.That(result.Code, Is.EqualTo(original.Code));
        }
    }

    [Test]
    public void SerializeAndDeserialize_MultipleStructs()
    {
        var structs = new[]
        {
            new TestStruct(1, true, [0x01, 0x02, 0x03, 0x04], "ABCD"),
            new TestStruct(2, false, [0x11, 0x22, 0x33, 0x44], "EFGH")
        };

        var serializer = new MarshalSerializer<TestStruct>();

        using var ms = new MemoryStream();
        foreach (var s in structs) serializer.SerializeTo(s, ms);

        ms.Position = 0;
        foreach (var expected in structs)
        {
            var actual = serializer.Deserialize(ms);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(actual.Number, Is.EqualTo(expected.Number));
                Assert.That(actual.Flag, Is.EqualTo(expected.Flag));
                Assert.That(actual.Data, Is.EqualTo(expected.Data));
                Assert.That(actual.Code, Is.EqualTo(expected.Code));
            }
        }
    }
}