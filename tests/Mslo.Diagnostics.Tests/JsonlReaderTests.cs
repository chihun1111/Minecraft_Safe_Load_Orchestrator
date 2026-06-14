using Mslo.Diagnostics.Json;
using Mslo.Diagnostics.Models;
using Xunit;

namespace Mslo.Diagnostics.Tests;

public class JsonlReaderTests
{
    [Fact]
    public async Task JsonlReaderAcceptsValidJsonlAndIgnoresEmptyLines()
    {
        var path = Path.GetTempFileName();
        await File.WriteAllTextAsync(path, "{\"run_id\":\"r\",\"source\":\"s\",\"timestamp_utc\":\"2026-01-01T00:00:00Z\",\"monotonic_ms\":1,\"event_type\":\"metric\"}\n\n");
        var result = await JsonlReader.ReadAsync<ExternalMetricEvent>(path);
        Assert.True(result.IsValid);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task JsonlReaderReportsInvalidJsonlWithLineNumber()
    {
        var path = Path.GetTempFileName();
        await File.WriteAllTextAsync(path, "\n{not-json}\n");
        var result = await JsonlReader.ReadAsync<ExternalMetricEvent>(path);
        var issue = Assert.Single(result.Issues);
        Assert.Equal(2, issue.LineNumber);
        Assert.Equal("invalid_jsonl", issue.Code);
    }
}
