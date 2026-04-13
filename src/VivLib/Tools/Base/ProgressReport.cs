namespace TheXDS.Vivianne.Tools.Base;

public record ProgressReport(double Progress, string? Status)
{
    public static implicit operator ProgressReport(double progress) => new(progress, null);
    public static implicit operator ProgressReport(string? status) => new(double.NaN, status);
    public static implicit operator double(ProgressReport progressReport) => progressReport.Progress;
    public static implicit operator string?(ProgressReport progressReport) => progressReport.Status;
}