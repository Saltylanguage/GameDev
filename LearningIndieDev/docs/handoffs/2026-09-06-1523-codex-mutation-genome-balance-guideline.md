# Mutation, Genome, and upgrade-balance direction

Follow-up clarification: Genome unlocks are permanent, active Genome nodes are
configurable between simulations, and Mutations are excluded from Biome
Simulations. See
[`2026-09-06-1754-codex-simulation-mode-and-active-genome-clarification.md`](2026-09-06-1754-codex-simulation-mode-and-active-genome-clarification.md).

[Working state](../WORKING_STATE.md) | Status: documentation baseline ready for review

- Owner and decision authority: Josh
- Date: 2026-09-06
- Scope: product language, scalable balance guideline, feature-plan integration,
  Lab/roadmap alignment, and future architecture boundaries

## Outcome

The project now has an official balance guideline and one consistent
progression model:

- **Mutations** are nine temporary adaptations for the selected species during
  a completed expedition.
- A species' **Genome** is permanent progression purchased in the Gene Lab.
- Every scenario population receives its species' Genome even when that species
  is not player-controlled.
- Effective rules are natural species rules, then that species' frozen Genome,
  then the selected species' ordered Mutations.
- The long-term objective is a diverse, resilient ecosystem rather than
  dominance by one species or completion of every collection entry.

## Balance direction

[`SG-005 — Upgrade and Ecology Balance`](../Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md)
defines the required approach:

- group raw stats beneath a small set of shared capabilities;
- use internal Adaptation Value only as a provisional planning budget;
- separate baseline value from environmental and matchup value;
- measure direct effects, marginal value, dominance, order, and synergy;
- review resources, other species, pressure cycles, extinction, and recovery;
- extend the existing deterministic `CellSim` path before building a new
  simulator or dashboard;
- preserve Experiment → Run → Report → Analysis → Human Decision;
- require a recorded human promotion decision for production balance.

Exact Adaptation Value exchange rates and tier budgets remain TBD until the
Forest Edge reference panel is approved and calibrated. The existing
herbivore-specific `ReplicationFitnessScore` is not a universal balance score.

## Feature-plan integration

[`UPGRADE_SYSTEM_DIRECTION.md`](../UPGRADE_SYSTEM_DIRECTION.md) now contains
UB-0 through UB-7:

1. vocabulary and boundaries;
2. shared capability map and reference panel;
3. Adaptation Value calibration;
4. Mutation paths and effect types;
5. Genome foundation;
6. matchup and ecosystem review;
7. targeted automation and the broader content gate.

The roadmap places Mutation calibration with species/build co-design, the first
Genome with profile and settlement work, and uneven/all-Genome ecosystem review
with vertical-slice validation. This work does not reopen the completed Sprint
2 scope.

## Architecture boundaries

- Keep the existing `SpeciesUpgradeAsset` / `SpeciesUpgradeSnapshot` path as
  the signed-additive Mutation V1 contract.
- Do not reinterpret its `PerRun` scope as Genome persistence.
- Define a separate, versioned per-species Genome profile and immutable launch
  snapshot when UB-4 is selected.
- Compose Genomes by stable `SpeciesId` for every species in the scenario; do
  not route permanent effects only through `PlayerSpeciesId`.
- Keep capability tags and Adaptation Value as balance metadata until repeated
  gameplay logic proves that a runtime capability layer is needed.
- One-time effects, abilities, and soft-cap formulas require explicit versioned
  contracts. They must not become hidden operations in the current signed
  additive modifier path.

## Documents aligned

The product context, product brief, GDD/TDD templates, production and future
roadmaps, feature triage, scientific-data economy, Main Menu/Lab plan,
GalapagOS Gene Lab plans, architecture map, Mutation authoring guide, Hare
Mutation acceptance matrix, reactive ecology plan, incomplete-feature plan,
simulation-tooling guide, and working state now link or conform to this
direction. Historical handoffs and experiment reports were not rewritten.

## Verification

- `git diff --check` passed; line-ending notices are repository checkout
  warnings rather than whitespace errors.
- Documentation-only change: Unity tests were not required or run.
- An independent `gpt-5.6-sol` architecture review inspected the plans, current
  Mutation code, `CellularSimData` composition seam, report tooling, and
  conflicts before the final alignment pass. Sol made no file changes.

## Decisions still needed

- Reconcile the current Skip option with the approved target of nine Mutations
  in a completed expedition.
- Decide whether Species Mastery reveals Genome nodes, remains a separate
  progress track, or does both.
- Approve the Forest Edge balance reference panel and capability map before
  assigning numeric Adaptation Value exchange rates.
- Choose the first one-time Mutation and ability contract after the signed-stat
  slice.
- Review the two proposed planning concerns about missing background-species
  Genome application and focal-species-only balance approval before adding them
  to the concern record.
