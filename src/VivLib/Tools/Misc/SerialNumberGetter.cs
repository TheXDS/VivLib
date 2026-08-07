using TheXDS.MCART.Types.Extensions;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Models.Viv;

namespace TheXDS.Vivianne.Tools.Misc;

/// <summary>
/// Provides functionality to retrieve the serial number from a VIV file if it
/// is consistent across all relevant entries.
/// </summary>
/// <remarks>
/// This class cannot be instantiated. Use the static method to obtain the
/// serial number from a VIV file.
/// The class is intended for scenarios where determining a unique, consistent
/// serial number from a VIV archive is required.
/// </remarks>
public class SerialNumberGetter : SerialNumberAnalyzerBase
{
    private SerialNumberGetter() { }

    /// <summary>
    /// Attempts to retrieve a consistent serial number from known data files
    /// within the specified VIV archive.
    /// </summary>
    /// <param name="viv">
    /// The VIV archive to search for serial number information. Cannot be
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The serial number if all relevant files contain the same nonzero serial
    /// number, alongside a value that determines if the serial needs fixing.
    /// </returns>
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
        var needFixing = !serialNumbers.IsQuorum(serialNumbers.Count, out var serialNumber) || serialNumber == 0;
        return (serialNumber, needFixing);
    }
}
