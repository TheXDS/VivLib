using System.Runtime.InteropServices;

namespace TheXDS.Vivianne.Serializers.Misc;

/// <summary>
/// Represents a 32-bit fixed-point decimal number in Q16.16 format.
/// This format uses 16 bits for the fractional part and 16 bits for the signed integer part.
/// The Q16.16 format was commonly used in 1990s game engines and applications before
/// floating-point units were ubiquitous, including games like Doom and Need For Speed II.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FixedPointDecimal32(ushort fractionalPart, short integralPart)
{
    /// <summary>
    /// The 16-bit unsigned fractional part of the fixed-point number.
    /// Represents values from 0 to 65535, scaled by 2^-16 (1/65536).
    /// </summary>
    public ushort FractionalPart = fractionalPart;

    /// <summary>
    /// The 16-bit signed integral part of the fixed-point number.
    /// Represents integer values from -32768 to +32767.
    /// </summary>
    public short IntegralPart = integralPart;

    /// <summary>
    /// The scaling factor used for Q16.16 fixed-point arithmetic.
    /// Equal to 2^16 = 65536.
    /// </summary>
    private const double ScalingFactor = 65536.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedPointDecimal32"/> struct from a float value.
    /// </summary>
    /// <param name="value">The floating-point value to convert.</param>
    public FixedPointDecimal32(float value) : this(ConvertFromDouble(value))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedPointDecimal32"/> struct from a double value.
    /// </summary>
    /// <param name="value">The double-precision floating-point value to convert.</param>
    public FixedPointDecimal32(double value) : this(ConvertFromDouble(value))
    {
    }

    /// <summary>
    /// Helper constructor that takes a tuple from the conversion method.
    /// </summary>
    private FixedPointDecimal32((ushort fractional, short integral) parts) 
        : this(parts.fractional, parts.integral)
    {
    }

    /// <summary>
    /// Converts a double value to Q16.16 format components.
    /// </summary>
    private static (ushort fractional, short integral) ConvertFromDouble(double value)
    {
        // Multiply by the scaling factor to get the raw 32-bit representation
        int rawValue = (int)Math.Round(value * ScalingFactor);
        
        // Extract fractional part (lower 16 bits)
        ushort fractional = (ushort)(rawValue & 0xFFFF);
        
        // Extract integral part (upper 16 bits)
        short integral = (short)(rawValue >> 16);
        
        return (fractional, integral);
    }

    /// <summary>
    /// Implicitly converts a <see cref="FixedPointDecimal32"/> to a float.
    /// </summary>
    /// <param name="value">The fixed-point value to convert.</param>
    public static implicit operator float(FixedPointDecimal32 value)
    {
        return (float)((double)value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="FixedPointDecimal32"/> to a double.
    /// </summary>
    /// <param name="value">The fixed-point value to convert.</param>
    public static implicit operator double(FixedPointDecimal32 value)
    {
        // Reconstruct the 32-bit signed integer
        int rawValue = (value.IntegralPart << 16) | value.FractionalPart;
        
        // Divide by the scaling factor to get the double value
        return rawValue / ScalingFactor;
    }

    /// <summary>
    /// Implicitly converts a float to a <see cref="FixedPointDecimal32"/>.
    /// </summary>
    /// <param name="value">The floating-point value to convert.</param>
    public static implicit operator FixedPointDecimal32(float value)
    {
        return new FixedPointDecimal32(value);
    }

    /// <summary>
    /// Implicitly converts a double to a <see cref="FixedPointDecimal32"/>.
    /// </summary>
    /// <param name="value">The double-precision floating-point value to convert.</param>
    public static implicit operator FixedPointDecimal32(double value)
    {
        return new FixedPointDecimal32(value);
    }

    /// <summary>
    /// Returns a string representation of the fixed-point number.
    /// </summary>
    /// <returns>A string representation of the numeric value.</returns>
    public override string ToString()
    {
        double doubleValue = this;
        return doubleValue.ToString();
    }
}
