# Next steps after the Hare purchase screen

2026-10-08. Recommendation by Codex / GPT-6. Bevin approved the first step with
"okay lets begin step by step": the 96,000-run confirmation and bounded existing
data diagnostics. Later research extensions and production tuning remain proposed.

## First: confirm the useful choices

Retain all four accepted candidates: D4/S19, D4/S14, C2/S14 and D5/S25.
Keep Skip all, Trailblazer, Warren and Gardeners on each candidate.
Test six existing purchase policies: none, late-five, each-one, each-three,
each-five, restore-toward-start. The proposed grid has 96 combinations.
Use 1,000 fresh matched seeds per combination, proposed range 60000–60999,
disjoint from the qualifying 40000–40099 and purchase-screen 50000–50099 cohorts.
Total: 4 candidates x 4 paths x 6 policies x 1,000 seeds = 96,000 runs.
Use 16 workers, six 100-tick phases, price 10, initial Field Data zero and the
existing wallet/placement/Mutation rules. Freeze the current approved screen's
scientific inputs and implementation for comparability; preserve old evidence.

At the observed 5.42 runs/second this is roughly five hours of simulation plus
analysis. It is an estimate: different run lengths and host load change throughput.

Reasons for retaining these policies: none supplies the control; late-five was
positive on both survival endpoints in all twelve candidate/path arms; each-one
and each-three retain lower-spend choices; each-five tests the high-spend
D5/S25 Gardeners finding; restoration supported all three paths on three of the
four candidates. Early-five and middle-five are omitted from this confirmation
because the screen gave them less support. Omitting them does not permanently
remove those choices or prove they are universally bad.

Both references remain mandatory: same path/no purchases isolates purchase
effect; Skip all/same purchases isolates Mutation effect. Retain all assigned
seeds in survival/spend summaries, APS/AHS applicability and observed-window
differences, matched confidence intervals and wins/losses. Do not pool correlated
paths as independent repeats or silently invent a significance/viability gate.
Seek stable effect direction, magnitude and understandable costs across paths;
the human owner decides whether the evidence warrants development.

## Alongside confirmation: explain the existing tradeoffs

Use the completed screen's raw slashlines, paired observations and 64,000
assigned window records to examine phase-specific birth, predation, starvation,
crowding, plant supply, Fox reproduction/energy blocks, and the population change
after immediate purchased ADD. Break APS/AHS into their components and account
for different observed durations; common-phase comparisons are conditional
diagnostics and must not replace all-seed outcome summaries.

The score implementation excludes purchased ADD from replication fitness and
includes predation/hunting, starvation and crowding terms. Lower cumulative
APS/AHS does not by itself isolate the ecological mechanism or the quality of
player experience. Investigate D5/S25 Gardeners each-five, the lower-spend
restoration/late alternatives, and C2/S14 Gardeners' negative purchase effect.

## Then: refine player decisions around confirmed scenarios

Prioritize D5/S25 for broad path support and D4/S14 for the strong Gardeners
restoration arm, while retaining D4/S19 and C2/S14 as useful comparisons until
the confirmation changes that assessment. Next targeted axes would be restore
thresholds at 50/75/100 percent of the starting Hare count, caps of 1/3/5, and
the eligible purchase windows. Keep price fixed at 10 for that investigation.
These are future research axes and need harness extensions plus their own
reviewed matrix, rather than changing the confirmation run in flight.

Review matched-seed gameplay examples of a purchase helping, doing little and
hurting. Assess readable recovery, decisions on timing/quantity, whether each
path remains useful, and whether buying feels compulsory. A later observation
tail through tick 700 can test persistence after purchases; that requires a
reviewed harness extension and does not define the open tick-700 viability gate.

## Evidence

Completed screen: `artifacts/cellsim-hare-purchase-20261008-132317/`.
Start with `purchase-analysis/focused-review.md` and `results.json`;
diagnostics are in `windows.csv` and `analysis/metrics.sqlite` / paired CSV.
Results handoff: `docs/handoffs/2026-10-08-codex-hare-purchase-results.md`.
Approved confirmation protocol: `../S4-04-Hare-Purchase-Confirmation/PROTOCOL.md`.
