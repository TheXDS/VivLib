using System;
using NUnit.Framework;
using TheXDS.Vivianne.Tools.Audio;

namespace TheXDS.Vivianne.Tools.Audio.Tests;

/// <summary>
/// Unit tests for the <see cref="AudioNormalizer"/> class.
/// </summary>
[TestFixture]
internal sealed class AudioNormalizerTests
{
    #region NormalizeVolume Tests

    [Test]
    public void NormalizeVolume_WithNullData_ReturnsEmptyArray()
    {
        // Arrange
        byte[] data = null!;
        int bits = 16;
        double level = 0.5;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void NormalizeVolume_WithEmptyData_ReturnsEmptyArray()
    {
        // Arrange
        byte[] data = [];
        int bits = 16;
        double level = 0.5;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void NormalizeVolume_With8BitsAndLevel0_5_ReturnsNormalizedData()
    {
        // Arrange: 8-bit signed samples (-128 to 127)
        sbyte[] samples = [-128, -64, 0, 64, 127];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 8;
        double level = 0.5;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        sbyte[] resultSamples = CommonHelpers.MaptoSByte(result);

        // Assert
        Assert.That(resultSamples, Has.Length.EqualTo(samples.Length));
        // Expected: scaled by 0.5, so max 63.5 -> 63
        Assert.That(resultSamples, Is.EquivalentTo(new sbyte[] { -64, -32, 0, 32, 63 }));
    }

    [Test]
    public void NormalizeVolume_With16BitsAndLevel0_5_ReturnsNormalizedData()
    {
        // Arrange: 16-bit signed samples
        short[] samples = [-32768, -16384, 0, 16384, 32767];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 16;
        double level = 0.5;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        short[] resultSamples = CommonHelpers.MapToInt16(result);

        // Assert
        Assert.That(resultSamples, Has.Length.EqualTo(samples.Length));
        // Expected: scaled by 0.5, so max 16383.5 -> 16383
        Assert.That(resultSamples, Is.EquivalentTo(new short[] { -16384, -8192, 0, 8192, 16383 }));
    }

    [Test]
    public void NormalizeVolume_With32BitsAndLevel0_5_ReturnsNormalizedData()
    {
        // Arrange: 32-bit signed samples
        int[] samples = [-2147483648, -1073741824, 0, 1073741824, 2147483647];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 32;
        double level = 0.5;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        int[] resultSamples = CommonHelpers.MapToInt32(result);

        // Assert
        Assert.That(resultSamples, Has.Length.EqualTo(samples.Length));
        // Expected: scaled by 0.5, so max 1073741823.5 -> 1073741823
        Assert.That(resultSamples, Is.EquivalentTo(new int[] { -1073741824, -536870912, 0, 536870912, 1073741823 }));
    }

    [Test]
    public void NormalizeVolume_WithLevel1_0_ReturnsFullScaleData()
    {
        // Arrange: 16-bit samples at maximum amplitude
        short[] samples = [-32768, 32767];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 16;
        double level = 1.0;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        short[] resultSamples = CommonHelpers.MapToInt16(result);

        // Assert
        Assert.That(resultSamples, Is.EquivalentTo(samples));
    }

    [Test]
    public void NormalizeVolume_WithLevel0_0_ReturnsZeroData()
    {
        // Arrange: 16-bit samples
        short[] samples = [-32768, -16384, 0, 16384, 32767];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 16;
        double level = 0.0;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        short[] resultSamples = CommonHelpers.MapToInt16(result);

        // Assert
        Assert.That(resultSamples, Is.All.Zero);
    }

    [Test]
    public void NormalizeVolume_WithUnsupportedBits_ThrowsInvalidOperationException()
    {
        // Arrange
        byte[] data = CommonHelpers.MapToByte(new short[] { -1000, 1000 });
        int bits = 24; // Unsupported
        double level = 0.5;

        // Act & Assert
        Assert.That(() => AudioNormalizer.NormalizeVolume(data, bits, level), 
            Throws.InstanceOf<InvalidOperationException>());
    }

    [TestCase(-0.000001)]
    [TestCase(1.000001)]
    [TestCase(double.NegativeInfinity)]
    [TestCase(double.PositiveInfinity)]
    [TestCase(double.NaN)]
    public void NormalizeVolume_WithLevelOutOfRange_ThrowsArgumentOutOfRangeException(double invalidLevel)
    {
        // Arrange
        byte[] data = CommonHelpers.MapToByte(new short[] { -1000, 1000 });
        int bits = 16;

        // Act & Assert
        Assert.That(() => AudioNormalizer.NormalizeVolume(data, bits, invalidLevel),
            Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void NormalizeVolume_WithSingleSample_ReturnsScaledSample()
    {
        // Arrange: Single 16-bit sample
        short[] samples = [1000];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 16;
        double level = 0.8;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        short[] resultSamples = CommonHelpers.MapToInt16(result);

        // Assert
        Assert.That(resultSamples, Has.Length.EqualTo(1));
        // Expected: 1000 * (32767 * 0.8) / 1000 = 26213.6 -> 26213
        Assert.That(resultSamples[0], Is.EqualTo(26213));
    }

    [Test]
    public void NormalizeVolume_WithAllZeroSamples_ReturnsAllZeroSamples()
    {
        // Arrange: All zero samples
        short[] samples = [0, 0, 0, 0];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 16;
        double level = 0.5;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        short[] resultSamples = CommonHelpers.MapToInt16(result);

        // Assert
        Assert.That(resultSamples, Is.All.Zero);
    }

    [Test]
    public void NormalizeVolume_WithMixedPositiveNegativeSamples_ReturnsCorrectlyScaled()
    {
        // Arrange: Mixed positive and negative samples
        short[] samples = [-1000, -500, 0, 500, 1000];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 16;
        double level = 0.75;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        short[] resultSamples = CommonHelpers.MapToInt16(result);

        // Assert
        Assert.That(resultSamples, Has.Length.EqualTo(samples.Length));
        // Max absolute value is 1000, so scale factor is (32767 * 0.75) / 1000 = 24.57525
        // Expected: [-24575, -12287, 0, 12287, 24575]
        Assert.That(resultSamples, Is.EquivalentTo(new short[] { -24575, -12287, 0, 12287, 24575 }));
    }

    [Test]
    public void NormalizeVolume_With8BitDataAndLevel0_25_ReturnsCorrectlyScaled()
    {
        // Arrange: 8-bit samples
        sbyte[] samples = [-100, -50, 0, 50, 100];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 8;
        double level = 0.25;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        sbyte[] resultSamples = CommonHelpers.MaptoSByte(result);

        // Assert
        Assert.That(resultSamples, Has.Length.EqualTo(samples.Length));
        // Max absolute value is 100, so scale factor is (127 * 0.25) / 100 = 0.3175
        // Expected: [-31, -15, 0, 15, 31]
        Assert.That(resultSamples, Is.EquivalentTo(new sbyte[] { -31, -15, 0, 15, 31 }));
    }

    [Test]
    public void NormalizeVolume_With32BitDataAndLevel0_1_ReturnsCorrectlyScaled()
    {
        // Arrange: 32-bit samples
        int[] samples = [-1000000, -500000, 0, 500000, 1000000];
        byte[] data = CommonHelpers.MapToByte(samples);
        int bits = 32;
        double level = 0.1;

        // Act
        byte[] result = AudioNormalizer.NormalizeVolume(data, bits, level);
        int[] resultSamples = CommonHelpers.MapToInt32(result);

        // Assert
        Assert.That(resultSamples, Has.Length.EqualTo(samples.Length));
        // Max absolute value is 1000000, so scale factor is (2147483647 * 0.1) / 1000000 = 214.7483647
        // Expected: [-214748364, -107374182, 0, 107374182, 214748364]
        Assert.That(resultSamples, Is.EquivalentTo(new int[] { -214748364, -107374182, 0, 107374182, 214748364 }));
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// CommonHelpers class for mapping between different array types.
    /// </summary>
    private static class CommonHelpers
    {
        /// <summary>
        /// Maps the input array as a <c><see cref="byte"/>[]</c> array.
        /// </summary>
        public static byte[] MapToByte(short[] inputArray)
        {
            var result = new byte[inputArray.Length * 2];
            Buffer.BlockCopy(inputArray, 0, result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Maps the input array as a <c><see cref="byte"/>[]</c> array.
        /// </summary>
        public static byte[] MapToByte(int[] inputArray)
        {
            var result = new byte[inputArray.Length * 4];
            Buffer.BlockCopy(inputArray, 0, result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Maps the input array as a <c><see cref="byte"/>[]</c> array.
        /// </summary>
        public static byte[] MapToByte(sbyte[] inputArray)
        {
            return [.. inputArray.Select(p => (byte)(p + 128))];
        }

        /// <summary>
        /// Maps the data in the <c><see cref="byte"/>[]</c> array as a
        /// <c><see cref="short"/>[]</c> array.
        /// </summary>
        public static short[] MapToInt16(byte[] inputArray)
        {
            var result = new short[inputArray.Length / 2];
            Buffer.BlockCopy(inputArray, 0, result, 0, inputArray.Length);
            return result;
        }

        /// <summary>
        /// Maps the data in the <c><see cref="byte"/>[]</c> array as an
        /// <c><see cref="int"/>[]</c> array.
        /// </summary>
        public static int[] MapToInt32(byte[] inputArray)
        {
            var result = new int[inputArray.Length / 4];
            Buffer.BlockCopy(inputArray, 0, result, 0, inputArray.Length);
            return result;
        }

        /// <summary>
        /// Maps the data in the <c><see cref="byte"/>[]</c> array as an
        /// <c><see cref="sbyte"/>[]</c> array.
        /// </summary>
        public static sbyte[] MaptoSByte(byte[] inputArray)
        {
            return [.. inputArray.Select(p => (sbyte)(p - 128))];
        }
    }

    #endregion
}