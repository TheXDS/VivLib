using TheXDS.Vivianne.Models.Fce.Common;
using St = TheXDS.Vivianne.Resources.Strings.Info.Fce.FceInfoExtractor;

namespace TheXDS.Vivianne.Info.Fce;

/// <summary>
/// Implements an information extractor for <see cref="FcePart"/> entities.
/// </summary>
public class FcePartInfoExtractor : IEntityInfoExtractor<FcePart>
{
    /// <inheritdoc/>
    public string[] GetInfo(FcePart entity)
    {
        return [
            string.Format(St.FcePartNfo_Name, entity.Name),
            string.Format(St.FcePartNfo_Origin, entity.Origin.X, entity.Origin.Y, entity.Origin.Z),
            string.Format(St.FcePartNfo_Vertices, entity.Vertices.Length),
            string.Format(St.FcePartNfo_Triangles, entity.Triangles.Length)
        ];
    }
}
