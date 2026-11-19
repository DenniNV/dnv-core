using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Entry must be the first scene in the build; scenes are loaded by name, so the game scene must be listed too.
    /// Scenes the project already has stay where they are.
    /// </summary>
    internal sealed class BuildScenesSetup
    {
        private readonly SetupReport _report;

        public BuildScenesSetup(SetupReport report)
        {
            _report = report;
        }

        public void Apply()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            var changed = EnsureFirst(scenes, StarterLayout.EntryScene) | EnsureListed(scenes, StarterLayout.GameScene);

            if(!changed)
            {
                _report.Unchanged("Build Settings: Entry first, Game listed");
                return;
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            _report.Changed("Build Settings: Entry first, Game listed");
        }

        private static bool EnsureFirst(List<EditorBuildSettingsScene> scenes, string path)
        {
            if(scenes.Count > 0 && scenes[0].path == path && scenes[0].enabled)
            {
                return false;
            }

            scenes.RemoveAll(scene => scene.path == path);
            scenes.Insert(0, new EditorBuildSettingsScene(path, true));
            return true;
        }

        private static bool EnsureListed(List<EditorBuildSettingsScene> scenes, string path)
        {
            if(scenes.Any(scene => scene.path == path && scene.enabled))
            {
                return false;
            }

            scenes.RemoveAll(scene => scene.path == path);
            scenes.Add(new EditorBuildSettingsScene(path, true));
            return true;
        }
    }
}
