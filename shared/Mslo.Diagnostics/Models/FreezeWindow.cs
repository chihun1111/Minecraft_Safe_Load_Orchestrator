namespace Mslo.Diagnostics.Models;

public sealed record FreezeWindow
{
    public required DateTimeOffset StartUtc { get; init; }
    public required DateTimeOffset EndUtc { get; init; }
    public required long StartMonotonicMs { get; init; }
    public required long EndMonotonicMs { get; init; }
    public required double DurationMs { get; init; }
    public string? PhaseName { get; init; }
    public DiagnosticSeverity? Severity { get; init; }
    public string? Notes { get; init; }
}
