using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers;
using Carp3 = TheXDS.Vivianne.Models.Carp.Nfs3.CarPerf;
using Carp4 = TheXDS.Vivianne.Models.Carp.Nfs4.CarPerf;
using Fe3 = TheXDS.Vivianne.Models.Fe.Nfs3.FeData;
using Fe4 = TheXDS.Vivianne.Models.Fe.Nfs4.FeData;

namespace TheXDS.Vivianne.Tools.Misc;

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
