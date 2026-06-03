# UXML and USS Patterns

## UXML structure

```text
ScreenRoot
├── Header
├── Content
│   └── RepeatedRows
└── FooterActions
```

## USS structure

```css
.settings-menu {}
.settings-menu__header {}
.settings-menu__content {}
.settings-menu__row {}
.settings-menu__actions {}
```

## Composition

- Keep UXML structural and readable.
- Put layout and visual styling in USS.
- Use C# for dynamic state, commands, and binding.
- Prefer smaller reusable UXML fragments only when reuse is real.
