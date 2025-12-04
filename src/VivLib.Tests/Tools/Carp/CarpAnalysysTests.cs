using TheXDS.Vivianne.Models.Carp;
using Moq;

namespace TheXDS.Vivianne.Tools.Carp;

[TestFixture]
internal class CarpAnalysisTests
{
    private Mock<ICarPerf> _carpMock;
    private CarpAnalysis _analysis;

    [SetUp]
    public void SetUp()
    {
        _carpMock = new Mock<ICarPerf>();
        _carpMock.Setup(p => p.Mass).Returns(3000.0);
        _carpMock.Setup(p => p.EngineMinRpm).Returns(500);
        _carpMock.Setup(p => p.EngineMaxRpm).Returns(6000);
        _carpMock.Setup(p => p.GearShiftDelay).Returns(2);
        _carpMock.Setup(p => p.FinalGearManual).Returns(3.0);
        _carpMock.Setup(p => p.TorqueCurve).Returns([ 150, 200, 250 ]);
        _carpMock.Setup(p => p.GearRatioManual).Returns([2.5, 3.0, 3.5]);
        _carpMock.Setup(p => p.GearEfficiencyManual).Returns([0.9, 0.95, 1.0]);
        _carpMock.Setup(p => p.TireWidthRear).Returns(205);
        _carpMock.Setup(p => p.TireSidewallRear).Returns(55);
        _carpMock.Setup(p => p.TireRimRear).Returns(16);
        _analysis = new CarpAnalysis(_carpMock.Object);
    }

    [Test]
    public void MaxTorque_Should_Return_Maximum_Torque_And_Rpm()
    {
        // Arrange
        (double Torque, int Rpm) = (250.0, 4500);

        using (Assert.EnterMultipleScope())
        {
            // Act & Assert
            Assert.That(_analysis.MaxTorque.Torque, Is.EqualTo(Torque).Within(0.0001));
            Assert.That(_analysis.MaxTorque.Rpm, Is.EqualTo(Rpm));
        }
    }

    [Test]
    public void MaxPower_Should_Return_Maximum_Power_And_Rpm()
    {
        // Arrange
        (double Hp, int Rpm) = (214.20411271896421, 4500);

        using (Assert.EnterMultipleScope())
        {
            // Act & Assert
            Assert.That(_analysis.MaxPower.Hp, Is.EqualTo(Hp).Within(0.0001));
            Assert.That(_analysis.MaxPower.Rpm, Is.EqualTo(Rpm));
        }
    }

    [Test]
    public void EstimateAcceleration_Should_Estimate_Time_To_Reach_Target_Speed()
    {
        // Arrange
        double targetSpeed = 60.0;

        // Act
        var estimatedTime = _analysis.EstimateAcceleration(targetSpeed);

        // Assert (Expected value is an approximation)
        Assert.That(estimatedTime, Is.EqualTo(11.39).Within(0.1));
    }

    [Test]
    public void EstimateAcceleration_Should_Include_Gear_Shift_Delay_If_Specified()
    {
        // Arrange
        double targetSpeed = 60.0; // MPH

        // Act
        var estimatedTimeWithDelay = _analysis.EstimateAcceleration(targetSpeed, withShiftDelay: true);
        var estimatedTimeWithoutDelay = _analysis.EstimateAcceleration(targetSpeed, withShiftDelay: false);

        // Assert (Expected value is an approximation)
        Assert.That(estimatedTimeWithDelay, Is.GreaterThanOrEqualTo(estimatedTimeWithoutDelay));
    }
}
