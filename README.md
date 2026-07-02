TZ для Win Wox Games

## Как запустить

1. Открыть проект в Unity **6000.2.11f1**.
2. Открыть сцену `Assets/Scenes/Labyrinth.unity`.
3. Нажать **Play**.

Вся геометрия лабиринта, игрок, бриллианты, враги, выход и UI создаются
программно при запуске сцены (`GameEntryPoint`), поэтому сцена
содержит только свет и один объект-точку входа - генерация каждый раз
даёт новый случайный лабиринт (если не задан `randomSeed` в инспекторе).

## Управление

Движение - `W A S D` 
Бег - `Shift` (удерживать)
Осмотр камерой - Мышь

## Архитектура

Проект разбит на небольшие классы с одной ответственностью, без единого
"God-объекта". Точка входа - компонент `GameEntryPoint`, который выступает
composition root: генерирует лабиринт, регистрирует общие сервисы в
`ServiceLocator` и делегирует создание всех игровых объектов и UI фабрикам
(`IGameFactory`, `IUIFactory`). Сам он не содержит игровых правил (кто кого
ловит, когда победа и т.п.) - только сборку уровня.

### ServiceLocator + фабрики

- `ServiceLocator` - статический реестр сервисов
  (`Register<T>`, `Get<T>`, `TryGet<T>`, `Clear`), хранящий по одному
  экземпляру на тип. `GameEntryPoint` очищает его в начале `Awake()` и
  регистрирует туда `GameSession`, `DiamondCollector`, `IGameFactory` и
  `IUIFactory'. Остальные классы залетают через `ServiceLocator.Get<T>()`.
- `IGameFactory` / `GameFactory`- фабрика игровых объектов:
  создаёт игрока, бриллианты, врагов и выход. Сама достаёт `GameSession` и
  `DiamondCollector` из `ServiceLocator`.
- `IUIFactory` / `UIFactory` - то же самое для интерфейса:
  строит канвас/худ/панели и связывает их с `UIManager`.
- `DiamondFactory` / `EnemyFactory` (`Factories/`) - низкоуровневые
  статические фабрики

  ### Хронология запуска

`GameEntryPoint` создаёт `GameSession` и `DiamondCollector` один раз,
регистрирует их вместе с `IGameFactory`/`IUIFactory` в `ServiceLocator`, а
дальше сами фабрики достают нужные сервисы оттуда. Конечные игровые объекты
(`Diamond`, `EnemyAI`, `ExitZone`, `UIManager`) по-прежнему получают ссылки
на конкретные зависимости явно через `Initialize(...)`, а не через
`ServiceLocator` напрямую - это оставляет `ServiceLocator` только на уровне
"составления уровня" (`GameEntryPoint` и фабрики), не размазывая его по всей
игровой логике. Поиска объектов по имени/тегу в рантайме нет (кроме тега
`Player`, который используется для проверки триггеров).

### Прочее

- **Core**
  - `GameEntryPoint` - точка входа/composition root. Генерирует лабиринт, регистрирует сервисы и спавнит игрока/бриллианты/врагов/выход
  - `GameSession` следит за состояниями `Playing / Won / Lost`.
  - `GameState` - enum состояния игры.

- **Labyrinth**
  - `LabyrinthGenerator` - чистый алгоритм генерации топологии лабиринта, не создаёт GameObject'ы.
  - `LabyrinthData` - неизменяемые данные о стенах лабиринта.
  - `LabyrinthBuilder` - превращает `LabyrinthData` в геометрию сцены (пол и стены
    из примитивов Cube).
  - `CellPicker` - вспомогательный класс: выбирает случайный набор клеток
    лабиринта без повторов (используется и `GameEntryPoint`, и `GameFactory`).

- **Player**
  - `PlayerMovement` - движение персонажа.
  - `PlayerCameraController` - управление камерой

- **Collectibles**
  - `Diamond` - поведение одного бриллианта: вращение/покачивание для
    заметности, обработка `OnTriggerEnter` с игроком, самоуничтожение.
  - `DiamondCollector` - счётчик: сколько всего бриллиантов и сколько
    собрано. Класс не знает ни о UI, ни об игроке напрямую.

- **Enemies**
  - `EnemyAI` - конечный автомат `Patrol / Chase` поверх `NavMeshAgent`
    (NavMesh печётся во время выполнения из cозданного лабиринта). Если игрок ближе радиуса обнаружения - преследование.

- **Interactables**
  - `ExitZone` - триггер зоны выхода; засчитывает победу через
    `GameSession.ReportVictory()`, только если `DiamondCollector.AllCollected`.

- **Factories**
  - `IGameFactory` / `GameFactory` - фабрика игровых объектов, читает
    `GameSession`/`DiamondCollector` из `ServiceLocator` (см. выше).
  - `DiamondFactory`, `EnemyFactory` - низкоуровневые статические фабрики:
    собирают полностью настроенный GameObject (примитивы + компоненты +
    материалы) для одного бриллианта или врага.

- **UI**
  - `IUIFactory` / `UIFactory` - фабрика UI, читает `GameSession`/
    `DiamondCollector` из `ServiceLocator` и строит иерархию UI.
  - `UIManager` - единственный класс, который подписывается на события
    `DiamondCollector` и `GameSession` и обновляет UI.
