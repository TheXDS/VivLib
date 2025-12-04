namespace TheXDS.Vivianne.Extensions;

[TestFixture]
internal class MiscExtensionsTests
{
    [TestCase(0, "0 Bytes")]
    [TestCase(1, "1 Bytes")]
    [TestCase(1023, "1023 Bytes")]
    [TestCase(1024, "1.0 KiB")]
    [TestCase(1536, "1.5 KiB")]
    [TestCase(1048575, "1.0 MiB")]
    [TestCase(1048576, "1.0 MiB")]
    public void GetSize_returns_byte_count_with_unit(int bytes, string expectedResult)
    {
        Assert.That(bytes.GetSize(), Is.EqualTo(expectedResult));
    }

    [TestCase(0, "0 Bytes")]
    [TestCase(1, "1 Bytes")]
    [TestCase(1023, "1023 Bytes")]
    [TestCase(1024, "1.0 KiB")]
    [TestCase(1536, "1.5 KiB")]
    [TestCase(1048575, "1.0 MiB")]
    [TestCase(1048576, "1.0 MiB")]
    public void GetSize_returns_byte_count_with_unit(long bytes, string expectedResult)
    {
        Assert.That(bytes.GetSize(), Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetSize_with_humanreadable_false_returns_byte_count_only()
    {
        Assert.That(1536.GetSize(humanReadable: false), Is.EqualTo("1536"));
    }
}
