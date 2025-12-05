using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Misc;

[TestFixture]
internal class FixedPointDecimal32Tests
{
    #region Roundtrip Conversion Tests - Float

    [Test]
    public void RoundtripConversion_PositiveFloat_PreservesValue()
    {
        // Arrange
        float original = 123.456f;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        float result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.0001f));
    }

    [Test]
    public void RoundtripConversion_NegativeFloat_PreservesValue()
    {
        // Arrange
        float original = -987.654f;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        float result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.0001f));
    }

    [Test]
    public void RoundtripConversion_ZeroFloat_PreservesValue()
    {
        // Arrange
        float original = 0.0f;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        float result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original));
    }

    [Test]
    public void RoundtripConversion_SmallPositiveFloat_PreservesValue()
    {
        // Arrange
        float original = 0.00015f;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        float result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.00001f));
    }

    [Test]
    public void RoundtripConversion_SmallNegativeFloat_PreservesValue()
    {
        // Arrange
        float original = -0.00015f;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        float result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.00001f));
    }

    [Test]
    public void RoundtripConversion_MaxRangeFloat_PreservesValue()
    {
        // Arrange - Q16.16 max range is approximately ±32767.99998
        float original = 32767.5f;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        float result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.01f));
    }

    [Test]
    public void RoundtripConversion_MinRangeFloat_PreservesValue()
    {
        // Arrange
        float original = -32768.0f;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        float result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.01f));
    }

    #endregion

    #region Roundtrip Conversion Tests - Double

    [Test]
    public void RoundtripConversion_PositiveDouble_PreservesValue()
    {
        // Arrange
        double original = 456.789;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        double result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.0001));
    }

    [Test]
    public void RoundtripConversion_NegativeDouble_PreservesValue()
    {
        // Arrange
        double original = -789.123;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        double result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.0001));
    }

    [Test]
    public void RoundtripConversion_ZeroDouble_PreservesValue()
    {
        // Arrange
        double original = 0.0;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        double result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original));
    }

    [Test]
    public void RoundtripConversion_SmallPositiveDouble_PreservesValue()
    {
        // Arrange
        double original = 0.000152587890625; // Exactly representable in Q16.16
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        double result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.00000001));
    }

    [Test]
    public void RoundtripConversion_SmallNegativeDouble_PreservesValue()
    {
        // Arrange
        double original = -0.000152587890625;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        double result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.00000001));
    }

    [Test]
    public void RoundtripConversion_MaxRangeDouble_PreservesValue()
    {
        // Arrange
        double original = 32767.99;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        double result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.01));
    }

    [Test]
    public void RoundtripConversion_MinRangeDouble_PreservesValue()
    {
        // Arrange
        double original = -32768.0;
        
        // Act
        FixedPointDecimal32 fixedPoint = original;
        double result = fixedPoint;
        
        // Assert
        Assert.That(result, Is.EqualTo(original).Within(0.01));
    }

    #endregion

    #region Marshaling Tests - Reading from Byte Array

    [Test]
    public void Marshaling_ReadPositiveNumber_CorrectValue()
    {
        // Arrange - Represents 123.5 in Q16.16 format
        // 123.5 * 65536 = 8093696 = 0x007B8000
        // Little-endian: 00 80 7B 00
        byte[] data = { 0x00, 0x80, 0x7B, 0x00 };
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            FixedPointDecimal32 value = Marshal.PtrToStructure<FixedPointDecimal32>(handle.AddrOfPinnedObject());
            double result = value;
            
            // Assert
            Assert.That(result, Is.EqualTo(123.5).Within(0.001));
            Assert.That(value.FractionalPart, Is.EqualTo(0x8000));
            Assert.That(value.IntegralPart, Is.EqualTo(123));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_ReadNegativeNumber_CorrectValue()
    {
        // Arrange - Represents -50.25 in Q16.16 format
        // -50.25 * 65536 = -3293184 = 0xFFCDC000
        // Little-endian: 00 C0 CD FF
        byte[] data = { 0x00, 0xC0, 0xCD, 0xFF };
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            FixedPointDecimal32 value = Marshal.PtrToStructure<FixedPointDecimal32>(handle.AddrOfPinnedObject());
            double result = value;
            
            // Assert
            Assert.That(result, Is.EqualTo(-50.25).Within(0.001));
            Assert.That(value.FractionalPart, Is.EqualTo(0xC000));
            Assert.That(value.IntegralPart, Is.EqualTo(-51));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_ReadZero_CorrectValue()
    {
        // Arrange
        byte[] data = { 0x00, 0x00, 0x00, 0x00 };
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            FixedPointDecimal32 value = Marshal.PtrToStructure<FixedPointDecimal32>(handle.AddrOfPinnedObject());
            double result = value;
            
            // Assert
            Assert.That(result, Is.EqualTo(0.0));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_ReadPositiveFractional_CorrectValue()
    {
        // Arrange - Represents 0.75 in Q16.16 format
        // 0.75 * 65536 = 49152 = 0x0000C000
        // Little-endian: 00 C0 00 00
        byte[] data = { 0x00, 0xC0, 0x00, 0x00 };
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            FixedPointDecimal32 value = Marshal.PtrToStructure<FixedPointDecimal32>(handle.AddrOfPinnedObject());
            double result = value;
            
            // Assert
            Assert.That(result, Is.EqualTo(0.75).Within(0.001));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_ReadNegativeFractional_CorrectValue()
    {
        // Arrange - Represents -0.5 in Q16.16 format
        // -0.5 * 65536 = -32768 = 0xFFFF8000
        // Little-endian: 00 80 FF FF
        byte[] data = { 0x00, 0x80, 0xFF, 0xFF };
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            FixedPointDecimal32 value = Marshal.PtrToStructure<FixedPointDecimal32>(handle.AddrOfPinnedObject());
            double result = value;
            
            // Assert
            Assert.That(result, Is.EqualTo(-0.5).Within(0.001));
        }
        finally
        {
            handle.Free();
        }
    }

    #endregion

    #region Marshaling Tests - Writing to Byte Array

    [Test]
    public void Marshaling_WritePositiveNumber_CorrectBytes()
    {
        // Arrange
        FixedPointDecimal32 value = 123.5;
        byte[] data = new byte[4];
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            
            // Assert - 123.5 * 65536 = 8093696 = 0x007B8000
            // Little-endian: 00 80 7B 00
            Assert.That(data[0], Is.EqualTo(0x00));
            Assert.That(data[1], Is.EqualTo(0x80));
            Assert.That(data[2], Is.EqualTo(0x7B));
            Assert.That(data[3], Is.EqualTo(0x00));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_WriteNegativeNumber_CorrectBytes()
    {
        // Arrange
        FixedPointDecimal32 value = -50.25;
        byte[] data = new byte[4];
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            
            // Assert - -50.25 * 65536 = -3293184 = 0xFFCDC000
            // Little-endian: 00 C0 CD FF
            Assert.That(data[0], Is.EqualTo(0x00));
            Assert.That(data[1], Is.EqualTo(0xC0));
            Assert.That(data[2], Is.EqualTo(0xCD));
            Assert.That(data[3], Is.EqualTo(0xFF));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_WriteZero_CorrectBytes()
    {
        // Arrange
        FixedPointDecimal32 value = 0.0;
        byte[] data = new byte[4];
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            
            // Assert
            Assert.That(data, Is.All.EqualTo(0x00));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_WritePositiveFractional_CorrectBytes()
    {
        // Arrange
        FixedPointDecimal32 value = 0.75;
        byte[] data = new byte[4];
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            
            // Assert - 0.75 * 65536 = 49152 = 0x0000C000
            // Little-endian: 00 C0 00 00
            Assert.That(data[0], Is.EqualTo(0x00));
            Assert.That(data[1], Is.EqualTo(0xC0));
            Assert.That(data[2], Is.EqualTo(0x00));
            Assert.That(data[3], Is.EqualTo(0x00));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_WriteNegativeFractional_CorrectBytes()
    {
        // Arrange
        FixedPointDecimal32 value = -0.5;
        byte[] data = new byte[4];
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act
            Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            
            // Assert - -0.5 * 65536 = -32768 = 0xFFFF8000
            // Little-endian: 00 80 FF FF
            Assert.That(data[0], Is.EqualTo(0x00));
            Assert.That(data[1], Is.EqualTo(0x80));
            Assert.That(data[2], Is.EqualTo(0xFF));
            Assert.That(data[3], Is.EqualTo(0xFF));
        }
        finally
        {
            handle.Free();
        }
    }

    #endregion

    #region Marshaling Roundtrip Tests

    [Test]
    public void Marshaling_RoundtripPositive_PreservesValue()
    {
        // Arrange
        FixedPointDecimal32 original = 456.789;
        byte[] data = new byte[4];
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act - Write
            Marshal.StructureToPtr(original, handle.AddrOfPinnedObject(), false);
            
            // Act - Read
            FixedPointDecimal32 result = Marshal.PtrToStructure<FixedPointDecimal32>(handle.AddrOfPinnedObject());
            
            // Assert
            double originalValue = original;
            double resultValue = result;
            Assert.That(resultValue, Is.EqualTo(originalValue).Within(0.001));
        }
        finally
        {
            handle.Free();
        }
    }

    [Test]
    public void Marshaling_RoundtripNegative_PreservesValue()
    {
        // Arrange
        FixedPointDecimal32 original = -1234.567;
        byte[] data = new byte[4];
        GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        
        try
        {
            // Act - Write
            Marshal.StructureToPtr(original, handle.AddrOfPinnedObject(), false);
            
            // Act - Read
            FixedPointDecimal32 result = Marshal.PtrToStructure<FixedPointDecimal32>(handle.AddrOfPinnedObject());
            
            // Assert
            double originalValue = original;
            double resultValue = result;
            Assert.That(resultValue, Is.EqualTo(originalValue).Within(0.001));
        }
        finally
        {
            handle.Free();
        }
    }

    #endregion

    #region Structure Size Tests

    [Test]
    public void StructSize_Is4Bytes()
    {
        // Assert
        Assert.That(Marshal.SizeOf<FixedPointDecimal32>(), Is.EqualTo(4));
    }

    #endregion

    #region ToString Tests

    [Test]
    public void ToString_PositiveNumber_ReturnsCorrectString()
    {
        // Arrange
        FixedPointDecimal32 value = 123.5;
        
        // Act
        string result = value.ToString();
        
        // Assert
        Assert.That(result, Does.Contain("123.5"));
    }

    [Test]
    public void ToString_NegativeNumber_ReturnsCorrectString()
    {
        // Arrange
        FixedPointDecimal32 value = -50.25;
        
        // Act
        string result = value.ToString();
        
        // Assert
        Assert.That(result, Does.Contain("-50.25"));
    }

    [Test]
    public void ToString_Zero_ReturnsCorrectString()
    {
        // Arrange
        FixedPointDecimal32 value = 0.0;
        
        // Act
        string result = value.ToString();
        
        // Assert
        Assert.That(result, Is.EqualTo("0"));
    }

    #endregion
}
