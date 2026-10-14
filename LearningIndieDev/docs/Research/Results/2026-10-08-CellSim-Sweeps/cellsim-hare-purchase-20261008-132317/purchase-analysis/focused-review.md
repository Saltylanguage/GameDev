# Completed Hare purchase panel: focused review

Analysis by Codex / GPT-6 on 2026-10-08. Human owner: Bevin. Scope was approved; production tuning and follow-up selection remain human decisions.

12,800 assigned runs completed with 16 workers in 39.35 simulation minutes; automatic analysis completed at 14:15 EDT. All 128 arms have 100 fresh matched seeds (50000–50099). APS/AHS have Valid n=100 in every arm. Frozen input, aggregate and analysis hashes were checked on completion review.

Observed: restoring toward the starting Hare count improved both survival endpoints across all three paths in D4/S19, D4/S14 and D5/S25. C2/S14 Gardeners declined. Late-five was the only tested policy with positive observed purchase effects on both survival endpoints in all twelve candidate/path arms. Early-five reduced all-species survival in 14 of 16 arms, was neutral in one, and improved one by one percentage point.

Interpretation: timing and current population appear more useful than unconditional early reinforcement. This screen does not establish the ecological cause or prove gameplay enjoyment. Survival gains often accompany lower APS/AHS; direct purchases are ADD rather than births and are excluded from biological growth accounting.

## Restore toward starting population

Buy up to five at each reached window, only while below the original Hare count. Arrow columns compare no purchases with restoration on the same path. ΔSkip is all-species survival versus Skip all with the same restoration policy. All survival measures are at tick 600; deltas are percentage points. APS/AHS are per-run means over actual observed durations, including early-ended runs, with 100 valid samples.

| Candidate | Path | Hare alive: no buy → restore | All alive: no buy → restore | ΔSkip all alive | Mean APS | Mean AHS |
| --- | --- | --- | --- | --- | --- | --- |
| D4/S19 | trailblazer | 33% → 45% | 20% → 34% | +16 | -0.710 | -0.119 |
| D4/S19 | warren | 27% → 31% | 16% → 24% | +6 | -0.713 | -0.055 |
| D4/S19 | gardeners | 20% → 27% | 16% → 24% | +6 | -0.657 | 0.111 |
| D4/S14 | trailblazer | 35% → 41% | 19% → 25% | +6 | -0.730 | -0.270 |
| D4/S14 | warren | 27% → 40% | 21% → 27% | +8 | -0.686 | -0.280 |
| D4/S14 | gardeners | 41% → 51% | 38% → 49% | +30 | -0.681 | -0.019 |
| C2/S14 | trailblazer | 45% → 48% | 30% → 33% | +12 | -0.474 | -0.054 |
| C2/S14 | warren | 43% → 51% | 29% → 36% | +15 | -0.482 | -0.094 |
| C2/S14 | gardeners | 44% → 42% | 34% → 26% | +5 | -0.304 | -0.036 |
| D5/S25 | trailblazer | 42% → 63% | 30% → 44% | +10 | -0.679 | -0.306 |
| D5/S25 | warren | 46% → 61% | 34% → 45% | +11 | -0.638 | -0.290 |
| D5/S25 | gardeners | 42% → 50% | 39% → 48% | +14 | -0.713 | -0.096 |

## Largest individual all-species survival result

D5/S25 Gardeners with each-five had the highest observed all-species survival in this panel, 59%. Its matched gain versus no purchases was +20 pp (35 wins / 15 losses; unadjusted approximate 95% CI +6.6 to +33.4 pp). Relative to Skip all with the same purchase policy it was +26 pp. This maximum was selected from 128 arms and needs fresh-seed confirmation.

| D5/S25 Gardeners policy | Hare alive | All alive | Mean APS | Mean AHS | Mean ADD | Mean Field Data spent |
| --- | --- | --- | --- | --- | --- | --- |
| none | 42% | 39% | -0.692 | -0.052 | 0.00 | 0.0 |
| late-five | 50% | 47% | -0.693 | -0.080 | 3.45 | 34.5 |
| restore-toward-start | 50% | 48% | -0.713 | -0.096 | 6.36 | 63.6 |
| each-five | 63% | 59% | -0.797 | -0.109 | 21.91 | 219.1 |

