# S4 balance publication checkpoint

[Working state](../WORKING_STATE.md) | Status: shared

- Handoff schema: 1
- Handoff ID: 2026-10-07-2049-codex-s4-balance-publication
- Owner: Codex for Bevin
- Branch: BevBranch
- Baseline commit: 6a0ab08e
- Date: 2026-10-07
- Supersedes: none

## Summary

Bevin authorized publishing all accumulated S4 work, then integrating Salty's latest ProjectMain before the team meeting. This checkpoint shares the optional wrapping-grid implementation and seven experiment packages. It does not adopt a balance candidate or alter authored scenario/species values.

## Changes

- Optional Inspector wrapping connects opposite edges for movement, perception, combat, reproduction, neighbor counts, and seed drops. Bounded remains the authored default.
- Explicit population overrides preserve zero counts. Threat Avoidance no longer depends on a removed flee-speed bonus, and its deterministic replay no longer depends on process-scoped entity IDs.
- Experiment CLI and runner support wrapping, population matrices, offer-aware strategy policies, and a research-only first-choice timing diagnostic, with focused regression coverage.
- Published reports cover the first paired screen, wrapping comparison, population sweep, 100-seed large screen, 200-seed threshold confirmation, offer-aware policy comparison, and matched timing diagnostic. Raw JSON, CSV, logs, analyzers, and validated summaries remain local in ignored artifacts.

## Decisions and assumptions

The current balance target is an exploratory 40-50% node clear rate with intervention, and collapse pressure without it. The six-phase experimental clear rule is tick 600 with Plants > 0, Hares >= 5, and Foxes >= 5. At wrapped 600/40/25, the offer-aware 200-seed rates are 16% Skip, 15% Trailblazer, 17% Warren, and 25.5% Gardeners. Timing alone did not help: current Trailblazer 30/200 versus early 28/200; paired difference -1 percentage point, exact McNemar p=.878.

The [timing interpretation](../Research/Experiments/S4-03-Trailblazer-Early-Choice-Timing-600-40-25/ANALYSIS.md) includes the October 7 failure review: early Hare extinction 79/200 versus 98/200, and phase-level Fox feeding/starvation tradeoffs. Diagnose matched failure categories before choosing one reversible tuning counterfactual. No Fox buff or production timing change is approved.

## Validation

Experiment-specific execution, accounting validation, source fingerprints, focused checks, and visual acceptance are recorded in the seven dated packages and handoffs. `git diff --check` passed at publication. These historical checks are not a fresh merged-revision full Unity suite or journey balance validation.

## Risks and incomplete work

The new Desktop journey has separate runtime cadence and choices; six-phase findings cannot be treated as a twelve-phase route clear rate. The complete raw evidence is not in Git. Repository readers can review the compact protocol/report/analysis packages, while exact replay should use the recorded source contracts and local artifacts.

## Next useful step

Merge the latest ProjectMain, inspect the preview/data seam, run focused integration checks, and review journey upgrade persistence, ecological clear criteria, and the next matched failure diagnostic at the meeting. Salty owns journey planning; Bevin's work remains ecology and strategy balance.

## October 7 journey integration

- Checkpoint `64a8e3b5` was pushed before merging Salty's `origin/ProjectMain` commit `fea55926` (`feat: integrate Forest Edge journey into Desktop flow`).
- `SpeciesSimulationPreview.cs` merged automatically. The wrapping flag remains applied when creating simulation data, and existing data-copy methods retain it. The only textual conflict was `WORKING_STATE.md`; both sides' entries were preserved.
- The existing PlayMode same-biome journey test now enables wrapping and checks that wrapped cells survive the condition/cycle boundary. It also verifies the same run, cells, history, retained Mutations, and final reset.
- Fresh Unity 6000.4.6f1 compilation and focused EditMode checks: 158 passed, 0 failed, 0 skipped (`JourneyMapAssetTests`, `PopulationConfigurationTests`, `GridTests`, `SpeciesDomainTests`). Focused PlayMode: 1 passed, 0 failed, 0 skipped (`JourneyNodeRewardsAndSimulationsContinueTheSameBiome`). Results/logs: local ignored `artifacts/integration-20261007-journey-merge/`.
- All five changed XAML files parsed as XML. Both changed PowerShell scripts parsed successfully, and Git whitespace/conflict checks passed. No production species/scenario tuning was introduced.
- The project Doctor wrapper falsely treated two Unity CLI MCP processes as Editors. CLI status showed no connected Editor, no project lock existed, and the focused checks used the installed Unity CLI with the explicit Editor path. No existing process or lock was removed.
- No fresh visual review, full test suite, player build, or twelve-phase balance study was performed. The new prototype's opening choice and persistent loadout differ from the frozen six-phase research contract.

Next: agree the ecological gate, node versus whole-journey difficulty, upgrade persistence, and one matched failure-diagnostic protocol at the team meeting before approving more tuning.
