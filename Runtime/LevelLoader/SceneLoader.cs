using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using ThreadPriority = UnityEngine.ThreadPriority;

namespace Libraries.DNV.LevelLoader
{
    public sealed class SceneLoader : ISceneLoader
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public async UniTask<Scene> LoadAsync(string sceneName, CancellationToken token)
        {
            if(Application.CanStreamedLevelBeLoaded(sceneName) == false)
            {
                throw new InvalidOperationException($"Scene '{sceneName}' is not in Build Settings");
            }

            _stopwatch.Restart();
            Application.backgroundLoadingPriority = ThreadPriority.High;

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: token);

            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);

            Log.Info(LogCategory.Level, $"Scene `{sceneName}` loaded in {_stopwatch.Elapsed.TotalMilliseconds} ms.");

            return scene;
        }

        public async UniTask UnloadAsync(string sceneName, CancellationToken token)
        {
            await SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
                .ToUniTask(cancellationToken: token);

            Log.Info(LogCategory.Level, $"Scene `{sceneName}` unloaded.");
        }
    }
}
