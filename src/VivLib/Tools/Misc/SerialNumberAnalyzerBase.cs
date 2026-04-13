using TheXDS.Vivianne.Info;
using TheXDS.Vivianne.Models;
using TheXDS.Vivianne.Models.Carp;
using TheXDS.Vivianne.Models.Fe;
using TheXDS.Vivianne.Serializers;
using CarpS3 = TheXDS.Vivianne.Serializers.Carp.Nfs3.CarpSerializer;
using CarpS4 = TheXDS.Vivianne.Serializers.Carp.Nfs4.CarpSerializer;
using FeS3 = TheXDS.Vivianne.Serializers.Fe.Nfs3.FeDataSerializer;
using FeS4 = TheXDS.Vivianne.Serializers.Fe.Nfs4.FeDataSerializer;

namespace TheXDS.Vivianne.Tools.Misc;

/// <summary>
/// Base class for the serial number analyzer's internal tools.
/// </summary>
public abstract class SerialNumberAnalyzerBase
{
    private protected static readonly FeS3 feS3 = new();
    private protected static readonly FeS4 feS4 = new();
    private protected static readonly CarpS3 carpS3 = new();
    private protected static readonly CarpS4 carpS4 = new();

    /// <summary>
    /// Reads an <see cref="IFeData"/> object from the specified raw data,
    /// automatically determining which serializer to use.
    /// </summary>
    /// <param name="rawFeData">Raw data to deserialize.</param>
    /// <returns>
    /// An <see cref="IFeData"/> object read form the specified raw data.
    /// </returns>
    protected static IFeData? ReadFeData(byte[] rawFeData)
    {
        return (VersionIdentifier.FeDataVersion(rawFeData) switch
        {
            NfsVersion.Nfs3 => feS3,
            NfsVersion.Nfs4 => feS4,
            _ => (IOutSerializer<IFeData>?)null
        })?.Deserialize(rawFeData);
    }

    /// <summary>
    /// Reads an <see cref="ICarPerf"/> object from the specified raw data,
    /// automatically determining which serializer to use.
    /// </summary>
    /// <param name="rawCarp">Raw data to deserialize.</param>
    /// <returns>
    /// An <see cref="ICarPerf"/> object read form the specified raw data.
    /// </returns>
    protected static ICarPerf? ReadCarp(byte[] rawCarp)
    {
        return (VersionIdentifier.CarpVersion(rawCarp) switch
        {
            NfsVersion.Nfs3 => carpS3,
            NfsVersion.Nfs4 => carpS4,
            _ => (IOutSerializer<ICarPerf>?)null
        })?.Deserialize(rawCarp);
    }

    /// <summary>
    /// Reads any of the supported formats that carry serial number information
    /// from the specified raw data.
    /// </summary>
    /// <param name="rawData">Raw data to deserialize.</param>
    /// <returns></returns>
    protected static ISerialNumberModel? Read(byte[] rawData)
    {
        return ReadFeData(rawData) ?? ReadCarp(rawData) as ISerialNumberModel;
    }
}
