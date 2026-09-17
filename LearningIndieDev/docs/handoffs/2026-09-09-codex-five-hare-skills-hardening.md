# Five Hare skills - final hardening

[Working state](../WORKING_STATE.md) | Status: shared

- Owner: Codex, requested by Bevin
- Branch: BevBranch
- Baseline: 9127195 plus the pending Reproductive Drive/Crowding Tolerance changes
- Date: 2026-09-09

## Accepted skill contract

All five skills retain the accepted Level 10 cap and balance values.

| Skill | Per-level effect | Primary outcome |
| --- | --- | --- |
| Threat Exposure | Level 1: +0.75 flee speed; every level: +0.08 avoidance (0.80 at 10) | ECN/PREY/EHS |
| Tough Hide | +2 BlockAmount (20 bonus at 10) | PREY / pAVI |
| Efficient Digestion | +0.1 DigestionEnergyBonus (1.0 bonus at 10) | STRV / sAVI |
| Reproductive Drive | +0.005 ReproductionChance (0.05 bonus at 10; Hare 0.04 to 0.09) | BIR / bAVG / FPO |
| Crowding Tolerance | +1 CrowdingTolerance (10 bonus at 10) | CRWD / cAVI |

## Findings and changes

- Continuous phase rewards converted legacy skills to one-time authored snapshots. This blocked repeat levels and omitted Threat Exposure avoidance. Legacy boundary purchases now use the same cap-aware progression as the established sweep path; authored upgrades retain their existing one-time contract.
- Repeated identical upgrade snapshots were collapsed into one acquisition event. Acquisition identity now includes the ordered loadout position, retaining each level and its effective tick.
- Successful legacy purchases retain their catalog snapshots for run provenance. As in the experiment runner, these describe the legacy catalog definition; the ordered IDs, effective rules, and experimental avoidance options are needed to reproduce level-dependent Threat Exposure. Repeated base snapshots alone must not be treated as a resolved additive Threat Exposure loadout.
- Upgrade application validates before spending currency. Invalid bonuses reject NaN and infinity; rejected application leaves currency, rules and purchase history unchanged.
- Threat Response alias queries share the canonical Threat Exposure level and cap. Reproductive Drive is appended to the enum to preserve the existing numeric values.
- The preview now always selects opposed-roll combat, including when experimental mode is disabled, matching Bevin's accepted combat direction.

## Verification contract

- Fresh Unity EditMode and PlayMode suites, including all five caps, forward/reverse combined purchases, birth eligibility with Reproductive Drive/Crowding Tolerance, and continuing phase purchases through Level 10.
- Existing regression coverage remains responsible for opposed-roll blocks and encounter accounting, fractional digestion and energy limits, crowding deaths, and species-wide EHS.
- Unity EditMode passed 226/226 with 0 failures in `artifacts/unity-tests-20260909-104710/EditMode-results.xml`.
- Unity PlayMode passed 19/20 with 0 failures; one existing graphics-only animal sprite test was skipped in `artifacts/unity-tests-20260909-104710/PlayMode-results.xml`.
- `git diff --check` passed. No new balance sweep is part of this hardening pass.

## Limits and next work

- The five skills are the existing experimental herbivore catalog. This does not replace the separate authored production upgrade assets.
- Crowding Tolerance's early saturation and high-level Reproductive Drive resource pressure remain accepted balance follow-ups. Player-facing outcome targets are expectations, not guarantees that every metric improves at every level.
- Graphics acceptance, predator skills and broader balance testing remain separate work.
- Parallel CellSim planning documents are unrelated and excluded from this commit.
