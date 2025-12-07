using Moq;
using TheXDS.Vivianne.Models.Audio.Bnk;
using TheXDS.Vivianne.Misc;

namespace TheXDS.Vivianne.Tools.Audio.Bnk;

/// <summary>
/// Unit tests for the <see cref="BnkNormalizer"/> class.
/// </summary>
[TestFixture]
internal sealed class BnkNormalizerTests
{
    [Test]
    public void NormalizeVolume_8bit_Scalefactor_50Percent_ReturnsExpectedValues()
    {
        sbyte[] samples = [-128, -64, 0, 64, 127];
        byte[] data = CommonHelpers.MapToByte(samples);
        double level = 0.5;

        byte[] result = BnkNormalizer.NormalizeVolume(data, 8, level);
        sbyte[] resultSamples = CommonHelpers.MaptoSByte(result);

        Assert.That(resultSamples, Has.Length.EqualTo(samples.Length));
        Assert.That(resultSamples, Is.EquivalentTo(new sbyte[] { -64, -32, 0, 32, 63 }));
    }

    [Test]
    public void NormalizeVolume_16bit_Scalefactor_50Percent_ReturnsExpectedValues()
    {
        short[] samples = [-32768, -16384, 0, 16384, 32767];
        byte[] data = CommonHelpers.MapToByte(samples);
        double level = 0.5;

        byte[] result = BnkNormalizer.NormalizeVolume(data, 16, level);
        short[] resultSamples = CommonHelpers.MapToInt16(result);

        Assert.That(resultSamples.Length, Is.EqualTo(samples.Length));
        Assert.That(resultSamples, Is.EquivalentTo(new short[] { -16384, -8192, 0, 8192, 16383 }));
    }

    [Test]
    public void NormalizeVolume_32bit_Scalefactor_50Percent_ReturnsExpectedValues()
    {
        int[] samples = [-2147483648, -1073741824, 0, 1073741824, 2147483647];
        byte[] data = CommonHelpers.MapToByte(samples);
        double level = 0.5;

        byte[] result = BnkNormalizer.NormalizeVolume(data, 32, level);
        int[] resultSamples = CommonHelpers.MapToInt32(result);

        Assert.That(resultSamples, Has.Length.EqualTo(samples.Length));
        Assert.That(resultSamples, Is.EquivalentTo([-1073741824, -536870912, 0, 536870912, 1073741823]));
    }

    [Test]
    public void NormalizeVolume_WithUnsupportedBits_ThrowsInvalidOperationException()
    {
        byte[] data = [0x00, 0x01, 0x02];
        double level = 0.5;

        Assert.That(() => BnkNormalizer.NormalizeVolume(data, 55, level), Throws.TypeOf<InvalidOperationException>());
    }

    [TestCase(-0.000001)]
    [TestCase(1.000001)]
    [TestCase(double.NegativeInfinity)]
    [TestCase(double.PositiveInfinity)]
    [TestCase(double.NaN)]
    public void NormalizeVolume_WithLevelOutOfRange_ThrowsValueOutOfRangeException(double invalidLevel)
    {
        byte[] data = [0x00, 0x01];

        Assert.That(() => BnkNormalizer.NormalizeVolume(data, 8, invalidLevel), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void NormalizeVolume_EmptyData_ReturnsEmptyArray()
    {
        // Arrange
        byte[] data = [];
        double level = 0.5;

        // Act
        byte[] result = BnkNormalizer.NormalizeVolume(data, 8, level);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void NormalizeVolume_StreamOverload_ReturnsNormalizedCopy()
    {
        // Arrange
        short[] samples = [-32768, -16384, 0, 16384, 32767];
        byte[] data = CommonHelpers.MapToByte(samples);
        var mockStream = new BnkStream()
        {
            SampleData = data,
            BytesPerSample = 2
        };
        
        double level = 0.5;
        byte[] result = BnkNormalizer.NormalizeVolume(mockStream, level);
        short[] resultSamples = CommonHelpers.MapToInt16(result);
        Assert.That(resultSamples, Is.EqualTo(new short[] { -16384, -8192, 0, 8192, 16383 }));
    }
}