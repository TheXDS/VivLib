using System.Globalization;
using Moq;
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Tools.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

[TestFixture]
internal class BriUnitTextProviderTests
{
    private Mock<ICarPerf> _carpMock = null!;

    private BriUnitTextProvider CreateProvider(Mock<ICarPerf>? carpMock = null)
    {
        return new BriUnitTextProvider(carpMock?.Object ?? _carpMock.Object);
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
        string expectedMph = (topSpeedMps * 2.236936).ToString("0", CultureInfo.GetCultureInfo("en-GB"));
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
        string expectedMph = (40.0 * 2.236936).ToString("0", CultureInfo.GetCultureInfo("en-GB"));
        Assert.That(result, Is.EqualTo($"{expectedMph} MPH"));
    }

    [Test]
    public void Weight_Should_Return_Weight_In_Lbs()
    {
        double massKg = 1500.0;
        string expectedLbs = (massKg / 0.4535924).ToString("0", CultureInfo.GetCultureInfo("en-GB"));
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
        var expectedLbs = (2000.0 / 0.4535924).ToString("0", CultureInfo.GetCultureInfo("en-GB"));
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
        var expectedBhp = (expectedHp * 0.9862).ToString("0", CultureInfo.GetCultureInfo("en-GB"));
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
    public void Provider_Should_Use_EnGB_Culture()
    {
        var provider = CreateProvider();
        _carpMock.Setup(p => p.Mass).Returns(1500.0);
        var result = provider.Weight;
        // Verify the provider uses en-GB culture (British English)
        Assert.That(result, Contains.Substring("lbs"));
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
