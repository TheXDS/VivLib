namespace TheXDS.Vivianne.Tools.Base;

/// <summary>
/// Represents a single model that can be used to report progress in a standardized way, containing both a progress value and an optional status message.
/// </summary>
/// <param name="Progress">The progress value, typically between 0 and 100.</param>
/// <param name="Status">An optional status message describing the current state of the operation.</param>
public record struct ProgressReport(double Progress, string? Status)
{
    /// <summary>
    /// Converts a double value representing progress to a ProgressReport instance.
    /// </summary>
    /// <remarks>This operator enables implicit conversion from a double to a ProgressReport, simplifying
    /// scenarios where progress values are reported as doubles.</remarks>
    /// <param name="progress">The progress value to convert, typically between 0.0 and 1.0, where 0.0 indicates no progress and 1.0 indicates
    /// completion.</param>
    public static implicit operator ProgressReport(double progress) => new(progress, null);

    /// <summary>
    /// Converts a string representing a status into a ProgressReport instance with an undefined progress value.
    /// </summary>
    /// <remarks>This operator allows implicit conversion from a status string to a ProgressReport, setting
    /// the progress value to NaN to indicate that progress is not specified.</remarks>
    /// <param name="status">The status message to associate with the progress report. Can be null to indicate no status.</param>
    public static implicit operator ProgressReport(string? status) => new(double.NaN, status);

    /// <summary>
    /// Converts a ProgressReport instance to a double representing the current progress value.
    /// </summary>
    /// <remarks>This operator enables implicit conversion of a ProgressReport object to its underlying
    /// progress value as a double. This can simplify code when only the progress value is needed.</remarks>
    /// <param name="progressReport">The ProgressReport instance to convert.</param>
    public static implicit operator double(ProgressReport progressReport) => progressReport.Progress;

    /// <summary>
    /// Converts a ProgressReport instance to its status string representation.
    /// </summary>
    /// <remarks>This operator enables implicit conversion of a ProgressReport object to a string, returning
    /// the value of its Status property.</remarks>
    /// <param name="progressReport">The ProgressReport instance to convert.</param>
    public static implicit operator string?(ProgressReport progressReport) => progressReport.Status;
}