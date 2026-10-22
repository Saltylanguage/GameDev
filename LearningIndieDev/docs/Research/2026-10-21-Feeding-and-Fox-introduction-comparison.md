# Earlier feeding and gradual Fox introduction

**Experiment:** `EXP-FEEDING-FOX-20261021-01`  
**Human decision owner:** Josh  
**Scope version:** 1, October 21, 2026 (America/Toronto)  
**Status:** Four-arm panel completed; production decision pending

Josh requested the proposed earlier-feeding test and a lower starting Fox
population with further Foxes introduced during the first minute. This plan
specifies the timing and measurement window before execution under SG-001.

## Question and predictions

Can earlier Hare feeding, gradual predator introduction, or their combination
reduce the initial Hare decline while supporting continuing predator–prey
activity after the first minute?

- Earlier feeding should move successful feeding and births earlier than the
  current 57/106-tick lower bounds, improving the opportunity to replace losses.
- Starting with fewer Foxes should reduce opening combat deaths. Introducing
  the remaining Foxes later should restore predator pressure without forcing
  all initial Hares through the same opening hunt.
- The combined arm may improve persistence or create excessive Hare growth;
  neither outcome is assumed. Lower opening pressure alone may merely delay
  collapse, so observation continues for a minute after introductions stop.

## Proposed panel

All arms use Forest Edge 36×20 bounded, 400 Plants / 20 Hares, Hare selected,
seeds 10120–10131, 0.1-second steps, 1200 ticks (two simulated minutes),
opposed-roll combat, natural attack opportunities, no Genome or Mutations.
Every arm uses the same sixteen continuing 75-tick windows with empty loadouts.
Windows provide the existing population-addition seam; they do not restart
the world or introduce player decisions.

| Arm | Hare feeding threshold | Initial Foxes | Later Fox introductions |
| --- | ---: | ---: | --- |
| Baseline | 6 | 10 | None |
| Earlier feeding | 25 | 10 | None |
| Fox ramp | 6 | 2 | One after ticks 75, 150, 225, 300, 375, 450, 525, 600 |
| Both | 25 | 2 | Same eight introductions |

The feeding trigger remains strictly below the threshold, with the existing
refill-to-full behavior. Other energy settings, gestation, combat and species
rules remain fixed. The threshold override is an isolated research input using
the existing registered `energy.forage-threshold` field, not a production edit.

The Fox schedule introduces ten Foxes in total including the initial two;
it does not maintain a target population or replace deaths. Births and deaths
can make the number alive differ from that total. Introduced Foxes use authored
starting energy/age and the existing deterministic placement on open passable
tiles. This test does not add an edge-entry migration mechanic. The injection
after tick 600 acts from tick 601, leaving 600 following observation ticks.

This is 48 runs if the combined arm is approved, or 36 if Josh chooses only
baseline plus separate changes. The same seeds are paired, but changing initial
counts changes the initializer's animal placements. Shared initial counts and
tick-zero worlds must match within each baseline/feeding pair; they are not
claimed identical across ten-Fox and two-Fox setups.

## Evidence and review criteria

Retain raw reports, requests, source/assets and hashes, schema and metric
versions, resolved feeding threshold, scheduled/actual introductions, seeds,
ruleset and intervention provenance. Arrival records include tick, amount and
Fox population before/after. Capacity failure stops the arm rather than silently
skipping or deferring an introduction.

Technical acceptance: every selected run reaches tick 1200; expected initial
counts and thresholds match; exactly eight Fox introductions succeed per ramp
run and none occur in other arms; population accounting reconciles per species
including additions; scientific inputs remain frozen across runs. Repeated
replay must preserve state and introduction events. Invalid, duplicate or
non-boundary schedule ticks must be rejected before execution.

Measure Hare population at ticks 25/48/100/150/300/600/900/1200, minimum/mean,
first birth and early births, combat/starvation deaths by window, food use,
crowding and reproduction gates. Measure plant availability, Fox births/kills/
food/deaths, first-minute additions, zero-Fox gaps and final persistence.
Zero Foxes during an introduction schedule is a gap, not irreversible extinction.
Keep focal Hare and ecological results separate. Compare same-seed differences,
variation and direction counts, including combination versus each single change.

Scientific success is evidence of which lever changes the opening and whether
the effect persists after the ramp. Null, mixed and harmful results are valid.
No universal health target, AV, fun score, production change, or automated
acceptance of a new baseline follows from this panel. If a technically valid
panel remains inconclusive, report it without adding seeds or changing values.

## Preparation and authority

- Current branch `codex/audio-direction-and-sourcing`, HEAD `6ffb7b10`, dirty
  shared checkout. The mortality attribution fix is present and verified.
- Reuse CellSim's existing registry-backed portable stat override and continuous
  phase population-addition path. Add only the research schedule input/report
  wiring needed for this comparison; keep unsupported policy/paired combinations
  explicitly rejected.
- The existing shared-tile diet-target rejection remains a known fixed behavior
  across arms. This experiment will not silently change it.
- Research support is implemented and verified before execution: public
  `-SpeciesStats`, `-FoxIntroductionTicks`, schema-39 intervention metadata,
  actual arrival records/logs and first feeding/mating/birth milestones.
  Focused runner checks pass 39/39 at
  `artifacts/unity-tests-20261021-071252/EditMode-results.xml`; standalone build
  has zero warnings/errors and the portable self-test passes. The first filter
  used the wrong Editor namespace and selected zero tests; the corrected class
  filter executed the stated 39. The final case-insensitive paired-mode guard
  passed 1/1 at `artifacts/unity-tests-20261021-071554/EditMode-results.xml`.
  Prepared command script:
  `artifacts/feeding-fox-comparison-20261021-01/run-comparison.ps1`.
- **Exact panel approval:** Josh selected “Run all four arms” in response to
  the exact schedule/window request before execution. This authorizes all
  48 declared runs, including the combined arm. Execution evidence is retained
  under `artifacts/feeding-fox-comparison-20261021-01/`.

### Execution correction (same approved scope)

The first baseline attempt ended early because the scheduled runner inherited
journey failure on Hare extinction. Its immutable report is retained at
`artifacts/cellular-experiment-20261021-112933/report.json` and excluded from
the comparison. `feeding-fox-comparison-20261021-01/failed-attempt.json` records
the rejection. Scheduled ecology observations now run through their declared
tick window after focal extinction; gameplay and purchase-policy runs retain
their existing stop behavior. A focused regression verifies both behaviors.
The corrected suite passes 40/40; the standalone build and portable self-test
also pass. All four arms will use this same corrected frozen source in
`artifacts/feeding-fox-comparison-20261021-02/`. No arm, value, seed or scientific
measurement window changed.

### Completed panel

All 48 corrected runs reached tick 1200, all 192 scheduled arrivals succeeded,
and all 144 species population accounts reconcile. The
[observed report](2026-10-21-Feeding-and-Fox-introduction-results.md) and separate
[AI analysis](2026-10-21-Feeding-and-Fox-introduction-analysis-v1.md) retain the
results and limits. The declared panel is complete; no production setting or
additional experiment has been adopted.
