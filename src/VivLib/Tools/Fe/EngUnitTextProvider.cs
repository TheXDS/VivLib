using System.Globalization;
using TheXDS.Vivianne.Models.Carp;

namespace TheXDS.Vivianne.Tools.Fe;

/// <summary>
/// Implements a <see cref="FeDataTextProvider"/> for ENG fedata files.
/// </summary>
/// <param name="carp">Performance data source.</param>
public class EngUnitTextProvider(ICarPerf carp) : FeDataTextProvider(carp, CultureInfo.GetCultureInfo("en-US"))
{
    /// <inheritdoc/>
   public override string TopSpeed => $"{(CarpData.TopSpeed * 2.23693629206234).ToString("0", Culture)} MPH";

    /// <inheritdoc/>
    public override string Weight => $"{(CarpData.Mass * 2.20462262185).ToString("0", Culture)} lbs";

    /// <inheritdoc/>
    public override string Power
    {
        get
        {
            var (hp, rpm) = Analysis.MaxPower;
            return $"{(hp * 0.98632).ToString("0", Culture)} bhp @ {rpm.ToString(Culture)} RPM";
        }
    }
}
