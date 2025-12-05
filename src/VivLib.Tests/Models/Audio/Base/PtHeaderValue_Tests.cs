// src/VivLib.Tests/Models/Audio/Base/PtHeaderValue_Tests.cs
using NUnit.Framework;

namespace TheXDS.Vivianne.Models.Audio.Base;

/// <summary>
/// Unit tests for <see cref="PtHeaderValue"/>.
/// </summary>
[TestFixture]
internal sealed class PtHeaderValue_Tests
{
    #region Helper: Pack Size Tests

    /// <summary>
    /// Ensures that the implicit conversion from integral types yields the expected
    /// <see cref="PtHeaderValue.Length"/> and <see cref="PtHeaderValue.Value"/>.
    /// </summary>
    /// <param name="value">The source value.</param>
    /// <param name="expectedLength">The expected pack length.</param>
    /// <param name="expectedValue">The expected value stored in the struct.</param>
    [TestCase(1, 1, 1)]
    [TestCase(255, 1, 255)]
    [TestCase(256, 2, 256)]
    [TestCase(65535, 2, 65535)]
    [TestCase(65536, 4, 65536)]
    [TestCase(int.MaxValue, 4, int.MaxValue)]
    [TestCase(0, 1, 0)]
    [TestCase(-1, 4, -1)]          // negative int → 4‑byte pack via uint cast
    [TestCase((short)32767, 2, 32767)]
    [TestCase((short)-1, 4, -1)]    // negative short → 4‑byte pack via uint cast
    [TestCase((ushort)65535, 2, 65535)]
    [TestCase((ushort)256, 2, 256)]
    [TestCase((ushort)1, 1, 1)]
    [TestCase((byte)127, 1, 127)]
    [TestCase((sbyte)-1, 1, -1)]
    [TestCase(uint.MaxValue, 4, -1)] // int cast of max uint
    [TestCase((uint)65535, 2, 65535)]
    [TestCase((uint)255, 1, 255)]
    [TestCase(false, 1, 0)]
    [TestCase(true, 1, 1)]
    public void ImplicitConversion_ShouldSetLengthAndValueCorrectly(
        object value,
        byte expectedLength,
        int expectedValue)
    {
        PtHeaderValue pv = value switch
        {
            bool b     => b,
            byte b     => b,
            sbyte b    => b,
            short s    => s,
            ushort u   => u,
            int i      => i,
            uint u     => u,
            _          => throw new InvalidOperationException()
        };

        using (Assert.EnterMultipleScope())
        {
            Assert.That(pv.Length, Is.EqualTo(expectedLength));
            Assert.That(pv.Value,  Is.EqualTo(expectedValue));
        }
    }

    #endregion

    #region Boolean & Primitive Conversions

    [Test]
    public void BoolTrue_ShouldHaveLengthOneAndValueOne()
    {
        PtHeaderValue pv = true;
        Assert.That(pv.Length, Is.EqualTo(1));
        Assert.That(pv.Value,  Is.EqualTo(1));
    }

    [Test]
    public void BoolFalse_ShouldHaveLengthOneAndValueZero()
    {
        PtHeaderValue pv = false;
        Assert.That(pv.Length, Is.EqualTo(1));
        Assert.That(pv.Value,  Is.EqualTo(0));
    }

    [Test]
    public void Byte_ShouldHaveLengthOne()
    {
        PtHeaderValue pv = (byte)42;
        Assert.That(pv.Length, Is.EqualTo(1));
        Assert.That(pv.Value,  Is.EqualTo(42));
    }

    [Test]
    public void SByte_ShouldHaveLengthOne()
    {
        PtHeaderValue pv = (sbyte)-7;
        Assert.That(pv.Length, Is.EqualTo(1));
        Assert.That(pv.Value,  Is.EqualTo(-7));
    }

    [Test]
    public void Short_ShouldHaveCorrectLength()
    {
        PtHeaderValue pv = (short)2000;
        Assert.That(pv.Length, Is.EqualTo(2));
        Assert.That(pv.Value,  Is.EqualTo(2000));
    }

    [Test]
    public void UShort_ShouldHaveCorrectLength()
    {
        PtHeaderValue pv = (ushort)2000;
        Assert.That(pv.Length, Is.EqualTo(2));
        Assert.That(pv.Value,  Is.EqualTo(2000));
    }

    [Test]
    public void Int_ShouldHaveCorrectLength()
    {
        PtHeaderValue pv = 123456;
        Assert.That(pv.Length, Is.EqualTo(4));
        Assert.That(pv.Value,  Is.EqualTo(123456));
    }

    [Test]
    public void UInt_ShouldHaveCorrectLength()
    {
        PtHeaderValue pv = (uint)123456;
        Assert.That(pv.Length, Is.EqualTo(4));
        Assert.That(pv.Value,  Is.EqualTo(123456));
    }

    #endregion

    #region Explicit Implicit Conversion to int

    [Test]
    public void ImplicitConversion_ToInt_ShouldReturnStoredValue()
    {
        PtHeaderValue pv = 9876;
        int i = pv;
        Assert.That(i, Is.EqualTo(9876));
    }

    #endregion
}