## Purchase-effect matrix

Each cell is the all-species survival change versus the same path with no purchases, ordered Trailblazer / Warren / Gardeners, in percentage points. This comparison isolates buying; it is different from beating Skip all when both arms use the same purchase policy.

| Policy | D4/S19 | D4/S14 | C2/S14 | D5/S25 |
| --- | --- | --- | --- | --- |
| none | +0 / +0 / +0 | +0 / +0 / +0 | +0 / +0 / +0 | +0 / +0 / +0 |
| early-five | -6 / +1 / +0 | -5 / -10 / -3 | -12 / -6 / -12 | -4 / -8 / -2 |
| middle-five | -1 / +3 / -1 | -3 / -1 / -1 | -7 / -7 / -4 | +3 / +4 / -3 |
| late-five | +8 / +6 / +4 | +1 / +1 / +5 | +4 / +3 / +2 | +4 / +3 / +8 |
| each-one | +9 / +4 / +9 | +3 / +4 / -1 | -7 / -1 / -10 | +9 / +9 / +2 |
| each-three | +3 / +10 / +0 | +0 / +6 / -1 | -4 / -10 / -10 | +5 / +9 / +4 |
| each-five | +3 / +10 / +3 | +0 / +0 / +0 | -8 / -5 / -10 | +11 / +2 / +20 |
| restore-toward-start | +14 / +8 / +8 | +6 / +6 / +11 | +3 / +7 / -8 | +14 / +11 / +9 |

## Both-endpoint all-path qualification

Positive here means positive observed matched point estimates for Hare survival and all-species survival on all three paths. It does not imply all confidence intervals exclude zero.

| Candidate | Purchases improve all three paths vs own no-buy | All three paths beat Skip all with same buys |
| --- | --- | --- |
| D4/S19 | late-five, each-one, restore-toward-start | early-five, middle-five, each-one, each-five, restore-toward-start |
| D4/S14 | late-five, restore-toward-start | none, early-five, middle-five, late-five, each-one, each-three, each-five, restore-toward-start |
| C2/S14 | late-five | none, early-five, middle-five, late-five, each-one, each-three, each-five, restore-toward-start |
| D5/S25 | late-five, each-three, each-five, restore-toward-start | none, early-five, middle-five, late-five, each-one, each-three, each-five, restore-toward-start |

## Diagnostics and next decisions

All 64,000 assigned window rows exist: 57,532 reached; 6,468 not reached. Stop reasons: {'policy-cap': 47307, 'not-reached': 6468, 'insufficient-currency': 5500, 'target-met': 4725}. No placement/capacity stops were observed in this panel. Means of additions and spending retain all assigned seeds, including zero purchases. Currency totals describe the five eligible windows; final-run rewards remain separately recorded.

Suggested follow-ups for Bevin: prioritize D5/S25 restoration for shared path support, contrast D5/S25 Gardeners each-five with restoration and late-five for spend/survival/APS tradeoffs, and retain D4/S14 Gardeners restoration as another strong arm. Keep C2/S14 as a context-dependent comparison. No follow-up run is launched or production change approved by this analysis.

Limits: 100 seeds per arm; many exploratory comparisons; selecting maxima adds optimism. Mean confidence intervals and McNemar values are unadjusted. Paired rows retain different-window counts; compare rate changes with actual observation durations. A lower AHS does not itself establish Fox extinction or a worse player experience. Tick-700 ecology viability and enjoyment require separate evidence.

Evidence: report.md (all 128 arms), results.json (means/statuses/paired uncertainty), windows.csv (assigned windows and following-phase recovery), ../analysis/metrics.csv, ../analysis/paired-deltas.csv and ../analysis/metrics.sqlite.

Batch identity: `7fc4666c6ad2459af4c6917e32ca21987d8399a0417f2cfaf6c8a1767899fafa`. Raw SHA256: `aa03a3c4a6943249bf479672009367bb4fa93b36a7cc2f4ef80090a77af94aba`.
