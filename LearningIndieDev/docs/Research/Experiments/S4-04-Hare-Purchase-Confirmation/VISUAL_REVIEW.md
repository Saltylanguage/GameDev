# D5/S25 Unity visual review

Prepared October 8, 2026 for Bevin, from the supplied eight-run inspection plan.
This is a local visual-review fixture, not a new production default or a tick-700 viability test.

## Start here

1. Open `Assets/Scenes/ForestEdge_D5_VisualReview.unity` and enter Play Mode.
2. Select **Cellular Automata Prototype** in the Hierarchy. In its **Species Simulation Preview** Inspector, enter **60000** in **Visual Review Seed**, then click **Reset D5 Review to Seed**.
3. Check the Inspector: seed **60000**, grid **54x32**, **Wrapping**, phase length **100**, total phases **6**, tick-zero Plants/Hares/Foxes **325 / 30 / 15**. Then click the Inspector's **Start**.
4. Watch the Game view. At each decision, buy Hares first and then choose a Mutation or Skip. The Mutation/Skip resumes the simulation.
5. Between runs, change **Visual Review Seed** and click **Reset D5 Review to Seed**, even when the next run uses the same seed. This clears the previous expedition, wallet, purchases and Mutations. Click **Start** again.

The current Game-view legacy setup panel is hidden (`V_Window_CellSimulation.xaml`); setup belongs to the Lab. The supplied instructions to enable Developer Mode and apply settings in this Game view are therefore outdated. This inspection scene uses the runtime Inspector for setup and playback, with the Game view for purchases and Mutations.

**Do not click Apply S4 Fixture.** That prepares a different 36x20 / 400-20-10 run. **Reset D5 Review to Seed** applies the review settings without writing PlayerPrefs defaults or changing scenario assets. It also replaces any saved startup settings that Unity may have loaded.

## Configuration

| Setting | Review value |
| --- | --- |
| Grid | 54x32, wrapping |
| Starting Plants / Hares / Foxes | 325 / 30 / 15 |
| Starting probabilities | 0 / 0 / 0 |
| Hare vision | 9 |
| Fox starting energy | 160 |
| Step interval / ticks | 0.1 seconds / 600 |
| Continuous phases | Six phases of 100 ticks |
| Maximum / minimum population | 0 / 0 |
| Experimental features / coupled responses | On / Off |
| Fox attack cooldown | 0 |
| Randomized seed / Journey | Off / Off |
| Starting Genome / Mutations | None |

The copied scenario and Hare/Fox definitions live in
`Assets/Data/Inspection/ForestEdgeD5VisualReview/`. Species IDs remain `hare` and `fox`.
The Plant reference is the original scenario's existing `Assets/Data/TestData/plant.asset`.
Other species rules and entry order are preserved. The original scene and production assets are unchanged.

## Decisions

**Late-five:** buy no Hares after ticks 100, 200, 300 or 400. At tick 500, buy up to five, limited by availability and affordability.

**Restoration:** at each tick-100 through tick-500 decision, buy at most five Hares, stopping as soon as the live Hare population reaches 30 or purchasing becomes unavailable. Each Hare costs 10 Field Data.

**Gardeners, both policies:** choose only Efficient Digestion or Seed Dispersal. If both are offered, choose the one selected fewer times during this run; on a tie choose Efficient Digestion. Choose the only eligible offer if just one appears, or Skip if neither appears. Do not force an alternating sequence.

Stop when a run ends early. Do not revive it or continue past the review endpoint.

## Run sheet and verified endpoints

All eight live Unity preview replays matched the supplied purchase counts and stated endpoints on October 8. The following endpoint counts are fresh Unity observations; they do not establish visual readability or enjoyment.

