using System.Text.Json;
using Mslo.Diagnostics.Models;

namespace Mslo.Diagnostics.Json;

public sealed record JsonlReadResult<T>(IReadOnlyList<T> Items, IReadOnlyList<DiagnosticValidationIssue> Issues)
{
    public bool IsValid => !Issues.Any(i => i.Severity == DiagnosticSeverity.Error);
}

public static class JsonlReader
{
    public static async Task<JsonlReadResult<T>> ReadAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        var items = new List<T>();
        var issues = new List<DiagnosticValidationIssue>();
        using var reader = new StreamReader(path, System.Text.Encoding.UTF8);
        var lineNumber = 0;
        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(cancellationToken);
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;
            try
            {
                var item = JsonSerializer.Deserialize<T>(line, DiagnosticJsonOptions.Default);
                if (item is null) throw new JsonException("Line did not contain a JSON object.");
                items.Add(item);
            }
            catch (JsonException ex)
            {
                issues.Add(new DiagnosticValidationIssue { Code = "invalid_jsonl", Message = ex.Message, Severity = DiagnosticSeverity.Error, Path = path, LineNumber = lineNumber });
            }
        }
        return new JsonlReadResult<T>(items, issues);
    }
}
