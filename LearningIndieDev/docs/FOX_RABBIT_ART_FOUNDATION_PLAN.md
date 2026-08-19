# Fox/Rabbit art foundation lane — production smart tiling

> Status: Proposed human-gated art/presentation lane | Owner: Josh + Sim | Branch: `codex/fox-rabbit-art-foundation`

This lane replaces the next upgrade-loop bucket for the current planning
window. Its purpose is to make the Fox/Rabbit cellular-simulation presentation
credible at gameplay scale: technically reliable, asset-complete, quiet enough
to read, and visually coherent as one art style.

The lane produces evidence and review candidates. It does not make the final
aesthetic decision on the team's behalf.

## Outcome

The smart-tiling system handles every supported terrain edge and corner case,
consumes versioned texture assets through an explicit import/atlas contract,
and presents Foxes, Rabbits, terrain, and plant resources without seams,
uncontrolled texture noise, or competing visual hierarchies.

## Work packages

### ART.1 — Smart-tiling correctness and edge cases

- Keep neighbor masks and atlas indices presentation-only.
- Define the supported mask model (cardinal 16-mask baseline; diagonal policy
  explicitly accepted or deferred).
- Test isolated cells, straight edges, inside/outside corners, four-way joins,
  full regions, empty regions, invalid masks, grid bounds, and terrain-family
  fallbacks.
- Verify deterministic results and a safe fallback when an atlas entry or
  terrain family is unavailable.
- Keep the editor 4×4 mask preview and runtime resolver on the same lookup
  table.

Acceptance: focused Edit Mode coverage passes for every supported mask and
fallback; no terrain seam or incorrect corner is observed in the preview or
runtime at native and gameplay scales.

### ART.2 — Production texture and atlas contract

- Replace the temporary Bare→desert mapping with an authored bare-ground
  family, or record a human-approved reason to retain it.
- Decide whether plant resources need a dedicated atlas or a deliberate terrain
  treatment; do not borrow an unrelated animal glyph.
- Verify exact sheet dimensions, cell size, pixels-per-unit, filter/compression
  settings, sprite names, row order, atlas packing, and `.meta` parity.
- Remove hidden or stale `Resources` assumptions; runtime and editor preview
  must consume the same explicit asset contract.
- Preserve source atlases and Unity GUIDs; rejected concepts stay under
  `artifacts/` and are never referenced by runtime code.

Acceptance: a clean import on another checkout resolves every required terrain,
Fox, and Rabbit sprite by stable name with no missing entry, wrong orientation,
or silent fallback.

### ART.3 — Fox/Rabbit presentation repair

- Repair or guard the current Noesis texture-source failure in
  `SpeciesSimulationViewModel.CreateAnimalAtlasSprites`.
- Validate Fox and Rabbit overrides, silhouette scale, role colors, contrast,
  and draw ordering in the actual Noesis view.
- Verify terrain, species, selection, and state feedback remain legible at
  1280×720 and 1920×1080.
- Capture gameplay-scale screenshots for the full authored species set that can
  appear in the simulation, not only the Fox/Rabbit pair.

Acceptance: the cellular preview opens without the atlas exception and the
runtime view shows readable, correctly scaled species over seamless terrain.

### ART.4 — Noise and visual-coherence review (human decision)

Prepare two or three bounded presentation variants if needed, varying only
texture density, contrast, accent usage, and edge treatment. Review them in the
real simulation composition, not as isolated tiles.

The human decision must answer:

- Does terrain read as one connected material rather than a repeated stamp?
- Are Fox and Rabbit silhouettes immediately distinguishable at gameplay scale?
- Do role colors and terrain contrast support comprehension without visual
  overload?
- Do selection, danger, food, and other feedback states compete with the art?
- Is the visual language cohesive enough to become the slice's art baseline?

Record the decision as `Accepted`, `Needs Revision`, or `Inconclusive`, with
links to the screenshots and rejected alternatives. AI-generated concepts may
inform the comparison but cannot approve the style.

## Verification matrix

| Surface | Required evidence |
| --- | --- |
| Resolver | 16-mask Edit Mode coverage, invalid/fallback cases, deterministic lookup |
| Editor preview | 4×4 mask sheet for each terrain family at native scale |
| Runtime | Cellular prototype with Fox/Rabbit and terrain at 1280×720 and 1920×1080 |
| Asset contract | Import settings, stable sprite names, atlas entries, `.meta` parity |
| Integration | Noesis atlas creation, species fallback, selection/state overlays |
| Review | Gameplay-scale screenshot set plus human style decision |

## Entry dependencies

- `docs/TILE_AUTHORING_GUIDE.md` is the asset-authoring contract.
- `docs/CELLULAR_SPRITE_TILING_PLAN.md` defines the current 16-mask resolver and
  known temporary mappings.
- P1-006, P1-007, and P1-008 in `docs/LOOSE_ENDS.md` are the current validation
  and atlas defects this lane closes.
- Existing Unity `ProjectSettings` edits are unrelated and must not be folded
  into this lane without explicit review.

## Explicitly out of scope

- Upgrade design or implementation.
- Save/load, persistence, economy, or simulation-rule changes.
- A generalized terrain framework or Unity `RuleTile` migration.
- Final-volume marketing art, audio, animation, or broad UI redesign.
- Aesthetic acceptance by automation or AI without human review.

## Definition of done

- All supported smart-tiling masks and documented fallbacks are covered by
  focused tests and visually checked.
- Terrain and species assets follow one explicit, reproducible import contract.
- The Noesis atlas exception is resolved or isolated behind a documented,
  reviewable fallback.
- Fox/Rabbit presentation is readable and low-noise in the actual simulation
  at both target resolutions.
- A human accepts the art baseline, requests a bounded revision, or records the
  direction as inconclusive; no silent aesthetic assumption is promoted.
