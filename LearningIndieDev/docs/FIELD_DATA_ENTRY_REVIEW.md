# FD-T01 - Field Data entry and compatibility review

- **Reviewed source:** `acc371ac84aa01476e562e9b29e3e441263b8f2e`, `BevBranch`, October 16, 2026
- **Ticket:** [FD-T01](https://trello.com/c/kl9F3o2l/119-fd-t01-agree-the-test-route-entry-and-first-implementation-boundaries)
- **Status:** accepted and complete on October 16, 2026; Bevin waived any further review, including the additional Salty review
- **Scope authority:** [reviewed gameplay design](FIELD_DATA_JOURNEY_DESIGN.md), [S5 control](Sprints/S5-control-record.md)

## Player entry selected by Bevin

Keep the larger journey map as the entry. The player opens **Simulation** from
the GalapagOS Desktop, sees the existing larger map, then selects the **Field
Data Test Route** from that map. This selection starts the complete five-node
test journey:

**Simulation 1 -> Shop 1 -> Simulation 2 -> Shop 2 -> Simulation 3**

Each simulation contains six 100-tick rounds. One world and wallet continue
through all 18 rounds. Shops hold the world at ticks 600 and 1,200; successful
completion is at tick 1,800. Extinction or abandonment still ends the journey
under the approved rules.

The larger map is the route-selection surface, not an extra encounter inside
the test journey. Selecting this test must not first claim one of the older
prototype's opening rewards or conditions. The test still starts with 0 FD and
no newly granted journey upgrades. The existing frozen launch Genome snapshot
remains input; this work adds no permanent Genome effects or currency.

## Accepted implementation boundary

Present a clearly named **Field Data Test Route** launch action within the
existing map view. Reuse that view's Noesis presentation rather than adding a
Desktop application, scene or general route-selection framework. The exact
button placement belongs to FD-T13's rendered review.

Keep the current 29-marker `ForestEdgeJourney` asset intact. Add a separate
five-node map plus an explicit Field Data definition reference in composition.
The larger map and the active test-route map must be separate references:
displaying or reopening the larger map must not select, start, reset or settle
the test route. The player's explicit launch action selects the frozen
definition and switches the active route presentation to its five nodes.

Use an explicit selection method on the preview/helper composition before
constructing the Field Data session. The definition's stable identity selects
the mode; scenario `ForestEdge` plus player species `hare` is not sufficient,
because the old map uses that same pair. Do not infer the mode from asset names,
scene names, map rows or the mere presence of a configured definition.

If the Field Data definition is absent, preserve the current prototype and
standalone behavior. If the player explicitly selects an invalid configured
definition, show a useful validation failure and remain at route selection;
do not silently start another route. Starting the older route must likewise
not instantiate the new economy session.

Bevin accepted this entry and compatibility plan on October 16 and explicitly
waived further review. The larger map remains the confirmed entry. No new API,
button, asset or runtime behavior has been implemented by this document.

## What current source establishes

| Current source | Observed behavior | Consequence for the selected route |
| --- | --- | --- |
| [Desktop host](../Assets/UI/GalapagOS/Scripts/GalapagOSDesktopNoesisHost.cs) | Binds the Desktop Simulation launcher, captures active-profile launch inputs, and opens the map when the preview is journey-ready | Keep this canonical entry and its profile failure recovery; add route selection inside the map |
| [Launch request](../Assets/Scripts/Game/Simulation/SimulationLaunchRequest.cs) and [default setup](../Assets/UI/Lab/VM_ExpeditionSetup.cs) | Launch data includes profile/scenario/species/seed and frozen Genome; defaults are ForestEdge/Hare/10100 | Keep those defaults for development; a route definition needs an explicit additional composition/selection seam |
| [Preview](../Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs) | Current journey has two six-round cycles; node eligibility assumes reward row 0, simulation row 1, condition row 2 and simulation row 3 | A different map asset alone cannot run the five-node route; new-mode commands delegate to the Field Data session and leave the legacy branches intact |
| [Map asset](../Assets/Scripts/Game/Simulation/JourneyMapAsset.cs) | Applies by scenario/species and owns node layout/connectivity; no Field Data definition exists | Preserve the large map's layout/identity; use an explicit selected definition to distinguish modes |
| [Node kinds](../Assets/Scripts/Game/Simulation/JourneyNodeAsset.cs) | Serialized kinds are Simulation=0 through Reward=5; no Shop exists | Append Shop=6 when needed; do not reorder existing values or reinterpret Condition as Shop |
| Preview clock and [runtime composition](../Assets/Scripts/Game/CaveGeneration/CellularAutomataPrototypeRuntime.cs) | Preview forwards one Update into the existing manager/helper | Retain one clock, runner and progression; no extra update loop in the journey helper |
| Preview settlement and next-run handling | Boundary/completion paths award legacy currency, and PlayNextSimulation can rebuild progression/world | Bypass those awards/resets only for the selected Field Data session; shops/node transitions never call the legacy next-run reset |

## Development ecology reference

This source review records the current authored inputs; it does not approve
their balance or freeze the later 200-seed panel.

- Scenario: [ProductionData ForestEdge](../Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset), 36 x 20, starting Plant 400 / Hare 55 / Fox 35.
- Scenario species references resolve to [plant](../Assets/Data/TestData/plant.asset), [hare](../Assets/Data/ProductionData/CellularSimulation/Species/hare.asset) and [fox](../Assets/Data/ProductionData/CellularSimulation/Species/fox.asset) through their actual asset GUIDs. The Plant input is currently under TestData; do not rename, relocate or replace it as part of FD-T01.
- Default launch seed: 10100. Use the existing frozen active-profile Genome snapshot; this review invents no approved Genome configuration for the later comparison.
- Development source baseline: `acc371ac84aa01476e562e9b29e3e441263b8f2e`; the latest commit touching this scenario/Hare/Fox asset set is `3086bfb`.
- Local SHA-256 source/input manifest: `artifacts/fd-t01-entry-20261016/source-baseline.json`. It records inspected source and input bytes, not the complete resolved runtime configuration.
- FD-T15 must freeze the then-current source revision, resolved scenario/rules/options/Genome, definition and actual 200 seeds before matched comparisons. The development reference above is not evidence from those runs.

## Compatibility and scope to preserve

| Boundary | Required behavior / future verification |
| --- | --- |
| Larger map | Retain all existing nodes, edges, placements, panorama/scroll/hover behavior and its older route. Selecting the test is explicit; merely viewing the map grants no rewards |
| Standalone path | No selected Field Data definition preserves existing six-round behavior and public APIs; retain ordinary Mutation caps and historical report meanings |
| World continuity | Test shops hold the same runner/board/history at ticks 600/1,200; no new grid, seed reset, cooldown reset or second wallet |
| Economy | FD starts at 0; approved rewards, six interventions, ordered income and signed Hare effects remain journey-local; permanent wallets/Genome purchases and ecology corrections are separate |
| Profile | Preserve banked currency, unlocks and feature availability; retain independent playtime/tick accounting and the frozen participating-species Genome input |
| Evidence | Existing tests are regression references, not newly passed tests. New route tests must exercise explicit selection, reject invalid definitions, and prove that reopening the larger map does not reset or settle a run |

Existing regression locations include [MainMenuPlayModeTests](../Assets/Tests/PlayMode/MainMenuPlayModeTests.cs),
[SpeciesPresentationPlayModeTests](../Assets/Tests/PlayMode/SpeciesPresentationPlayModeTests.cs),
and [JourneyMapAssetTests](../Assets/Tests/Editor/JourneyMapAssetTests.cs).
Focused coverage and actual results belong to the implementation tickets.
No Unity test, runtime, player-build or economy-panel claim is made here.

## FD-T01 acceptance status

| Criterion | Evidence / remaining review |
| --- | --- |
| Launch path and map/definition selection reviewed by Bevin and Salty | Bevin accepted the plan and explicitly waived further review on October 16. This replaces the additional Salty-review requirement; no Salty review is claimed |
| Five-node/18-round route and compatibility boundary | Recorded above, consistent with the reviewed design and current source |
| Field Data-only scope | Recorded above; permanent rewards/Genome effects and broader ecology corrections remain separate |
| PDD/SDD technical proposal revisions | Updated PDD launch process and SDD rollout to preserve the larger map as entry and distinguish entry map from active test route |

FD-T01 is complete by Bevin's explicit acceptance: "we dont need any further
review that FD-T01 is good to go" (October 16, 2026). The original joint-review
criterion is satisfied under this user-directed exception; it does not record
a review by Salty. FD-T02 may proceed within the accepted boundaries.
