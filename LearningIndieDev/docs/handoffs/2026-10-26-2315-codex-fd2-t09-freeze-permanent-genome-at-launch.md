# fd2-t09-freeze-permanent-genome-at-launch

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-26-2315-codex-fd2-t09-freeze-permanent-genome-at-launch
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 21d6b4b
- Date: 2026-10-26
- Supersedes: none

## Summary

T09 freezes the latest permanent FD2 loadout at the first simulation start of
each profile-owned journey, while preserving the existing POC resolver and
all-species runtime. Profile reset/rearrangement after launch does not alter the
retained journey, phases, checkpoint or results. Work began October26; final
validation and board closeout are on October27.

## Changes

- Added `ProfileSessionSnapshot.CreateLaunchGenomeSnapshot`. Canonical owned
  Hare IDs or an existing reset generation identify permanent FD2 state. It
  requires a license for active effects, rejects mixed refundable allocations,
  validates all owned ancestors/capacity and freezes approved T01 values/order
  and catalog fingerprints. A supplied FD2 map must match the definition.
- Kept generic `CreateGenomeSnapshot` and the POC active-parent resolver intact.
  POC-only/other-species maps retain their existing rules. All species are
  composed independently of player selection; the effective natural-rule map
  remains the existing simulation entry point.
- Desktop now uses the launch resolver and binds an explicit capture callback
  to the preview, retaining the original loaded profile identity. The callback
  refreshes immediately before the first simulation starts, including direct
  next-journey commands; a changed loaded profile blocks launch. It is unbound
  during host destruction and is not called during journey continuation.
- Rebased opening ordered Mutations onto freshly captured Genome rules without
  stacking. Retained the chosen opening map node/visited IDs while rebuilding
  the seeded run. Configuring the Field Data test route retains its prepared
  Genome instead of clearing it through `ResetToStart`.
- Added13 domain cases and4 real profile/Desktop PlayMode cases. No schema,
  asset, GUID, scene, prefab, package or configuration change is required.

## Decisions and assumptions

- The versioned T01 definition is authoritative for permanent FD2 Hare effects;
  the existing Desktop authoring provider still serves its POC/other-species
  maps until T11. A mixed permanent/POC active selection fails visibly.
- Capture is a direct callback owned by the Desktop composition root, with no
  discovery/service locator or global event bus. Preparation can display a
  preview, but actual first start captures the current saved profile again.
- A journey retains one immutable snapshot. Changing the profile or catalog
  while it runs affects only a later journey; opening Mutations stay temporary.
- Work is isolated on `codex/fd2-t01-integration`; original source remains
  preserved, and no push or integration into the open checkout is authorized.

## Validation

- Direct Unity6000.4.6f1 batch CLI targeted the isolated project. Initial
  focused domain run11/13 exposed two fixture mistakes: generic default IDs
  needed explicit Hare/Fox keys, and checkpoints require a phase boundary.
  Corrected fixtures;13/13 pass at `launch-editmode-fixed.xml`.
- Initial focused scene run4/5 exposed a checkpoint-timing test mistake;
  corrected capture to occur before continuation. The five selected scene tests
  then passed, including all four new T09 checks and existing all-species stats.
- Broader related PlayMode initially16/18: one matched the retained incoming
  baseline (population-control expected114, actual90). The other was introduced:
  first-start re-seeding cleared the chosen opening map node. Preserved that
  node/visits and the existing reward/unlock journey passed again. Extended the
  new test through six rounds; corrected its explicit pause between rounds.
  Initial and intermediate XML/logs are retained under
  `artifacts/fd2-t09-20261026/`.
- Final full clean EditMode680 total:679 passed, zero failed, one existing
  parity-manifest skip at `all-editmode-final.xml`. All13 new domain cases pass;
  all31 valid owned loadouts and five exact effects are covered.
- Portable Release build passed with zero warnings/errors; existing deterministic
  runner self-test passed at `portable-self-test.log` in the same artifact folder.
- Final related PlayMode:18 total,17 passed, one unchanged baseline failure at
  `related-playmode-final.xml`. All4 new T09 cases and the previously regressed
  three-journey reward/unlock test pass. The sole remaining failure is
  `PopulationControlsRefreshAndSeedCacheUseTheLiveJourney`, expected114/actual90,
  identical to `artifacts/fd2-integration/baseline-playmode.xml`. This is not a
  green full-suite claim. Existing POC purchase, selection, population upgrades,
  all-species stats and T04-T08/profile migration checks pass in this run.
- Exact original profile/recovery preference names/types/values and all four
  source hashes match the protected checkpoints. The original Editor reports
  ready through the CLI. Diff hygiene passes; Unity's incidental ProBuilder
  line-ending rewrite was restored. Live T09 is In Progress pending closeout.
- Handoff validator reports73 historical journal issues, none naming T09;
  retained `handoff-validation.log` in the artifact folder. A stale empty Git
  index lock was removed only after checking its worktree owner, age over six
  minutes and absence of Git processes, then normal checkpointing resumed.

## Risks and incomplete work

- The historical population-control failure is retained as a baseline issue,
  not disabled or changed. No full PlayMode/player-build/human-visual/balance
  acceptance is claimed; seven historical full-PlayMode failures remain open.
- T10 still owns settlement/results integration and T11 visible Gene Lab controls.
  POC and FD2 purchase semantics must not be mixed through legacy setters.
- No fresh balance experiment or ecology approval follows from these functional
  checks. Profile storage remains v7, and the original Editor is not modified.

## Next useful step

Continue with T10: present the frozen permanent settlement/failure receipt,
retry-pending state and before/after wallets through the existing results UI,
without paying again on results reentry. Use T04/T05 settlement APIs and keep
calculations out of XAML/ViewModels.
