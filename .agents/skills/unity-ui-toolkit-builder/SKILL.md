---
name: unity-ui-toolkit-builder
description: Plan Unity UI Toolkit screens, UXML, USS, bindings, controllers, and UI lifecycle.
---

## Purpose

Use this skill to design or review UI Toolkit screens. It covers UXML structure, USS naming, controller or presenter boundaries, data binding, UI Document setup, composition, and lifecycle risks.

## Use this skill when

- Creating a UI Toolkit screen plan.
- Designing UXML and USS file structure.
- Reviewing binding direction and presenter responsibilities.
- Planning UI Document setup and screen composition.
- Checking style names, lifecycle cleanup, and event subscriptions.

## Do not use this skill when

- The task is prefab composition outside UI Toolkit; use `$unity-prefab-assembler`.
- The task is broader code architecture or asmdefs; use `$unity-architecture-guardian`.
- The user asks for IMGUI-only editor tooling.

## Inputs

- Screen name, purpose, and navigation context.
- Data sources, commands, and binding requirements.
- Existing UXML, USS, controller, presenter, or UI Document files.
- Target input mode and platform constraints.

## Workflow

1. Define screen purpose and state model.
2. Plan UXML hierarchy with stable names for queried elements.
3. Plan USS classes and reusable style blocks.
4. Separate controller/presenter responsibilities from domain logic.
5. Define bindings, event subscriptions, and cleanup points.
6. Run `scripts/validate_naming.py` for file and class naming.
7. Return required files, risks, and validation checklist.

## Output contract

```md
## UI Toolkit screen plan

### Screen purpose
...

### UXML structure
...

### USS structure
...

### Controller / presenter responsibilities
...

### Bindings
...

### Required files
...

### Risks
...

### Validation checklist
...
```

## References

- `references/ui-toolkit-style-guide.md`
- `references/uxml-uss-patterns.md`
- `references/binding-guidelines.md`
- `assets/ui-screen-template.md`

## Examples

```text
Use $unity-ui-toolkit-builder to plan a settings menu screen with bindings and controller responsibilities.
```

```text
Use $unity-ui-toolkit-builder to review UXML/USS naming and lifecycle cleanup for InventoryScreen.
```
