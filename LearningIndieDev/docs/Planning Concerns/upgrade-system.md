# Planning concerns — Upgrade System

**Scope:** Scriptable Object authoring, stable attribute IDs, runtime application,
deterministic evidence, and the first production-quality species per-run upgrade
slice; permanent Lab upgrades are future consumers of the contract, not current
implementation scope; excludes Lab persistence and a generalized modifier/plugin
framework.
**Canonical plan:** [`../UPGRADE_SYSTEM_DIRECTION.md`](../UPGRADE_SYSTEM_DIRECTION.md)
and [`../NEXT_WORK_BUCKET_PLAN.md`](../NEXT_WORK_BUCKET_PLAN.md)
**Human owner:** Josh (sole feature owner and decision authority)
**Status:** Active

## Active concerns

### UPG-C01 — Runtime/editor mapping drift

- **Severity:** Extreme
- **Status:** Acknowledged
- **Trigger:** An upgrade asset or editor-exposed target enters a run without a stable attribute ID in the single registry and a corresponding explicit runtime applier, telemetry mapping, and test; reflection or field names are treated as execution authority.
- **Why it matters:** An upgrade can preview and serialize successfully while changing the wrong rule or no rule, which would invalidate deterministic reports and predictive-AI evidence.
- **Evidence:** `Assets/Scripts/Game/Species/SpeciesUpgrade.cs` currently applies a closed explicit switch; `SpeciesRules` is immutable; the research plan requires exact intervention provenance and returns **Not currently testable** for unsupported model fields; Unity engineering standards require visible domain dependencies and discourage hidden discovery.
- **Smallest mitigation:** Make one registry authoritative for stable IDs and metadata; have assets serialize IDs and signed values only; validate every target against the registry and applier when authored and when building the immutable runtime snapshot; reject unmapped targets with an actionable diagnostic; cover registry/applier parity and fingerprint inclusion with focused tests.
- **Owner:** Josh
- **Recorded:** 2026-09-03, user-confirmed planning decision

### UPG-C02 — V1 modifier semantics and bounds

- **Severity:** Mild
- **Status:** Acknowledged
- **Trigger:** A V1 asset uses an operation other than a signed additive value, or introduces implicit range, clamping, multiplication, setting, or conditional syntax.
- **Why it matters:** Different consumers could interpret the same modifier differently, producing inconsistent gameplay, previews, and research results.
- **Evidence:** User decision: positive values (with an optional `+`) increase an attribute and `-` decreases it; no other operations or range/clamping syntax are supported in V1; current `SpeciesUpgrade` is positive additive-only.
- **Smallest mitigation:** Keep each V1 modifier as a stable attribute ID plus one signed numeric value; treat omitted/`+` as positive and `-` as negative; reject unsupported operation syntax and validate attribute-specific numeric constraints in the registry.
- **Owner:** Josh
- **Recorded:** 2026-09-03, user-confirmed planning decision

### UPG-C03 — Snapshot, version, and provenance drift

- **Severity:** Mild
- **Status:** Acknowledged
- **Trigger:** A run or prediction retains only a live Scriptable Object reference, Unity metadata (GUID/path/name), or upgrade ID and does not capture the resolved values, order, schema/catalog version, and fingerprint at run start.
- **Why it matters:** Later edits to an asset could make historical runs non-replayable, non-diffable, or falsely appear to test the same intervention.
- **Evidence:** The research plan requires immutable run data, upgrade/event loadout, build and ruleset provenance, and model/evidence fingerprints; the current launch request primarily carries ordered upgrade IDs.
- **Progress:** The authored research path now resolves production assets into
  immutable snapshots with `SpeciesUpgradePredictionInputAdapter`, records the
  ordered loadout fingerprint, and emits the full prediction input in schema-23
  reports. The adapter can resolve an explicit research fixture catalog while
  retaining the same snapshot shape used by production loadouts. Legacy
  experiment IDs remain intentionally supported for historical evidence.
- **Smallest mitigation:** Resolve authoring assets once at launch into a plain immutable snapshot containing schema/version, stable upgrade ID, ordered modifier IDs and signed values, and registry/catalog fingerprint; retain the asset GUID/path only as optional provenance; emit the snapshot in run, report, and prediction artifacts and never consult the mutable asset during execution.
- **Owner:** Josh
- **Recorded:** 2026-09-03, user-confirmed planning decision

### UPG-C04 — Non-commutative order and combination rules

- **Severity:** Mild
- **Status:** Acknowledged
- **Trigger:** A multi-upgrade loadout is silently reordered, implicitly stacked, or allowed to combine without explicit scope, prerequisite, exclusion, and order rules.
- **Why it matters:** Applying A then B can produce a different result from B then A; consumers could disagree about the effective ruleset and invalidate comparisons or player-facing explanations.
- **Evidence:** User decision: upgrade order is not commutative and both/all relevant orders must be tested; Sprint 2 explicitly requires ordered loadouts plus stacking and exclusion rules, while the current implementation has only single-upgrade evidence.
- **Smallest mitigation:** Preserve authored purchase order; define scope, stacking, exclusion, and prerequisite behavior explicitly; record order in the snapshot, fingerprint, and report; run paired A→B, B→A, and all relevant order variants in research tests rather than assuming equivalence.
- **Progress:** EX-009 completed the locked A→B/B→A comparison on the same
  held-out seeds (106–110). The two additive upgrades produced identical
  outcomes and available telemetry in all five pairs. A focused commutativity
  regression test remains the appropriate follow-up as future stateful upgrade
  types are introduced.
- **Owner:** Josh
- **Recorded:** 2026-09-03, user-confirmed planning decision

### UPG-C05 — Research asset folder separation

- **Severity:** Mild
- **Status:** Acknowledged
- **Trigger:** A dummy or research-only upgrade is placed where the production catalog can discover it.
- **Why it matters:** The low-probability failure is accidental exposure of a test lever as player content or confusion between research evidence and approved game balance.
- **Evidence:** User decision: dummy upgrades are useful for theory testing, but this is a very low-risk concern best handled by folder separation rather than lifecycle machinery.
- **Smallest mitigation:** Keep research/dummy Scriptable Objects in a separate folder from production catalog assets and keep catalog discovery explicit.
- **Owner:** Josh
- **Recorded:** 2026-09-03, user-confirmed planning decision

### UPG-C06 — Existing runtime compatibility boundary

- **Severity:** Mild
- **Status:** Acknowledged
- **Trigger:** The Scriptable Object authoring path replaces or changes the current `SpeciesUpgrade`, `SpeciesUpgradeCatalog`, `SpeciesProgression`, or ordered-loadout semantics in-place without an adapter and focused regression coverage.
- **Why it matters:** Existing reward previews, domain tests, deterministic baseline behavior, and historical experiment comparisons could break while the new authoring workflow is being introduced.
- **Evidence:** The current runtime and presentation path consume plain-C# upgrades and ordered IDs; existing tests cover those APIs; research artifacts depend on stable intervention IDs and fingerprints.
- **Smallest mitigation:** Introduce Scriptable Objects as an authoring adapter into the immutable runtime upgrade contract; preserve existing IDs and legacy behavior until parity tests pass; migrate consumers one seam at a time with baseline, snapshot, and report-regression tests.
- **Owner:** Josh
- **Recorded:** 2026-09-03, user-confirmed planning decision

## Closed concerns
