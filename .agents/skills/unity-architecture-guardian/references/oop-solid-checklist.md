# OOP and SOLID Checklist

Use this checklist during architecture review.

## Responsibility

- MonoBehaviours should coordinate Unity lifecycle, scene references, and view-facing behavior.
- Business rules should move into plain C# services, models, policies, or use cases when possible.
- A class should have one dominant reason to change.
- Avoid managers that own unrelated systems.

## Dependencies

- Runtime code must not reference Editor-only assemblies or namespaces.
- Domain logic should not depend on UI Toolkit, scene objects, or prefab details.
- High-level orchestration may depend on interfaces; low-level implementations should not control high-level policy.

## Extension

- Prefer composition over inheritance for gameplay variations.
- Use interfaces or strategy objects when behavior changes independently from GameObject structure.
- Avoid adding boolean mode flags that split a class into several hidden responsibilities.

## ScriptableObjects

- Good for shared static data, tunable config, registries, and editor-authored content.
- Risky for mutable per-instance runtime state unless lifecycle and reset rules are explicit.

## Tests

- Plain C# logic should be testable without scenes.
- EditMode tests should cover editor-time validation and pure logic.
- PlayMode tests should cover lifecycle, spawning, pooling, scene integration, and input timing.
