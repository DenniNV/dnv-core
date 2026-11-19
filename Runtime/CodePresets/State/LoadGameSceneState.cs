using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.CodePresets.State.Interfaces;
using Libraries.DNV.Core;
using Libraries.DNV.DependencyInjection.Context;
using Libraries.DNV.LevelLoader;
using Libraries.DNV.MVC.Transition;
using UnityEngine.SceneManagement;

namespace Libraries.DNV.CodePresets.State
{
    /// <summary>
    /// Activates instantly and loads the start scene under the state token; when the scene is built it asks for InGameState.
    /// </summary>
    public sealed class LoadGameSceneState : IState
    {
        private readonly IStateRunner _stateRunner;
        private readonly ISceneLoader _sceneLoader;
        private readonly UIRoot _uiRoot;
        private readonly string _sceneName;

        public LoadGameSceneState(IStateRunner stateRunner, ISceneLoader sceneLoader, UIRoot uiRoot, string sceneName)
        {
            _stateRunner = stateRunner;
            _sceneLoader = sceneLoader;
            _uiRoot = uiRoot;
            _sceneName = sceneName;
        }

        public void Activate(CancellationToken stateToken)
        {
            LoadAsync(stateToken).Forget();
        }

        public void Release()
        {
        }

        public void Execute()
        {
        }

        private async UniTask LoadAsync(CancellationToken stateToken)
        {
            var scene = await _sceneLoader.LoadAsync(_sceneName, stateToken);
            await FindSceneContext(scene).Build(stateToken);

            stateToken.ThrowIfCancellationRequested();

            _uiRoot.Get<TransitionController>().Hide();
            _stateRunner.SwitchTo<InGameState>();
        }

        private static SceneContext FindSceneContext(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var context = root.GetComponentInChildren<SceneContext>(true);

                if(context != null)
                {
                    return context;
                }
            }

            throw new InvalidOperationException($"SceneContext not found in scene '{scene.name}'");
        }
    }
}
