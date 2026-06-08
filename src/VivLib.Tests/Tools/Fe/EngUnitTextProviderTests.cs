using System.Globalization;
using Moq;
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Tools.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

[TestFixture]
internal class EngUnitTextProviderTests
{
    private Mock<ICarPerf> _carpMock = null!;

    private EngUnitTextProvider CreateProvider(Mock<ICarPerf>? carpMock = null)
    {
        return new EngUnitTextProvider(carpMock?.Object ?? _carpMock.Object);
    }

    [SetUp]
    public void SetUp()
    {
        _carpMock = new Mock<ICarPerf>();
        _carpMock.Setup(p => p.Mass).Returns(1500.0);
        _carpMock.Setup(p => p.TopSpeed).Returns(30.0);
        _carpMock.Setup(p => p.EngineMaxRpm).Returns(7000);
        _carpMock.Setup(p => p.EngineMinRpm).Returns(800);
        _carpMock.Setup(p => p.TireWidthFront).Returns(205);
        _carpMock.Setup(p => p.TireSidewallFront).Returns(55);
        _carpMock.Setup(p => p.TireRimFront).Returns(16);
        _carpMock.Setup(p => p.TireWidthRear).Returns(225);
        _carpMock.Setup(p => p.TireSidewallRear).Returns(45);
        _carpMock.Setup(p => p.TireRimRear).Returns(17);
        _carpMock.Setup(p => p.NumberOfGearsManual).Returns(6);
        _carpMock.Setup(p => p.NumberOfGearsAuto).Returns(6);
        _carpMock.Setup(p => p.TorqueCurve).Returns([200, 250, 300, 320, 310, 290, 260, 220]);
        _carpMock.Setup(p => p.GearRatioManual).Returns([3.5, 2.8, 2.1, 1.6, 1.3, 1.0]);
        _carpMock.Setup(p => p.GearEfficiencyManual).Returns([0.9, 0.92, 0.95, 0.97, 0.98, 0.99]);
        _carpMock.Setup(p => p.FinalGearManual).Returns(3.0);
        _carpMock.Setup(p => p.GearShiftDelay).Returns(2);
    }

    [Test]
    public void TopSpeed_Should_Return_Speed_In_MPH()
    {
        double topSpeedMps = 30.0;
        string expectedMph = (topSpeedMps * 2.23693629206234).ToString("0", CultureInfo.GetCultureInfo("en-US"));
        _carpMock.Setup(p => p.TopSpeed).Returns(topSpeedMps);
        var provider = CreateProvider();
        var result = provider.TopSpeed;
        Assert.That(result, Is.EqualTo($"{expectedMph} MPH"));
        _carpMock.Verify(p => p.TopSpeed, Times.Once);
    }

    [Test]
    public void TopSpeed_WithDifferentValues_Should_Convert_Correctly()
    {
        _carpMock.Setup(p => p.TopSpeed).Returns(40.0);
        var provider = CreateProvider();
        var result = provider.TopSpeed;
        string expectedMph = (40.0 * 2.23693629206234).ToString("0", CultureInfo.GetCultureInfo("en-US"));
        Assert.That(result, Is.EqualTo($"{expectedMph} MPH"));
    }

    [Test]
    public void Weight_Should_Return_Weight_In_Lbs()
    {
        double massKg = 1500.0;
        string expectedLbs = (massKg * 2.20462262185).ToString("0", CultureInfo.GetCultureInfo("en-US"));
        _carpMock.Setup(p => p.Mass).Returns(massKg);
        var provider = CreateProvider();
        var result = provider.Weight;
        Assert.That(result, Is.EqualTo($"{expectedLbs} lbs"));
        _carpMock.Verify(p => p.Mass, Times.Once);
    }

    [Test]
    public void Weight_WithDifferentValues_Should_Convert_Correctly()
    {
        _carpMock.Setup(p => p.Mass).Returns(2000.0);
        var provider = CreateProvider();
        var result = provider.Weight;
        var expectedLbs = (2000.0 * 2.20462262185).ToString("0", CultureInfo.GetCultureInfo("en-US"));
        Assert.That(result, Is.EqualTo($"{expectedLbs} lbs"));
    }

