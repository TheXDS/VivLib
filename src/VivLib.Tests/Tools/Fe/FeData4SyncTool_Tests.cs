using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TheXDS.Vivianne.Models.Carp.Nfs4;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs4;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Tools.Fe;

namespace TheXDS.Vivianne.Tests.Tools.Fe;

[TestFixture]
internal class FeData4SyncTool_Tests
{
    private Mock<ISerializer<FeData>> _feDataSerializerMock;
    private Mock<ISerializer<CarPerf>> _carpSerializerMock;
    private IDictionary<string, byte[]> _directory;

    [SetUp]
    public void SetUp()
    {
        _feDataSerializerMock = new Mock<ISerializer<FeData>>();
        _carpSerializerMock = new Mock<ISerializer<CarPerf>>();
        _directory = new Dictionary<string, byte[]>();
    }

    [Test]
    public void Sync_FeData_SyncsAllFeDataFiles()
    {
        // Arrange
        var source = new FeData
        {
            CarName = "Test",
            CarId = 1,
            SerialNumber = 100,
            VehicleClass = 0,
            PoliceFlag = false,
            Upgradable = false,
            Roof = 0,
            EngineLocation = 0,
            IsDlc = false,
            DefaultCompare = 0,
            CompareUpg1 = 0,
            CompareUpg2 = 0,
            CompareUpg3 = 0,
            IsBonus = false
        };

        var originalBytes = new byte[] { 1, 2, 3 };
        _directory["fedata.txt"] = originalBytes;
        _directory["fedata.dat"] = new byte[] { 4, 5, 6 };

        _feDataSerializerMock.Setup(s => s.Deserialize(It.IsAny<byte[]>())).Returns(new FeData());
        _feDataSerializerMock.Setup(s => s.Serialize(It.IsAny<FeData>())).Returns(originalBytes);

        // Act
        FeData4SyncTool.Sync(source, ".txt", _directory);

        // Assert
        Assert.That(_directory["fedata.txt"], Is.EqualTo(originalBytes));
        Assert.That(_directory["fedata.dat"], Is.EqualTo(originalBytes));
        
        _feDataSerializerMock.Verify(s => s.Deserialize(originalBytes), Times.Once);
        _feDataSerializerMock.Verify(s => s.Deserialize(new byte[] { 4, 5, 6 }), Times.Once);
    }

    [Test]
    public void Sync_FeData_SyncsCarpFiles()
    {
        // Arrange
        var source = new FeData
        {
            SerialNumber = 999,
            VehicleClass = 5
        };

        var carpBytes = new byte[] { 10, 20, 30 };
        _directory["carp.txt"] = carpBytes;
        _directory["carpsim.txt"] = new byte[] { 40, 50, 60 };

        var mockCarPerf = new Mock<CarPerf>();
        _carpSerializerMock.Setup(s => s.Deserialize(It.IsAny<byte[]>())).Returns(mockCarPerf.Object);
        _carpSerializerMock.Setup(s => s.Serialize(It.IsAny<CarPerf>())).Returns(carpBytes);

        // Act
        FeData4SyncTool.Sync(source, ".txt", _directory);

        // Assert
        _carpSerializerMock.Verify(s => s.Deserialize(carpBytes), Times.Once);
        _carpSerializerMock.Verify(s => s.Deserialize(new byte[] { 40, 50, 60 }), Times.Once);
        
        _carpSerializerMock.VerifySet(c => c.SerialNumber = 999, Times.Exactly(2));
        _carpSerializerMock.VerifySet(c => c.CarClass = 5, Times.Exactly(2));
    }

    [Test]
    public void Sync_CarPerf_SyncsFeDataFiles_WithPerformanceData()
    {
        // Arrange
        var source = new CarPerf
        {
            SerialNumber = 100,
            CarClass = 2,
            Weight = 1500,
            TopSpeed = 200,
            Power = 300,
            Torque = 400,
            MaxRpm = 7000,
            Tires = 1,
            Gearbox = 2,
            Accel0To60 = 5.5,
            Accel0To100 = 12.0
        };

        var fedataBytes = new byte[] { 1, 2, 3 };
        _directory["fedata.txt"] = fedataBytes;

        var mockFeData = new Mock<FeData>();
        _feDataSerializerMock.Setup(s => s.Deserialize(It.IsAny<byte[]>())).Returns(mockFeData.Object);
        _feDataSerializerMock.Setup(s => s.Serialize(It.IsAny<FeData>())).Returns(fedataBytes);

        // Act
        FeData4SyncTool.Sync(source, "carp.txt", _directory);

        // Assert
        _feDataSerializerMock.Verify(s => s.Deserialize(fedataBytes), Times.Once);
        _feDataSerializerMock.Verify(s => s.Serialize(It.IsAny<FeData>()), Times.Once);
    }

    [Test]
    public void Sync_CarPerf_ExcludesSourceCarpFile()
    {
        // Arrange
        var source = new CarPerf { SerialNumber = 100, CarClass = 2 };
        var carpBytes = new byte[] { 10, 20, 30 };
        _directory["carp.txt"] = carpBytes;
        _directory["carpsim.txt"] = new byte[] { 40, 50, 60 };

        var mockCarPerf = new Mock<CarPerf>();
        _carpSerializerMock.Setup(s => s.Deserialize(It.IsAny<byte[]>())).Returns(mockCarPerf.Object);
        _carpSerializerMock.Setup(s => s.Serialize(It.IsAny<CarPerf>())).Returns(carpBytes);

        // Act
        FeData4SyncTool.Sync(source, "carp.txt", _directory);

        // Assert
        _carpSerializerMock.Verify(s => s.Deserialize(carpBytes), Times.Never);
        _carpSerializerMock.Verify(s => s.Deserialize(new byte[] { 40, 50, 60 }), Times.Once);
    }

    [Test]
    public void Sync_CarPerf_DoesNotSyncPerformanceData_WhenSourceIsNotCarpTxt()
    {
        // Arrange
        var source = new CarPerf
        {
            SerialNumber = 100,
            CarClass = 2,
            Weight = 1500,
            TopSpeed = 200,
            Power = 300,
            Torque = 400,
            MaxRpm = 7000,
            Tires = 1,
            Gearbox = 2,
            Accel0To60 = 5.5,
            Accel0To100 = 12.0
        };

        var fedataBytes = new byte[] { 1, 2, 3 };
        _directory["fedata.txt"] = fedataBytes;

        var mockFeData = new Mock<FeData>();
        _feDataSerializerMock.Setup(s => s.Deserialize(It.IsAny<byte[]>())).Returns(mockFeData.Object);
        _feDataSerializerMock.Setup(s => s.Serialize(It.IsAny<FeData>())).Returns(fedataBytes);

        // Act
        FeData4SyncTool.Sync(source, "carpsim.txt", _directory);

        // Assert
        _feDataSerializerMock.Verify(s => s.Deserialize(fedataBytes), Times.Once);
        _feDataSerializerMock.Verify(s => s.Serialize(It.IsAny<FeData>()), Times.Once);
    }
}
