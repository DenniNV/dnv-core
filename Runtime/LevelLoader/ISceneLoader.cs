using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Libraries.DNV.LevelLoader
{
    /// <summary>
    /// Game scenes are loaded additively on top of the Entry scene and unloaded explicitly,
    /// so a scene's SceneContext is released together with the scene.
    /// </summary>
    public interface ISceneLoader
    {
        UniTask<Scene> LoadAsync(string sceneName, CancellationToken token);
        UniTask UnloadAsync(string sceneName, CancellationToken token);
    }
}
