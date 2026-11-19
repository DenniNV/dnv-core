using System.Linq;
using Libraries.DNV.Logging;
using UnityEditor;
using UnityEngine;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Host project only: warns on editor start that the starter in Assets was changed but not synced into the package,
    /// since the Assets copy is not tracked by git and would be lost.
    /// </summary>
    [InitializeOnLoad]
    internal static class StarterDriftWatcher
    {
        private const int ShownFiles = 10;

        static StarterDriftWatcher()
        {
            EditorApplication.delayCall += Check;
        }

        private static void Check()
        {
            if(Application.isBatchMode || !StarterPackage.IsEmbedded())
            {
                return;
            }

            var drift = StarterTemplateSync.FindDrift();

            if(drift.Count == 0)
            {
                return;
            }

            var shown = string.Join("\n", drift.Take(ShownFiles));
            Log.Warning(LogCategory.Editor,
                $"Starter in Assets differs from the package ({drift.Count} file(s)). " +
                $"Run Template > Tools > Sync Starter to Package, or the changes are lost: Assets copy is not tracked.\n{shown}");
        }
    }
}
