# S6 predator-first Mutation calibration — factual report

**Experiment:** `S6-PF-MUTATION-TUNING-20261111` ([contract](CONTRACT.md)).  
**Execution:** November 11, 2026, on local branch `codex/s6-predator-first`.  
**Decision owner:** Josh. This report records measurements; it does not approve final balance values.

The avoidance and reproduction follow-up tests supersede the earlier readiness recommendation in this report. See [`FOLLOWUP-REPORT.md`](FOLLOWUP-REPORT.md); no manual review has started.

## Baseline calibration

The selected test setup starts Node 1 with 55 Hares, 12 Foxes, and 500 Plants. At Node 2, Foxes reset to 48 and Plants reset to 500; only the Node 1 closing Hare count carries. Hares are recreated with default individual state. Each node has six 100-tick phases. The baseline uses no Mutation, neutral Genome, and no purchases or population actions.

On the disjoint 100-seed confirmation panel (20200–20299), the no-upgrade control had **3/100 Node 1 failures (3%)** and **60/100 cumulative failures by Node 2 (60%)**. Their 95% Wilson intervals are 1.0–8.5% and 50.2–69.1%, respectively. Among the 97 seeds that survived Node 1, 57 failed during Node 2 (58.8%). The baseline meets both diagnostic targets.

All run records opened Node 1 at 55 Hares / 12 Foxes / 500 Plants. Every Node 2 record carried the Node 1 closing Hare count and reset to 48 Foxes / 500 Plants. The Batch validator and the summary script also reconciled the recorded population and reproduction activity. At the 100-tick Plant checkpoints, the minimum control count was 381; no baseline confirmation seed reached zero Plants at a checkpoint.

## 100-seed matched Mutation confirmation

All four arms used seeds 20200–20299. Each Mutation was absent during phase 1, then one rank remained active through phase 12.

| Condition | Node 1 failures | Cumulative Node 2 failures, 95% Wilson interval | Mean Hares at Node 1 close | Mean Hares at Node 2 close | Node 2 failures among Node 1 survivors |
| --- | ---: | ---: | ---: | ---: | ---: |
| Control | 3/100 | 60/100, 50.2–69.1% | 16.79 | 6.88 | 57/97 (58.8%) |
| Faster Movement `+0.5` | 6/100 | 55/100, 45.2–64.4% | 20.29 | 9.10 | 49/94 (52.1%) |
| Tough Hide `+2 Health` | 2/100 | 42/100, 32.8–51.8% | 24.08 | 12.81 | 40/98 (40.8%) |
| Reproductive Drive `+10 percentage points` | 5/100 | 69/100, 59.4–77.2% | 20.61 | 5.71 | 64/95 (67.4%) |

Matched Node 2 outcomes compared with the control:

| Mutation | Control failures rescued | New failures in Mutation arm | Net fewer failures |
| --- | ---: | ---: | ---: |
| Faster Movement | 26 | 21 | 5 |
| Tough Hide | 31 | 13 | 18 |
| Reproductive Drive | 17 | 26 | -9 |

The confidence intervals overlap. The paired counts are descriptive; they do not establish a causal effect size on their own.

## Mechanism and ecology measurements

