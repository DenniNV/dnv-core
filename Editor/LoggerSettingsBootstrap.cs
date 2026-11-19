using System.IO;
using Libraries.DNV.Logging;
using UnityEditor;
using UnityEngine;

namespace Libraries.DNV.Editor
{
    /// <summary>
    /// Makes sure every project has its LoggerSettings asset with all library categories, right after the editor
    /// loads — a fresh project then logs from the first run instead of silently printing nothing.
    /// </summary>
    [InitializeOnLoad]
    public static class LoggerSettingsBootstrap
    {
        static LoggerSettingsBootstrap()
        {
            EditorApplication.delayCall += () => EnsureAsset();
        }

        public static LoggerSettings EnsureAsset()
        {
            var settings = AssetDatabase.LoadAssetAtPath<LoggerSettings>(LoggerSettings.AssetPath);

            if (settings == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LoggerSettings.AssetPath));
                settings = ScriptableObject.CreateInstance<LoggerSettings>();
                settings.SyncWithCategories();
                AssetDatabase.CreateAsset(settings, LoggerSettings.AssetPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"[Logger] Created {LoggerSettings.AssetPath}");
                return settings;
            }

            if (settings.SyncWithCategories())
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }
    }
}
