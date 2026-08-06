# AI contribution instructions

For Unity work in this project:

- Follow [`docs/UNITY_ENGINEERING_STANDARDS.md`](docs/UNITY_ENGINEERING_STANDARDS.md) and use [`docs/UNITY_STANDARDS_ADOPTION_PLAN.md`](docs/UNITY_STANDARDS_ADOPTION_PLAN.md) for migration scope.
- For terrain added on top of existing tiles or textures, follow [`docs/TILE_AUTHORING_GUIDE.md`](docs/TILE_AUTHORING_GUIDE.md) before creating or wiring production art.
- Inspect nearby code before choosing a convention; preserve the dominant first-party convention where it is stable.
- Preserve serialized fields, Unity GUIDs, and every `.meta` file. Do not move or rename Unity assets unless the goal explicitly requests a Unity Editor migration.
- Keep dependencies visible; avoid global state, service locators, hidden discovery, premature patterns, and unmeasured optimizations.
- Treat named design patterns as solutions to demonstrated problems: the current `IActivityTarget.CreateActivity()` factory is approved; simple enums remain the default for state; do not add Singleton, Builder, Proxy, Observer/event bus, Mediator, or Prototype without the trigger defined in `docs/UNITY_ENGINEERING_STANDARDS.md`.
- Add focused tests for changed domain logic. Profile before claiming a performance improvement.
- Keep Editor code out of runtime assemblies and avoid broad refactors during feature work.
- Report any intentional deviation from the stylesheet in the change summary.
