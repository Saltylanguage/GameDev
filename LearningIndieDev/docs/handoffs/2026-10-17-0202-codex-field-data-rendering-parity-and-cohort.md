# Field Data rendering, replay parity and matched panel — October 17, 2026

## Outcome and remaining human work

The retained five-node route is playable from the larger Desktop journey map.
Round decisions, six controls, both shops, income ordering and terminal wallet
handling have runtime and rendered evidence. Five authored-action fixtures
match between Unity and the headless runner at every decision boundary. The
current-source 200-seed panel completed all 600 scheduled runs.

FD-T13's independent human visual acceptance and FD-T16's team self-play and
accept/revise decision remain unperformed. Use the
[self-play record](../FIELD_DATA_SELF_PLAY_REVIEW.md); automated runtime is not
human decision time. No balance or content decision has been invented.
Changes remain local on `BevBranch`, based on `acc371ac`; no implementation
commit or push is claimed by this handoff.

## Corrected implementation issues

- The domain and UI now require the free node choice to resolve before paid
  shop actions. The old behavior was reproduced by a failing regression at
  `artifacts/unity-tests-20261017-011920`.
- Field Data controls and shop/reorder rows have bounded button dimensions.
  The resolved free-choice cards collapse, and a long chosen order scrolls
  within its list so confirmation remains reachable. Missing shop slots
  collapse instead of displaying placeholder cards.
- Shops start at 1, the wallet fits four-digit balances, and offers show signed
  target/effect, ownership and family cap. Purchase, capped reroll and reorder
  availability are projected explicitly; domain rejection remains authoritative.
- Payouts use readable operation labels instead of internal offer IDs. Income
  acquisition explains next-payout activation. Successful results show the
  balance retained until exit and actual journey upgrade counts.
- The visual runner passes capture settings through inherited process
  environment as well as the live-editor request file; clean Editor startup
  can recreate `Temp`.
- Replay formerly drew ordinary choices from the portable authored litter
  catalog, while this Desktop configuration used the three standard skills.
  A shared reward-pool resolver and explicit `fieldDataOrdinaryRewards` export
  now preserve the same pool. Journey replay rejects missing-pool snapshots
  with an explicit crashed record requiring a fresh export. Ordinary ecology
  reports retain their existing meaning.
- Replay records before/after board digests and actual `chargedFieldData`,
  separately from the offer/receipt cost. A rejected action can quote a cost
  while charging zero.

## Verification

| Evidence | Result and retained path |
| --- | --- |
| Full clean EditMode | 397 passed, 0 failed, 1 dedicated-fixture skip; `artifacts/unity-tests-20261017-015437/EditMode-results.xml` |
| Full clean PlayMode after correcting a test's missing UI update frame | 49 passed, 0 failed, 6 graphics-runner skips; `artifacts/unity-tests-20261017-015735/PlayMode-results.xml` |
| Five-fixture Unity/headless parity | 1 test passed, 0 skipped; `artifacts/unity-tests-20261017-015621/EditMode-results.xml` |
| Graphics, 1280×720 | 2/2 passed; final display revision, `artifacts/visual-evidence-20261017-015907` |
| Graphics, 1920×1080 | 2/2 passed; `artifacts/visual-evidence-20261017-014742`, before the later wallet abbreviation and text-only readability changes |
| Standalone | Release build, 0 warnings/errors, and complete self-test passed; frozen tested DLL in `artifacts/field-data-t15/verified-20261017/frozen-runner` |

The graphics route enters through the Desktop icon and map, completes seed
10100, chooses naturally available incomes, visits both shops, opens/moves/
cancels the first draft, checks unaffordable confirmation in shop two, reaches
success, starts a fresh seed-10125 journey and captures natural failure. A
separate, explicitly synthetic ownership fixture covers capped offers, free
reroll exhaustion and successful paid random/chosen reorders. That fixture is
not evidence of authored ecology balance or a naturally attainable starter cap.

