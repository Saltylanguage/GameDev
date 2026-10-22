# multi-node-genome-research-runner

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-20-2245-codex-multi-node-genome-research-runner
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-20
- Supersedes: none

## Summary

Completed step 1 of the post-Gene-Lab plan: legal multi-node Rabbit builds now
run through CellSim with the same immutable Genome composition as Desktop,
complete resolved input in reports, and frozen portable replay. This is a
technical delivery slice; the proposed ecology comparison has not run.

## Changes

- `CellSim.ps1` and `tools/Run-CellularExperiment.ps1` accept
  `-RabbitGenomeCatalogPath` alongside ordered `-RabbitGenomeNodeIds`. The
  production Hare map is the default; POC use requires an explicit map path.
- The runner resolves the map into the existing immutable Genome snapshot,
  validates unique/nonempty IDs, Hare identity, purchasable/implemented nodes,
  active prerequisites in allocation order, eight-point capacity, and final
  rule bounds through the existing registry applier. Research does not need a
  profile, license purchase or wallet.
- Genome applies to natural scenario rules before initial populations and
  ordered Mutations. Generic phase schedules and Mutation policies now carry
  the same snapshot into their run states. Background Hares receive it too.
- Report schema 38 adds `genomeInput`: schema/registry/catalog identity,
  ordered IDs, full node metadata and modifiers, capacity, and explicit current
  Fertile Droppings seed/scent constants. Each run carries the header's Genome
  fingerprint. No production values or runtime mechanics changed.
- `CellSimPortable` freezes the whole explicit Rabbit catalog, restores it
  with node/map fingerprint validation, and accepts the same scientific flags.
  `Export-CellSimSnapshot.ps1` forwards those flags. Missing/mismatched frozen
  catalogs cannot be replaced implicitly with current assets.
- Corrected an existing scheduled-loadout handoff: `CreateReport` previously
  passed already Mutation-adjusted rules into a phase runner that applied the
  initial loadout again. Phase flows now receive Genome-adjusted natural
  rules. Each phase still declares its complete active Mutation loadout.
- Added focused allocation/report/transport/phase tests and updated the
  prerequisite checklist, shared context, POC limits and tooling commands.

## Decisions and assumptions

- Josh approved step 1, including focused verification and a small technical
  smoke. The pre-run question, hypothesis, criteria and scope are recorded in
  `artifacts/genome-runner-validation-20261020/validation-plan.md`.
- Keep the input path bounded to Rabbit. Other species' allocation economies
  and a universal ability framework are not introduced.
- Preserve allocation order rather than sorting IDs. Research enforces the
  purchase order that Desktop transactions can produce. Wallet balance and
  completed-run gates do not restrict a synthetic research build.
- Paired-lockstep diagnostics remain explicitly unsupported for Genome input;
  their independent two-arm runner must be extended deliberately. Generic
  phases are not the complete Desktop map/shop/cashout journey route.
- No research comparison or balance promotion. The smoke removes predators
  and shortens the window to isolate input delivery and reproducibility.

## Validation

- Elevated host diagnosis healthy; project closed, Unity 6000.4.6f1 clean lane.
- Focused `PopulationConfigurationTests`: **28/28 passed**, including ten new
  cases for Desktop-rule/report parity, malformed/illegal builds, catalog
  round-trip/corruption, scheduled/policy freeze and compact/detailed parity,
  and direct single-application parity for scheduled initial Mutations.
  Evidence: `artifacts/unity-tests-20261020-224446/EditMode-results.xml`.
- The earlier focused set passed 27/27. An added phase parity check then
  exposed a state mismatch; source inspection found the repeated application.
  The final test uses repeated cumulative loadouts to match an unchanged
  Desktop allocation and passes after the handoff correction. Retained initial
  failure: `artifacts/unity-tests-20261020-224338/EditMode-results.xml`.
- Shared portable project builds with **zero warnings/errors**. Its existing
  self-test passes snapshot/gestation round-trip, state normalization,
  compact/detailed parity, determinism, invalid input, eight purchase policies,
  wallet carryover, capacity/placement and no-purchase parity. An initial
  `--no-restore` build found missing local restore metadata; ordinary restore
  resolved it. The host lacks the .NET 8 runtime but has newer runtimes, so
  execution used `dotnet --roll-forward Major` with no installation or target
  framework change. That runtime choice is part of this verification's limits.
- Public-wrapper smoke: **2/2 seeds completed 20 ticks**, Forest Edge 8×8,
  plant=20/hare=4/fox=0, seeds 10100–10101, Fertile Droppings plus POC Efficient
  Digestion. The report records 3/8 capacity, +0.25 digestion and scent values;
  both runs retain the header's Genome fingerprint. Raw report and command,
  source/dirty-state and metric-dictionary manifest:
  `artifacts/cellular-experiment-20261020-224725/`.
- Frozen export's portable header matches the Unity ruleset and Genome
  fingerprints. Both Unity reference seeds match detailed .NET replays under
  the existing verifier: **exact state/counters/choices/timing**, with only
  existing report-derived rates allowing 1e-6 tolerance. Snapshot, reference,
  arguments, replays, verifier output and summary:
  `artifacts/genome-runner-validation-20261020/`.
- Scoped `git diff --check` passed. No runtime UI changes, production asset
  edits, player save writes, commit/push, branch switch or Trello mutation.
  Preserve the shared checkout's unrelated pre-existing dirty work.

## Risks and incomplete work

- Old scheduled reports with a nonempty initial Mutation loadout can contain
  the former double application. Do not treat them as matched evidence for
  schema 38; rerun those configurations before using them for balance.
- Old exports without the frozen Rabbit catalog cannot accept a new Genome
  merely by editing arguments. Export a fresh catalog/source pair.
- For baseline/build portable batches, export natural inputs with the explicit
  catalog path but omit the node-ID flag. Batch arms can then add their legal
  allocations; the export carries the whole map.
- Full Desktop route evidence, paired-lockstep support, new ability telemetry,
  production node/economy design and ecological tuning remain separate.

## Next useful step

Step 2: agree and execute the small matched baseline/Fertile Droppings/
Gardener/alternative-branch screening comparison. Select the scenario window
and health measures explicitly; do not infer fun or production balance from
this short predator-free delivery smoke.
