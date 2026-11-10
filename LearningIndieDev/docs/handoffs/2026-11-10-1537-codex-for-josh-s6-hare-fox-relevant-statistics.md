# S6 Hare Fox relevant statistics

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-10-1537-codex-for-josh-s6-hare-fox-relevant-statistics
- Owner: Codex for Josh
- Branch: codex/s6-predator-first
- Baseline commit: 652c87c9
- Date: 2026-11-10
- Supersedes: none

## Summary

Created a source-backed reference for the playable Hare (Rabbit) versus Fox matchup. It records current authored rules, clarifies how those values affect opposed-roll combat and reproduction, and names the telemetry most relevant to the avoidance, survival, and faster-reproduction strategies.

## Changes

- Added `docs/Research/S6-Predator-First-Relevant-Statistics-20261110.md`.
- No runtime code, species assets, scenario values, or simulation outputs changed.

## Decisions and assumptions

- “Rabbit” refers to the player species with simulation ID `hare`.
- Authored Forest Edge starts 55 Hares, 20 Foxes, and 400 Plants. The recent experiments used per-arm Fox-count overrides, which remain distinct from the authored defaults.
- The reference separates inputs from observed outcomes and keeps the approved three Mutation response categories distinct from any Genome build path.

## Validation

- Cross-checked values against the production Hare/Fox assets, Forest Edge scenario, `SpeciesRules`, initial-grid creation, opposed-roll combat, and exported simulation metrics.
- Derived Fox-versus-base-Hare hit chance is 229/400 (57.25%) from the production opposed-roll comparison `d20 + 2 > d20 + 0`; a hit applies 2 damage to a Hare with 1 health.
- No Unity Editor, simulation, or test suite ran; this was a read-only source audit.

## Risks and incomplete work

- Source values describe the current authored build and can change with later asset/code updates.
- Avoidance needs encounter and attack measures separated from generic movement; pre-contact avoidance removes an interaction before combat resolution.
- The reference is not evidence that the three Mutations are available or effective in a fresh player route.

## Next useful step

Use the report’s metric shortlist to shape the next controlled experiment and ensure every claimed response has a visible, causally relevant outcome measure.
