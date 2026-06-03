---
name: unity-prefab-assembler
description: Plan Unity prefab composition, components, serialized references, ScriptableObjects, and inspector wiring.
---

## Purpose

Use this skill to design or review a Unity prefab composition. It focuses on GameObject hierarchy, required components, serialized references, ScriptableObject config, lifecycle ownership, pooling or spawning risks, and inspector setup.

## Use this skill when

- Creating a new prefab plan.
- Restructuring prefab hierarchy or component ownership.
- Connecting MonoBehaviours to ScriptableObject configuration.
- Reviewing serialized references and inspector wiring.
- Checking spawned, pooled, or disposable object lifecycle risks.

## Do not use this skill when

- The main task is asmdef or architecture boundaries; use `$unity-architecture-guardian`.
- The main task is UI Toolkit screen layout; use `$unity-ui-toolkit-builder`.
- The user only asks for asset art, icons, or texture generation.

## Inputs

- Target prefab name and purpose.
- Expected runtime behavior and ownership.
- Required visual, physics, animation, audio, or VFX components.
- Existing scripts, ScriptableObjects, and prefab constraints.

## Workflow

1. Define the prefab root responsibility and child GameObject hierarchy.
2. Map each component to a clear owner and lifecycle.
3. Separate view components, runtime logic, and ScriptableObject configuration.
4. List required serialized references and inspector wiring.
5. Check pooling, spawning, event subscription, and cleanup risks.
6. Run `scripts/build_component_map.py` when reviewing existing assets.
7. Run `scripts/validate_naming.py` before finalizing file and type names.

## Output contract

```md
## Prefab composition plan

### Target prefab
...

### GameObject hierarchy
...

### Required components
...

### Serialized references
...

### ScriptableObject/config requirements
...

### Runtime ownership
...

### Inspector setup checklist
...

### Risks
...

### Validation checklist
...
```

## References

- `references/prefab-composition-rules.md`
- `references/scriptableobject-usage.md`
- `references/sample-prefab-blueprints.md`
- `assets/prefab-spec-template.md`

## Examples

```text
Use $unity-prefab-assembler to create a composition plan for an enemy turret prefab.
```

```text
Use $unity-prefab-assembler to review serialized references and pooling cleanup for Projectile.prefab.
```
