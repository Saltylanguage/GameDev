# Baseline Parity predator-balance pass

[Working state](../WORKING_STATE.md) | Status: ready-for-playtest

- Owner: Codex
- Branch: BevLaptopBranch
- Baseline commit: f1a585c
- Date: 2026-08-15

## Scope

This pass intentionally changes only the canonical Plant/Herbivore/Carnivore trio used by `BaselineParity`. The authored Forest Edge, Wetland, and Open Range species retain their distinct scenario balance.

## Applied tuning

- Carnivore: forage threshold `16 -> 48`, vision range `4 -> 6`, reproduction chance `0.40 -> 0.50`.
- Herbivore: reproduction chance `0.50 -> 0.35`, maximum litter size `3 -> 2`.
- Baseline Parity: carnivore starting probability `0.004 -> 0.007`.
- The scenario-generation editor tool was updated to preserve the canonical species values if the assets are regenerated.

## Rationale

Carnivores spawn at 48 energy but previously did not seek herbivores until they fell to 16 energy. Matching the forage threshold to starting energy makes predation possible from the opening simulation step. The remaining changes reduce herbivore compounding growth and give the predator population a slightly more reliable initial presence.

## Validation

- `dotnet build Assembly-CSharp.csproj -v:q` completed with 0 errors.
- Unity 6000.4.6f1 completed a headless import and script compilation with exit code 0.
- No runtime balance outcome is claimed yet. Test `BaselineParity` across multiple fixed seeds and compare final populations, extinction frequency, kills, births, and starvation before changing the other authored scenarios.
