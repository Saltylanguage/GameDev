# GLJP route seed diagnostic

**Status:** Bevin approved the bounded diagnostics on November 7 and decided on November 8 that ordinary Fox pursuit of Hare movement scent should default off. The revised standalone ecology comparison produced four detailed 100-tick traces and four compact batch validation failures. Disabling ordinary scent matched October populations through tick 100 for both seeds. The subsequent pinned scratch full-route comparison changed completion in opposite directions by seed (10100: on failed at 237, off closed at 1800; 10108: on closed at 1800, off failed at 236). This older diagnostic build does not validate the newly implemented Unity default or establish balance. Current focused Unity `Simulation` EditMode passed 518/518; current PlayMode/default journey verification remains separate. **Decision owner:** Bevin. [GLJP-06](https://trello.com/c/eo0iqoRL/154-gljp-06-diagnose-route-seed-survival-mismatch-before-further-development) tracks the remaining route test. See the [preflight and execution evidence](../../handoffs/2026-11-07-gljp06-route-preflight.md).

**November 7 preflight finding:** The frozen runner contains the historical shared sources as embedded resources; their recomputed source hash matches the historical manifest and snapshot. The current runner rejects snapshots with a different embedded source hash, so step 2 below cannot run unchanged. Review the provenance and compatibility implications before selecting an alternative; do not silently bypass the guard or present re-exported inputs as identical. See [route evidence comparison](../../handoffs/2026-11-06-gljp-route-evidence-comparison.md).

## Question and hypothesis

Why did the historical no-action seed 10100 complete the 1,800-tick route while the current fresh-profile Unity test fails at tick 237? One hypothesis is a change in simulation code or data between the two snapshots; the available evidence does not identify a particular change. A second question is whether seed 10108's historical natural failure versus its current shop arrival reflects the same difference. Both seeds are deliberately selected diagnostic cases, not a representative balance sample.

## Historical evidence and read-only inputs

- Historical report: [Field Data current parity and matched spending comparison](FD-Current-Loop-Refresh-20261019/REPORT.md); exact shared-source hash `476b1e8d8d5e06e084247976a21c7b2ed8c4ffa34f05431e85324356d991d21a` included uncommitted source, so the recorded HEAD alone is insufficient.
- Historical snapshot: `artifacts/field-data-t14/verified-20261019/forest-edge.json`; on November 6 its SHA-256 matched the parity manifest: `4fa29d34f328eb21041c738bb556f993247116dc72beb822492aa617bfd7a40b`.
- Frozen historical runner and dependencies: `artifacts/field-data-t15/verified-20261019/frozen-runner/`. Historical no-action schedule and retained reports: `artifacts/field-data-t14/verified-20261019/`.
- Historical setup: 36×20 nonwrapping, 400 Plants / 55 Hares / 20 Foxes, empty active Genome, 18 rounds of 100 steps. The matched comparison's policies skipped free rewards. Do not overwrite any historical input, runner, report, or manifest.

## Approved bounded scope

1. Freeze a new output directory containing the current Git HEAD, dirty-worktree disclosure, shared-source identity, current runner and dependencies, commands, two seed IDs, and all inputs. Verify the old snapshot and historical runner against their manifests before use.
2. Compare old and current runners using the *same historical snapshot and no-action schedule*, provided the current runner accepts them without conversion or silent migration. Check snapshot/schema fingerprints, scenario, topology, initial populations, simulation mode, research/Mutation eligibility, journey definition, and action semantics before comparing outputs. If any cannot be matched, stop and report the mismatch; do not call the runs paired.
3. For seeds 10100 and 10108, retain raw reports and compare first divergence in initial state and per-tick/phase population, births, deaths, feeding, predation, earned rewards, terminal reason and tick, as supported by both report versions. Distinguish source/runtime changes from input and policy differences. If old reports lack enough per-tick telemetry, state that limit instead of inferring a cause.
4. Only after establishing input and runner parity, consider a separately specified current-authored-snapshot arm. Any difference there combines authored-data and code changes unless independently isolated. It cannot by itself identify a causal change.

## Completion and failure criteria

The diagnostic succeeds if both implementations produce complete, validated reports on matched, explicitly identified inputs and the first observable divergence (or absence of one) is recorded with its evidence limits. It fails as a paired comparison if either run crashes, provenance is missing, schemas or policies differ, or fixed inputs cannot be demonstrated equivalent. Neither outcome changes game tuning, closes GLJP tickets, approves balance, or replaces a player review. A wider seed sample requires a separately approved scope.

## Scope superseded — November 9, 2026

Bevin set the November build as the canonical baseline and directed that October-value comparisons stop. The October comparisons above remain historical records only. Do not run more October comparisons or use October values to guide current tuning. GLJP-06 should focus on current November route behavior when Unity can be safely recovered. No root cause or balance adjustment is approved by this direction.