The full-suite attempt at `015437` initially had one PlayMode assertion failure:
it read result text immediately after the terminal tick, before the ViewModel's
next update frame. Adding that frame passed in the full `015735` PlayMode run.
The latest text-only changes also passed the `015907` graphics test.

## T14 reproducible evidence

Canonical current bundle:
`artifacts/field-data-t14/verified-20261017/with-reward-pool/`.

- `forest-edge.json`: authored scenario, explicit ordinary pool, definition and
  shared-source fingerprint; one fresh Unity ecology reference is alongside it.
- `parity-manifest.json`: exact snapshot, runner, actions and report hashes.
- Five fixtures: income/shop/random reorder, paid chosen reorder, population
  actions, signed species purchase and natural failure. They use existing
  seeds 10100, 10129 and 10125, selected for deterministic action coverage rather
  than as a representative balance sample.
- Unity verifies reward/shop identities, prices and cap flags, receipts,
  charges, order, all-species awards, income steps, every before/after board
  digest and final failure/success/exit state.
- `compatibility-check.json`: an authored replay repeated deterministically in
  all fields except measured runtime; a missing-pool fixture rejects explicitly
  and retains a crashed report. `legacy-ecology-report.json` verifies ordinary
  ecology replay still uses its existing report shape.

To repeat the cross-runtime test, set the process environment variable
`CELLSIM_FIELD_DATA_PARITY_MANIFEST` to that manifest's absolute path and run
`CellSim.ps1 -Command Test -Execution Clean -Mode EditMode -TestFilter
FrozenJourneyActionsMatchUnityAtEveryDecisionAndTerminalState` with the normal
explicit Editor path. Without that variable, the fixture test intentionally
skips rather than claiming parity against absent artifacts.

The earlier 200-reference export was stopped after the ordinary-pool mismatch
was identified. Its partial reference is retained with
`STOPPED-PRE-REWARD-POOL-FIX.json` and is not accepted current evidence. Earlier
frozen October 17 v2 artifacts were preserved.

## T15 current-source matched panel

Bundle: `artifacts/field-data-t15/verified-20261017/`.
The frozen manifest pins the DLL/dependencies, snapshot, empty active Genome,
definition/source fingerprints, all 200 seeds and exact copies of the approved
v2 action schedules. Those schedules already resolve node choices before shop
actions; no policy revision or ecology tuning was needed.

| Declared path | Completed | Survived | Failed | Successful paid actions | Rejected | Not executed |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Remove Hare | 200 | 50 | 150 | 158 | 1,047 | 2,195 |
| Add Hare | 200 | 0 | 200 | 589 | 460 | 2,351 |
| Save for second-shop ×2 | 200 | 45 | 155 | 2 | 46 | 152 |

`cohort-index.json` and all 600 reports are complete. Validation confirms each
seed 10100–10299 appears once per arm and all frozen hashes/source identities
still match. `paired-summary.json` records all 200 matched sets, actual spend,
first-10-FD tick, shop arrival balances, ×2 opportunity/purchase, failure loss
and success discard. Original per-run reports retain detailed offers, receipts,
affordability and action status.

Aggregate measured replay runtime is **984,824.0 ms** across the 600 processes.
The output-file creation window is **141.4 seconds**, not an exact total wall
time. The parent reporting wrapper incorrectly tested a null `LASTEXITCODE`
after a successful PowerShell parallel pipeline; its final timing record did
not execute. The completed cohort was validated without rerunning or replacing
it. Exact parent wall time and earlier active human effort are unrecorded.
The frozen manifest and execution record distinguish measured assistant setup/
analysis elapsed segments, automated runtime, and absent human play time.

These outcomes reproduce the earlier arm totals. They expose spending and
availability questions for self-play; they are not a numeric acceptance gate,
a causal explanation for Hare ecology, or authority to change tuning.
