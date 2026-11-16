# S6 Fox carcass-value diagnostic — factual report

**Experiment:** `S6-FOX-CARCASS-VALUE-20261111` ([approved contract](CONTRACT-DRAFT.md)).
**Execution:** November 13, 2026, from `codex/s6-predator-first`; simulation source commit `8a92425866b4b5ef3cd9c77968f53554e17f63b8`.
**Status:** Initial five-arm panel and the contract-triggered scale extension completed. The report records 340 accepted runs; no production balance decision has been made.

## Question and inputs

In one 600-tick node (the agreed 48-hour comparison window), how many Hares does a starting Fox population need to avoid starvation, and does that relationship stay similar as both populations scale up?

All runs used the authored Forest Edge scenario (36×20, nonwrapping), 400 starting Plants, neutral Genome, no Mutations or upgrades, no player actions, opposed-roll combat, current production behaviors, and natural reproduction. Each completed run lasted 600 ticks. Only the opening Fox and Hare counts varied. The three core 2:1 arms share 100 matched seeds each; the two fixed-six-Fox controls retain the initial 20 matched seeds. No predictive model was used.

## Provenance and validation

- Unity resolved the local `Genome.meta` conflict to the branch's canonical GUID, `c576ac05bbb49f7488aa54f8ac574eaa`. The resolved asset has no outstanding tracked diff.
- Focused `SpeciesDomainTests`: **160/160 passed**, 0 skipped. Evidence: [NUnit XML](../../../../artifacts/unity-tests-20261113-152327/EditMode-results.xml), [Unity log](../../../../artifacts/unity-tests-20261113-152327/EditMode.log), and the scoped [diagnostics index](../../../../artifacts/unity-tests-20261113-152327/diagnostics-summary.md).
- Full Clean test run on Unity `6000.4.6f1` (revision `722204`): EditMode **764/773 passed, 8 failed, 1 skipped**; PlayMode **92/103 passed, 4 failed, 7 skipped**. The 12 failures are preserved in the [EditMode XML](../../../../artifacts/unity-tests-20261113-152426/EditMode-results.xml) and [PlayMode XML](../../../../artifacts/unity-tests-20261113-152426/PlayMode-results.xml); logs and the scoped [diagnostics index](../../../../artifacts/unity-tests-20261113-152426/diagnostics-summary.md) are retained beside them. EditMode failures cover Field Data rewards/population controls, shared plant/creature capacity, Warren crowding snapshots, and a Tough Hide health expectation. Three PlayMode failures report an unknown Hare Mutation; one authored route fails at seed 10101, tick 442. These failures are not recorded as passing validation.
- The first fresh batch attempt was rejected by the analyzer because no-preset Hare `PREY` telemetry was zero despite Fox kills. It contributed no accepted seed results. The telemetry condition was corrected in `8a924258`; a four-seed Unity reference then reconciled Fox and Hare population accounting. A separate one-run smoke at seed 62020 also passed and is excluded from the inferential panels.
- The successful frozen snapshot was exported after commit `8a924258`. Source hash: `b2cf7f809a3108b33d6c6878ec9c5006ed3173bccb96ae2eaf57c99cb901fd7f`; snapshot SHA-256: `12664e4915cca1c720b70002dbdd39c1a0f696aedf9deb028821b398b022b941`. Forest Edge fingerprint: `a8c005fdb3696275f469699cbf739da337e656b35f912abcaee48f88d18b253b`; asset SHA-256: `0ece03f19616942f052b16f07fe171d9bd73663cc1deea26604c75438024f69e`.
- The original plan completed 100/100 runs (identity `1ee2921528c1682a0c06f1c5c40def48729bbee2ee5a83770330a3adcba72b54`). The contract's uncertainty condition was met, so the 2:1 scale arms were expanded with 80 new matched seeds each, 62021–62100: 240/240 more runs (identity `e9241fac1ec978aa071d3dbb361a033a33a7454ad4aef4f5062ed4d828965560`). The 6:1 and 1:1 controls were not expanded. The accepted run panels therefore contain 340 runs total; seed 62020 was reserved for smoke and is not included. Both batches used the same worker assembly SHA-256 `08579eb01c377964e9d35e417e2be8df2181a40f0084a24c8e303c58eab5df6a`.
- The .NET 8-targeted portable worker was built with no warnings or errors and run using installed .NET `10.0.4` with process-local `DOTNET_ROLL_FORWARD=Major`, because .NET 8 was not installed. Main-panel raw JSONL SHA-256: `eba3861abed5f0d18047baf6640fa58192dace153053ebc11b2b59a49b2cebd6`; extension raw JSONL SHA-256: `59512dee320704f9e48c98501d45d4ad5b4a71840e8c6019ab527067284d8164`. All recorded Fox/Hare FPO and reproduction ledgers reconciled, openings matched the plans, and carcass energy consumed did not exceed energy created.

