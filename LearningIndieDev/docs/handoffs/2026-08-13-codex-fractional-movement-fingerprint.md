# Fractional movement and fingerprint correction

[Working state](../WORKING_STATE.md) | Status: implemented; Unity execution pending

- Owner: Codex
- Branch: `codex/xaml-migration`
- Date: 2026-08-13

## Summary

Movement speed now represents an expected number of move attempts per tick:
the whole-number portion is guaranteed and the fractional portion is a seeded
chance for one additional attempt. For example, `1.8` grants one attempt plus
an 80% chance of a second; `0.55` grants a 55% chance of one attempt.

The ruleset fingerprint now includes movement speed and uses canonical version
`cellular-sim-data-v4`. Existing v3 fingerprints and experiment baselines are
therefore intentionally not comparable by hash to newly generated runs.

## Validation

- Added an Edit Mode regression test proving fractional speed produces both the
  guaranteed and fractional outcomes across deterministic seeds.
- Extended fingerprint coverage to prove movement-speed changes alter the hash.
- `dotnet build SaltyGame.Runtime.csproj --no-restore` passed with zero errors.
- `dotnet build SaltyGame.Tests.csproj --no-restore` passed with zero errors;
  existing obsolete compatibility warnings remain.
- Unity Edit Mode execution is pending because the project was open and the
  project tooling correctly refused to run a second Unity process.

## Follow-up

Run `CellSim Test -Mode EditMode` after closing Unity. Regenerate rather than
compare against any balance baseline whose fingerprint was produced under v3;
fractional movement semantics will also change those outcomes.
