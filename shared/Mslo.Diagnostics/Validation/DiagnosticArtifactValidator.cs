using System.Text.Json;
using Mslo.Diagnostics.Json;
using Mslo.Diagnostics.Models;

namespace Mslo.Diagnostics.Validation;

public sealed record DiagnosticArtifacts(RunMetadata? Run, IReadOnlyList<ExternalMetricEvent> ExternalEvents, IReadOnlyList<InternalPhaseEvent> InternalEvents, ReportSummary? ReportSummary);

public sealed record DiagnosticArtifactValidationResult(DiagnosticArtifacts Artifacts, IReadOnlyList<DiagnosticValidationIssue> Issues)
{
    public bool IsValid => !Issues.Any(i => i.Severity == DiagnosticSeverity.Error);
}

public sealed class DiagnosticArtifactValidator
{
    private static readonly string[] RequiredFiles = ["run.json", "external.jsonl", "internal.jsonl", "report-summary.json"];

    public async Task<DiagnosticArtifactValidationResult> ValidateAsync(string runDirectory, CancellationToken cancellationToken = default)
    {
        var issues = new List<DiagnosticValidationIssue>();
        foreach (var file in RequiredFiles)
        {
            var path = Path.Combine(runDirectory, file);
            if (!File.Exists(path)) issues.Add(Issue("missing_required_file", $"Required artifact '{file}' is missing.", path));
        }

        RunMetadata? run = await ReadJson<RunMetadata>(Path.Combine(runDirectory, "run.json"), issues, cancellationToken);
        ReportSummary? summary = await ReadJson<ReportSummary>(Path.Combine(runDirectory, "report-summary.json"), issues, cancellationToken);
        var external = await ReadJsonl<ExternalMetricEvent>(Path.Combine(runDirectory, "external.jsonl"), issues, cancellationToken);
        var internalEvents = await ReadJsonl<InternalPhaseEvent>(Path.Combine(runDirectory, "internal.jsonl"), issues, cancellationToken);

        if (run is not null)
        {
            CheckRunId(run.RunId, summary?.RunId, Path.Combine(runDirectory, "report-summary.json"), issues);
            foreach (var item in external) CheckRunId(run.RunId, item.RunId, Path.Combine(runDirectory, "external.jsonl"), issues);
            foreach (var item in internalEvents) CheckRunId(run.RunId, item.RunId, Path.Combine(runDirectory, "internal.jsonl"), issues);
        }
        if (summary is not null)
        {
            foreach (var window in summary.FreezeWindows)
            {
                if (window.EndUtc < window.StartUtc || window.EndMonotonicMs < window.StartMonotonicMs)
                    issues.Add(Issue("incoherent_freeze_window", "Freeze window end must be after start.", Path.Combine(runDirectory, "report-summary.json")));
            }
        }

        return new DiagnosticArtifactValidationResult(new DiagnosticArtifacts(run, external, internalEvents, summary), issues);
    }

    private static async Task<T?> ReadJson<T>(string path, List<DiagnosticValidationIssue> issues, CancellationToken ct)
    {
        if (!File.Exists(path)) return default;
        try { return await JsonFileReader.ReadAsync<T>(path, ct); }
        catch (Exception ex) when (ex is JsonException or IOException)
        { issues.Add(Issue("invalid_json", ex.Message, path)); return default; }
    }

    private static async Task<IReadOnlyList<T>> ReadJsonl<T>(string path, List<DiagnosticValidationIssue> issues, CancellationToken ct)
    {
        if (!File.Exists(path)) return [];
        var result = await JsonlReader.ReadAsync<T>(path, ct);
        issues.AddRange(result.Issues);
        return result.Items;
    }

    private static void CheckRunId(string expected, string? actual, string path, List<DiagnosticValidationIssue> issues)
    {
        if (actual != expected) issues.Add(Issue("run_id_mismatch", $"Expected run_id '{expected}' but found '{actual}'.", path));
    }

    private static DiagnosticValidationIssue Issue(string code, string message, string path) => new() { Code = code, Message = message, Severity = DiagnosticSeverity.Error, Path = path };
}
