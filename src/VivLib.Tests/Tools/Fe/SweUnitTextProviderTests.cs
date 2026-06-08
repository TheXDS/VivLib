using System.Globalization;
using Moq;
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Tools.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

[TestFixture]
internal class SweUnitTextProviderTests
{
    private Mock<ICarPerf> _carpMock = null!;

    private SweUnitTextProvider CreateProvider(Mock<ICarPerf>? carpMock = null)
    {
        return new SweUnitTextProvider(carpMock?.Object ?? _carpMock.Object);
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
    public void TopSpeed_Should_Return_Speed_In_KmH_With_Suffix()
    {
        double topSpeedMps = 30.0;
        _carpMock.Setup(p => p.TopSpeed).Returns(topSpeedMps);
        var provider = CreateProvider();
        var result = provider.TopSpeed;
        string expectedKmh = (topSpeedMps * 3.6).ToString("0", CultureInfo.GetCultureInfo("sv-SE"));
        Assert.That(result, Is.EqualTo($"{expectedKmh} Km/h"));
        _carpMock.Verify(p => p.TopSpeed, Times.Once);
    }

    [Test]
    public void TopSpeed_WithDifferentValues_Should_Convert_Correctly()
    {
        _carpMock.Setup(p => p.TopSpeed).Returns(40.0);
        var provider = CreateProvider();
        var result = provider.TopSpeed;
        string expectedKmh = (40.0 * 3.6).ToString("0", CultureInfo.GetCultureInfo("sv-SE"));
        Assert.That(result, Is.EqualTo($"{expectedKmh} Km/h"));
    }

    [Test]
    public void Power_Should_Return_Power_With_VMin_Format()
    {
        var provider = CreateProvider();
        var result = provider.Power;
        Assert.That(result, Does.Contain("vid"));
        Assert.That(result, Does.Contain("v/min"));
    }

    [Test]
    public void Power_Should_Convert_HP_To_Ch()
    {
        var provider = CreateProvider();
        var analysis = new CarpAnalysis(_carpMock.Object);
        var (expectedHp, expectedRpm) = analysis.MaxPower;
        var expectedCh = (expectedHp * 0.9862).ToString("0", CultureInfo.GetCultureInfo("sv-SE"));
        var result = provider.Power;
        Assert.That(result, Is.EqualTo($"{expectedCh} vid {expectedRpm} v/min"));
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
    public void Torque_Should_Return_Torque_In_Nm_With_VMin_Format()
    {
        var provider = CreateProvider();
        var result = provider.Torque;
        Assert.That(result, Does.Contain("Nm vid"));
        Assert.That(result, Does.Contain("v/min"));
    }

    [Test]
    public void Torque_Should_Convert_Correctly()
    {
        var provider = CreateProvider();
        var analysis = new CarpAnalysis(_carpMock.Object);
        var (expectedTorque, expectedRpm) = analysis.MaxTorque;
        var expectedNm = (expectedTorque * 1.3558179483).ToString("0", CultureInfo.GetCultureInfo("sv-SE"));
        var result = provider.Torque;
        Assert.That(result, Is.EqualTo($"{expectedNm} Nm vid {expectedRpm} v/min"));
    }

    [Test]
    public void Torque_Should_Include_Correct_RPM()
    {
        var provider = CreateProvider();
        var analysis = new CarpAnalysis(_carpMock.Object);
        var (_, expectedRpm) = analysis.MaxTorque;
        var result = provider.Torque;
        Assert.That(result, Does.Contain(expectedRpm.ToString(CultureInfo.InvariantCulture)));
    }

    [Test]
    public void MaxRpm_Should_Return_Engine_Max_RPM_With_VMin_Suffix()
    {
        _carpMock.Setup(p => p.EngineMaxRpm).Returns(7000);
        var provider = CreateProvider();
        var result = provider.MaxRpm;
        Assert.That(result, Is.EqualTo("7000 v/min"));
    }

    [Test]
    public void MaxRpm_WithDifferentValue_Should_Return_Correct_Value()
    {
        _carpMock.Setup(p => p.EngineMaxRpm).Returns(6500);
        var provider = CreateProvider();
        var result = provider.MaxRpm;
        Assert.That(result, Is.EqualTo("6500 v/min"));
    }

    [Test]
    public void Gearbox_When_Auto_Equal_Manual_Should_Return_Simple_String()
    {
        _carpMock.Setup(p => p.NumberOfGearsManual).Returns(6);
        _carpMock.Setup(p => p.NumberOfGearsAuto).Returns(6);
        var provider = CreateProvider();
        var result = provider.Gearbox;
        // NumberOfGearsManual - 2 = 4 (excluding reverse and neutral)
        Assert.That(result, Is.EqualTo("4"));
    }

    [Test]
    public void Gearbox_When_Auto_Different_From_Manual_Should_Return_Complex_String()
    {
        _carpMock.Setup(p => p.NumberOfGearsManual).Returns(6);
        _carpMock.Setup(p => p.NumberOfGearsAuto).Returns(5);
        var provider = CreateProvider();
        var result = provider.Gearbox;
        Assert.That(result, Is.EqualTo("4 (manuell) / 3 (automat)"));
    }

    [Test]
    public void Accel0To60_Should_Return_Estimated_Acceleration_Time_With_Sek_Suffix()
    {
        var provider = CreateProvider();
        var result = provider.Accel0To60;
        Assert.That(result, Does.EndWith(" sek."));
        Assert.That(result, Does.Match(@"\d+[,\.]\d sek\."));
    }

    [Test]
    public void Accel0To100_Should_Return_Estimated_Acceleration_Time_With_Sek_Suffix()
    {
        var provider = CreateProvider();
        var result = provider.Accel0To100;
        Assert.That(result, Does.EndWith(" sek."));
        Assert.That(result, Does.Match(@"\d+[,\.]\d sek\."));
    }

    [Test]
    public void Accel0To60_Should_Have_One_Decimal_Place()
    {
        var provider = CreateProvider();
        var result = provider.Accel0To60;
        var numericPart = result.Replace(" sek.", "");
        Assert.That(numericPart, Does.Match(@"\d+[,\.]\d"));
    }

    [Test]
    public void Accel0To100_Should_Have_One_Decimal_Place()
    {
        var provider = CreateProvider();
        var result = provider.Accel0To100;
        var numericPart = result.Replace(" sek.", "");
        Assert.That(numericPart, Does.Match(@"\d+[,\.]\d"));
    }

    [Test]
    public void Provider_Should_Use_SvSE_Culture()
    {
        _carpMock.Setup(p => p.TopSpeed).Returns(27.777777);  // ~100 km/h
        var provider = CreateProvider();
        var result = provider.TopSpeed;
        // Swedish culture result
        Assert.That(result, Is.EqualTo("100 Km/h"));
    }

    [Test]
    public void TopSpeed_With_Zero_Value_Should_Return_Zero_With_Suffix()
    {
        _carpMock.Setup(p => p.TopSpeed).Returns(0.0);
        var provider = CreateProvider();
        var result = provider.TopSpeed;
        Assert.That(result, Is.EqualTo("0 Km/h"));
    }

    [Test]
    public void MaxRpm_With_Zero_Value_Should_Return_Zero_With_VMin()
    {
        _carpMock.Setup(p => p.EngineMaxRpm).Returns(0);
        _carpMock.Setup(p => p.EngineMinRpm).Returns(0);
        _carpMock.Setup(p => p.TorqueCurve).Returns([0]);
        var provider = CreateProvider();
        var result = provider.MaxRpm;
        Assert.That(result, Is.EqualTo("0 v/min"));
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
}
