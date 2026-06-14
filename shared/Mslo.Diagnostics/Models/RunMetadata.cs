namespace Mslo.Diagnostics.Models;

public sealed record RunMetadata
{
    public required string RunId { get; init; }
    public required string SchemaVersion { get; init; }
    public required DiagnosticMode Mode { get; init; }
    public required DateTimeOffset StartedAtUtc { get; init; }
    public DateTimeOffset? EndedAtUtc { get; init; }
    public required string Status { get; init; }
    public string? MinecraftVersion { get; init; }
    public string? ModLoader { get; init; }
    public string? ModLoaderVersion { get; init; }
    public string? MinecraftInstance { get; init; }
    public string? Notes { get; init; }
}
