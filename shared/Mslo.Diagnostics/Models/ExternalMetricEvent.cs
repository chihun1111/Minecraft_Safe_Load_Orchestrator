namespace Mslo.Diagnostics.Models;

public sealed record ExternalMetricEvent
{
    public required string RunId { get; init; }
    public required string Source { get; init; }
    public required DateTimeOffset TimestampUtc { get; init; }
    public required long MonotonicMs { get; init; }
    public required string EventType { get; init; }
    public int? ProcessId { get; init; }
    public string? ProcessName { get; init; }
    public double? CpuPercent { get; init; }
    public double? MemoryWorkingSetMb { get; init; }
    public double? MemoryPrivateMb { get; init; }
    public double? DiskReadMbS { get; init; }
    public double? DiskWriteMbS { get; init; }
    public int? ThreadCount { get; init; }
    public int? HandleCount { get; init; }
    public bool? IsResponding { get; init; }
    public long? LatestLogOffset { get; init; }
}
