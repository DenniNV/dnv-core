# DNV — project base

Reusable foundation for Unity 6 projects: DI with scoped contexts, game state flow, execution pipeline,
CMS with code generation, Addressables facade, pooling, MVC UI, logging. The library knows nothing about a concrete game:
game types, generated ids and game content are passed in through installers, factories and CMS parts.

Author: Denis Naumenkov — Senior Unity Developer, [denninv.github.io](https://denninv.github.io).
Source-available under the [PolyForm Noncommercial License 1.0.0](LICENSE.md): free to read, learn from and use
for noncommercial purposes; commercial use requires my written permission (Telegram [@DenniNV](https://t.me/DenniNV)).

## Installation

Add both lines to `Packages/manifest.json` (UPM cannot declare a git dependency inside a package, so UniTask goes first):

```json
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
"com.dnv.core": "https://github.com/DenniNV/dnv-core.git#v0.3.3"
```

After the recompile the package offers **Set up the starter project** (also `Template > Tools > Setup Project`).
Updating = changing the tag. On Windows also run `git config --global core.longpaths true`.
A* Pathfinding Project ≥ 5.0.0 is needed only for `DNV.Navigation`.

## Project setup

`Template > Tools > Setup Project` fills in what is missing and never overwrites existing files:

- starter assets from `Samples~/Starter` (copied with their `.meta`, GUIDs kept): `Assets/Scenes/Entry.unity`,
  `Assets/Scenes/Game.unity` (SceneContext), `ProjectContext` / `UIContext` / `ProjectDisposer` prefabs,
  `CmsDatabase` (ScenesPart with start scene Game, PoolsPart), the transition view;
- project settings: Run In Background, Linear color space, Active Input Handling = Both;
- Addressables settings and the `EntryPoint` / `UI` folders; Build Settings with Entry first and Game listed;
- generated ids and `LoggerSettings`.

It prints a report of what changed and what was already in place, so running it again works as a check.

Projects set up before 0.3.2 have the misspelled `Assets/Adressable`: Setup moves it to `Assets/Addressable` first (GUIDs and
references are kept) and renames the Addressables addresses of its folders. If both folders exist it only warns — merge by hand.

## Assemblies

| Assembly | Folder | Notes |
|---|---|---|
| `DNV.Runtime` | `Runtime/` | References UniTask (+Addressables), Addressables, ResourceManager, Input System, TextMeshPro, uGUI, `MackySoft.SerializeReferenceExtensions`, `DNV.Attributes`. Newtonsoft is an auto-referenced precompiled DLL. No DOTween dependency |
| `DNV.Editor` | `Editor/` | Editor only: CMS Editor, code generation, logger settings, project setup, main toolbar buttons, Copy Component window. References Addressables (Editor) |
| `DNV.Navigation` | `Navigation/` | Optional. References A* Pathfinding Project (`AstarPathfindingProject`, `PackageTools`, `Drawing`). Compiles only when package `com.arongranberg.astar` ≥ 5.0.0 is installed (`DNV_ASTAR` version define), so the core never depends on A* |
| `DNV.Attributes` / `DNV.Attributes.Editor` | `Attributes/` | Conditional inspector attributes and their drawers |
| `DNV.Tests.Editor` | `Tests/Editor/` | EditMode tests (NUnit). Compiled only when the package is in `testables`, i.e. in the host project |
| `MackySoft.SerializeReferenceExtensions(.Editor)`, `LazySquirrelLabs.MinMaxRangeAttribute(.Editor)` | `ThirdParty/` | Bundled third-party code, see `THIRD-PARTY-NOTICES.md` |

Game code lives in `Assembly-CSharp` and sees only the public API. `internal` types (`ResolverRegistry`, `Container`,
`ExecutionRoot`, `AssetCache`, `InstanceRegistry`) are deliberately unreachable from the game.

`[SerializeReference]` stores `{class, ns, asm}` in YAML: library types are serialized with `asm: DNV.Runtime`.
Moving a top-level type needs `[MovedFrom]`; nested types (`Resolver/Project`) need a direct YAML edit.

## Startup flow

`EntryPoint` (Entry scene) → registers `Resolver.Entry` with `IAssetProvider` → instantiates `ProjectContext`, `UIContext`,
`ProjectDisposer` from Addressables and builds them → registers the states from `IGameStatesFactory` into `GameStateMachine` →
`InitializeState`.

`DefaultGameStatesFactory`: `InitializeState` → `LoadGameSceneState` (start scene from `ScenesPart`) → `InGameState`.
A game supplies its own flow by picking another factory on `EntryPoint` (`[SerializeReference]`); the factory must include `InitializeState`.

Game scenes are loaded **additively** on top of Entry and unloaded explicitly, so a scene's `SceneContext` dies with the scene.

## Modules

| Module | Namespace | Entry point / rules |
|---|---|---|
| **DI** | `Libraries.DNV.DependencyInjection` | `Injector.Get<T>()` / `Get<T>(tag)` — resolve only. The key is always the registered type `T`. Scopes: `Resolver.Entry`, `.Project`, `.Game`, `.UI`, `.Unit`; adding/removing scopes is internal to the contexts. Inside an installer use `resolver.Get<T>()` for scope-local lookups |
| **Contexts** | `...DependencyInjection.Context` | `ProjectContext`, `SceneContext`, `UIContext`, `UnitContext` — sealed `ContextContainer`s: a `[SerializeReference]` list of `IInstaller` (`Bind(Resolver)`) and a list of builders. The scope is removed and builders disposed in `OnDestroy` |
| **Installers** | `...DependencyInjection.Installers` | `ProjectInstaller` (CMS from `CmsDatabase`, pools, `ISceneLoader`, `GameStateMachine`, `ExecutionPipeline`), `SceneInstaller`, `UIInstaller`. Game installers go after the library one in the same list |
| **Builders** | `Libraries.DNV.CodePresets` | `IAsyncBuilder.Build(CancellationToken)`; `IBaseBuilder` (`...DependencyInjection.Context`) = `IAsyncBuilder` + `IDisposable` — what context builder lists hold. MonoBehaviours derive from `AutoComponent<TComponent>` |
| **Execution** | `Libraries.DNV.ExtendedUpdate` | One `ExecutionRoot` drives `ExecutionPipeline` (Update/LateUpdate/FixedUpdate). Groups derive from `ExecutionGroup` and are added with an order from `ExecutionOrder` (`Project` 0, `Brain` 250, `Default` 500, `Pool` 1000). Decide, then act: owners of state machines (the only ones calling `SwitchTo`) join `BrainExecutionGroup` (`Injector.Get<BrainExecutionGroup>()`) and run before the systems in `Default`, so those read a settled state within the frame. Inside a group elements run in reverse order of adding. Executables implement `IExecutable` / `ILateExecutable` / `IFixedExecutable` |
| **State machine** | `Libraries.DNV.CodePresets.State` | One synchronous machine: `IState` (`Activate(stateToken)` / `Release()` / `Execute()`), `StateSwitcher` / `BaseStateMachine`, `SwitchTo<T>()`. `GameStateMachine` is the global one; its states get dependencies via constructor from the factory. A transition completes within the call and never leaves the machine "between" states. A request made from `Activate` / `Release` / `Execute` is queued and run by the outermost call, so these methods never nest. One token per state, cancelled **before** `Release` — start async work in `Activate` with `.Forget()` bound to it, and check the token before `SwitchTo` after an `await`. `Release` is synchronous: awaited cleanup belongs to the next state or to an owner outside the machine. An exception in `Release` is logged and the transition goes on; one in `Activate` reaches the caller (the machine stays in the new state, the queue is dropped). `BaseState` delegates to `IBehaviorOfState` (`Run(stateToken)` / `Execute()` / `Resolve()`); `StateCondition<T>` polls `CurrentState` |
| **Modules** | `Libraries.DNV.CodePresets.Modules` | `ModuleSwitcher<TFamily>` / `ModuleRegistry`: one active module of a family (`SwitchTo<T>()`, `StopCurrent()`) |
| **CMS** | `Libraries.DNV.ContentManagerSystem` | `CmsDatabase` ScriptableObject (edited in CMS Editor) → runtime `Cms` works on a copy. `GetEntityById` (throws), `TryGetEntity`, `GetEntityByName`, `GetFirstPart<TPart>()`. Library parts in `ContentManagerSystem/Parts/` (`ScenesPart`, `PoolsPart`, `ProjectConfigPart`), one per file |
| **Code generation** | `Libraries.DNV.Editor.CodeGeneration` | "Regenerate IDs" in CMS Editor runs every non-abstract `ICmsCodeGenerator` (found via `TypeCache`). For "part ids → constants" subclass `PartIdsGenerator<TPart>`. Library generators: `CmsIds`, `PoolIds`, `SceneIds`; `LogCategoriesGenerator` writes `GameLogCategories`. Output: `ProjectPath.Generated.Root` (`Assets/LibraryDependence/AutoGenerated/`), namespace `AutoGenerated`, written by `GeneratedCodeWriter`. Never edit generated files |
| **Addressables** | `Libraries.DNV.Addressable` | `IAssetProvider` (registered in `Resolver.Entry`): `LoadAsync` / `Release`, `LoadPrefabAsync` / `ReleasePrefab`, `InstantiateAsync(CustomReferences<T>, SpawnValues, token)` / `ReleaseAndDestroy` / `ReleaseAndDestroyAll`. Refcounting and instance ownership are internal |
| **Pools** | `Libraries.DNV.CodePresets.Pools` | Configured in `PoolsPart` (CMS). All pools are global: the `PoolBuilder` of `ProjectContext` builds and warms them up once at startup. A `PoolBuilder` in another context skips the pools that already exist and warns. `IPoolProvider.Get<T>(PoolIds.X)` → `IPoolRent<T>.Rent(position, rotation)`; return is driven by `IReturnRule`. `IPoolable` gets `OnRent` / `OnReturn` |
| **Scenes** | `Libraries.DNV.LevelLoader` | `ISceneLoader.LoadAsync(sceneName, token)` / `UnloadAsync`. Scene names come from `ScenesPart` → `SceneIds` |
| **MVC** | `Libraries.DNV.MVC` | `UIRoot` builds controllers from a `[SerializeReference]` list of `IControllerFactory`: the factory instantiates its view and returns a ready `IController` (Show / Hide / Dispose / Execute). State lives in the controller, the view (`UIElement`) only displays. New screen = new factory in UIRoot of the UIContext prefab |
| **Notifications** | `Libraries.DNV.Notifications` | `IGameNotificationService`: `Subscribe<T>(handler)` / `Unsubscribe<T>` / `Send<T>(message)`. Any type is a message; prefer small readonly structs |
| **Logging** | `Libraries.DNV.Logging` | `Log.Info/Warning/Error(LogCategory, message)`. `LogCategory` is a struct: built-in categories are static fields, game ones are added in Project Settings → Logger and generated into `GameLogCategories` (ids from `LogCategory.FirstGameId`). Settings: `Assets/Resources/Settings/LoggerSettings.asset` |
| **Commands** | `Libraries.DNV.CodePresets.Commands` | `ICommand` (`CanExecute`, `Execute()` → `CommandResult`). Decorators: `ConditionalCommand` (gated by `ICondition[]`), `FirstMatchCommand`, `HandledCommand` (`ICommandHandler` on success/failure), `NullCommand`. Compose with `CommandBuilder.When(...).OnSuccess(...).OnFailed(...).Build()`; `CommandBinding` disposes an event subscription; `LogCommandHandler` logs results to `LogCategory.Commands` |
| **Casters** | `Libraries.DNV.Casters` | `RayCaster`, `SphereCaster`, `CapsuleCaster`, `BoxCaster` → `TryCast<T>()` returning `HitInfo<T>`; `HitExtensions` filter hit arrays (by dot side, distance, neighbours) |
| **Sequences** | `Libraries.DNV.CodePresets.Sequence` | `SequentialRunner` / `SimultaneousRunner` over `ISequenceAction.DoAction(token)`. Delays: `IDelay`, `Delay`, `FrameDelayer`, `AnimationDelayer` |
| **Input models** | `Libraries.DNV.InputTemplate` | `ButtonModel` (IsPressed / IsReleased / IsHold), `ActionModel`, `Vector2Model`, `FloatModel`; conditions `InputCondition`, `HoldRule`, `DebugInputCondition` (`...InputTemplate.Conditions`). The input service and actions asset belong to the game |
| **Extensions** | `Libraries.DNV.Extensions` | Float (`CustomRound` → -1/0/1, `Normalize`, `Clamp`), Vector3 (`WithX/Y/Z`, `ClampOnlyX/Z`), List (`GetRandom`, `PopRandom`, `PeekFirst/Last`, `PopFirst/Last`, `Shuffle`, `TryAdd`), Transform (`FindNearPoint`, `ActivateAll`), `ButtonModel.IsInputMatchesType` |
| **Utility** | `Libraries.DNV.Utility` | `MyLerp` (frame-rate independent exponential decay), `EasingUtilities`, `CancellationTokenHelper`, `SmootherFloat` |
| **Navigation** (optional) | `Libraries.DNV.Navigation` | `NavigationAgent` (on a `Seeker`), `NavigationBrain` (`INavigationBrain`: `IRunnable` + `IExecutable`) walking a path from a switchable `IPathGenerator` (`PathGeneratorByForward`, `InCircle`, `AroundAgent`, `ByAgentBehind`, `ByTransform`); settings in `NavigationBrainSettings` |
| **Editor toolbar** | `Libraries.DNV.Editor.Toolbar` | Buttons left/right of Play in the main toolbar. Subclass `ToolbarButton` (`Label`, `Side`, `OnClick`; optional `Order`, `Width`, `Icon`, `ShouldDraw`) in any Editor assembly — found via `TypeCache`, no registration. Library button: **Start game** — saves open scenes and enters Play Mode from the first enabled Build Settings scene. Hooks the internal `MainToolbarWindow` (Unity 6000.3+); if Unity changes it, a warning is logged and the buttons are skipped |
| **Paths / menus** | `Libraries.DNV.Core` | `MenuPath` — root of every editor menu (never hardcode a menu string); `ProjectPath` — generated code root |

## State as a marker

A state machine is the single source of "what is the current behavior"; movement, rotation, input, animation and the like live in separate systems
that follow it, not inside states. This keeps states small and the systems replaceable.

- **Level — "while in the state".** Read `IStateHistory.CurrentState` (or `StateCondition<T>`) every frame.
- **Edge — "just entered / just left".** Subscribe to `IStateHistory.StateChanged` (`StateChange`: `Previous`, `Current`, `Token`). It is raised synchronously inside the transition,
  after the new state's `Activate`, also when `Activate` threw. Every state of a queued chain gets its own event. A throwing subscriber is logged and affects nobody else;
  do not rely on the order between subscribers.
- **Cancellation.** Bind async work started for a state to `StateChange.Token` / `CurrentStateToken`: it is cancelled when the state is released.
- **One owner.** Systems receive only `IStateHistory` (read). `IStateRunner.SwitchTo` is held by the owner of the machine — the entity's brain, or the flow states of `GameStateMachine`.
  Two systems requesting different states in one frame is a race the queue can only order, not resolve.
- **One machine per entity.** `new StateMachine()` (the global flow uses `GameStateMachine`); register states with `IStateRegistry` and add the machine to an `ExecutionGroup` to get `Execute`.

## Extension points for a game

- `IInstaller` added to a context's installer list (after the library installer).
- `IGameStatesFactory` on `EntryPoint`.
- `IControllerFactory` in `UIRoot`.
- `CmsEntityPart` subclasses for game content; `ICmsCodeGenerator` / `PartIdsGenerator<TPart>` for their ids.
- `ExecutionGroup` subclass added to `ExecutionPipeline` with `ExecutionOrder.Default`.
- `ToolbarButton` subclass in an Editor assembly for a main toolbar button.

## Rules

- The library never references game types or generated ids — data comes in through constructors, installers and CMS.
- All async work is `UniTask` + `CancellationToken`, passed down the whole chain; no coroutines.
- Domain Reload may be disabled: static state is reset at `SubsystemRegistration`.
