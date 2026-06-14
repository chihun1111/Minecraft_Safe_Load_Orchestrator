using Mslo.Diagnostics.Models;

namespace Mslo.Diagnostics.Validation;

public sealed class SampleRunScenarioValidator
{
    // Sample validation guard only; final diagnosis thresholds belong in later report logic.
    public const double SampleHighDiskIoThresholdMbS = 100.0;

    public IReadOnlyList<DiagnosticValidationIssue> Validate(DiagnosticArtifactValidationResult artifactResult, string path)
    {
        var issues = new List<DiagnosticValidationIssue>();
        if (!artifactResult.IsValid || artifactResult.Artifacts.ReportSummary is null) return issues;
        var artifacts = artifactResult.Artifacts;
        var summary = artifacts.ReportSummary;
        if (summary.FreezeWindows.Count != 1) issues.Add(Issue("sample_freeze_window_count", "Sample must contain exactly one freeze window.", path));
        var freeze = summary.FreezeWindows.SingleOrDefault();
        if (freeze is null) return issues;
        if (!string.Equals(freeze.PhaseName, "client_resource_reload", StringComparison.OrdinalIgnoreCase)) issues.Add(Issue("sample_freeze_phase", "Sample freeze must occur during client_resource_reload.", path));
        var metricDuringFreeze = artifacts.ExternalEvents.Where(e => e.MonotonicMs >= freeze.StartMonotonicMs && e.MonotonicMs <= freeze.EndMonotonicMs).ToList();
        if (metricDuringFreeze.Count == 0) issues.Add(Issue("sample_missing_external_freeze_metric", "Sample needs at least one external metric during the freeze window.", path));
        if (!metricDuringFreeze.Any(e => (e.DiskReadMbS ?? 0) + (e.DiskWriteMbS ?? 0) >= SampleHighDiskIoThresholdMbS)) issues.Add(Issue("sample_disk_io_not_high", $"Sample disk I/O must reach at least {SampleHighDiskIoThresholdMbS} MB/s during freeze.", path));
        if (!summary.BottleneckCandidates.Any(c => c.Type.Contains("disk", StringComparison.OrdinalIgnoreCase) && c.Type.Contains("io", StringComparison.OrdinalIgnoreCase))) issues.Add(Issue("sample_missing_disk_bottleneck", "Sample report must identify disk I/O saturation or equivalent.", path));
        if (summary.MajorGcPauseDuringFreeze == true || artifacts.InternalEvents.Any(e => e.MonotonicMs >= freeze.StartMonotonicMs && e.MonotonicMs <= freeze.EndMonotonicMs && (e.GcPauseMs ?? 0) >= 1000)) issues.Add(Issue("sample_claims_major_gc", "Sample must not claim a major GC pause during the freeze.", path));
        var reloadEvents = artifacts.InternalEvents.Where(e => e.PhaseName == "client_resource_reload").ToList();
        if (!reloadEvents.Any(e => e.EventType == "phase_start" && e.MonotonicMs <= freeze.StartMonotonicMs) || !reloadEvents.Any(e => e.EventType == "phase_end" && e.MonotonicMs >= freeze.EndMonotonicMs)) issues.Add(Issue("sample_reload_not_active", "Internal phase markers must show client_resource_reload active during the freeze.", path));
        if (freeze.EndUtc < freeze.StartUtc || freeze.DurationMs <= 0) issues.Add(Issue("sample_incoherent_timestamps", "Sample freeze timestamps must be coherent.", path));
        return issues;
    }

    private static DiagnosticValidationIssue Issue(string code, string message, string path) => new() { Code = code, Message = message, Severity = DiagnosticSeverity.Error, Path = path };
}
