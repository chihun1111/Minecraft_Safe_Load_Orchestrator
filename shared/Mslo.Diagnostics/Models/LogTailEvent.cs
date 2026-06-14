namespace Mslo.Diagnostics.Models;

public sealed record LogTailEvent
{
    public required string RunId { get; init; }
    public required string Source { get; init; }
    public required DateTimeOffset TimestampUtc { get; init; }
    public required long MonotonicMs { get; init; }
    public required string EventType { get; init; }
    public required long LatestLogOffset { get; init; }
    public string? Line { get; init; }
    public DiagnosticSeverity? Severity { get; init; }
}
