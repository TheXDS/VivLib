using TheXDS.MCART.Helpers;
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers;
using TheXDS.Vivianne.Serializers.Viv;
using TheXDS.Vivianne.Tools.Base;

namespace TheXDS.Vivianne.Tools.Misc;

/// <summary>
/// Helper class that allows the user to scan and fix the serial numbers for cars in NFS3/4.
/// </summary>
public static class SerialNumberAnalyzer
{
    private static readonly ISerializer<VivFile> vivSerializer = new VivSerializer();
    private static readonly EnumerationOptions EnumOpts = new() { MatchCasing = MatchCasing.CaseInsensitive };

    /// <summary>
    /// Scans the specified directory and fixes all serial numbers in each .VIV file.
    /// </summary>
    /// <param name="carsDir">Directory path where the cars are stored. This value will be <c>GAMEDATA\CARMODEL</c> for NFS3 and <c>DATA\CARS</c> for NFS4.</param>
    /// <param name="progress">Object used to report progress on the operation.</param>
    public static void FixSerials(DirectoryInfo carsDir, IProgress<ProgressReport> progress)
    {
        HashSet<ushort> goodSerials = [];
        List<FileInfo> toFix = [];
        List<FileInfo> toScan = [..carsDir.GetDirectories().SelectMany(p => p.GetFiles("car.viv", EnumOpts))];

        foreach (var (index, file) in toScan.WithIndex())
        {
            try
            {
                progress.Report(new(index * 100.0 / toScan.Count, $"Reading {file.FullName}..."));

                using var fs = file.OpenRead();
                if (SerialNumberGetter.GetSerialNumber(vivSerializer.Deserialize(fs)) is { serial: { } sn, needsFixing: { } nf })
                {
                    if (nf || !goodSerials.Add(sn))
                    {
                        toFix.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                progress.Report(new(index * 100.0 / toScan.Count, $"Could not read {file.FullName}: {ex.Message}. Skipping..."));
            }
        }        
        Random rnd = new();
        foreach (var (index, j) in toFix.WithIndex())
        {
            try
            {
                progress.Report(new(index * 100.0 / toFix.Count, $"Fixing SN on {j.FullName}..."));
                VivFile viv;
                using (var fs = j.OpenRead())
                {   
                    viv = vivSerializer.Deserialize(fs);
                }
                ushort newSerial;
                do
                {
                    newSerial = (ushort)rnd.Next(1, ushort.MaxValue);
                } while (!goodSerials.Add(newSerial));
                SerialNumberSetter.SetSerialNumber(viv, newSerial);
                using (var wfs = j.OpenWrite())
                {
                    vivSerializer.SerializeTo(viv, wfs);
                }
            }
            catch (Exception ex)
            {
                progress.Report(new(index * 100.0 / toFix.Count, $"Could not write {j.FullName}: {ex.Message}. Skipping..."));
            }
        }
    }
}
