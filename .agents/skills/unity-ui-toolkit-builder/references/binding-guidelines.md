# Binding Guidelines

## Direction

- UI reads from a view model, presenter, or screen state object.
- UI writes through commands or explicit callbacks.
- Domain logic should not depend on `VisualElement`.

## Lifecycle

- Register callbacks when the screen is attached, shown, or initialized.
- Unregister callbacks when hidden, detached, disabled, or disposed.
- Refresh UI after model changes through a single render or bind method.

## Risks

- Querying elements before the UXML tree is cloned.
- Leaking callbacks across screen opens.
- Storing scene or UI references inside shared ScriptableObjects.
- Mixing domain decisions into button click handlers.
