# S6 starting Fox count and node-two pressure — experiment contract

- **Experiment ID:** `S6-FOX-START-20261110`
- **Human decision owner and scope approval:** Josh, November 10 design chat: run the proposed starting-Fox sweep, confirm valid counts, then check node-two pressure before deciding the next change.
- **Status:** Approved for execution; no result or balance decision recorded here.
- **Question:** Does lowering only the starting Fox population from 20 to 18 or 16 make the first 600-tick Hare node unlikely to fail while retaining roughly 60% cumulative no-action seed failure by the end of node two?
- **Hypothesis:** Fewer initial Foxes reduce early Hare combat losses and first-node extinction, but may also make node two too safe. Plants may become the later pressure if more Hares survive.

## Frozen inputs and arms

| Input | Value |
| --- | --- |
| Published source | `feature/s6-fun-pass` at `b94929aa1f5cf2a16fc258c1ba77fc00d314f55b`; clean worktree before this contract; no simulation code or authored asset edit |
| Frozen Unity export | `LearningIndieDev/artifacts/s6-pf-diag-20261109/forest-edge-v2.json`, SHA-256 `429ea327fd8a6275cec833da710e04696bc64f01e94a6062b25fd38098daf455`, shared-source hash `6fcc6913b758717217150094b7bdd9c8afca85cb96eb96484ab31669f3db3990` |
| Scenario | Authored Forest Edge asset, 36×20 nonwrapping, 400 Plants and 55 Hares, opposed-roll combat, `bev-experimental`, ordinary Fox movement scent off, neutral active Genome |
| Intended change | Starting Fox count only: 20 (current baseline), 18, or 16. All use the existing `-startingPopulations` input override. No later Fox introductions. |
| Journey proxy | One retained world, 12 consecutive 100-tick phases (ticks 1–1200), no Mutations, shop purchases, population actions, or player research. The batch runner continues observation after Hare extinction. |
| Diagnostic seeds | 10100–10111 inclusive, matched across all three counts. These include previously inspected seeds and are a parity/mechanism screen only. |
| Disjoint confirmation seeds | 10200–10299 inclusive, matched across all three counts. Run all three counts, regardless of diagnostic result, to expose the first-node/node-two tradeoff without selecting a winner from 12 seeds. |
| Worker | Current `CellSim.Batch` Release build from the published source; record assembly hash and .NET runtime in the run manifest. |

The current 20-Fox, no-action 24-phase ecology trace already matched the authored four-node journey at every shared decision boundary on the diagnostic seeds. Before interpreting this 12-phase sweep, compare its 20-Fox arm through tick 1200 with that frozen trace on the same seeds: first Hare extinction tick, every 100-tick population, and first-node death/activity counters. A mismatch invalidates the route proxy comparison and stops the confirmation panel until explained. The 16- and 18-Fox arms are isolated input overrides, not authored route or UI playthroughs.

## Validation and report contract

- **Technical validity:** Runner accepts all three starting populations; every run starts with exactly 400 Plants, 55 Hares, and its declared Fox count on a 720-cell board; all 336 scheduled runs complete with source/snapshot/plan identities and matched seed coverage. A failed run or mismatched fixed input is reported, not silently replaced.
- **First-node safety:** Count Hare extinction by tick 600 and first-zero tick on each seed. The design's “possible but unlikely” opening is operationalized here as at most 5/100 confirmed seed failures; this threshold is a screen for Josh, not an approved balance value.
- **Node-two pressure:** Count cumulative extinction by tick 1200 and extinction during ticks 601–1200 among first-node survivors. Compare the approximately 60% desired cumulative rate using a 50–70/100 screening band, with uncertainty intervals and per-seed differences. A count near 60 that mostly fails in node one does not pass.
- **Predator-first mechanism and ecology:** Report Hare combat and starvation deaths by node, Fox kills/attack opportunities, Hare births and populations at ticks 100/600/1200, Fox populations at 600/1200, Plant stock and first zero tick. Note any shift from predation to food depletion.
- **Falsification:** If 16 and 18 still lose many seeds in node one, a small starting-count cut does not solve the opening. If they make node one safe but node two rarely fails, a constant initial Fox count alone cannot produce the target ramp under this scenario.
- **Separation:** Raw batch JSONL and manifests remain immutable in ignored `artifacts/`; version a factual report, compact per-seed summary, separate AI analysis, and Josh's later decision independently. No source or authored scenario value changes in this experiment.
