using System.Linq;
using Libraries.DNV.Logging;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Libraries.DNV.Editor.Toolbar
{
    /// <summary>
    /// Runs the game the way a build does — from the first enabled Build Settings scene (Entry),
    /// whatever scene is open now.
    /// </summary>
    internal sealed class StartGameButton : ToolbarButton
    {
        public override string Label => "Start game";
        public override string Tooltip => "Save open scenes and run the game from the first Build Settings scene";
        public override float Width => 120f;
        public override float SpaceBefore => 8f;
        public override ToolbarSide Side => ToolbarSide.Right;

        public override void OnClick()
        {
            var entryScene = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.enabled);
            if(entryScene == null)
            {
                Log.Error(LogCategory.Editor, $"[{nameof(StartGameButton)}] Build Settings have no enabled scenes, nothing to start from.");
                return;
            }

            if(!EditorSceneManager.SaveOpenScenes())
            {
                return;
            }

            EditorSceneManager.OpenScene(entryScene.path);
            EditorApplication.EnterPlaymode();
        }
    }
}
