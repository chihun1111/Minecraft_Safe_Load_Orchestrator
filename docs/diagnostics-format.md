# Diagnostics Format

The diagnostics contract is a set of newline-delimited and regular JSON artifacts written under a run directory.

Contract assets live in:

- JSON Schemas: `schemas/diagnostics/`
- .NET typed models and readers: `shared/Mslo.Diagnostics/`
- sample validation tests: `tests/Mslo.Diagnostics.Tests/`

The sample artifacts in `examples/sample-run/` are validated so future components keep a common artifact contract before runtime monitor, mod, or report-generator work begins.

## Required files

- `run.json`: run metadata.
- `external.jsonl`: external monitor metric events, one JSON object per non-empty line.
- `internal.jsonl`: internal mod phase/log events, one JSON object per non-empty line.
- `report-summary.json`: summarized freeze windows and candidate bottlenecks.

## Common fields

- `run_id` (required): stable identifier shared by all artifacts in a run.
- `source` (required for events): producer name such as `external_monitor`, `neoforge_diagnostic_mod`, or `report_generator`.
- `timestamp_utc` (required): ISO-8601 UTC timestamp.
- `monotonic_ms` (optional): monotonic milliseconds since run start.
- `severity` (optional): `info`, `warning`, or `error`.

## run.json

Required fields: `run_id`, `schema_version`, `mode`, `started_at_utc`, `status`.

Optional fields include `ended_at_utc`, `minecraft_version`, `mod_loader`, `mod_loader_version`, `minecraft_instance`, `notes`.

`mode` is one of `diagnostic`, `sample`, or `report_only`.

## external.jsonl event

Required fields: `run_id`, `source`, `timestamp_utc`, `monotonic_ms`, `event_type`.

External metric events may include `process_id`, `process_name`, `cpu_percent`, `memory_working_set_mb`, `memory_private_mb`, `disk_read_mb_s`, `disk_write_mb_s`, `thread_count`, `handle_count`, `is_responding`, and `latest_log_offset`.

## internal.jsonl event

Required fields: `run_id`, `source`, `timestamp_utc`, `monotonic_ms`, `event_type`.

Internal phase events may include `phase_name`, `thread_name`, `duration_ms`, `message`, and `gc_pause_ms`. Expandable phase names remain strings, for example `client_resource_reload`.

## report-summary.json

Required fields: `run_id`, `source`, `generated_at_utc`, `status`, `freeze_windows`, `bottleneck_candidates`.

Freeze windows include `start_utc`, `end_utc`, `start_monotonic_ms`, `end_monotonic_ms`, `duration_ms`, optional `phase_name`, optional `severity`, and optional `notes`.

Bottleneck candidates include `type`, `description`, `confidence`, `severity`, and optional `evidence`.

The sample run demonstrates one freeze window during `client_resource_reload`, high disk I/O, and no major GC pause.
