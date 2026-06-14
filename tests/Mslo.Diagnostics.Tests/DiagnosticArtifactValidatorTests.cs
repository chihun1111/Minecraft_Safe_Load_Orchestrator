using Mslo.Diagnostics.Validation;
using Xunit;

namespace Mslo.Diagnostics.Tests;

public class DiagnosticArtifactValidatorTests
{
    [Fact]
    public async Task MissingRequiredFileIsDetected()
    {
        var dir = Directory.CreateTempSubdirectory().FullName;
        var result = await new DiagnosticArtifactValidator().ValidateAsync(dir);
        Assert.Contains(result.Issues, i => i.Code == "missing_required_file" && i.Path.EndsWith("run.json"));
    }

    [Fact]
    public async Task RunIdMismatchIsDetected()
    {
        var dir = CopySample();
        var external = await File.ReadAllTextAsync(Path.Combine(dir, "external.jsonl"));
        await File.WriteAllTextAsync(Path.Combine(dir, "external.jsonl"), ReplaceFirst(external, "sample-run-001", "other-run"));
        var result = await new DiagnosticArtifactValidator().ValidateAsync(dir);
        Assert.Contains(result.Issues, i => i.Code == "run_id_mismatch");
    }

    [Fact]
    public async Task ReportSummaryRunIdMismatchIsDetected()
    {
        var dir = CopySample();
        var summary = await File.ReadAllTextAsync(Path.Combine(dir, "report-summary.json"));
        await File.WriteAllTextAsync(Path.Combine(dir, "report-summary.json"), ReplaceFirst(summary, "sample-run-001", "other-run"));
        var result = await new DiagnosticArtifactValidator().ValidateAsync(dir);
        Assert.Contains(result.Issues, i => i.Code == "run_id_mismatch" && i.Path.EndsWith("report-summary.json"));
    }

    private static string ReplaceFirst(string value, string oldValue, string newValue)
    {
        var index = value.IndexOf(oldValue, StringComparison.Ordinal);
        return index < 0 ? value : value[..index] + newValue + value[(index + oldValue.Length)..];
    }

    private static string CopySample()
    {
        var source = Path.Combine(TestPaths.RepositoryRoot(), "examples", "sample-run");
        var dest = Directory.CreateTempSubdirectory().FullName;
        foreach (var file in Directory.EnumerateFiles(source)) File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
        return dest;
    }
}
