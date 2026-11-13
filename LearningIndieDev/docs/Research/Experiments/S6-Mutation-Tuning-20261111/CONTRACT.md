# S6 predator-first Mutation calibration

- **Experiment ID:** `S6-PF-MUTATION-TUNING-20261111`
- **Decision owner:** Josh
- **Approval:** Josh approved baseline calibration and tuning the three introductory Mutation strategies for a later manual playtest on November 11, 2026.
- **State:** Simulation prep is complete for the tested avoidance and reproduction candidates; no manual review has started. Keep the fixed Fox schedule as the provisional calibration baseline. Tough Hide remains the only tested Mutation with a 100-seed point estimate inside the 35–45% failure target. The two-rank Quick Escape and larger-litter paths performed worse than control on 100 matched seeds. Threat Exposure ranks 2–5 did not produce a consistent screen improvement; the rank-3 path finished at 62% failure on its 100-seed confirmation versus 60% for control. A stronger run-start gestation diagnostic also performed worse on 100 seeds. See [`FOLLOWUP-REPORT.md`](FOLLOWUP-REPORT.md) for those checks. No final Mutation set is approved.

## Question and targets

Can static starting populations make Node 1 usually survivable and produce about 60% cumulative no-upgrade Hare failure by Node 2? After that baseline is confirmed, does one early Mutation response at a time reduce cumulative Node 2 failure toward 35–45% while producing its intended visible effect?

Working screens:

- Baseline: at most 5% Hare failure at Node 1 and 50–70% cumulative failure by Node 2.
- Mutation: about 35–45% cumulative Node 2 failure, a mechanism signal in the predicted direction, and no shift to zero-Plant collapse.
- These are diagnostic balance targets, not player-tested acceptance criteria.

## Frozen baseline and run protocol

| Input | Value |
| --- | --- |
| Scenario | Authored Forest Edge, 36×20, nonwrapping |
| Played species | Hare (`hare`) |
| Node 1 counts | 55 Hares, 12 Foxes, 500 Plants |
| Node 2 nonplayer counts | Reset to 48 Foxes and 500 Plants; carry only the Node 1 closing Hare count |
| Hare state at reset | Recreate the carried count as new Hares with default individual state |
| Combat and options | Opposed-roll; `bev-experimental`; natural attack opportunities; Fox scent off |
| Genome and actions | Neutral Genome; no purchases, population actions, or other intervention |
| Observation | Two nodes, six 100-tick phases each; close at ticks 600 and 1200 |
| Acquisition timing | Phase 1 has no Mutation; one rank is active from phase 2 through phase 12 |
| Baseline confirmation | Seeds 20200–20299; all comparison arms use these same seeds |
| Diagnostic screens | Seeds 10100–10111; all arms within each screen use the same seeds |

Counts and reset openings were validated from every run's phase records. A reset was valid when Node 2 opened with the Node 1 closing Hare count, 48 Foxes, and 500 Plants. The portable worker validates population and reproduction accounting. The reset recreates individual Hare state according to the existing runner implementation and tests; the compact run file does not serialize every creature's reset state.

## Mutation candidates

| Strategy | Candidate tested | Expected local signal |
| --- | --- | --- |
| Avoidance | Faster Movement `+0.5` and Keen Senses `+1` predator-detection tile, separately; then their two-choice route; also the existing Escape Instinct `+8` percentage points of pre-contact attack avoidance | Hare movement and Threatened-state ticks, Fox proximity observations, Fox attack activity, deaths, and survival. No direct detection or pre-contact-avoidance event count is currently available. |
| Survive attacks | Tough Hide `+2` maximum/current health; a base Hare goes from 1 to 3 HP, so a basic 2-damage Fox hit leaves 1 HP | Fox hits split into lethal and nonlethal hits; Hare carry and failure |
| Faster reproduction | Reproductive Drive `+10` percentage points per rank from a 0.5 base; a separate 12-seed screen tried `+20` percentage points | Eligible and successful attempts, births, Fox kills, starvation, and Hare carry |

The separate avoidance candidates were screened before the combo. The combo acquired Faster Movement after phase 1 and added Keen Senses after phase 2. The `+20` Reproductive Drive run was diagnostic only: it did not qualify for 100-seed confirmation.

