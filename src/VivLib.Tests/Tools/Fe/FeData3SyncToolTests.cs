using TheXDS.Vivianne.Models.Carp.Nfs3;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Fe.Nfs3;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Carp.Nfs3;
using TheXDS.Vivianne.Serializers.Fe.Nfs3;

namespace TheXDS.Vivianne.Tools.Fe;

[TestFixture]
internal class FeData3SyncToolTests
{
    [Test]
    public void Sync_FromFeData_CopiesBasicPropertiesToAllFeDataFiles()
    {
        var source = new FeData
        {
            CarName = "TestCar",
            CarId = "TCID",
            SerialNumber = 42,
            VehicleClass = CarClass.A,
            Seat = DriverSeatPosition.Left,
            IsPolice = true,
            IsBonus = false,
            AvailableToAi = true,
            IsDlc = 0,
        };
        var targetFeData = new FeData
        {
            CarName = "OldCar",
            CarId = "OCID",
            SerialNumber = 99,
            VehicleClass = CarClass.C,
        };
        ISerializer<FeData> serializer = new FeDataSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.eng", serializer.Serialize(source) },
            { "fedata.bri", serializer.Serialize(targetFeData) }
        };
        FeData3SyncTool.Sync(source, ".eng", directory);
        var result = serializer.Deserialize(directory["fedata.bri"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.CarName, Is.EqualTo(source.CarName));
            Assert.That(result.CarId, Is.EqualTo(source.CarId));
            Assert.That(result.SerialNumber, Is.EqualTo(source.SerialNumber));
            Assert.That(result.VehicleClass, Is.EqualTo(source.VehicleClass));
            Assert.That(result.IsBonus, Is.EqualTo(source.IsBonus));
        }
    }

    [Test]
    public void Sync_FromFeData_CopiesUnknownValuesToAllFeDataFiles()
    {
        var source = new FeData
        {
            CarId = "TCID",
            Unk_0x0c = 1,
            Unk_0x14 = 2,
            Unk_0x16 = 3,
            Unk_0x1a = 4,
            Unk_0x1c = 5,
            Unk_0x1e = 6,
            Unk_0x20 = 7,
            Unk_0x22 = 8,
            Unk_0x24 = 9,
            Unk_0x26 = 10,
            Unk_0x2c = 11,
        };
        var targetFeData = new FeData
        {
            CarId = "OCID",
            Unk_0x0c = 99,
        };
        ISerializer<FeData> serializer = new FeDataSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.bri", serializer.Serialize(source) },
            { "fedata.fre", serializer.Serialize(targetFeData) }
        };
        FeData3SyncTool.Sync(source, ".bri", directory);
        var result = serializer.Deserialize(directory["fedata.fre"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Unk_0x0c, Is.EqualTo(source.Unk_0x0c));
            Assert.That(result.Unk_0x14, Is.EqualTo(source.Unk_0x14));
            Assert.That(result.Unk_0x1e, Is.EqualTo(source.Unk_0x1e));
            Assert.That(result.Unk_0x26, Is.EqualTo(source.Unk_0x26));
        }
    }

    [Test]
    public void Sync_FromFeData_CopiesPerformanceDataToAllFeDataFiles()
    {
        var source = new FeData
        {
            CarId = "TCID",
            CarAccel = 9,
            CarTopSpeed = 18,
            CarHandling = 8,
            CarBraking = 8,
        };
        var targetFeData = new FeData
        {
            CarId = "OCID",
            CarAccel = 0,
            CarTopSpeed = 0,
        };
        ISerializer<FeData> serializer = new FeDataSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.fre", serializer.Serialize(source) },
            { "fedata.spa", serializer.Serialize(targetFeData) }
        };
        FeData3SyncTool.Sync(source, ".fre", directory);
        var result = serializer.Deserialize(directory["fedata.spa"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.CarAccel, Is.EqualTo(source.CarAccel));
            Assert.That(result.CarTopSpeed, Is.EqualTo(source.CarTopSpeed));
            Assert.That(result.CarHandling, Is.EqualTo(source.CarHandling));
            Assert.That(result.CarBraking, Is.EqualTo(source.CarBraking));
        }
    }

    [Test]
    public void Sync_FromFeData_ExcludesSourceExtension()
    {
        var source = new FeData { CarId = "TCD" };
        var feDataEng = new FeData { CarId = "OLD" };
        var feDataBri = new FeData { CarId = "OLD" };
        ISerializer<FeData> serializer = new FeDataSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.eng", serializer.Serialize(feDataEng) },
            { "fedata.bri", serializer.Serialize(feDataBri) }
        };
        FeData3SyncTool.Sync(source, ".eng", directory);
        var resultEng = serializer.Deserialize(directory["fedata.eng"]);
        var resultBri = serializer.Deserialize(directory["fedata.bri"]);

        using (Assert.EnterMultipleScope())
        {
            // Source extension should not be synced - CarId should remain OLD
            Assert.That(resultEng.CarId.TrimEnd('\0'), Is.EqualTo("OLD"));
            // Other extension should be synced to source value
            Assert.That(resultBri.CarId.TrimEnd('\0'), Is.EqualTo(source.CarId));
        }
    }

    [Test]
    public void Sync_FromFeData_SyncsToKnownCarpFiles()
    {
        var source = new FeData
        {
            CarId = "TCID",
            SerialNumber = 42,
            VehicleClass = CarClass.B,
        };
        var carp = new CarPerf
        {
            SerialNumber = 99,
            CarClass = CarClass.C,
        };
        ISerializer<FeData> feDataSerializer = new FeDataSerializer();
        ISerializer<CarPerf> carpSerializer = new CarpSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.eng", feDataSerializer.Serialize(new FeData()) },
            { "carp.txt", carpSerializer.Serialize(carp) }
        };
        FeData3SyncTool.Sync(source, ".eng", directory);
        var resultCarp = carpSerializer.Deserialize(directory["carp.txt"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultCarp.SerialNumber, Is.EqualTo(source.SerialNumber));
            Assert.That(resultCarp.CarClass, Is.EqualTo(source.VehicleClass));
        }
    }

    [Test]
    public void Sync_FromFeData_SyncsToAllKnownCarpFiles()
    {
        var source = new FeData
        {
            CarId = "TCID",
            SerialNumber = 55,
            VehicleClass = CarClass.C,
        };
        var carpSimData = new CarPerf
        {
            SerialNumber = 1,
            CarClass = CarClass.A,
        };
        ISerializer<FeData> feDataSerializer = new FeDataSerializer();
        ISerializer<CarPerf> carpSerializer = new CarpSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.eng", feDataSerializer.Serialize(new FeData()) },
            { "carp.txt", carpSerializer.Serialize(new CarPerf()) },
            { "carpsim.txt", carpSerializer.Serialize(carpSimData) }
        };
        FeData3SyncTool.Sync(source, ".eng", directory);
        var resultCarpSim = carpSerializer.Deserialize(directory["carpsim.txt"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultCarpSim.SerialNumber, Is.EqualTo(source.SerialNumber));
            Assert.That(resultCarpSim.CarClass, Is.EqualTo(source.VehicleClass));
        }
    }

    [Test]
    public void Sync_FromFeData_IgnoresMissingFeDataFiles()
    {
        var source = new FeData { CarId = "TCID" };
        ISerializer<FeData> feDataSerializer = new FeDataSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.eng", feDataSerializer.Serialize(new FeData()) }
        };
        Assert.DoesNotThrow((Action)(() =>
        {
            FeData3SyncTool.Sync(source, ".eng", directory);
        }));
    }

    [Test]
    public void Sync_FromFeData_IgnoresMissingCarpFiles()
    {
        var source = new FeData { CarId = "TCID" };
        ISerializer<FeData> feDataSerializer = new FeDataSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.eng", feDataSerializer.Serialize(new FeData()) }
        };
        Assert.DoesNotThrow((Action)(() =>
        {
            FeData3SyncTool.Sync(source, ".eng", directory);
        }));
    }

    [Test]
    public void Sync_FromCarPerf_CopiesSerialNumberAndClassToAllFeDataFiles()
    {
        var source = new CarPerf
        {
            SerialNumber = 77,
            CarClass = CarClass.A,
            EngineMinRpm = 500,
            EngineMaxRpm = 6000,
            Mass = 1200,
        };
        source.TorqueCurve.Add(100);
        source.TorqueCurve.Add(150);
        source.TorqueCurve.Add(200);
        var feDataTarget = new FeData
        {
            SerialNumber = 1,
            VehicleClass = CarClass.C,
        };
        ISerializer<FeData> feDataSerializer = new FeDataSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "fedata.eng", feDataSerializer.Serialize(feDataTarget) },
            { "fedata.bri", feDataSerializer.Serialize(new FeData()) }
        };
        FeData3SyncTool.Sync(source, "carp.txt", directory);
        var resultEng = feDataSerializer.Deserialize(directory["fedata.eng"]);
        var resultBri = feDataSerializer.Deserialize(directory["fedata.bri"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultEng.SerialNumber, Is.EqualTo(source.SerialNumber));
            Assert.That(resultEng.VehicleClass, Is.EqualTo(source.CarClass));
            Assert.That(resultBri.SerialNumber, Is.EqualTo(source.SerialNumber));
            Assert.That(resultBri.VehicleClass, Is.EqualTo(source.CarClass));
        }
    }

    [Test]
    public void Sync_FromCarPerf_SyncsToKnownCarpFiles()
    {
        var source = new CarPerf
        {
            SerialNumber = 88,
            CarClass = CarClass.C,
        };

        var carpData = new CarPerf
        {
            SerialNumber = 1,
            CarClass = CarClass.A,
        };

        ISerializer<CarPerf> carpSerializer = new CarpSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "carp.txt", carpSerializer.Serialize(new CarPerf()) },
            { "carpsim.txt", carpSerializer.Serialize(carpData) }
        };
        FeData3SyncTool.Sync(source, "carp.txt", directory);
        var resultCarpSim = carpSerializer.Deserialize(directory["carpsim.txt"]);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultCarpSim.SerialNumber, Is.EqualTo(source.SerialNumber));
            Assert.That(resultCarpSim.CarClass, Is.EqualTo(source.CarClass));
        }
    }

    [Test]
    public void Sync_FromCarPerf_ExcludesSourceCarpFile()
    {
        var source = new CarPerf
        {
            SerialNumber = 77,
            CarClass = CarClass.B,
        };
        var carpTxtData = new CarPerf
        {
            SerialNumber = 99,
            CarClass = CarClass.A,
        };
        var carpSimData = new CarPerf
        {
            SerialNumber = 88,
            CarClass = CarClass.B,
        };
        ISerializer<CarPerf> carpSerializer = new CarpSerializer();
        var directory = new Dictionary<string, byte[]>
        {
            { "carp.txt", carpSerializer.Serialize(carpTxtData) },
            { "carpsim.txt", carpSerializer.Serialize(carpSimData) }
        };
        FeData3SyncTool.Sync(source, "carpsim.txt", directory);
        var resultCarpTxt = carpSerializer.Deserialize(directory["carp.txt"]);
        var resultCarpSim = carpSerializer.Deserialize(directory["carpsim.txt"]);
        using (Assert.EnterMultipleScope())
        {
            // carp.txt should be updated
            Assert.That(resultCarpTxt.SerialNumber, Is.EqualTo(source.SerialNumber));
            // carpsim.txt should not be updated
            Assert.That(resultCarpSim.SerialNumber, Is.EqualTo(88));
        }
    }

    [Test]
    public void Sync_FromCarPerf_HandlesMissingCarpFiles()
    {
        var source = new CarPerf
        {
            SerialNumber = 50,
            CarClass = CarClass.B,
        };
        var directory = new Dictionary<string, byte[]>();
        Assert.DoesNotThrow((Action)(() =>
        {
            FeData3SyncTool.Sync(source, "carp.txt", directory);
        }));
    }
}
