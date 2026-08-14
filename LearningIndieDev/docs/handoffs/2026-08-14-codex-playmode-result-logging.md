# Play Mode result logging

## Outcome

Completed `SpeciesSimulationPreview` runs now emit a static completion event.
An editor-only listener persists the last completed run to the ignored
`artifacts/playmode-last-run.json` and `artifacts/playmode-last-run.md` files.

The JSON includes the scenario path/name, seed, grid dimensions, duration/ticks,
player species, ruleset fingerprint, full per-tick population history, and
per-species activity totals. The Markdown file is a short human/agent summary
of final populations and activity.

## Validation

- `dotnet build SaltyGame.Runtime.csproj --no-restore -v:q` - passed, existing warnings only.
- `dotnet build Assembly-CSharp-Editor.csproj --no-restore -v:q` - passed, existing warnings only.
- Unity Play Mode execution remains pending because the editor was not driven for a full run in this pass.
