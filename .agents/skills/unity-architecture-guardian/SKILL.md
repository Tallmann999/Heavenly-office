---
name: unity-architecture-guardian
description: Review Unity architecture, asmdefs, OOP/SOLID, naming, and runtime/editor/tests boundaries.
---

## Purpose

Use this skill to review or design Unity code architecture. It focuses on maintainable folder structure, assembly boundaries, dependency direction, thin MonoBehaviours, ScriptableObject boundaries, naming, and component maps.

## Use this skill when

- Reviewing `Assets/` structure, `asmdef` files, or code ownership.
- Splitting Runtime, Editor, and Tests folders.
- Checking OOP/SOLID risks, responsibility boundaries, and dependency direction.
- Auditing MonoBehaviour responsibilities or ScriptableObject usage.
- Generating a component map before refactoring.

## Do not use this skill when

- The task is only prefab hierarchy or inspector wiring; use `$unity-prefab-assembler`.
- The task is only UI Toolkit UXML/USS or bindings; use `$unity-ui-toolkit-builder`.
- The user asks for gameplay implementation without architecture review.

## Inputs

- Unity project path or relevant `Assets/` subtree.
- Existing folder tree, C# files, `.asmdef` files, and prefab-like assets if available.
- Desired platform, package boundaries, and testing constraints.

## Workflow

1. Inventory project folders and identify Runtime, Editor, Tests, Prefabs, UI, and ScriptableObjects areas.
2. Run `scripts/scan_asmdefs.py` when `.asmdef` files may exist.
3. Run `scripts/validate_naming.py` for path and type naming warnings.
4. Run `scripts/build_component_map.py` when component ownership or prefab references matter.
5. Compare findings with `references/oop-solid-checklist.md` and `references/asmdef-policy.md`.
6. Propose minimal structural changes and validation commands.
7. Preserve existing user-created files and avoid creating Unity `.meta` files manually.

## Output contract

```md
## Architecture review

### Summary
...

### Existing structure
...

### Violations / risks
...

### Recommended structure
...

### File/folder plan
...

### Validation commands
...

### Assumptions
...

### Next actions
...
```

## References

- `references/oop-solid-checklist.md`
- `references/asmdef-policy.md`
- `references/architecture-examples.md`
- `assets/report-template.md`

## Examples

```text
Use $unity-architecture-guardian to review Assets/Game/Core and propose Runtime/Editor/Tests separation.
```

```text
Use $unity-architecture-guardian to scan asmdef dependency direction and naming risks in this Unity project.
```
