using System.IO;
using UnityEditor;
using UnityEngine;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Offers the project setup once per project, right after the package lands in a project without the starter.
    /// The menu command stays available afterwards.
    /// </summary>
    [InitializeOnLoad]
    internal static class StarterSetupPrompt
    {
        private const string OfferedKeyPrefix = "DNV.Core.SetupOffered.";

        static StarterSetupPrompt()
        {
            EditorApplication.delayCall += OfferOnce;
        }

        private static void OfferOnce()
        {
            if(Application.isBatchMode || File.Exists(StarterLayout.EntryScene))
            {
                return;
            }

            var key = OfferedKeyPrefix + Application.dataPath;

            if(EditorPrefs.GetBool(key))
            {
                return;
            }

            EditorPrefs.SetBool(key, true);

            var accepted = EditorUtility.DisplayDialog("DNV Core",
                "Set up the starter project?\n\nAdds the Entry and Game scenes, contexts, CmsDatabase, Addressables and build settings, " +
                "and applies the project settings. Existing files are not overwritten.\n\nLater: Template > Tools > Setup Project.",
                "Set up", "Not now");

            if(accepted)
            {
                ProjectSetup.Run();
            }
        }
    }
}
