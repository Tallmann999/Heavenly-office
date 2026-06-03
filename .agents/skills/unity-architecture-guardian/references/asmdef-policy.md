# Assembly Definition Policy

## Folder boundaries

- Runtime assemblies live outside `Editor` folders.
- Editor assemblies live under `Editor` folders and may reference `UnityEditor`.
- Tests should be split into EditMode and PlayMode folders.

## Naming

- Runtime assemblies: `Company.Product.Feature` or `Game.Feature`.
- Editor assemblies: append `.Editor`.
- Tests assemblies: append `.Tests.EditMode` or `.Tests.PlayMode`.

## References

- Runtime assemblies must not reference Editor assemblies.
- Feature assemblies should reference shared runtime abstractions rather than concrete peer features where possible.
- UI assemblies may reference presentation models, but domain logic should not reference UI assemblies.

## Review warnings

Flag these during review:

- `.asmdef` in an `Editor` path without `.Editor` naming.
- Runtime path assembly referencing an assembly with `.Editor` in the name.
- Test folders without test-specific assembly names.
- Circular feature references or broad shared assemblies that become dumping grounds.
