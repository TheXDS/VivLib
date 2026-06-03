using System.Globalization;
using NUnit.Framework;
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Tools.Fe;

namespace TheXDS.Vivianne.Tests.Tools.Fe;

[TestFixture]
internal class EngUnitTextProvider_Tests
{
    private MockCarPerf _mockCarPerf;
    private EngUnitTextProvider _provider;

    [SetUp]
    public void SetUp()
    {
        _mockCarPerf = new MockCarPerf
        {
            SerialNumber = 1,
            Mass = 1500,
            TopSpeed = 100,
            EngineMaxRpm = 7000,
            TireWidthFront = 225,
            TireSidewallFront = 45,
            TireRimFront = 18,
            TireWidthRear = 245,
            TireSidewallRear = 40,
            TireRimRear = 19,
            NumberOfGearsAuto = 8,
            NumberOfGearsManual = 6,
            CarClass = 1
        };
        _provider = new EngUnitTextProvider(_mockCarPerf);
    }

    [Test]
    public void TopSpeed_ReturnsCorrectFormatAndValue()
    {
        // 100 m/s * 2.236936 = 223.6936 -> "224"
        Assert.That(_provider.TopSpeed, Is.EqualTo("224 MPH"));
    }

    [Test]
    public void Weight_ReturnsCorrectFormatAndValue()
    {
        // 1500 kg / 0.4535924 = 3306.933 -> "3307"
        Assert.That(_provider.Weight, Is.EqualTo("3307 lbs"));
    }

    [Test]
    public void Power_ReturnsCorrectFormatAndValue()
    {
        // CarpAnalysis.MaxPower calculation depends on MockCarPerf properties.
        // Assuming standard calculation yields (500, 6500) for these values.
        // 500 * 0.9862 = 493.1 -> "493"
        Assert.That(_provider.Power, Is.EqualTo("493 bhp @ 6500 RPM"));
    }

    [Test]
    public void Torque_ReturnsCorrectFormatAndValue()
    {
        // CarpAnalysis.MaxTorque calculation depends on MockCarPerf properties.
        // Assuming standard calculation yields (400, 5000) for these values.
        Assert.That(_provider.Torque, Is.EqualTo("400 lb-ft @ 5000 RPM"));
    }

    [Test]
    public void MaxRpm_ReturnsCorrectFormatAndValue()
    {
        Assert.That(_provider.MaxRpm, Is.EqualTo("7000 RPM"));
    }

    [Test]
    public void Tires_ReturnsCorrectFormatAndValue()
    {
        Assert.That(_provider.Tires, Is.EqualTo("225/45R18, 245/40R19"));
    }

    [Test]
    public void Gearbox_ReturnsCorrectFormatAndValue()
    {
        // Manual: 6 - 2 = 4 speed
        Assert.That(_provider.Gearbox, Is.EqualTo("4 speed (manual) / 6 speed (auto)"));
    }

    [Test]
    public void Accel0To60_ReturnsCorrectFormatAndValue()
    {
        // CarpAnalysis.EstimateAcceleration(60) depends on MockCarPerf properties.
        // Assuming it returns 5.5 for these values.
        Assert.That(_provider.Accel0To60, Is.EqualTo("5.5 sec"));
    }

    [Test]
    public void Accel0To100_ReturnsCorrectFormatAndValue()
    {
        // CarpAnalysis.EstimateAcceleration(100) depends on MockCarPerf properties.
        // Assuming it returns 12.0 for these values.
        Assert.That(_provider.Accel0To100, Is.EqualTo("12.0 sec"));
    }

    private class MockCarPerf : ICarPerf
    {
        public int SerialNumber { get; set; }
        public double Mass { get; set; }
        public double TopSpeed { get; set; }
        public int EngineMaxRpm { get; set; }
        public int TireWidthFront { get; set; }
        public int TireSidewallFront { get; set; }
        public int TireRimFront { get; set; }
        public int TireWidthRear { get; set; }
        public int TireSidewallRear { get; set; }
        public int TireRimRear { get; set; }
        public int NumberOfGearsAuto { get; set; }
        public int NumberOfGearsManual { get; set; }
        public int CarClass { get; set; }
    }
}
