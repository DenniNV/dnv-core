# Changelog

All notable changes to DNV Core are recorded here. Format: [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
versions follow [SemVer](https://semver.org/). Anything that forces a game to change its code or assets is marked **BREAKING**
and comes with migration steps.

Release routine: move the `Unreleased` entries under a new version heading with the date, bump `version` in
`package.json` of `com.dnv.core` and `com.dnv.installer`, commit, tag `vX.Y.Z`.

## [Unreleased]

## [0.3.3] - 2026-10-17

### Fixed
- Host project: the warning that `Assets` and `Samples~/Starter` have diverged no longer fires for files that differ only in line endings
  (git writes `Samples~` with CRLF on Windows, Unity writes `Assets` with LF). Text files are compared without `\r`, binary files byte by byte.

## [0.3.2] - 2026-10-17

### Changed
- **BREAKING** Pools no longer have a lifetime by scene: `PoolLifetime`, `PoolConfig.Lifetime` / `Scene` / `Matches` and the `_lifetime` / `_scene` fields of `PoolConfig` and `PoolBuilder` are removed.
  All pools are global and the `PoolBuilder` of `ProjectContext` builds them at startup. Migration: pools that were `Scene` are now built (and warmed up) at startup too — check startup time and memory;
  remove the `PoolBuilder` from your scene contexts (a leftover one skips existing pools and logs a warning, it does not fail). Stale `_lifetime` / `_scene` data in assets is dropped on the next save.
  `PoolBuilder` also stops removing the shared `PoolProvider` from the execution group when it built nothing itself.
- **BREAKING** The state machine is synchronous only; the async machine is gone. Migration of game code:
  - `IState` is now the synchronous state (the former `ISyncState`): `Activate(CancellationToken stateToken)` / `Release()` / `Execute()`
    instead of `EnterAsync` / `ExitAsync` (async) and `Enter` / `Exit` (sync).
  - `EnterTo<T>()` → `SwitchTo<T>()`; it returns `void`, there is nothing to `await`.
  - Removed: `IAsyncStateMachine`, `IAsyncStateSwitcher`, `AsyncStateMachine`, `AsyncStateSwitcher`, the async twins of `IStateHistory` / `IStateRegistry` / `IStateRunner`.
    `GameStateMachine` derives from `BaseStateMachine`; `IGameStatesFactory.Create` takes an `IStateRunner` and returns `IEnumerable<IState>`.
  - Work that used to be awaited in `EnterAsync` is started in `Activate` with `.Forget()` under `stateToken`; when it finishes it calls `SwitchTo`
    (check `stateToken.ThrowIfCancellationRequested()` first). Awaited cleanup in `ExitAsync` has no replacement: do it in the next state or in an owner outside the machine.
  - A transition requested from `Activate` / `Release` / `Execute` is queued and run by the outermost call, so these methods no longer nest
    (before, a nested request released a state whose `Activate` had not returned).
  - An exception in `Release` is logged and the transition continues; an exception in `Activate` reaches the caller, the machine stays in the new state, the queue is dropped.
  - `InitializeState`, `LoadGameSceneState`, `InGameState` rewritten accordingly; `EntryPoint` no longer awaits the first transition.
- **BREAKING** Starter folder `Assets/Adressable` (typo) is now `Assets/Addressable`, in `Samples~/Starter` and in the Addressables
  addresses of its `EntryPoint` and `UI` folders. Migration: run `Template > Tools > Setup Project` — it moves the old folder
  through AssetDatabase (GUIDs and references stay) and renames the addresses. If both folders exist it only warns; merge by hand.
  Game code that hardcodes `Assets/Adressable/...` paths must be updated.
- The starter template has a single tracked copy, `Samples~/Starter`. In the host project `Assets/Scenes` and `Assets/Addressable`
  are an untracked working copy (see the root README).

### Added
- `PoolProvider.Contains(id)`.
- `ExecutionOrder.Brain` (250) and `BrainExecutionGroup`, registered by `ProjectInstaller`: the decide phase before `Default`, for owners of state machines. Existing groups keep their order.
- `IStateHistory.StateChanged` (`StateChange`: `Previous`, `Current`, `Token`) and `IStateHistory.CurrentStateToken`: systems outside a machine follow its transitions and cancel their work with the state.
  The event is raised after `Activate` (also when it threw); subscribers are isolated from each other. Anyone implementing `IStateHistory` themselves must add both members.
- `StateMachine`: a ready machine for entities (character, enemy) next to the global `GameStateMachine`.
- EditMode tests of the state machine (`Tests/Editor`, assembly `DNV.Tests.Editor`, compiled only in the host project).
- `Template > Tools > Sync Starter to Package` (host project only): copies the starter from `Assets` back into `Samples~/Starter`.
- Warning on editor start in the host project when `Assets` and `Samples~/Starter` have diverged.

### Fixed
- README: installer tag in the root README (`v0.3.0` → `v0.3.1`) and the `LoggerSettings` asset path
  (`Assets/Resources/Settings/LoggerSettings.asset`).

## [0.3.1] - 2026-10-13

### Added
- Main toolbar buttons moved into the package (`Libraries.DNV.Editor.Toolbar`): subclass `ToolbarButton`, no registration.
  Built-in **Start game** button.
- Copy Component window moved into the package.

### Changed
- The toolbar hooks the internal `MainToolbarWindow` (Unity 6000.3+) instead of the old `ToolbarExtender` copy in `Assets/Editor`.

## [0.3.0] - 2026-10-13

### Added
- One-click installer package `com.dnv.installer` (adds UniTask and DNV Core, then removes itself).
- `Template > Tools > Setup Project`: starter assets from `Samples~/Starter`, project settings, Addressables, Build Settings,
  generated ids, `LoggerSettings`. Idempotent, prints a report.

### Removed
- `ImageExtensions`.

## [0.2.0] - 2026-10-12

### Added
- Library extracted into the `com.dnv.core` package; starting template in `Assets/`.

## [0.1.0] - 2026-04-05

### Added
- Initial version of the base.
