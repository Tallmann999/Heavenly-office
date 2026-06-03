# Sample Prefab Blueprints

## Enemy turret

```text
EnemyTurret
├── Model
├── AimPivot
│   └── Barrel
│       └── Muzzle
├── Colliders
└── Audio
```

Required components:

- `TurretController` on root.
- `HealthView` or `DamageReceiver` on root or colliders.
- `TurretConfig` ScriptableObject reference.
- Serialized `Transform` references for aim pivot and muzzle.

## Pooled projectile

```text
Projectile
├── Visual
├── Trail
└── Collision
```

Validation points:

- Reset velocity, trail, timers, and hit flags on spawn.
- Unsubscribe impact callbacks on despawn.
- Keep damage config separate from per-shot state.
