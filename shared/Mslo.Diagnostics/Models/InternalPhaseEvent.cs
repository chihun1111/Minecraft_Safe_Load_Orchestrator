namespace Mslo.Diagnostics.Models;

public sealed record InternalPhaseEvent
{
    public required string RunId { get; init; }
    public required string Source { get; init; }
    public required DateTimeOffset TimestampUtc { get; init; }
    public required long MonotonicMs { get; init; }
    public required string EventType { get; init; }
    public string? PhaseName { get; init; }
    public string? ThreadName { get; init; }
    public double? DurationMs { get; init; }
    public string? Message { get; init; }
    public double? GcPauseMs { get; init; }
}
