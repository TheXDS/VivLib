using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Viv;
using Carp3 = TheXDS.Vivianne.Models.Carp.Nfs3.CarPerf;
using Carp4 = TheXDS.Vivianne.Models.Carp.Nfs4.CarPerf;
using CarpS3 = TheXDS.Vivianne.Serializers.Carp.Nfs3.CarpSerializer;
using CarpS4 = TheXDS.Vivianne.Serializers.Carp.Nfs4.CarpSerializer;
using Fe3 = TheXDS.Vivianne.Models.Fe.Nfs3.FeData;
using Fe4 = TheXDS.Vivianne.Models.Fe.Nfs4.FeData;
using FeS3 = TheXDS.Vivianne.Serializers.Fe.Nfs3.FeDataSerializer;
using FeS4 = TheXDS.Vivianne.Serializers.Fe.Nfs4.FeDataSerializer;

namespace TheXDS.Vivianne.Tools.Misc;


public class SerialNumberAnalyzer
{
    private static ISerializer<VivFile> vivSerializer = new VivSerializer();
    private static readonly EnumerationOptions EnumOpts = new() { MatchCasing = MatchCasing.CaseInsensitive };

    public void FixSerials(DirectoryInfo carsDir)
    {
        HashSet<ushort> goodSerials = [];
        List<FileInfo> toFix = [];
        foreach (var file in carsDir.GetDirectories().SelectMany(p => p.GetFiles("car.viv", EnumOpts)))
        {
            using var fs = file.OpenRead();
            if (SerialNumberGetter.GetSerialNumber(vivSerializer.Deserialize(fs)) is { serial: { } sn, needsFixing: { } nf })
            {
                if (nf || !goodSerials.Add(sn))
                {
                    toFix.Add(file);
                }
            }
        }
        Random rnd = new();
        foreach (var j in toFix)
        {
            using var fs = j.OpenRead();
            var viv = vivSerializer.Deserialize(fs);
            ushort newSerial;
            do
            {
                newSerial = (ushort)rnd.Next(1, ushort.MaxValue);
            } while (!goodSerials.Add(newSerial));
            SerialNumberSetter.SetSerialNumber(viv, newSerial);
            using var wfs = j.OpenWrite();
            vivSerializer.SerializeTo(viv, wfs);
        }
    }
}

public abstract class SerialNumberAnalyzerBase
{
    private protected static readonly FeS3 feS3 = new();
    private protected static readonly FeS4 feS4 = new();
    private protected static readonly CarpS3 carpS3 = new();
    private protected static readonly CarpS4 carpS4 = new();

    protected static IFeData? ReadFeData(byte[] rawFeData)
    {
        return (VersionIdentifier.FeDataVersion(rawFeData) switch
        {
            NfsVersion.Nfs3 => feS3,
            NfsVersion.Nfs4 => feS4,
            _ => (IOutSerializer<IFeData>?)null
        })?.Deserialize(rawFeData);
    }

    protected static ICarPerf? ReadCarp(byte[] rawCarp)
    {
        return (VersionIdentifier.CarpVersion(rawCarp) switch
        {
            NfsVersion.Nfs3 => carpS3,
            NfsVersion.Nfs4 => carpS4,
            _ => (IOutSerializer<ICarPerf>?)null
        })?.Deserialize(rawCarp);
    }

    protected static ISerialNumberModel? Read(byte[] rawData)
    {
        return ReadFeData(rawData) ?? ReadCarp(rawData) as ISerialNumberModel;
    }
}

/// <summary>
/// Provides functionality to retrieve the serial number from a VIV file if it is consistent across all relevant
/// entries.
/// </summary>
/// <remarks>This class cannot be instantiated. Use the static method to obtain the serial number from a VIV file.
/// The class is intended for scenarios where determining a unique, consistent serial number from a VIV archive is
/// required.</remarks>
public class SerialNumberGetter : SerialNumberAnalyzerBase
{
    private SerialNumberGetter() { }

    /// <summary>
    /// Attempts to retrieve a consistent serial number from known data files within the specified VIV archive.
    /// </summary>
    /// <remarks>This method examines multiple known data files within the archive. If the files contain
    /// differing serial numbers or no valid serial number is found, the method returns null.</remarks>
    /// <param name="viv">The VIV archive to search for serial number information. Cannot be null.</param>
    /// <returns>The serial number if all relevant files contain the same nonzero serial number; otherwise, null.</returns>
    public static (ushort serial, bool needsFixing) GetSerialNumber(VivFile viv)
    {
        ArgumentNullException.ThrowIfNull(viv);
        List<ushort> serialNumbers = [];
        foreach (var j in FeDataBase.KnownExtensions.Select(j => $"fedata{j}").Concat(((string[])["", "sim", "1", "2", "3"]).Select(p => $"carp{p}.txt")))
        {
            if (viv.Directory.TryGetValue(j, out var rawBytes) && Read(rawBytes) is { } entity)
            {
                serialNumbers.Add(entity.SerialNumber);
            }
        }
        var needFixing = !serialNumbers.IsQuorum(serialNumbers.Count, out var serialNumber);
        return (serialNumber, needFixing);
    }
}

/// <summary>
/// Implements a tool that will set the serialNumber number of all internal files in a VIV file.
/// </summary>
public class SerialNumberSetter : SerialNumberAnalyzerBase
{
    private SerialNumberSetter() { }

    /// <summary>
    /// Sets the serialNumber number for specific entries in the provided <see cref="VivFile"/> directory.
    /// </summary>
    /// <remarks>This method updates the serialNumber number for entries in the <paramref name="viv"/> directory
    /// that match known extensions or specific file names. If an entry is found and successfully parsed, its serialNumber
    /// number is updated, and the modified data is written back to the directory.</remarks>
    /// <param name="viv">The <see cref="VivFile"/> containing the directory to update. Cannot be <see langword="null"/>.</param>
    /// <param name="serialNumber">The serialNumber number to assign to the relevant entries.</param>
    public static void SetSerialNumber(VivFile viv, ushort serialNumber)
    {
        ArgumentNullException.ThrowIfNull(viv);
        ProcessEntries(viv, serialNumber, FeDataBase.KnownExtensions.Select(j => $"fedata{j}"), ReadFeData);
        ProcessEntries(viv, serialNumber, ((string[])["", "sim", "1", "2", "3"]).Select(j => $"carp{j}.txt"), ReadCarp);
    }

    private static void ProcessEntries<T>(VivFile viv, ushort serialNumber, IEnumerable<string> fileNames, Func<byte[], T?> parser) where T : notnull, ISerialNumberModel
    {
        foreach (var j in fileNames)
        {
            if (viv.Directory.TryGetValue(j, out var rawBytes) && parser.Invoke(rawBytes) is { } entity)
            {
                entity.SerialNumber = serialNumber;
                viv.Directory[j] = Write(entity);
            }
        }
    }

    private static byte[] Write(ISerialNumberModel entity)
    {
        return entity switch
        {
            Fe3 f => ((IInSerializer<Fe3>)feS3).Serialize(f),
            Fe4 f => ((IInSerializer<Fe4>)feS4).Serialize(f),
            Carp3 f => ((IInSerializer<Carp3>)carpS3).Serialize(f),
            Carp4 f => ((IInSerializer<Carp4>)carpS4).Serialize(f),
            _ => [],
        };
    }
}
