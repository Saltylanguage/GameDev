# Field Data implementation handoff — October 17, 2026

## Result

The authored-settings Desktop success path, natural failure, and manual
abandonment now have clean PlayMode coverage. The Field Data code can replay the
five-node journey headlessly from an authored Forest Edge snapshot, and the
frozen 200-seed comparison has been executed for all three approved action
arms. T13 and T14 still have explicit acceptance gaps; T16 needs team
self-play and judgment.

## T13 evidence

- `artifacts/unity-tests-20261017-004043/PlayMode-results.xml`: 4/4 passed,
  including route entry, all 18 rounds/two shops, natural failure, and manual
  abandonment through End confirmation.
- Success route uses seed 10100 and the authored 0.2-second interval. It
  confirms profile currency and feature unlocks survive successful results and
  a new journey.
- Natural failure uses seed 10125 at the authored interval. It confirms the
  extinction-round payout is skipped, the journey wallet is forfeited, and
  profile currency/features survive restart.
- Manual abandonment confirms the user-facing End dialog, failure forfeiture,
  and profile currency/features through a new journey.
- `artifacts/unity-tests-20261017-002151/EditMode-results.json`: FieldData
  suite passed 46/46.

The earlier seed 10125 route-success test ran with a 0.1-second editor setting.
It does not match the authored scenario and should not be used as evidence for
the frozen scenario. T13 still needs repeated Continue/results, shop re-entry
and lifecycle checks, fresh graphics captures, and independent human review.

## T14 evidence

Authored snapshot and 200-row Unity reference:

- Snapshot: `artifacts/field-data-t14/forest-edge-field-data-20261017.json`
- Unity reference: `artifacts/field-data-t14/forest-edge-field-data-20261017.reference.jsonl`
- Empty-action schedule: `artifacts/field-data-t14/empty-actions.json`
- Successful headless seed 10100: `artifacts/field-data-t14/seed-10100-empty-replay.json`
- Failing authored seed 10125: `artifacts/field-data-t14/seed-10125-empty-replay.json`
- Unity route test: `artifacts/unity-tests-20261017-003411/PlayMode-results.xml`

For seed 10100, Unity reference and headless journey both ended at Hare 1, Fox
0, Plant 717. The headless report has 18 population awards, three node-clear
awards, and 52 FD discarded when the player exits successful results. The
selected inputs and artifacts have these fingerprints:

| Artifact/input | SHA-256 or fingerprint |
| --- | --- |
| Authored snapshot JSON | `33fd190765379ab85c37716b5c07edf63dfdaf32d0f548f6255ad9a292d5ab96` |
| Unity reference JSONL | `3e499799beff1aea29a9b5d7cbab22a7a0c8df076b8c6b9674b86c6d087a911f` |
| CellSim.Batch Release DLL | `7ee89c9c3ab4afb213d05db7a2993a489a3add4f45cf22f3ee1d467a00838831` |
| Portable runtime source hash | `3d4de5dda9f00494024fbec6e89a1e01b0c8aa6d9f8c1873ef381ec32a84fa74` |
| Field Data definition | `ac5dacc3bca2a54615472d62e31aa04dc97d6c8c6f1c56d9f8290f253070051b` |

The snapshot records HEAD `acc371ac` as its source anchor, but the worktree was
dirty. Treat that SHA as a base anchor; the content fingerprints and DLL hash
identify the tested inputs and runner. The active Genome snapshot for this
first-test baseline was empty.

T14 remains In Progress. Current authored parity covers the no-purchase route's
final populations. It does not yet compare Unity and headless offer identities,
purchase receipts, reorder histories, intermediate board outcomes or report
schema compatibility on the same action fixture.

## T15 comparison

Run script: `artifacts/field-data-t15/run-20261017/run-matched-field-data-study.ps1`

Frozen v2 schedules, all 600 complete report files, cohort index and aggregate:
`artifacts/field-data-t15/run-20261017/cohort-200x3/`.
The machine-readable provenance and run summary is
`artifacts/field-data-t15/run-20261017/execution-record.json`.

| Matched arm | Survived / 200 | Failed / 200 | Action successes | Rejections | Not executed |
| --- | ---: | ---: | ---: | ---: | ---: |
| Unity authored reference baseline | 45 | 155 | — | — | — |
| Remove Hare when affordable | 50 | 150 | 158 | 1,047 | 2,195 |
| Add Hare when affordable | 0 | 200 | 589 | 460 | 2,351 |
| Save for second-shop ×2 | 45 | 155 | 2 | 46 | 152 |

The removal rejections were 844 final-individual guards and 203 insufficient
Field Data. Add-Hare rejections were 452 insufficient balance, seven
no-valid-space results, and one invalid boundary. In the save arm, the ×2
offer was unavailable or already purchased 30 times and unaffordable 16 times.
Only two attempted purchases succeeded. These values are descriptive; no
numeric survival gate or balance revision was approved.

All 600 reports completed, each of the 200 seeds appeared once in every arm,
and all runs shared the same scenario and definition fingerprints. Measured
simulation runtime totals 872,424.8 ms (1,454 ms mean, 3,129.6 ms maximum).
Active team setup and analysis effort was not recorded.

## T16 handoff needed

The team still needs to play the matched weaker/stronger paths, note real time
spent while decisions are available and the corresponding simulation ticks,
and record concrete intervention-versus-saving opportunity-cost examples.
Then record the team's accept/revise judgment and its evidence. Do not ask
players to justify their choices or teach back the payout math. Do not choose a
new content item or revision from the automated survival counts alone.

T15 active setup/analysis time is also missing. T13 visual and repeat-entry
acceptance and T14 cross-runtime action parity remain open. No Trello status or
comment was changed during this work.
