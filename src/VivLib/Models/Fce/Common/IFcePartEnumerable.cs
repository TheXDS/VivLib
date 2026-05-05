namespace TheXDS.Vivianne.Models.Fce.Common;

/// <summary>
/// Defines a set of members to be implemented by a type that represents the
/// essential contents of an FCE file, with a specific type of Part.
/// </summary>
/// <typeparam name="TPart"></typeparam>
public interface IFcePartEnumerable<out TPart> : IFceFile where TPart : FcePart
{
    /// <summary>
    /// Gets anenumeration of all defined Parts in the FCE.
    /// </summary>
    /// <remarks>
    /// This table should never exceed <c>64</c>
    /// <typeparamref name="TPart"/> elements.
    /// elements.
    /// </remarks>
    IEnumerable<TPart> Parts { get; }
}
