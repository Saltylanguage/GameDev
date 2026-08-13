# Species field activation, scenario ownership, and cell-layer cleanup

Date: 2026-08-13

## Outcome

- Occupied entities age once at the beginning of each tick; newborns remain age
  zero for their birth tick.
- Destination terrain movement cost divides effective movement speed, retaining
  seeded fractional movement behavior.
- Resource terrain applies `RegrowthPerTick`, even underneath an occupying
  creature.
- Starting probability moved from reusable species assets to serialized entries
  in each scenario. BaselineParity, ForestEdge, OpenRange, and Wetland preserve
  their previous values.
- Resource depletion preserves terrain and any creature occupant. Movement,
  mortality, feeding, creature reproduction, wilt, and population limiting now
  preserve or remove the intended layer; layered populations count separately.
- The species/scenario generation editor utility writes the new scenario entry
  format and no longer carries unused probability/energy parameters.

## Scope boundary

- Terrain presentation color remains authored presentation metadata. It was not
  pushed into simulation behavior or a new rendering abstraction.
- No generalized entity/component framework was introduced; the existing
  `SpeciesCell` terrain/resource/creature representation remains the boundary.

## Validation

- `dotnet build SaltyGame.Runtime.csproj --no-restore`: passed.
- `dotnet build SaltyGame.Tests.csproj --no-restore`: passed with existing
  `SpeciesArchetype` deprecation warnings.
- Focused tests cover terrain-cost movement, aging/regrowth under an occupant,
  scenario-specific probability, resource-preserving movement/depletion,
  resource-preserving creature birth, and layered population limiting.
- Unity Edit Mode execution and imported-asset validation remain pending while
  the project is open in Unity.
