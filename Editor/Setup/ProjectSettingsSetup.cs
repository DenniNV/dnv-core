using UnityEditor;
using UnityEngine;

namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Project settings the base is developed and tested with. Each one is changed only when it differs.
    /// </summary>
    internal sealed class ProjectSettingsSetup
    {
        private const string PlayerSettingsPath = "ProjectSettings/ProjectSettings.asset";
        private const string ActiveInputHandlerProperty = "activeInputHandler";
        private const int InputHandlingBoth = 2;

        private readonly SetupReport _report;

        public ProjectSettingsSetup(SetupReport report)
        {
            _report = report;
        }

        public void Apply()
        {
            ApplyRunInBackground();
            ApplyColorSpace();
            ApplyInputHandling();
        }

        private void ApplyRunInBackground()
        {
            if(PlayerSettings.runInBackground)
            {
                _report.Unchanged("Run In Background");
                return;
            }

            PlayerSettings.runInBackground = true;
            _report.Changed("Run In Background → on");
        }

        private void ApplyColorSpace()
        {
            if(PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                _report.Unchanged("Color Space: Linear");
                return;
            }

            PlayerSettings.colorSpace = ColorSpace.Linear;
            _report.Changed("Color Space → Linear");
        }

        private void ApplyInputHandling()
        {
            var playerSettings = AssetDatabase.LoadAllAssetsAtPath(PlayerSettingsPath);

            if(playerSettings.Length == 0)
            {
                _report.Warning("ProjectSettings.asset not found, Active Input Handling not checked");
                return;
            }

            var serialized = new SerializedObject(playerSettings[0]);
            var property = serialized.FindProperty(ActiveInputHandlerProperty);

            if(property == null)
            {
                _report.Warning("Active Input Handling property not found");
                return;
            }

            if(property.intValue == InputHandlingBoth)
            {
                _report.Unchanged("Active Input Handling: Both");
                return;
            }

            property.intValue = InputHandlingBoth;
            serialized.ApplyModifiedProperties();
            _report.Changed("Active Input Handling → Both");
            _report.RequireRestart();
        }
    }
}