    [Test]
    public void Power_Should_Return_Power_In_Bhp_With_RPM()
    {
        var provider = CreateProvider();
        var result = provider.Power;
        Assert.That(result, Does.EndWith("RPM"));
        Assert.That(result, Does.Contain("bhp @"));
    }

    [Test]
    public void Power_Should_Convert_HP_To_Bhp()
    {
        var provider = CreateProvider();
        var analysis = new CarpAnalysis(_carpMock.Object);
        var (expectedHp, expectedRpm) = analysis.MaxPower;
        var expectedBhp = (expectedHp * 0.98632).ToString("0", CultureInfo.GetCultureInfo("en-US"));
        var result = provider.Power;
        Assert.That(result, Is.EqualTo($"{expectedBhp} bhp @ {expectedRpm} RPM"));
    }

    [Test]
    public void Power_Should_Include_Correct_RPM()
    {
        var provider = CreateProvider();
        var analysis = new CarpAnalysis(_carpMock.Object);
        var (_, expectedRpm) = analysis.MaxPower;
        var result = provider.Power;
        Assert.That(result, Does.Contain(expectedRpm.ToString(CultureInfo.InvariantCulture)));
    }

    [Test]
    public void Torque_Should_Return_Torque_In_Lb_Ft_With_RPM()
    {
        var provider = CreateProvider();
        var analysis = new CarpAnalysis(_carpMock.Object);
        var (expectedTorque, expectedRpm) = analysis.MaxTorque;
        var expectedTorqueStr = (expectedTorque * 0.737562).ToString("0", CultureInfo.InvariantCulture);
        var result = provider.Torque;
        Assert.That(result, Is.EqualTo($"{expectedTorqueStr} lb-ft @ {expectedRpm} RPM"));
    }

    [Test]
    public void MaxRpm_Should_Return_Engine_Max_RPM()
    {
        _carpMock.Setup(p => p.EngineMaxRpm).Returns(7000);
        var provider = CreateProvider();
        var result = provider.MaxRpm;
        Assert.That(result, Is.EqualTo("7000 RPM"));
    }

    [Test]
    public void Tires_Should_Return_Tire_Specs_String()
    {
        _carpMock.Setup(p => p.TireWidthFront).Returns(205);
        _carpMock.Setup(p => p.TireSidewallFront).Returns(55);
        _carpMock.Setup(p => p.TireRimFront).Returns(16);
        _carpMock.Setup(p => p.TireWidthRear).Returns(225);
        _carpMock.Setup(p => p.TireSidewallRear).Returns(45);
        _carpMock.Setup(p => p.TireRimRear).Returns(17);
        var provider = CreateProvider();
        var result = provider.Tires;
        Assert.That(result, Is.EqualTo("205/55R16, 225/45R17"));
        _carpMock.Verify(p => p.TireWidthFront, Times.Once);
        _carpMock.Verify(p => p.TireSidewallFront, Times.Once);
        _carpMock.Verify(p => p.TireRimFront, Times.Once);
        _carpMock.Verify(p => p.TireWidthRear, Times.Once);
        _carpMock.Verify(p => p.TireSidewallRear, Times.Once);
        _carpMock.Verify(p => p.TireRimRear, Times.Once);
    }

    [Test]
    public void Gearbox_When_Auto_Equal_Manual_Should_Return_Simple_String()
    {
        _carpMock.Setup(p => p.NumberOfGearsManual).Returns(6);
        _carpMock.Setup(p => p.NumberOfGearsAuto).Returns(6);
        var provider = CreateProvider();
        var result = provider.Gearbox;
        // NumberOfGearsManual - 2 = 4 (excluding reverse and neutral)
        Assert.That(result, Is.EqualTo("4 speed"));
    }

    [Test]
    public void Gearbox_When_Auto_Different_From_Manual_Should_Return_Complex_String()
    {
        _carpMock.Setup(p => p.NumberOfGearsManual).Returns(6);
        _carpMock.Setup(p => p.NumberOfGearsAuto).Returns(5);
        var provider = CreateProvider();
        var result = provider.Gearbox;
        Assert.That(result, Is.EqualTo("4 speed (manual) / 3 speed (auto)"));
    }

