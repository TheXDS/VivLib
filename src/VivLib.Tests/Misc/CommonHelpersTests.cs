namespace TheXDS.Vivianne.Misc;

[TestFixture]
internal class CommonHelpersTests
{
    [Test]
    public void MapToByte_ShortArray_ConvertsCorrectly()
    {
        short[] input = [1, -2, 300];

        byte[] result = CommonHelpers.MapToByte(input);

        Assert.That(result.Length, Is.EqualTo(input.Length * 2));

        short[] roundTrip = CommonHelpers.MapToInt16(result);

        Assert.That(roundTrip, Is.EqualTo(input));
    }

    [Test]
    public void MapToByte_ShortArray_Empty()
    {
        short[] input = [];
        byte[] result = CommonHelpers.MapToByte(input);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void MapToByte_IntArray_ConvertsCorrectly()
    {
        int[] input = [1, -2, int.MaxValue, int.MinValue];

        byte[] result = CommonHelpers.MapToByte(input);

        Assert.That(result.Length, Is.EqualTo(input.Length * 4));

        int[] roundTrip = CommonHelpers.MapToInt32(result);

        Assert.That(roundTrip, Is.EqualTo(input));
    }

    [Test]
    public void MapToByte_IntArray_Empty()
    {
        int[] input = [];
        byte[] result = CommonHelpers.MapToByte(input);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void MapToByte_SByteArray_ConvertsCorrectly()
    {
        sbyte[] input = [-128, -1, 0, 1, 127];

        byte[] result = CommonHelpers.MapToByte(input);

        Assert.That(result, Is.EquivalentTo(new byte[] { 0, 127, 128, 129, 255 }));
    }

    [Test]
    public void MapToByte_SByteArray_Empty()
    {
        sbyte[] input = [];
        byte[] result = CommonHelpers.MapToByte(input);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void MapToInt16_ByteArray_ConvertsCorrectly()
    {
        short[] original = [-300, 0, 300, short.MaxValue, short.MinValue];

        byte[] bytes = CommonHelpers.MapToByte(original);

        short[] result = CommonHelpers.MapToInt16(bytes);

        Assert.That(result, Is.EqualTo(original));
    }

    [Test]
    public void MapToInt16_ByteArray_Empty()
    {
        byte[] input = [];
        short[] result = CommonHelpers.MapToInt16(input);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void MapToInt32_ByteArray_ConvertsCorrectly()
    {
        int[] original = [-500000, 0, 500000, int.MaxValue, int.MinValue];

        byte[] bytes = CommonHelpers.MapToByte(original);

        int[] result = CommonHelpers.MapToInt32(bytes);

        Assert.That(result, Is.EqualTo(original));
    }

    [Test]
    public void MapToInt32_ByteArray_Empty()
    {
        byte[] input = [];
        int[] result = CommonHelpers.MapToInt32(input);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void MapToSByte_ByteArray_ConvertsCorrectly()
    {
        byte[] input = [0, 127, 128, 129, 255];

        sbyte[] result = CommonHelpers.MaptoSByte(input);

        Assert.That(result, Is.EquivalentTo(new sbyte[] { -128, -1, 0, 1, 127 }));
    }

    [Test]
    public void MapToSByte_ByteArray_Empty()
    {
        byte[] input = [];
        sbyte[] result = CommonHelpers.MaptoSByte(input);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void MapToByte_MapToSByte_RoundTrip()
    {
        sbyte[] original = [-128, -10, 0, 10, 127];

        byte[] bytes = CommonHelpers.MapToByte(original);
        sbyte[] result = CommonHelpers.MaptoSByte(bytes);

        Assert.That(result, Is.EqualTo(original));
    }

    [Test]
    public void MapToByte_MapToInt16_RoundTrip()
    {
        short[] original = [-12345, 0, 12345];

        byte[] bytes = CommonHelpers.MapToByte(original);
        short[] result = CommonHelpers.MapToInt16(bytes);

        Assert.That(result, Is.EqualTo(original));
    }

    [Test]
    public void MapToByte_MapToInt32_RoundTrip()
    {
        int[] original = [-12345678, 0, 12345678];

        byte[] bytes = CommonHelpers.MapToByte(original);
        int[] result = CommonHelpers.MapToInt32(bytes);

        Assert.That(result, Is.EqualTo(original));
    }
}
