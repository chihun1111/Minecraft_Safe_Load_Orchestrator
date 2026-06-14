namespace Mslo.Diagnostics.Models;

public sealed record BottleneckCandidate
{
    public required string Type { get; init; }
    public required string Description { get; init; }
    public required double Confidence { get; init; }
    public required DiagnosticSeverity Severity { get; init; }
    public IReadOnlyList<string> Evidence { get; init; } = [];
}