Machine-readable evidence: [expanded validated summary](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/expanded-validated-summary.json), [initial plan](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/plan.json), [extension plan](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/scale-extension-plan.json), [initial raw runs](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/batch/runs.jsonl), [extension raw runs](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/scale-extension-batch/runs.jsonl), and [source snapshot](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/source.json). The raw files are retained under ignored `artifacts/`.

## Seed outcomes

“Any Fox starvation” means at least one Fox starvation death occurred in a seed. “Foxes remaining” means at least one Fox was alive at tick 600; it does not mean every starting Fox survived. Intervals are 95% Wilson intervals over seed outcomes.

| Opening Fox:Hare | Matched seeds | Seeds with any Fox starvation | Seeds with Foxes remaining | Hare extinct at tick 600 |
| --- | ---: | ---: | ---: | ---: |
| 2:1 (2 Fox/1 Hare) | 100 | 47/100 (47%; 37.51–56.71%) | 100/100 (100%; 96.30–100%) | 100/100 (100%; 96.30–100%) |
| 2:1 (4 Fox/2 Hares) | 100 | 82/100 (82%; 73.33–88.30%) | 100/100 (100%; 96.30–100%) | 100/100 (100%; 96.30–100%) |
| 2:1 (6 Fox/3 Hares) | 100 | 100/100 (100%; 96.30–100%) | 100/100 (100%; 96.30–100%) | 100/100 (100%; 96.30–100%) |
| 6:1 (6 Fox/1 Hare) | 20 | 20/20 (100%; 83.89–100%) | 20/20 (100%; 83.89–100%) | 20/20 (100%; 83.89–100%) |
| 1:1 (6 Fox/6 Hares) | 20 | 6/20 (30%; 14.55–51.90%) | 20/20 (100%; 83.89–100%) | 15/20 (75%; 53.13–88.81%) |

## Population and feeding measures

Values are per-seed medians with observed minimum–maximum in brackets. Fox starvation is individual Fox deaths; the seed-level incidence above is a separate measure.

| Opening Fox:Hare | Fox births | Fox starvation deaths | Hare births | Fox kills | Carcass energy consumed per starting Fox | Closing Foxes | Closing Hares |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 2:1 (2 Fox/1 Hare) | 2 [0–4] | 0 [0–2] | 0 [0–0] | 1 [1–1] | 48 [25–48] | 3 [1–5] | 0 [0–0] |
| 2:1 (4 Fox/2 Hares) | 5 [0–12] | 2 [0–7] | 0 [0–0] | 2 [2–2] | 48 [32.5–48] | 8 [3–12] | 0 [0–0] |
| 2:1 (6 Fox/3 Hares) | 11 [4–19] | 3 [1–8] | 0 [0–0] | 3 [2–3] | 48 [32–48] | 14 [8–19] | 0 [0–0] |
| 6:1 (6 Fox/1 Hare) | 6 [4–9] | 6 [4–8] | 0 [0–0] | 1 [1–1] | 16 [16–16] | 6 [5–8] | 0 [0–0] |
| 1:1 (6 Fox/6 Hares) | 13 [3–31] | 0 [0–6] | 3 [0–120] | 8.5 [5–20] | 119.42 [80–272] | 19 [9–31] | 0 [0–107] |

Aggregate death and energy ledgers across each arm:

| Opening Fox:Hare | Fox births | Fox starvation deaths | Hare births | Hare combat deaths | Hare starvation deaths | Carcass energy created / consumed / remaining |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 2:1 (2 Fox/1 Hare), 100 seeds | 160 | 51 | 0 | 100 | 0 | 9,600 / 9,357 / 243 |
| 2:1 (4 Fox/2 Hares), 100 seeds | 530 | 162 | 0 | 200 | 0 | 19,200 / 19,084 / 116 |
| 2:1 (6 Fox/3 Hares), 100 seeds | 1,059 | 303 | 0 | 299 | 1 | 28,704 / 28,658 / 46 |
| 6:1 (6 Fox/1 Hare), 20 seeds | 116 | 116 | 0 | 20 | 0 | 1,920 / 1,920 / 0 |
| 1:1 (6 Fox/6 Hares), 20 seeds | 286 | 14 | 334 | 206 | 61 | 19,776 / 17,303 / 2,473 |

All 340 seed ledgers recorded zero Fox crowding/other deaths and zero Hare crowding/other deaths. The 6:3 arm recorded one Hare starvation death across its 100 seeds; other Hare deaths in that arm were combat deaths. The analyzer derives energy created as 96 times Fox kills and remaining energy as created minus Fox energy consumed; this first pass has no carcass decay or other Fox food source.

## Scope limits

The run tests one authored 36×20 scenario, one 600-tick window, and no player actions, purchases, Mutations, or upgrades. Fox and Hare natural reproduction remain enabled. The expanded 100-seed panel applies only to the three 2:1 scale arms; the fixed-six-Fox controls have 20 seeds each. Compact batch output retains aggregate counters and per-seed population histories but omits detailed death-event timing and the full combat-roll sequence. The full Unity suite has the 12 failures listed above, and no visual board inspection was performed. The report makes no production Fox energy, population-ratio, or reproduction-value change.
