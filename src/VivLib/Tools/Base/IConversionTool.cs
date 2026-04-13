namespace TheXDS.Vivianne.Tools.Base;

/// <summary>
/// Defines a contract for asynchronously converting an input value of type
/// <typeparamref name="TInput"/> to an output value of type
/// <typeparamref name="TOutput"/>.
/// </summary>
/// <remarks>
/// Implementations should report conversion progress using the provided
/// <see cref="IProgress{T}"/> instance and support cancellation via the
/// <see cref="CancellationToken"/> parameter. The conversion operation may
/// return <see langword="null"/> if the conversion cannot be performed.
/// </remarks>
/// <typeparam name="TInput">
/// The type of the input value to be converted.
/// </typeparam>
/// <typeparam name="TOutput">
/// The type of the output value produced by the conversion.
/// </typeparam>
public interface IConversionTool<TInput, TOutput> where TOutput : notnull
{
    /// <summary>
    /// Converts the specified input to the output type asynchronously,
    /// reporting progress and supporting cancellation.
    /// </summary>
    /// <remarks>
    /// Progress updates are reported via the provided progress reporter, if
    /// specified. The operation can be canceled by passing a cancellation
    /// token. Callers should handle cancellation and potential null results
    /// appropriately.
    /// </remarks>
    /// <param name="input">
    /// The input data to be converted. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="progress">
    /// A progress reporter instance that receives updates as a percentage
    /// value between 0 and 100, indicating the conversion progress.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the asynchronous conversion
    /// operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous conversion operation. The task
    /// result is the converted output of type
    /// <typeparamref name="TOutput"/>, or <see langword="null"/> if the
    /// conversion fails.
    /// </returns>
    Task<TOutput?> ConvertAsync(TInput input, IProgress<ProgressReport> progress, CancellationToken cancellationToken);
}
