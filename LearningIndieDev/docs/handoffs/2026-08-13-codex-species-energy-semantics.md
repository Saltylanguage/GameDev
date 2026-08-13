# Species energy semantics and rule preservation

Date: 2026-08-13

## Outcome

- Runtime rule edits and upgrades now preserve `SpeciesRole` and the authored
  forage threshold.
- `Energy` is the hunger/survival value. A creature attacks or seeks food only
  when its energy is at or below `ForageBelowEnergy`.
- `FoodReserve` is finite carried seed material. A successful seed drop consumes
  one reserve.
- Creature reproduction transfers the configured energy amount from the parent
  to the newborn instead of spawning the newborn at `StartingEnergy`.
- The Noesis and fallback runtime settings expose the forage threshold and label
  reproduction energy by its transfer behavior.

## Compatibility decisions

- Existing authored consumer species use their starting energy as the initial
  forage threshold. This preserves immediate feeding at run start while allowing
  a sufficiently large meal to produce a satiated period.
- Plant propagation retains its resource-production behavior. Alpha offspring
  bonuses also remain an explicit special-rule energy source.
- The serialized name `reproductionFoodRequired` remains in place to avoid an
  asset migration solely for terminology; player-facing/dev-facing labels now
  describe it as energy transferred to offspring.

## Validation

- `dotnet build SaltyGame.Runtime.csproj --no-restore`: passed.
- `dotnet build SaltyGame.Tests.csproj --no-restore`: passed with existing
  legacy `SpeciesArchetype` deprecation warnings.
- Focused tests cover role/threshold preservation, satiated versus hungry
  predation, reserve consumption on seed drop, offspring energy transfer, and
  fingerprint sensitivity.
- Unity test execution remains pending while the project is open in the editor.
