# Feature Index

Статусы основаны на аудите репозитория от 2026-06-04:

- `done` — фича присутствует в рабочей сцене или подтверждена полноценными assets и связями.
- `in_progress` — часть фичи существует, но сквозная работоспособность или полный scope не подтверждены.
- `planned` — значимая реализация в проекте не найдена.

| feature_id | feature_name | brief | player_value | priority | depends_on | distribution | status | spec_doc |
|---|---|---|---|---|---|---|---|---|
| legacy_office_loop | Первая итерация Heaven Office | Сгенерированные дела, четыре печати, таймер, счёт, ошибки, комбо, финал смены | Уже играбельная проверка базовой сортировки | MVP | legacy_main_ui, legacy_rules | premium | done | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| legacy_main_ui | Рабочий UI первой итерации | Старт, досье, печати, HUD, фидбек, финальная панель | Позволяет пройти существующую сессию | MVP | tmp_font | premium | done | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| legacy_rules | Базовые правила решений | Оценка добрых/плохих поступков и приоритет особых тегов | Даёт понятный исход решения | MVP | legacy_content_generator | premium | done | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| core_divine_office | Сквозной Divine Office Flow | 5 заданных душ: досье → печать → реинкарнация → результат → следующая душа | Проверяет главную игровую гипотезу | MVP | integration_strategy, content_souls, content_reincarnations, persistence, localization_service | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| integration_strategy | Интеграция с первой итерацией | Сохранить первую итерацию отдельным режимом и сделать ясный выбор режима | Исключает конфликт двух flow | MVP | legacy_office_loop | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| soul_investigation | Кармическое расследование | Намерения, раскаяние, скрытые сведения, архивные ошибки и инструменты раскрытия | Главное отличие игры от простой сортировки | MVP | content_souls, karma_evaluator, main_office_experience | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| stamp_decision | Выбор и применение печати | Рай, Ад, Проверка, Апелляция с выразительным фидбеком | Ключевое решение игрока | MVP | karma_evaluator, main_office_experience | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| reincarnation_wheel | Барабан реинкарнаций | После печати игрок запускает и может остановить барабан форм из пула выбранного маршрута | Добавляет азарт, юмор, сюрприз и желание увидеть варианты | MVP | content_reincarnations, reincarnation_resolver | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| return_case_queue | Очередь возвратных дел | Души после Проверки/Апелляции возвращаются позже с новыми сведениями | Создаёт многоэтапные истории и последствия решений | MVP | soul_investigation, content_souls | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| reincarnation_resolver | Определение результата реинкарнации | Выбрать итог по решению, правилам и доступным вариантам | Связывает решение с последствием | MVP | karma_evaluator, content_reincarnations | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| karma_evaluator | Оценка качества решения | Оценить печать, раскрытые сведения и реинкарнацию | Делает решения осмысленными | MVP | soul_investigation | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| main_office_experience | Главный экран кабинета | Очередь, душа, досье, инструменты, печати, порталы, рычаг, HUD | Формирует запоминаемый игровой стол | MVP | integration_strategy, localization_service | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| reincarnation_result_ui | Экран результата | Исходная душа, новая форма, причина, награды, CTA продолжения | Даёт понятный и смешной payoff | MVP | reincarnation_resolver, localization_service | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| content_souls | Контент дел душ | 5 тестовых assets сейчас; 40 душ для vertical slice | Обеспечивает разнообразие решений | MVP | localization_service | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| content_reincarnations | Каталог реинкарнаций | 8 тестовых вариантов с допустимыми печатями | Обеспечивает последствия и юмор | MVP | localization_service | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| reward_and_cards | Награды и открытие карт | Очки судейства, монеты и карта за идеальное решение без замечаний | Создаёт кратко- и среднесрочную мотивацию | MVP | persistence, karma_evaluator, soul_card_collection | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| persistence | Сохранение прогресса | Язык, день, обработанные души, карты и ресурсы | Сохраняет прогресс между запусками | MVP | integration_strategy | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| localization_service | RU/EN локализация | Текущие SO-таблицы, binder и переключение языка | Делает прототип доступным на двух языках | MVP | localization_strategy | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| localization_strategy | Целевая стратегия локализации | Выбрать Unity Localization или собственные SO-таблицы | Исключает дублирование систем | MVP | - | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| soul_card_collection | Коллекция карт душ | Доступна сразу; фильтры, сетка, детали, прогресс, редкость, бонусы и новые уникальные карточки | Даёт долгосрочную цель и ценность редких душ | MVP | reward_and_cards, persistence | premium | planned | `docs/features/soul-card-collection.md` |
| world_state_day_summary | Итоги дня и состояние миров | Карта миров, баланс, рейтинг судьи, новые карточки, ресурсы и новые ветки галактик/миров | Показывает влияние игрока | MVP | world_impact, persistence | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| world_impact | Ветвящаяся карта миров | Решения открывают стабильные/тревожные/кризисные/проклятые ветки и влияют на очередь душ | Делает последствия накопительными и создаёт будущие кейсы | MVP | karma_evaluator | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| judge_rating | Рейтинг судьи | Показатель качества судейства, влияющий на доступ к веткам душ | Даёт понятную долгосрочную оценку игрока | MVP | world_impact, reward_and_cards | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| bureaucracy_risk | Бюрократический риск | Побочный рейтинг частых Проверок/Апелляций | Ограничивает абуз осторожной стратегии | P1 | return_case_queue, judge_rating | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| office_upgrades | Улучшения канцелярии | Зеркало/пронзительный взгляд, весы и другие инструменты стола | Формирует мета-прогрессию и помогает решать сложные души | P1 | reward_and_cards, world_state_day_summary | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| day_events | События и новые правила дня | Модификаторы смены и новые ситуации | Поддерживает разнообразие | P1 | world_state_day_summary, content_souls | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| ftue_tutorial | Обучение первой смены | Объяснить расследование, печати и реинкарнацию через реальный flow | Снижает риск непонимания правил | P1 | core_divine_office | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| presentation_feedback | Визуальный и звуковой feedback | Реакции души, удар печати, рычаг, портал, reveal карты | Делает действия приятными и продаваемыми | P1 | main_office_experience, reincarnation_result_ui | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| analytics_and_playtest | Аналитика прототипа и плейтест | События core loop и критерии проверки гипотезы | Позволяет решить, работает ли концепт | P1 | core_divine_office | premium | in_progress | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |
| automated_tests | Автоматизированные проверки | Тесты правил, сохранения, локализации и core flow | Снижает риск регрессий | P1 | core_divine_office, persistence, localization_service | premium | planned | `docs/2026-06-04_divine-office-feature-roadmap_gd-spec.md` |

