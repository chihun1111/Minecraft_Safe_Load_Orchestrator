using Mslo.Diagnostics.Validation;
using Xunit;

namespace Mslo.Diagnostics.Tests;

public class SampleRunValidationTests
{
    [Fact]
    public async Task SampleRunValidationPasses()
    {
        var path = Path.Combine(TestPaths.RepositoryRoot(), "examples", "sample-run");
        var result = await new DiagnosticArtifactValidator().ValidateAsync(path);
        var scenarioIssues = new SampleRunScenarioValidator().Validate(result, path);
        Assert.True(result.IsValid, string.Join(Environment.NewLine, result.Issues.Select(i => i.Message)));
        Assert.Empty(scenarioIssues);
    }

    [Fact]
    public async Task SampleScenarioFailsWhenFreezeWindowIsMissing()
    {
        var temp = CopySample();
        await File.WriteAllTextAsync(Path.Combine(temp, "report-summary.json"), """
        {"run_id":"sample-run-001","source":"report_generator","generated_at_utc":"2026-01-15T12:03:05Z","status":"completed","freeze_windows":[],"bottleneck_candidates":[],"major_gc_pause_during_freeze":false}
        """);
        var result = await new DiagnosticArtifactValidator().ValidateAsync(temp);
        var issues = new SampleRunScenarioValidator().Validate(result, temp);
        Assert.Contains(issues, i => i.Code == "sample_freeze_window_count");
    }

    private static string CopySample()
    {
        var source = Path.Combine(TestPaths.RepositoryRoot(), "examples", "sample-run");
        var dest = Path.Combine(Path.GetTempPath(), "mslo-test-" + Guid.NewGuid());
        Directory.CreateDirectory(dest);
        foreach (var file in Directory.EnumerateFiles(source)) File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
        return dest;
    }
}
