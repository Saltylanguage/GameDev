# S6 Fox-start sweep — design-boundary correction, AI analysis v2

- **Analysis ID:** `S6-FOX-START-A2`
- **Date / analyst:** November 10, 2026; Codex (GPT-6), code and contract inspection
- **Sources:** [experiment contract](CONTRACT.md), [factual report](REPORT.md), [previous analysis](AI_ANALYSIS.md), and Josh's November 10 node-transition correction
- **Decision state:** Previous node-two balance recommendation withdrawn; corrected experiment and balance decision pending

## Corrected design rule

Josh clarified that each simulation node has trackable fixed starting populations for all species other than the one being played. Only the played species' population carries into the next node. In the introductory Hare route, Hares carry over; Foxes reset to the authored starting count at every new node. Plants and any future nonplayed species likewise use their own authored starting populations. Continuous simulation rounds *within* a node remain distinct from the boundary between nodes. Whether carried population means headcount only or persistent individual state is a separate implementation detail under discussion.

## What the published sweep actually measured

The batch plan used one retained grid for 12 consecutive 100-tick phases. The current Field Data implementation does the same: `SpeciesSimulationPreview.PrepareNextRun` creates one `SimulationRunState` for the full route; `FieldDataJourneySession` binds that run in its constructor and `Continue` resumes it at each round and shop. `CellSim.Batch` journey replay likewise creates one run for all nodes. The existing `AuthoredRoutePausesAtAllShopsAndCompletesOnTheSameRun` Editor test asserts that behavior. Thus the batch was faithful to the *current implementation* and to the previous continuous-world documents, but the implementation and experiment contract conflict with Josh's clarified design.

The [factual report](REPORT.md) and raw records remain valid observations of the retained-world variant. Their first-node starting counts and tick-600 outcomes can still inform the opening: 19/100, 15/100, and 20/100 Hare extinctions with 16, 18, and 20 starting Foxes on the confirmation seeds. **The 25/100, 26/100, and 31/100 cumulative tick-1200 results do not estimate the intended two-node route.** Nor do the low node-two Fox kills, Fox disappearances, Plant exhaustion, or the proposed need for a separate Fox escalation establish what happens when Foxes and Plants are reset at the node boundary. Resetting them may materially change both pressure and food availability; the direction and size require a corrected run.

## Superseded recommendation and next check

Withdraw the v1 recommendation to introduce a separate timed Fox escalation or tune Plant renewal based on the old node-two trace. First implement or isolate the correct node transition, then rerun the same 16/18/20 starting-Fox comparison with fixed nonplayed starting counts at each node, matched seeds, Hare carryover, and explicit boundary telemetry. Validate a node-two start snapshot for every seed before interpreting any failure rate. Preserve old artifacts under their original experiment ID and write a new contract/report for the corrected model; do not silently overwrite the earlier run. A fresh-profile Unity route and portable runner should agree on boundary populations before using the result for balance.

**Confidence:** High that the published sweep and current game implementation retain all populations across nodes; high that its node-two conclusions are inapplicable to the clarified design; unknown whether a fixed Fox reset alone produces the desired difficulty ramp.
