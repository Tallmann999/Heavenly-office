# Prefab Composition Rules

## Root object

- The root GameObject owns identity, top-level lifecycle, and public prefab-facing components.
- Keep root components few and intentional.
- Child objects own visuals, colliders, sockets, audio, VFX, and UI anchors.

## Component ownership

- One component should own one lifecycle concern.
- Avoid two components writing the same transform, health state, or pooled lifecycle.
- Serialized references should point inward to children or outward to stable services only through explicit boundaries.

## View, config, runtime logic

- View components display state and forward interaction.
- Runtime logic coordinates behavior and lifecycle.
- ScriptableObjects hold shared config, not hidden per-instance mutable state.

## Validation risks

- Missing required references.
- Components that assume a child exists by name instead of serialized reference.
- Event subscriptions not removed on disable, despawn, or destroy.
- Pooled prefabs that do not reset transient state.