## Dependency Map

- `core_divine_office` → `integration_strategy`, `main_office_experience`, `soul_investigation`, `stamp_decision`, `reincarnation_wheel`, `reincarnation_result_ui`, `persistence`, `localization_service`
- `karma_evaluator` → `soul_investigation`, правила печатей и правила качества реинкарнации
- `reward_and_cards` → `karma_evaluator`, `persistence`, затем `soul_card_collection`
- `world_state_day_summary` → `world_impact`, `judge_rating`, `persistence`, затем `office_upgrades` и `day_events`

## Что Уже Есть

- Полностью играбельная первая итерация базовой сессии в `SampleScene`.
- Четыре печати, базовые правила, процедурные дела, таймер, счёт, ошибки, комбо и лог аналитики.
- Частичный модуль DivineOffice: контентные assets, сохранение, локализация, flow controller и два UI-префаба.
- 5 тестовых душ, 8 реинкарнаций, RU/EN таблицы, TMP-шрифт с кириллицей.

## Что Нужно Реализовать В Первую Очередь

1. Сделать отдельный запуск Steam-демо, сохранив первую итерацию отдельным режимом.
2. Подключить DivineOffice к рабочей сцене и подтвердить сквозной проход демо.
3. Реализовать барабан реинкарнаций после печати и определить правила качества решения.
4. Довести расследование души до состояния, где намерения и скрытая информация влияют на выбор.
5. Завершить результат, награды, сохранение и смену языка без потери состояния.
6. После стабильного цикла добавить коллекцию карт и итоги дня.

## Top-3 Рисковые Фичи

1. `integration_strategy`: первая итерация должна остаться отдельным режимом, но два flow не должны конфликтовать.
2. `soul_investigation`: это главное отличие продукта, но его точные правила не определены.
3. `return_case_queue`: возврат дел после Проверки/Апелляции требует ясных правил времени, новых сведений и завершения.

## Предложенный Порядок Разработки

1. Принять решения по вопросам P0 из сводного GD-дока.
2. Сделать один сквозной flow на одной рабочей сцене.
3. Зафиксировать правила расследования, оценки и реинкарнации.
4. Довести 5 тестовых душ до полноценного контента и закрыть сохранение/локализацию.
5. Проверить гипотезу плейтестом: интересно ли изучать дело, ставить печать и видеть результат.
6. Только после этого расширять вертикальный срез коллекцией, мирами, улучшениями и событиями.
