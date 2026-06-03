# UI Toolkit Style Guide

## File naming

- Screen UXML: `ScreenName.uxml`
- Screen USS: `ScreenName.uss`
- Controller: `ScreenNameController.cs`
- Presenter or view model: `ScreenNamePresenter.cs` or `ScreenNameViewModel.cs`

## USS classes

- Use lowercase kebab-case class names.
- Prefix screen-scoped classes with the screen block name, for example `settings-menu__row`.
- Keep reusable classes generic only when they are actually shared.

## VisualElement names

- Use stable names for queried elements.
- Avoid relying on child index queries for core behavior.
- Keep names descriptive and local to the screen.
