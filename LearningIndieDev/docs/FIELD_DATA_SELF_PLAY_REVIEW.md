# Field Data — internal self-play and decision record

**Published and closed, October 21, 2026:** Bevin authorized publication and
closing the FD workstream after acceptance. All 16 Trello cards are now Done
and complete. T16's checklist explicitly resolves uncollected timing and matched
human comparison notes as deferred, the no-revision replay condition as not
applicable, and next content as undecided. These are scope dispositions, not
claims that missing observations were performed. See the
[T16 acceptance comment](https://trello.com/c/osmzcZVf/134-fd-t16-review-internal-self-play-and-record-accept-revise#comment-6ad8527f794bb2a3e26f1825)
and [publication handoff](handoffs/2026-10-21-0153-codex-field-data-publication-and-ticket-closeout.md).
Implementation `e6cadcd` and integration `dc4f6c5` are on BevBranch and ProjectMain.
Sprint 5 remains Active for other work. Earlier dated statements below retain
their original pre-publication context.

**Accepted, October 21, 2026:** Bevin explicitly confirmed that playtesting of
the initial five-node implementation looks good, that it is a good starting
point despite being bare bones, and that APS is fixed. Human acceptance of
this initial playable slice is satisfied. Further content, polish and balance
development remain future work; no new content scope or price change was chosen.
Unrecorded timing/matched-human-path details below remain data gaps, not grounds
to withhold this explicit acceptance. No live Trello closure or publication is
claimed by this local decision record.

**Status, October 20, 2026:** Bevin completed the full route and reported that
the loop felt good, including near population misses for both Foxes and Hares.
Record this as positive acceptance of the first loop with an APS reporting
defect to correct; no ecology/price revision was requested. The APS defect is
now fixed and verified in Unity. See the [playthrough/fix handoff](handoffs/2026-10-20-0007-codex-field-data-human-playthrough-and-aps-accounting-fix.md).
Matched human policy notes, real decision times and next-content decisions
remain unrecorded rather than inferred from this custom-path run.
The October19 frozen gameplay revision passed eight direct Unity/headless
parity fixtures and a600-journey comparison; this later accounting fix has the
focused checks above, without claiming a new cohort/parity run. See the
[comparison report](Research/Experiments/FD-Current-Loop-Refresh-20261019/REPORT.md).
Earlier rendered checks retain their dates; FD-T13 now has direct full-route
human feedback, and FD-T16 has a positive keep-loop decision with the remaining
evidence limits above. This is not a claim that all Trello criteria were closed. The October 17 economy
panel is historical evidence, preserved separately.

## Open the playable route

1. Open `Assets/Scenes/GalapagOSDesktopTest.unity` in Unity and enter Play Mode.
2. Open **Simulation** from the Desktop. Keep the larger journey map as the
   entry, then select **View Field Data Test Route**. Review the five nodes,
   then click **Start This Route** to begin. **Back** before starting returns
   to setup. During a run, use **Map** on the HUD or inside the round/shop and
   results panels; **Back** returns to the same run or decision.
3. Use the authored Forest Edge scenario, Hare player, empty active Genome and
   0.2-second simulation interval. Use a fresh journey for every comparison.
   As approved October 18, the authored opener is **400 Plants, 55 Hares and
   20 Foxes**. Earlier 35-Fox panels remain historical; do not compare their
   outcomes as if the starting populations were matched to this setting.
   The freshly verified successful route uses actual seed **10100**; **10129**
   completed the historical October 17 saved action fixtures. Verify the
   actual seed shown by the running simulation because a new run increments
   the base seed by the run number.
4. Keep those starting conditions matched between spending paths. Playback
   speed can reduce waiting; changing the simulation interval changes the
   tested ecology and is a different setup.
5. At a simulation-node end, review Field Data settlement and resolve the
   free node choice, then **Return to Map**. Population controls appear only
   between ordinary rounds. Click the ready **Shop** card to shop. Return to
   the map when finished and click the ready **Simulation** card to resume.
   Nothing advances while waiting for node entry. Final success returns to
   the completed map; starting a new journey is a separate action.

## Play matched spending paths

The automated panel uses seeds **10100–10299**, once per path, with the same
scenario and no ecology tuning. The October 19 v3 schedules allow population
purchases only at ordinary rounds 1–5, excluding node-end settlement. All free rewards are skipped in these three
declared policies. For a bounded human check, play the same seed on the paths
being compared; the team can choose additional seeds without changing the
frozen automated panel.

| Path | Decision at each round break |
| --- | --- |
| Remove Hare | After Skip, attempt one 10-FD Remove Hare purchase; insufficient funds and final-individual rejection are expected possibilities. |
| Add Hare | After Skip, attempt one 10-FD Add Hare purchase; keep the same setup and seed as the removal path. |
| Save for ×2 | Skip free choices, save through the first shop, and attempt the 50-FD ×2 offer at shop two if it is actually offered and affordable. |

Also inspect the shop/reorder interactions directly: actual effect and price,
owned copies, disabled actions, editable order, cancellation and the wallet.
Income modifiers remain eligible rather than guaranteed. The two-shop starter
route cannot naturally accumulate three copies of a single species family;
its capped/reroll display has separate automated fixture evidence.

## Team observations

Record observations from playing. Do not fill these from automated runtimes,
screenshots or assumed player reactions. No external-player interviews or
explanations of purchase decisions are required.

| Participant/date | Actual seed and path | Node, round and tick | Real decision time | Observation or clarity problem |
| --- | --- | --- | --- | --- |
| Bevin / captured 2026-10-20 | Seed10100, custom mixed purchases;17 ordinary skills,1 shop species effect,0 income modifiers | Completed all5nodes/18rounds, tick1800 | Not measured/reported | Felt good; near population misses for both Foxes and Hares. APS INVALID at completion. Live capture confirms143 Hares/26 Foxes/434 Plants;8 Hares added,9 removed;43 Foxes added,9 removed. |
| Bevin / 2026-10-21 | Seed10100, custom choices; screenshot shows0 income modifiers,1 species effect,11 ordinary skills | Failed at tick1168; this is a later run, separate from the earlier full completion | Not measured/reported | Explicitly accepts the initial loop and confirms APS fixed. Screenshot showsADD6/REM7, FPO0/expected0, ReconciledTrue and validAPS-1.04. Identity55+6-7+325births-239preyed-140starved=0. |
|  |  |  |  |  |

Concrete spending tradeoff observed, if any:

________________________________________________________________________

________________________________________________________________________

Visual review of map entry, payout, controls, shops/reordering and results:

☑ Accept the current first-slice presentation  ☐ Revise

Participant/date: Bevin, October21,2026. Initial implementation accepted;
APS correction confirmed in play. Future visual polish is still expected.

## Accept or revise the loop

☑ Accept this first loop for now; APS fix confirmed October21  ☐ Revise a bounded price/effect bundle

Decision owner/date: Bevin; feedback captured around the October19–20 boundary,
recorded October20,2026 and explicitly reaffirmed October21 after further play.
Basis: completed full playthrough, positive loop feedback and human confirmation
that APS is fixed. No numeric gate or further economy/content approval is implied.

Evidence from direct play and the matched panel: direct custom-path completion
above plus the separately frozen October19 automated comparison. The automated
policies skip free mutations and do not reproduce Bevin's chosen build. APS
failed because paid interventions were missing from population accounting,
not because the completed journey failed. Focused Unity checks now cover it.

________________________________________________________________________

________________________________________________________________________

If revising, name the related prices/effects and the bounded follow-up:

________________________________________________________________________

Replay both matched paths after that revision. A multi-value bundle is allowed;
do not claim that one value alone caused the outcome. No numeric survival gate
is imposed. Choose the next content addition only after the findings and this
decision exist; permanent currency, Fox/Plant/multispecies shop effects and
random events remain separate decisions.
