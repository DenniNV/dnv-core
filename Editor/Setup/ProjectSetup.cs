using Libraries.DNV.ContentManagerSystem;
using Libraries.DNV.Core;
using Libraries.DNV.Editor.CodeGeneration;
using UnityEditor;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// One command that turns an empty project into a runnable one: starter assets, project settings,
    /// Addressables, build scenes, generated ids and logger settings. Safe to run again — it only fills in what is missing.
    /// </summary>
    public static class ProjectSetup
    {
        private const string DialogTitle = "DNV Core";

        [MenuItem(MenuPath.Tools + "Setup Project")]
        public static void Run()
        {
            var report = new SetupReport();

            new LegacyLayoutMigration(report).Apply();
            new StarterAssetsInstaller(report).Install();
            new ProjectSettingsSetup(report).Apply();
            new AddressablesSetup(report).Apply();
            new BuildScenesSetup(report).Apply();
            RegenerateIds(report);
            EnsureLoggerSettings(report);

            AssetDatabase.SaveAssets();
            report.Write();
            EditorUtility.DisplayDialog(DialogTitle, "Project setup finished.\n\n" + report.Summary(), "OK");
        }

        private static void RegenerateIds(SetupReport report)
        {
            var database = AssetDatabase.LoadAssetAtPath<CmsDatabase>(StarterLayout.CmsDatabase);

            if(database == null)
            {
                report.Warning($"{StarterLayout.CmsDatabase} not found, ids not generated");
                return;
            }

            CmsCodeGeneration.RegenerateAll(database);
            report.Changed("Generated ids (CmsIds, PoolIds, SceneIds and game generators)");
        }

        private static void EnsureLoggerSettings(SetupReport report)
        {
            LoggerSettingsBootstrap.EnsureAsset();
            report.Unchanged("Logger settings");
        }
    }
}
