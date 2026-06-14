namespace Mslo.Diagnostics.Models;

public sealed record ReportSummary
{
    public required string RunId { get; init; }
    public required string Source { get; init; }
    public required DateTimeOffset GeneratedAtUtc { get; init; }
    public required string Status { get; init; }
    public IReadOnlyList<FreezeWindow> FreezeWindows { get; init; } = [];
    public IReadOnlyList<BottleneckCandidate> BottleneckCandidates { get; init; } = [];
    public bool? MajorGcPauseDuringFreeze { get; init; }
}
