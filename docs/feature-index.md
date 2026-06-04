| feature_id | feature_name | brief | player_value | priority | depends_on | distribution | status | spec_doc |
|---|---|---|---|---|---|---|---|---|
| core_divine_office | Core Divine Office Flow | Основной цикл: очередь душ → досье → печать → реинкарнация → результат | Основной игровой опыт | MVP | ui_main_screen, karma_evaluator, persistence | all | planned | docs/TBD_core_divine_office_gd-spec.md |
| ui_main_screen | Main Office UI | PF_DivineOfficeMainScreen: HUD, очередь, печати, рычаг | Доступ к основным действиям | MVP | localization_service, ui_prefabs_lib | all | planned | docs/TBD_ui_main_screen_gd-spec.md |
| ui_reincarnation_result | Reincarnation Result UI | PF_ReincarnationResultScreen: результат и награды | Ясность результата; награды | MVP | ui_prefabs_lib, localization_service | all | planned | docs/TBD_ui_reincarnation_result_gd-spec.md |
| content_souls | Soul Case Content | ScriptableObjects `SoulCaseData` (5 тестовых) | Контент для дел | MVP | content_reincarnations | all | planned | docs/TBD_content_souls_gd-spec.md |
| content_reincarnations | Reincarnation Catalog | ScriptableObjects `ReincarnationData` (8 тестовых) | Возможные результаты реинкарнации | MVP | - | all | planned | docs/TBD_content_reincarnations_gd-spec.md |
| karma_evaluator | Karma Evaluator | Доменная логика оценки решения (adapter to existing rules) | Корректность результатов | MVP | core_divine_office | all | planned | docs/TBD_karma_evaluator_gd-spec.md |
| reincarnation_resolver | Reincarnation Resolver | Логика выбора и визуал результата | Управление результатом / доступность карт | MVP | karma_evaluator, content_reincarnations | all | planned | docs/TBD_reincarnation_resolver_gd-spec.md |
| reward_and_cards | Rewards & Cards | Логика начисления наград и открытия карт | Долгосрочная мотивация (коллекция) | MVP | persistence, content_souls | all | planned | docs/TBD_reward_and_cards_gd-spec.md |
| persistence | Save System | `DivineOfficeSaveService` JSON + PlayerPrefs fallback | Сохранение прогресса | MVP | - | all | planned | docs/TBD_persistence_gd-spec.md |
| localization_service | Localization Service | SO‑based RU/EN tables + binder | Локализация UI | MVP | - | all | planned | docs/TBD_localization_service_gd-spec.md |
| ui_prefabs_lib | UI Prefabs Library | Набор префабов (HUD, SoulView, StampButton, ResultScreen) | Быстрое прототипирование UI | MVP | - | all | planned | docs/TBD_ui_prefabs_lib_gd-spec.md |
| integration_adapter | Integration Adapter | Адаптеры для работы с `HeavenOfficeView`/manager | Безопасная интеграция в проект | MVP | HeavenOffice existing | all | planned | docs/TBD_integration_adapter_gd-spec.md |
| ui_card_collection | Card Collection UI | PF_CardCollectionScreen (фильтры, сетка, детали) | Просмотр и прогресс коллекции | P1 | ui_prefabs_lib, persistence | all | planned | docs/TBD_ui_card_collection_gd-spec.md |
| ui_world_state | World State Screen | PF_WorldStateScreen (4 мира, состояние) | Отчёт по итогу дня | P1 | persistence | all | planned | docs/TBD_ui_world_state_gd-spec.md |
| visual_placeholders | Visual Placeholders | Placeholder sprites/animators для прототипа | Поддержка UI композиции | P1 | ui_prefabs_lib | all | planned | docs/TBD_visual_placeholders_gd-spec.md |
| analytics_hooks | Analytics Hooks | Лёгкая запись событий (опционально) | Аналитика базовых KPI | P2 | - | all | planned | TBD |


## Dependency map
- `core_divine_office` → `ui_main_screen`, `karma_evaluator`, `persistence`
- `ui_reincarnation_result` → `ui_prefabs_lib`, `localization_service`
- `reward_and_cards` → `persistence`, `content_reincarnations`, `ui_card_collection`

## Top‑3 рисковые фичи
1. Интеграция с `HeavenOfficeView` — риск регрессий; стратегия: адаптер, модульность, smoke тесты.
2. Система сохранения — возможный конфликт с проектным SaveService; стратегия: сначала аудит, затем интеграция или изоляция `DivineOfficeSaveService`.
3. Локализация/шрифты — TMP font assets с кириллицей могут отсутствовать; стратегия: проверить и подготовить fallback.

## Предложенный порядок разработки
1. Audit + ответы на опросник
2. Localization + persistence (фундамент)
3. ScriptableObjects контент (5 souls, 8 reincarnations)
4. UI prefabs (main + result) и привязка `LocalizedTextBinder`
5. Flow controller + adapter + базовый loop (5 souls подряд)
6. Card collection UI + reward logic
7. Полировка, тесты и acceptance checklist
# Feature Index

- `soul_card_collection`
  - Название: Коллекция уникальных карт душ
  - Описание: Локальная коллекция карточек редких и уникальных душ с наградой за правильную постановку печати.
  - Приоритет: P1
  - Распространение: premium
  - Зависимости: UI главного экрана, меню паузы, система документов душ, система печатей
  - Статус: proposed
