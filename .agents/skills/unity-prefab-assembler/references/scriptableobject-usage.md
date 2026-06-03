# ScriptableObject Usage

## Good uses

- Shared balance/config data.
- Author-authored ability, item, enemy, or UI definitions.
- Registries that are read at runtime.
- Factory configuration for prefabs and pooled objects.

## Risky uses

- Per-instance mutable state shared by multiple spawned objects.
- Runtime caches with no reset strategy.
- Scene object references stored in reusable assets.

## Prefab integration

- Prefab components may reference ScriptableObject config assets.
- Runtime instance state should be stored on the spawned instance or a plain C# model.
- Config assets should be treated as read-only during play unless mutation is explicitly designed and reset.