Follow-up screens added Quick Escape, larger maximum/minimum litter paths, and a gestation-time diagnostic. The two-rank Quick Escape and larger-minimum-litter paths received matched 100-seed confirmation; both increased cumulative Node 2 failure versus control. Full results and provenance are in [`FOLLOWUP-REPORT.md`](FOLLOWUP-REPORT.md).

## Confirmed baseline

The 100-seed no-upgrade control recorded **3/100 (3%)** Node 1 failures (95% Wilson interval 1.0–8.5%) and **60/100 (60%)** cumulative failures by Node 2 (95% Wilson interval 50.2–69.1%). Among the 97 Node 1 survivors, 57 failed during Node 2 (58.8%). This meets both working screens.

Across control seeds, the minimum Plant checkpoint was 381 and no seed had zero Plants at a recorded phase checkpoint. Starvation still occurred, so food remains a secondary pressure. Fox kills were the main recorded Hare mortality.

## Confirmed Mutation comparison

| Condition | Node 1 failures | Cumulative Node 2 failures (95% Wilson) | Mean Hares at Node 1 / Node 2 close | Outcome |
| --- | ---: | ---: | ---: | --- |
| No Mutation | 3/100 | 60/100 (50.2–69.1%) | 16.79 / 6.88 | Baseline target met |
| Faster Movement `+0.5` | 6/100 | 55/100 (45.2–64.4%) | 20.29 / 9.10 | More movement, but no clear survival shift; 26 seeds rescued and 21 regressed versus control |
| Tough Hide `+2` Health | 2/100 | 42/100 (32.8–51.8%) | 24.08 / 12.81 | Target point estimate met; 31 seeds rescued and 13 regressed versus control |
| Reproductive Drive `+10` points | 5/100 | 69/100 (59.4–77.2%) | 20.61 / 5.71 | Birth and success rates rose, but survival missed the target |

The minimum recorded Plant checkpoints were 356 for Faster Movement, 249 for Tough Hide, and 277 for Reproductive Drive. No confirmation seed had zero Plants at a checkpoint. The Tough Hide 95% interval still extends above the desired 45% failure ceiling, so its point estimate is promising rather than settled.

## Additional diagnostic screens

| Screen | Node 1 failures | Cumulative Node 2 failures | Mean Hare count at Node 1 / Node 2 close |
| --- | ---: | ---: | ---: |
| No Mutation, 12-seed screen | 0/12 | 8/12 | 16.92 / 6.42 |
| Keen Senses `+1` tile | 0/12 | 11/12 | 12.83 / 0.08 |
| Escape Instinct `+8` points | 1/12 | 8/12 | 11.75 / 1.08 |
| Faster Movement then Keen Senses | 1/12 | 11/12 | 6.75 / 0.08 |
| Reproductive Drive `+20` points | 2/12 | 7/12 | 9.58 / 0.42 |

These small screens have wide uncertainty and are not confirmations. They reject none of the strategy concepts, but they give no support for the tested avoidance values or the stronger reproduction value. The two-choice avoidance route is a path-level diagnostic, not an equal-cost single-rank comparison.

## Decision and limits

- Keep 12 Node 1 Foxes and 48 Node 2 Foxes as the working test baseline; it meets the seed-level targets in a disjoint 100-seed confirmation.
- Keep Tough Hide `+2` Health as the only tested Mutation with a 100-seed point estimate in the target window. Its result remains provisional; no manual review has started.
- Do not treat Quick Escape, the larger-litter paths, the gestation-time diagnostic, or the earlier movement, detection, and reproduction values as viable responses yet. Review the follow-up results and analysis in [`FOLLOWUP-REPORT.md`](FOLLOWUP-REPORT.md) and [`AI_ANALYSIS.md`](AI_ANALYSIS.md).
- No trained prediction model was used. Current research does not calibrate a predictor to this fixed-count, fresh-node setup; matched seeded runs provide the relevant evidence.
- Plant values are phase-checkpoint minima, not tick-by-tick minima. Compact runs do not include detailed combat-roll events, direct predator-detection counts, or direct pre-contact-avoidance counts.
- The Forest Edge portable snapshot was previously Unity-exported. For these runs, its data fingerprint was validated and its worker-source hash rebound to the current linked source; no fresh Unity Editor export or Unity playtest ran.
