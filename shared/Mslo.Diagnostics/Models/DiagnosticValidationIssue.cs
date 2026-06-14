namespace Mslo.Diagnostics.Models;

public sealed record DiagnosticValidationIssue
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public required DiagnosticSeverity Severity { get; init; }
    public required string Path { get; init; }
    public int? LineNumber { get; init; }
}
