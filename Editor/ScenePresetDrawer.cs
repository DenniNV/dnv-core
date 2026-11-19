using System.IO;
using Libraries.DNV.LevelLoader;
using UnityEditor;
using UnityEngine;

namespace Libraries.DNV.Editor
{
    /// <summary>
    /// Scene name field tinted green when the scene is enabled in Build Settings, red otherwise —
    /// a typo or a scene missing from the build is visible right in the CMS.
    /// </summary>
    [CustomPropertyDrawer(typeof(ScenePreset))]
    public sealed class ScenePresetDrawer : PropertyDrawer
    {
        private const string NameField = "<Name>k__BackingField";
        private const float Padding = 2f;

        private static readonly Color InBuildColor = new Color(0.15f, 0.6f, 0.15f, 0.2f);
        private static readonly Color MissingColor = new Color(0.75f, 0.15f, 0.15f, 0.2f);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var nameProperty = property.FindPropertyRelative(NameField);
            var isInBuild = nameProperty != null && IsSceneInBuild(nameProperty.stringValue);

            EditorGUI.DrawRect(position, isInBuild ? InBuildColor : MissingColor);

            var fieldRect = new Rect(position.x + Padding, position.y + Padding,
                position.width - Padding * 2f, EditorGUIUtility.singleLineHeight);

            if (nameProperty != null)
            {
                EditorGUI.PropertyField(fieldRect, nameProperty, new GUIContent("Scene Name"));
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + Padding * 2f;
        }

        private static bool IsSceneInBuild(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled && Path.GetFileNameWithoutExtension(scene.path) == sceneName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
