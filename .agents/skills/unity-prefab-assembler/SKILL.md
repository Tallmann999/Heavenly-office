---
name: unity-prefab-assembler
description: Plan Unity prefab composition, components, serialized references, ScriptableObjects, and inspector wiring.
---

## Purpose

Use this skill to design or review a Unity prefab composition. It focuses on GameObject hierarchy, required components, serialized references, ScriptableObject config, lifecycle ownership, pooling or spawning risks, and inspector setup.

Mode: create|review — choose one. "create" produces a minimal prefab composition plan and output contract; "review" performs a deeper analysis against existing assets and runs available repository tooling when present.

## Use this skill when

- Creating a new prefab plan.
- Restructuring prefab hierarchy or component ownership.
- Connecting MonoBehaviours to ScriptableObject configuration.
- Reviewing serialized references and inspector wiring.
- Checking spawned, pooled, or disposable object lifecycle risks.

If repository tooling referenced below (e.g., `scripts/build_component_map.py` or `scripts/validate_naming.py`) is missing or not executable, note "Tooling unavailable: <path>" in the Validation checklist and continue with manual analysis.

## Do not use this skill when

- The main task is asmdef or architecture boundaries; use `$unity-architecture-guardian`.
- The main task is UI Toolkit screen layout; use `$unity-ui-toolkit-builder`.
- The user only asks for asset art, icons, or texture generation.

## Inputs

- Target prefab name and purpose.
- Expected runtime behavior (describe what the prefab does at runtime) and explicit ownership semantics (one of: `scene-owned`, `pool-owned`, `factory-owned`, `network-server-authoritative`, or `system:<name>`), including who creates it, who is responsible for disposal, and when.
- Required visual, physics, animation, audio, or VFX components.
- Existing scripts, ScriptableObjects, and prefab constraints.
- Unity version (e.g. `2020.3`/`2021.3`/`2022.3`), required packages (e.g. `URP`, `DOTS`, `Addressables`), and target platforms (mobile/pc/console).
- List any third-party frameworks the prefab depends on and check known integration patterns or anti-patterns (e.g., DOTS/ECS vs MonoBehaviour, custom pooling APIs).

If required inputs are missing, respond with: "Missing inputs: <list>. Please provide these before analysis: target prefab name, runtime behavior, and any existing scripts/ScriptableObjects."

## Workflow

1. Define the prefab root responsibility and child GameObject hierarchy. Handle nested prefabs and prefab variants explicitly: for each child prefab reference indicate if it is an instance, nested prefab, or variant, and whether changes should be applied at variant or root level.
2. For each component, specify: (a) owner: one of {`self (GameObject)`, `parent GameObject`, `manager <name>`, `ScriptableObject`, `network-authority <client/server>`}, and (b) lifecycle phases: list explicit Unity lifecycle/events (Awake, OnEnable, Start, Update, OnDisable, OnDestroy) plus creation and disposal triggers.
3. For each script, classify as one of: `View` (purely visual, no game state changes), `Runtime Logic` (state changes, gameplay behavior), or `Config` (ScriptableObject-backed). If a script mixes categories, mark as `Mixed` and list the responsibilities that violate separation of concerns.
4. For each serialized field, provide: field name, type, required:true|false, inspector label, default value (if any), and a short wiring note (e.g., "assign prefab: Projectile.prefab" or "auto-assigned at runtime by Spawner"). If the prefab contains missing script references, report them in "Risks" and suggest replacement or removal steps; include file paths and component indices where possible.
5. For pooling/spawning/event handling, check specifically for: (1) missing event unsubscriptions in OnDisable/OnDestroy, (2) fields not reset on reuse, (3) static caches holding references to instances, (4) double-registration of event handlers, and (5) ownership mismatches causing multiple disposals. Report any findings per prefab.
6. If `scripts/build_component_map.py` is present and executable in the repository, run it and include its output. If it is not present, produce a manual component map and add the note: "Tooling missing: scripts/build_component_map.py not found.".
7. If `scripts/validate_naming.py` is present and executable, run it and include the output. If it is missing, note "Tooling missing: scripts/validate_naming.py not found." and perform naming checks manually.

For nested or variant-aware changes, explicitly indicate whether edits should apply to the variant instance or the shared base prefab to avoid accidental upstream changes.

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

Output formatting: For each section produce a 1–2 sentence summary, then a bullet list of items; avoid more than 200 words per section.

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
