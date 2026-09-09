# EX-010 — execution report

## Status

- Complete for the original sequence and its matched reverse-order comparison.
- Each sequence has a development panel of 20 runs, seeds 1–20.
- Each sequence has a held-out panel of 5 runs, seeds 106–110.
- Every run completed 10 phases and 2,000 ticks.
- The original development and held-out records captured the original contract
  SHA-256:
  `390c8f104ed84079597df6950611c06db7945b00f9f1d838d84696de6d164831`.
- The alternate development and held-out records captured the alternate-order
  contract SHA-256:
  `7e38fd1e0bbb206510b42751bf812c6367dbc984decce05c7784273cf1766848`.
- Both manifests recorded source commit `ac2c06f41fef587c5797414d0166dbdfbe416b9e`
  and disclosed the pre-existing dirty working tree.

## Original sequence

- Scenario: ForestEdge.
- Player species: Hare.
- Phase length: 200 ticks.
- One fixed sequential history, used for every seed:
  - no upgrade;
  - Skip;
  - Trailblazer: Long Stride;
  - Skip;
  - Trailblazer: Far Sight;
  - Warren: Guarded Burrow;
  - Warren: Room to Breed;
  - Skip;
  - Gardeners: Careful Sowing;
- Familial Bond: Large Litters.
- Acquisition ticks: 400, 800, 1000, 1200, 1600, and 1800.

## Alternate sequence

The alternate sequence used the same controls and reversed the six upgrade
acquisitions:

- Familial Bond: Large Litters;
- Gardeners: Careful Sowing;
- Warren: Room to Breed;
- Warren: Guarded Burrow;
- Trailblazer: Far Sight;
- Trailblazer: Long Stride.

The Skip decisions and acquisition ticks stayed unchanged. The full addendum
is [ALTERNATE_ORDER_CONTRACT.md](ALTERNATE_ORDER_CONTRACT.md).

## Checks that passed

- Unity EditMode: 210/210 tests passed.
- Development bundle validation: `VALID`.
- Held-out bundle validation: `VALID`.
- EX-010 phase/report validation: `VALID` for both panels.
- Development outputs: 200 phase rows, 200 delta rows, 20 final rows.
- Held-out outputs: 50 phase rows, 50 delta rows, 5 final rows.
- Alternate development outputs: 200 phase rows, 200 delta rows, 20 final
  rows.
- Alternate held-out outputs: 50 phase rows, 50 delta rows, 5 final rows.
- No phase was marked `NO_DATA`.
- The first phase in each run is marked `NO_PREVIOUS`; later phase deltas are
  chronological deltas from the immediately preceding phase.
- Derived Stat-Line values and their validity statuses are both included.

The existing Herbivore Stat-Line validator reports
`VALIDATED_WITH_LIMITATIONS`; that is the validator's known limitation for
some independent raw-event cross-checks, not a failed EX-010 bundle. The
EX-010 structure and outputs themselves are valid.

## Files to review

Development bundle: `artifacts/cellular-experiment-20260906-043829/`

- `report.json` — complete raw report.
- `phase-statlines.csv` — one complete Stat-Line per phase.
- `phase-statline-deltas.csv` — per-stat chronological deltas.
- `final-statlines.csv` — independent final expedition Stat-Lines.
- `ex010-validation.json` and `ex010-report.md` — EX-010 checks and summary.

Held-out bundle: `artifacts/cellular-experiment-20260906-045030/`

The held-out bundle contains the same files for seeds 106–110.

Alternate development bundle: `artifacts/cellular-experiment-20260906-052118/`

Alternate held-out bundle: `artifacts/cellular-experiment-20260906-052945/`

The alternate bundles contain the same files, with the reverse acquisition
order defined in [ALTERNATE_ORDER_CONTRACT.md](ALTERNATE_ORDER_CONTRACT.md).

## Matched alternate-order result

The alternate run reversed the six upgrade acquisitions while keeping the same
Skip positions, seeds, scenario, options, and tick boundaries. Its contract is
[ALTERNATE_ORDER_CONTRACT.md](ALTERNATE_ORDER_CONTRACT.md).

The paired comparison contains 275 records: 220 phase records and 55 final
records across both panels. It is stored in
`artifacts/ex010-sequence-comparison-20260906-053441/`.

Observed final Stat-Line differences are alternate minus original:

- `SPO` did not change in any matched run.
- `BIR`, `MAT`, and `STRV` increased in every matched run in both panels.
  Their mean changes were `+557.55`, `+64,594.85`, and `+549.30` in
  development, and `+550.60`, `+67,195.80`, and `+544.00` held out.
- `RFS` was lower in every matched run in both panels.
- `FPO` changed direction between panels: mean `-0.50` in development and
  `+1.80` held out.
- `APS` was lower on average in both panels, but individual runs went in both
  directions: mean `-0.1485` in development and `-0.0365` held out.

These are direct Stat-Line comparisons, not a new scoring system. The safe
finding is that acquisition order changes later phase and final outcomes in
this scenario. The size and direction are not uniform enough to claim that one
order is generally better.

## What this does and does not show

This gives us clean phase-by-phase and final Stat-Line evidence for one
scenario, one ruleset, two fixed upgrade histories, and the recorded seed
panels.

It isolates a bounded upgrade-order effect for this tested scenario and these
two sequences. It does not establish a universal upgrade-order rule, a balance
target, or a general ranking of the upgrades.
