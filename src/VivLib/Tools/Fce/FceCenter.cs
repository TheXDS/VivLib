using System.Numerics;
using TheXDS.MCART.Math;
using TheXDS.Vivianne.Models.Fce.Common;
using TheXDS.Vivianne.Tools.Base;

namespace TheXDS.Vivianne.Tools.Fce;

/// <summary>
/// Contains methods to help center FCE models.
/// </summary>
public class FceCenter : IInPlaceTransformTool<IFceFile<FcePart>>
{
    /// <summary>
    /// Centers an FCE model.
    /// </summary>
    /// <param name="fce">FCE model to center.</param>
    public static void Center(IFceFile<FcePart> fce)
    {
        var vertices = fce.SelectMany(p => p.TransformedVertices).ToArray();
        var minX = vertices.Min(p => p.X);
        var minY = vertices.Min(p => p.Y);
        var minZ = vertices.Min(p => p.Z);
        var xDiff = minX + fce.XHalfSize;
        var yDiff = minY + fce.YHalfSize;
        var zDiff = minZ + fce.ZHalfSize;
        if (((IEnumerable<float>)[xDiff, yDiff, zDiff]).AreZero()) return;
        var diffVector = new Vector3(xDiff, yDiff, zDiff);
        foreach (var j in fce.Parts) j.Origin -= diffVector;        
        foreach (var j in fce.Dummies) j.Position -= diffVector;        
    }

    /// <inheritdoc/>
    public Task<bool> TransformAsync(IFceFile<FcePart> item, IProgress<ProgressReport> progress, CancellationToken cancellationToken)
    {
        Center(item);
        return Task.FromResult(true);
    }
}
