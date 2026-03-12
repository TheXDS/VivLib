namespace TheXDS.Vivianne.Tools.Base;

/// <summary>
/// Defines a contract for tools that perform in-place transformations on
/// objects of type <typeparamref name="TObject"/> with additional parameters
/// of type <typeparamref name="TParameters"/>.
/// </summary>
/// <typeparam name="TObject">The type of object to transform.</typeparam>
/// <typeparam name="TParameters">
/// The type of parameters required for the transformation.
/// </typeparam>
public interface IInPlaceTransformTool<TObject, TParameters> : IInPlaceTransformTool<TObject>, IInPlaceTransformTool<(TObject, TParameters)>
{
    /// <summary>
    /// Transforms the specified item in-place asynchronously with the provided
    /// parameters.
    /// </summary>
    /// <param name="item">The item to transform.</param>
    /// <param name="parameters">
    /// The parameters required for the transformation.
    /// </param>
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
    Task<bool> TransformAsync(TObject item, TParameters parameters, IProgress<double> progress, CancellationToken cancellationToken);

    /// <summary>
    /// Transforms the specified item in-place asynchronously using default
    /// parameters.
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
    Task<bool> IInPlaceTransformTool<TObject>.TransformAsync(TObject item, IProgress<double> progress, CancellationToken cancellationToken)
    {
        return TransformAsync(item, default!, progress, cancellationToken);
    }

    /// <summary>
    /// Transforms the specified item in-place asynchronously using the
    /// parameters from the tuple.
    /// </summary>
    /// <param name="item">
    /// A tuple containing the item to transform and its parameters.
    /// </param>
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
    Task<bool> IInPlaceTransformTool<(TObject, TParameters)>.TransformAsync((TObject, TParameters) item, IProgress<double> progress, CancellationToken cancellationToken)
    {
        return TransformAsync(item.Item1, item.Item2, progress, cancellationToken);
    }
}
