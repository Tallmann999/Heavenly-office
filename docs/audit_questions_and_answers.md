# Опросник и ответы — аудит проекта

Ниже — набор ключевых вопросов по проекту и развёрнутые ответы с указанием релевантных файлов/локаций в репозитории.

**1) Продуктовое обещание**
- Вопрос: В одном предложении — какое обещание игра даёт игроку?
  - Ответ: Короткие сессии моральных решений с немедленными последствиями и коллекционированием уникальных карт душ.
- Вопрос: Для кого игра в первую очередь и почему это интересно?
  - Ответ: Казуально‑инди аудитория (18–45) — ценит быстрые сессии, ощутимые награды и коллекционирование.
  - См. концепт: [docs/project-concept.md](docs/project-concept.md#L1-L20).

**2) Core loop и сессии**
- Вопрос: Что игрок делает снова и снова?
  - Ответ: Открывает досье, выбирает печать, запускает анимацию/результат, получает карту/награду, переходит к следующей душе.
- Вопрос: Длительность типичной сессии?
  - Ответ: 5–15 минут; в MVP одна смена = 5 душ.
  - Код менеджера сессий: [Assets/Scripts/Runtime.Core/GameSessionManager.cs](Assets/Scripts/Runtime.Core/GameSessionManager.cs#L1-L30).

**3) Прогрессия и контентный каркас**
- Вопрос: Долгосрочная цель игрока?
  - Ответ: Собрать коллекцию карт, разблокировать миры/типы дел.
- Вопрос: Какие контентные единицы нужны?
  - Ответ: `SoulCaseData` (дела), `ReincarnationData` (возможные результаты), карточки‑награды.
  - Генератор тестового контента (Editor): [Assets/Editor/DivineOfficeTestContentGenerator.cs](Assets/Editor/DivineOfficeTestContentGenerator.cs#L1-L40).
  - Модель дела: [Assets/Scripts/SoulDocumentData.cs](Assets/Scripts/SoulDocumentData.cs#L1-L40).

**4) Ограничения и рамки производства**
- Вопрос: Что входит в MVP, что уходит в P1/P2?
  - Ответ: MVP — основной цикл (5 душ), прототип UI (PF_DivineOfficeMainScreen, PF_ReincarnationResultScreen), SO‑контент, локализация RU/EN (SO), модульное сохранение (JSON). P1 — Card Collection, World State; P2 — аналитика/онлайн.
  - Таблица фич: [docs/feature-index.md](docs/feature-index.md#L1-L20).

**5) Дистрибуция и экономика**
- Вопрос: Тип проекта и наличие платных механик?
  - Ответ: Прототип ориентирован на premium / локальный прототип; в MVP IAP отсутствуют.
  - Концепт: [docs/project-concept.md](docs/project-concept.md#L1-L20).

**6) Разрезание на фичи**
- Вопрос: Какие ключевые фичи формируют MVP?
  - Ответ: `core_divine_office`, `ui_main_screen`, `ui_reincarnation_result`, `content_souls`, `content_reincarnations`, `karma_evaluator`, `persistence`, `localization_service`, `ui_prefabs_lib`, `integration_adapter`.
  - Feature index: [docs/feature-index.md](docs/feature-index.md#L1-L20).

**7) Приоритизация**
- Вопрос: Какие фичи обязательны для первого играбельного билда?
  - Ответ: `core_divine_office`, `ui_main_screen`, `persistence`, `localization_service`, `content_souls`, `content_reincarnations`.

**Технические факты / подтверждения**
- Unity Editor (проект): [ProjectSettings/ProjectVersion.txt](ProjectSettings/ProjectVersion.txt#L1-L2) — `m_EditorVersion: 6000.2.14f1`.
- Пакеты: `com.unity.inputsystem`, URP; Unity Localization пакета нет — [Packages/manifest.json](Packages/manifest.json#L1-L40).
- Save: модульный `DivineOfficeSaveService` — JSON в `Application.persistentDataPath`, API `Load/Save/ResetSave`. См. [Assets/Scripts/Modules/DivineOffice/Runtime/Persistence/DivineOfficeSaveService.cs](Assets/Scripts/Modules/DivineOffice/Runtime/Persistence/DivineOfficeSaveService.cs#L1-L40).
- UI префабы: сборщик префабов `Tools/DivineOffice/Assemble UI Prefabs` создаёт `PF_DivineOfficeMainScreen` и `PF_ReincarnationResultScreen`, использует `LocalizedTextBinder`. См. [Assets/Editor/DivineOfficePrefabAssembler.cs](Assets/Editor/DivineOfficePrefabAssembler.cs#L1-L40).
- Правила/оценка решений: [Assets/Scripts/HeavenOfficeRulesEvaluator.cs](Assets/Scripts/HeavenOfficeRulesEvaluator.cs#L1-L40).

**Риски и рекомендации**
- Риск: Несоответствие Unity Editor ожиданиям (раньше обсуждалась 2022.3) — подтвердить локально, версия в проекте указана в [ProjectSettings/ProjectVersion.txt](ProjectSettings/ProjectVersion.txt#L1-L2).
- Риск: Отсутствие Unity Localization package — текущая SO‑реализация ок для прототипа; при желании миграции подготовить экспорт таблиц.
- Риск: TMP font assets для кириллицы — проверить наличие TMP font assets с поддержкой кириллицы (могу выполнить поиск по проекту).

Если хочешь, выполню один из следующих шагов:
- Проверю наличие TMP font assets с кириллицей сейчас.
- Подготовлю подробный GDD для выбранной фичи (укажи `feature_id`).
- Добавлю этот документ в `docs/` репозитория (сделано) и закоммичу при твоём одобрении.