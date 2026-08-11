# Next architecture batch

The first bounded pass of the higher-risk work is now in place. It deliberately
stops before generalized rule plugins or asset-driven runtime state.

## CS-04 - Custom rule logic

### Activated prototype

**Alpha offspring** is the first custom mechanic. It is applied immediately
after a non-plant offspring is created, before population limiting. The rule
uses the simulation's seeded random source and changes only that newborn cell.

`AlphaOffspringRule` is data owned by `CellularSimData`, not a delegate or a
runtime callback. Its chance and bonuses affect `CellularSimData.Fingerprint`.

### Deliberate limits

1. Alpha qualification is only a probability in this pass.
2. Alphas receive health and energy bonuses at birth; status persists on the
   creature cell through normal simulation updates.
3. Diet qualification, inheritance, alpha caps, and pack behavior remain open.
4. Vision/sight should introduce its own focused seam when it has a concrete
   target-selection rule; it should not be forced into alpha logic.

### Guardrail

Do not add a universal event bus, delegate registry, scripting language, or
plugin system until at least two real mechanics demonstrably share an interface.

## CS-06 - Data asset/editor authoring

### Implemented direction

`CellularSimDataAsset` is a Unity `ScriptableObject` definition that converts
to a new immutable `CellularSimData` snapshot when `CreateRuntimeData()` is
called. It provides Inspector fields for globals, arbitrary species IDs,
patterns, species rules, and alpha-offspring settings.

Keep these boundaries separate:

```text
CellularSimDataAsset (serialized authoring definition)
    -> validation and conversion
CellularSimData (immutable runtime snapshot + fingerprint)
    -> SimulationRunState (mutable run state)
```

### Remaining decisions

1. Serialized custom terrain definitions are deferred; the asset currently
   uses the proven bare/grass defaults.
2. The preview scene does not yet select scenario assets because its current
   settings UI is hard-coded to the three prototype species.
3. Editor-specific validation UX can be added when authored assets become a
   regular workflow; runtime conversion already validates IDs and rules.

### Guardrail

Do not expose mutable runtime dictionaries through the asset or use the asset as
global run state. Keep the code-authored path beside the asset path while the
prototype schema continues to change.

## Current status

- Ruleset fingerprints and prototype audit are pushed in `97b77bc9`.
- CS-07 audit found no safe deletion candidate.
- CS-04 alpha offspring and CS-06 asset authoring now have bounded first
  implementations. The next useful experiment is alpha qualification or sight,
  not generalized frameworks.
