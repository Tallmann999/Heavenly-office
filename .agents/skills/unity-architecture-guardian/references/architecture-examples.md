# Architecture Examples

## Small feature layout

```text
Assets/Game/FeatureName/
├── Runtime/
│   ├── FeatureController.cs
│   ├── FeatureConfig.cs
│   └── FeatureService.cs
├── Editor/
│   └── FeatureConfigEditor.cs
└── Tests/
    ├── EditMode/
    └── PlayMode/
```

## Thin MonoBehaviour pattern

```text
MonoBehaviour:
- reads serialized references
- forwards Unity lifecycle events
- delegates decisions to plain C# logic
- owns cleanup of subscriptions started by the component

Plain C# logic:
- holds rules and calculations
- accepts dependencies through constructor or method parameters
- can be tested without Unity scenes
```

## Review response shape

Prefer small, concrete moves:

- Move editor-only inspectors to `Editor/`.
- Extract mutable runtime state out of shared ScriptableObjects.
- Add an asmdef only where dependency direction becomes clearer.
- Add tests around extracted plain C# logic before broad refactors.