- **Tough Hide:** Foxes landed 26,151 successful hits in this arm. Of those, 12,521 (47.9%) were recorded as nonlethal; the control had zero nonlethal Fox hits. The mutation therefore demonstrated the requested “survive a successful basic Fox hit” mechanic. It also left more Hares at both node closes. Fox kills and starvation deaths were higher in total because more Hares remained available for longer: 11,626 Fox kills and 3,934 Hare starvation deaths during Node 1, plus 2,004 Fox kills and 366 starvation deaths during Node 2. Its lowest recorded Plant checkpoint was 249, with no zero-Plant checkpoint.
- **Faster Movement:** Hare movement steps after acquisition rose from 2,105,073 in control to 2,770,031 (+31.6%). Fox successful hits rose from 12,273 to 12,721 (+3.7%). The Node 2 point estimate was five percentage points lower than control, but Node 1 failures rose from 3/100 to 6/100 and the paired result was 26 rescued / 21 regressed seeds.
- **Reproductive Drive `+10`:** Successful Hare reproduction attempts after acquisition increased from 3,869/7,853 eligible attempts (49.3%) to 4,578/7,753 (59.0%). Births increased from 11,019 to 12,373 (+12.3%). Cumulative Node 2 failure rose to 69/100. Fox kills rose to 11,276 in Node 1 and 1,817 in Node 2; starvation deaths rose to 3,967 and 242. The lowest Plant checkpoint was 277, without a zero-Plant checkpoint.

These counts describe events in the run; they do not by themselves prove why failure rates moved.

## Additional 12-seed screens

The following diagnostic screens used seeds 10100–10111. Their 95% intervals are wide; they were not treated as 100-seed confirmation.

| Condition | Node 1 failures | Cumulative Node 2 failures | Mean Hares at Node 1 / Node 2 close |
| --- | ---: | ---: | ---: |
| Control | 0/12 | 8/12 | 16.92 / 6.42 |
| Keen Senses `+1 detection tile` | 0/12 | 11/12 | 12.83 / 0.08 |
| Escape Instinct `+8 percentage points` | 1/12 | 8/12 | 11.75 / 1.08 |
| Faster Movement, then add Keen Senses | 1/12 | 11/12 | 6.75 / 0.08 |
| Reproductive Drive `+20 percentage points` | 2/12 | 7/12 | 9.58 / 0.42 |

The `+20` reproduction rank produced 1,631 post-acquisition births versus 1,342 for its matched control (+21.5%), but had more Node 1 failures, lower Hare carry at both closes, and a lower Plant checkpoint minimum (295 vs 396). It failed the preregistered Node 1 screen, so it did not receive 100-seed confirmation. The two-choice avoidance route used Faster Movement from phase 2 and added Keen Senses from phase 3.

## Provenance and validation

- The primary 12-seed Mutation screen completed 60/60 runs; the matched confirmation completed 400/400 runs. The two follow-up batches completed 36/36 and 24/24 runs. Every batch completed without a reset or validation error.
- The primary confirmation batch identity is `ac7391baf7f1a12265f09845e40b87e1dd752e5e837336aae1b545dca2b02faf`; its `runs.jsonl` SHA-256 is `86164227ed57e0917f7a5dc241c08983ba29557e1256231f9758b2adadc9c92b`.
- The Release CellSim worker build passed. The portable worker self-test passed, including deterministic replay, snapshot round-trip, population placement/capacity, and purchase-policy checks. Build emitted one NU1900 warning because the NuGet vulnerability endpoint was unavailable; there were no compiler errors.
- The primary worker source hash was `df0dd7165eaad9817a52a092d494fa33977f208ac8ab4dc8e94d0bf87909d25c`; the snapshot data fingerprint was `b7cac6867848cf29338350381e5d03122339bc8304ff7d399091d3c6aa17f8b3`.
- The Forest Edge portable snapshot was previously Unity-exported. Its stored data fingerprint was validated and rebound to the current worker source; the authored scenario asset blob is unchanged. No fresh Unity export, Unity Editor test suite, or human playtest ran for this report.
- Compact runs retained phase activity and behavior totals but not detailed combat rolls, death events, or behavior transitions. Plant minima are phase-checkpoint observations. Predator detections and pre-contact attack avoidance do not have direct event counters in the current report format.

The [machine-readable paired summary](paired-summary.json) retains seed-level outcomes, reset validation, confidence intervals, paired counts, mechanism counters, and run hashes. Raw JSONL files and compressed per-condition summaries remain in the ignored `artifacts/S6-Mutation-Tuning-20261111/` directory.
