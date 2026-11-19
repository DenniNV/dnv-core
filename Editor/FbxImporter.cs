using System;
using System.Collections.Generic;
using Libraries.DNV.Core;
using Libraries.DNV.Logging;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Libraries.DNV.Editor
{
    public class FbxImporter : EditorWindow
    {
        private const string NameInEditor = "FBX Importer";
        private readonly string _motionNodeName = "Man/Root";

        private SerializedObject _serializedObject;

        [SerializeField] private Avatar _avatar;

        [SerializeField] private List<Object> _objects;

        [SerializeField] private bool _isNeedLoop;

        [SerializeField] private ModelImporterAnimationType _type;


        [SerializeField] private string _thisChar;
        [SerializeField] private string _toChar;

        [MenuItem(MenuPath.Tools + NameInEditor)]
        private static void OpenWindow()
        {
            var window = GetWindow<FbxImporter>();
            window.Construct();
            window.Show();
        }

        private void Construct()
        {
            _serializedObject = new SerializedObject(this);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_avatar"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_objects"));

            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_isNeedLoop"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_type"));

            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_thisChar"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_toChar"));

            EditorGUILayout.EndVertical();

            _serializedObject.ApplyModifiedProperties();


            if (GUILayout.Button("Build Avatar"))
            {
                BuildAvatar();
            }

            if (GUILayout.Button("Rename To"))
            {
                Rename();
            }

            if (GUILayout.Button("Clear"))
            {
                Clear();
            }
        }

        private void BuildAvatar()
        {
            foreach (var item in _objects)
            {
                var path = AssetDatabase.GetAssetPath(item);
                var asset = AssetImporter.GetAtPath(path);

                if (asset is ModelImporter importer)
                {
                    importer.animationType = ModelImporterAnimationType.Generic;
                    importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                    importer.sourceAvatar = _avatar;
                    importer.motionNodeName = _motionNodeName;

                    var clips = importer.clipAnimations;

                    if (clips.Length == 0)
                        clips = importer.defaultClipAnimations;

                    foreach (var clip in clips)
                    {
                        clip.loopTime = _isNeedLoop;
                    }


                    Log.Error(LogCategory.Editor, $"{importer.avatarSetup}");

                    importer.clipAnimations = clips;
                    importer.SaveAndReimport();
                }

                AssetDatabase.SaveAssets();
                Log.Error(LogCategory.Editor, $"{nameof(BuildAvatar)}");
                // File.Move(oldName, newName);
            }
        }

        private void Rename()
        {
            foreach (var item in _objects)
            {
                var path = AssetDatabase.GetAssetPath(item);

                var oldName = item.name;
                var newName = oldName.Replace(_thisChar, _toChar);

                var asset = AssetImporter.GetAtPath(path);

                if (asset is ModelImporter importer)
                {
                    Log.Error(LogCategory.Editor, "modelImporter");

                    var clips = importer.clipAnimations;


                    if (clips.Length == 0)
                        clips = importer.defaultClipAnimations;

                    foreach (var clip in clips)
                    {
                        var clipOldName = clip.name;

                        Log.Error(LogCategory.Editor, $"{clipOldName} clipOldName");
                        var clipNewName = clipOldName.Replace(_thisChar, _toChar);
                        clip.name = clipNewName;

                        Log.Error(LogCategory.Editor, $"{clipNewName} clipNewName");
                    }

                    importer.clipAnimations = clips;
                    importer.SaveAndReimport();
                }


                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();
                Log.Warning(LogCategory.Editor, $"{oldName}, {newName}");

                // File.Move(oldName, newName);
            }
        }

        private void Clear()
        {
            _objects.Clear();
            _thisChar = String.Empty;
            _toChar = String.Empty;
        }
    }
}