| Order | Seed | Policy | Purchases at 100 / 200 / 300 / 400 / 500 | Endpoint tick | Hares / Foxes / Plants |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | 60000 | Late-five | 0 / 0 / 0 / 0 / not reached | 476 | 0 / 24 / 86 |
| 2 | 60000 | Restoration | 0 / 0 / 0 / 5 / 5 | 600 | 2 / 6 / 90 |
| 3 | 60002 | Late-five | 0 / 0 / 0 / 0 / 5 | 600 | 70 / 3 / 113 |
| 4 | 60002 | Restoration | 0 / 0 / 5 / 5 / 2 | 600 | 12 / 0 / 220 |
| 5 | 60005 | Late-five | 0 / 0 / 0 / 0 / 5 | 600 | 2 / 20 / 39 |
| 6 | 60005 | Restoration | 0 / 0 / 0 / 2 / 5 | 600 | 20 / 14 / 32 |
| 7 | 60001 | Late-five | 0 / 0 / 0 / 0 / 5 | 520 | 0 / 35 / 99 |
| 8 | 60001 | Restoration | 0 / 0 / 0 / 0 / 5 | 520 | 0 / 35 / 99 |

For seed 60000, both policies had **41 / 121 / 63 / 22 Hares** before purchases at ticks **100 / 200 / 300 / 400**. If a manual run differs, pause and record the first mismatching boundary and Inspector settings. Do not tune rules to force a match.

For seed 60002 Restoration, only two Hares were affordable at tick 500. For seed 60005 Restoration, two purchases at tick 400 restored 28 Hares to 30. Seed 60001 has identical actions and outcomes across both policies.

## What to observe and record

Use the Game-view speed controls to slow playback after tick 300. The Inspector also provides **Pause**, **Advance One Simulation Tick** and **Resume**. Record live populations before purchases; the verified checkpoint JSON records pre-purchase counts.

Read the live **PLANTS: total** beside the **POPULATION** heading in the Field Ledger. It updates from the simulation snapshot and remains visible during Mutation/purchase decisions. The separate `PLANTS 6/8` label below the collectible jars is not the ecological population total. The display fix was validated on October 8 at tick zero (325), the first decision (316), and the seed-60000 endpoint (86); see the [display handoff](../../../handoffs/2026-10-08-2242-codex-live-plant-population-display.md).

For each run, fill this template:

```text
Seed / policy:
Tick 400 Hares / Foxes / Plants (before purchases):
Tick 500 Hares / Foxes / Plants (before purchases, or not reached):
Endpoint tick and Hares / Foxes / Plants:
Purchases at 100 / 200 / 300 / 400 / 500:
Blocked purchase and reason:
Could I see why the ecology improved or failed?
Did purchasing feel meaningful, too late, repetitive, or like delaying collapse?
Screenshot paths: before/after an important purchase; endpoint.
```

Pay particular attention to the decline after tick 400 in seed 60000; recovery and remaining predator pressure in seed 60002; exposure of the last Hares in seed 60005; and whether the last purchase creates a visible recovery opportunity in seed 60001.

## Evidence and limits

- [Live preview replay data](../../../../artifacts/d5-visual-review/runtime-verification.json), including every reached pre-purchase boundary and final population.
- [Ready-state Game view](../../../../artifacts/d5-visual-review/ready.png).
- [Replay script](../../../../artifacts/d5-visual-review/verify-runs.cs). This invokes the real preview, wallet and Mutation APIs, advancing paused ticks rather than clicking the UI.
- [Inspector reset validation](../../../../artifacts/d5-visual-review/control-verification.json): all four review seeds reset to fresh runs and reproduced their first boundary (41 / 52 / 30 / 42 Hares respectively). Reset also restored seed 60000 Ready at tick zero afterward.
- Unity recompiled the Editor change with zero errors. The existing `S4EditorFixtureIsTransientAndSupportsPausedSingleTicks` regression passed 1/1.
- The replay exceeded the Pipeline response timeout, then completed and wrote all eight results. The JSON was independently checked for eight matches; the Editor returned to Ready. The two captured Console errors were Pipeline response timeouts. No compilation or gameplay error was observed.
- Human visual review, manual purchase interaction and player judgment remain pending. Tick 700 and the full test suites were not run.

The guide, copied assets and Inspector control remain local until committed and shared.
