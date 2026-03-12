namespace TheXDS.Vivianne.Tools.Base;

/// <summary>
/// Defines a contract for tools that perform in-place transformations on
/// objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of object to transform.</typeparam>
public interface IInPlaceTransformTool<T>
{
    /// <summary>
    /// Transforms the specified item in-place asynchronously.
    /// </summary>
    /// <param name="item">The item to transform.</param>
    /// <param name="progress">
    /// A progress reporter to track transformation progress.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to cancel the operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result
    /// indicates whether the transformation was successful.
    /// </returns>
    Task<bool> TransformAsync(T item, IProgress<double> progress, CancellationToken cancellationToken);
}