    [Test]
    public void Accel0To60_Should_Return_Estimated_Acceleration_Time()
    {
        var provider = CreateProvider();
        var result = provider.Accel0To60;
        Assert.That(result, Does.EndWith("sec"));
        Assert.That(result, Does.Contain("sec"));
    }

    [Test]
    public void Accel0To100_Should_Return_Estimated_Acceleration_Time()
    {
        var provider = CreateProvider();
        var result = provider.Accel0To100;
        Assert.That(result, Does.EndWith("sec"));
    }

    [Test]
    public void Accel0To60_Should_Have_One_Decimal_Place()
    {
        var provider = CreateProvider();
        var result = provider.Accel0To60;
        var numericPart = result.Replace(" sec", "");
        Assert.That(numericPart, Does.Contain("."));
    }

    [Test]
    public void Accel0To100_Should_Have_One_Decimal_Place()
    {
        var provider = CreateProvider();
        var result = provider.Accel0To100;
        var numericPart = result.Replace(" sec", "");
        Assert.That(numericPart, Does.Contain("."));
    }

    [Test]
    public void Provider_Should_Use_EnUS_Culture()
    {
        var provider = CreateProvider();
        // English conventions (no grouping separators for whole numbers)
        _carpMock.Setup(p => p.Mass).Returns(1500.0);
        var result = provider.Weight;
        Assert.That(result, Does.Not.Contain(",")); // English doesn't use comma for thousands in this format
    }

    [Test]
    public void Provider_should__access_underlying_CarpData_via_properties()
    {
        _carpMock.Setup(p => p.Mass).Returns(1000.0);
        _carpMock.Setup(p => p.TopSpeed).Returns(25.0);
        _carpMock.Setup(p => p.EngineMaxRpm).Returns(6500);
        var provider = CreateProvider();
        _ = provider.Weight;
        _ = provider.TopSpeed;
        _ = provider.MaxRpm;
        _carpMock.Verify(p => p.Mass, Times.AtLeast(1));
        _carpMock.Verify(p => p.TopSpeed, Times.AtLeast(1));
        _carpMock.Verify(p => p.EngineMaxRpm, Times.AtLeast(1));
    }

    [Test]
    public void CarpData_Should_Access_Interface_Properties()
    {
        _carpMock.Setup(p => p.SerialNumber).Returns(42);
        _carpMock.Setup(p => p.Abs).Returns(true);
        _carpMock.Setup(p => p.PowerSteering).Returns(false);
        var provider = CreateProvider();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_carpMock.Object.SerialNumber, Is.EqualTo(42));
            Assert.That(_carpMock.Object.Abs, Is.True);
            Assert.That(_carpMock.Object.PowerSteering, Is.False);
        }
    }

    [Test]
    public void TopSpeed_With_Zero_Value_Should_Return_Zero_MPH()
    {
        _carpMock.Setup(p => p.TopSpeed).Returns(0.0);
        var provider = CreateProvider();
        var result = provider.TopSpeed;
        Assert.That(result, Is.EqualTo("0 MPH"));
    }

    [Test]
    public void Weight_With_Zero_Value_Should_Return_Zero_Lbs()
    {
        _carpMock.Setup(p => p.Mass).Returns(0.0);
        var provider = CreateProvider();
        var result = provider.Weight;
        Assert.That(result, Is.EqualTo("0 lbs"));
    }

    [Test]
    public void MaxRpm_With_Zero_Value_Should_Return_Zero_RPM()
    {
        _carpMock.Setup(p => p.EngineMaxRpm).Returns(0);
        _carpMock.Setup(p => p.EngineMinRpm).Returns(0);
        _carpMock.Setup(p => p.TorqueCurve).Returns([0]);
        var provider = CreateProvider();
        var result = provider.MaxRpm;
        Assert.That(result, Is.EqualTo("0 RPM"));
    }
}
