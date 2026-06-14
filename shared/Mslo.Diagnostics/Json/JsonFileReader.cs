using System.Text.Json;

namespace Mslo.Diagnostics.Json;

public static class JsonFileReader
{
    public static async Task<T> ReadAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(stream, DiagnosticJsonOptions.Default, cancellationToken)
            ?? throw new JsonException($"File '{path}' did not contain a JSON object.");
    }
}
