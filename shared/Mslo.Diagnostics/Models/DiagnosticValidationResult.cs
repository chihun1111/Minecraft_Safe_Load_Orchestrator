namespace Mslo.Diagnostics.Models;

public sealed record DiagnosticValidationResult
{
    public IReadOnlyList<DiagnosticValidationIssue> Issues { get; init; } = [];
    public bool IsValid => !Issues.Any(i => i.Severity == DiagnosticSeverity.Error);
}
