using System;
using System.IO;
using Libraries.DNV.ContentManagerSystem;
using Libraries.DNV.Core;
using Libraries.DNV.Editor.CodeGeneration;
using Libraries.DNV.Logging;
using UnityEditor;
using UnityEngine;

namespace Libraries.DNV.Editor
{
    /// <summary>
    /// Custom EditorWindow (IMGUI) for managing the CMS.
    /// Provides search, sort, CRUD, and JSON export/import.
    /// </summary>
    public class CmsEditor : EditorWindow
    {
        private const float LeftPanelWidth = 300f;
        private const float ToolbarHeight = 24f;

        private enum SortMode
        {
            ByName,
            ById
        }

        private CmsDatabase _cms;
        private SerializedObject _serializedCms;
        private SerializedProperty _entitiesProperty;
        private int _databaseCount;

        private string _searchQuery = string.Empty;
        private SortMode _sortMode = SortMode.ByName;
        private int _selectedIndex = -1;

        private Vector2 _listScrollPosition;
        private Vector2 _detailsScrollPosition;

        [MenuItem(MenuPath.Tools + "CMS Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<CmsEditor>("CMS Editor");
            window.minSize = new Vector2(700f, 400f);
        }

        private void OnEnable()
        {
            FindDatabase();
        }

        private void OnFocus()
        {
            FindDatabase();
        }

        private void FindDatabase()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(CmsDatabase)}");
            _databaseCount = guids.Length;
            _cms = guids.Length > 0
                ? AssetDatabase.LoadAssetAtPath<CmsDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]))
                : null;

            if (_cms != null)
            {
                _serializedCms = new SerializedObject(_cms);
                _entitiesProperty = _serializedCms.FindProperty("_entities");
            }
            else
            {
                _serializedCms = null;
                _entitiesProperty = null;
            }
        }

        private void OnGUI()
        {
            if (_cms == null)
            {
                EditorGUILayout.HelpBox(
                    $"No {nameof(CmsDatabase)} asset found. Create one via Assets > Create > {MenuPath.Root}CMS Database " +
                    "and assign it to ProjectInstaller.",
                    MessageType.Warning);

                if (GUILayout.Button("Refresh"))
                    FindDatabase();

                return;
            }

            if (_databaseCount > 1)
            {
                EditorGUILayout.HelpBox(
                    $"Found {_databaseCount} {nameof(CmsDatabase)} assets, editing {AssetDatabase.GetAssetPath(_cms)}.",
                    MessageType.Warning);
            }

            _serializedCms.Update();

            DrawToolbar();
            DrawSearchAndSort();

            EditorGUILayout.BeginHorizontal();
            DrawEntityList();
            DrawEntityDetails();
            EditorGUILayout.EndHorizontal();

            _serializedCms.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(ToolbarHeight));

            if (GUILayout.Button("Create Entity", EditorStyles.toolbarButton, GUILayout.Width(100f)))
            {
                CreateNewEntity();
            }

            if (GUILayout.Button("Export JSON", EditorStyles.toolbarButton, GUILayout.Width(90f)))
            {
                ExportJson();
            }

            if (GUILayout.Button("Import JSON", EditorStyles.toolbarButton, GUILayout.Width(90f)))
            {
                ImportJson();
            }

            if (GUILayout.Button("Regenerate IDs", EditorStyles.toolbarButton, GUILayout.Width(110f)))
            {
                CmsCodeGeneration.RegenerateAll(_cms);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSearchAndSort()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Search:", GUILayout.Width(50f));
            _searchQuery = EditorGUILayout.TextField(_searchQuery);

            EditorGUILayout.LabelField("Sort:", GUILayout.Width(35f));
            _sortMode = (SortMode)EditorGUILayout.EnumPopup(_sortMode, GUILayout.Width(100f));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(2f);
        }

        private void DrawEntityList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(LeftPanelWidth));
            _listScrollPosition = EditorGUILayout.BeginScrollView(_listScrollPosition);

            var entities = GetFilteredAndSortedEntities();

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var originalIndex = FindOriginalIndex(entity);

                EditorGUILayout.BeginHorizontal(
                    _selectedIndex == originalIndex ? "SelectionRect" : "Box");

                if (GUILayout.Button(
                        $"{entity.Name}\n<size=9><color=#888888>{entity.Id}</color></size>",
                        new GUIStyle(EditorStyles.label) { richText = true, wordWrap = true },
                        GUILayout.MinHeight(36f)))
                {
                    _selectedIndex = originalIndex;
                }

                if (GUILayout.Button("X", GUILayout.Width(24f), GUILayout.Height(36f)))
                {
                    if (EditorUtility.DisplayDialog("Delete Entity",
                            $"Delete entity '{entity.Name}'?", "Delete", "Cancel"))
                    {
                        _entitiesProperty.DeleteArrayElementAtIndex(originalIndex);
                        _serializedCms.ApplyModifiedProperties();

                        if (_selectedIndex >= _entitiesProperty.arraySize)
                            _selectedIndex = _entitiesProperty.arraySize - 1;

                        GUIUtility.ExitGUI();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawEntityDetails()
        {
            EditorGUILayout.BeginVertical("Box");
            _detailsScrollPosition = EditorGUILayout.BeginScrollView(_detailsScrollPosition);

            if (_selectedIndex >= 0 && _selectedIndex < _entitiesProperty.arraySize)
            {
                var entityProperty = _entitiesProperty.GetArrayElementAtIndex(_selectedIndex);

                var idProp = entityProperty.FindPropertyRelative("_id");
                var nameProp = entityProperty.FindPropertyRelative("_name");
                var partsProp = entityProperty.FindPropertyRelative("_parts");

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(idProp, new GUIContent("ID"));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(nameProp, new GUIContent("Name"));

                EditorGUILayout.Space(8f);
                EditorGUILayout.LabelField("Parts", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(partsProp, new GUIContent("Parts"), true);
            }
            else
            {
                EditorGUILayout.LabelField("Select an entity to view details.");
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private CmsEntityContainer[] GetFilteredAndSortedEntities()
        {
            var list = string.IsNullOrEmpty(_searchQuery)
                ? new System.Collections.Generic.List<CmsEntityContainer>(_cms.Entities)
                : _cms.Search(_searchQuery);

            Comparison<CmsEntityContainer> comparison = _sortMode switch
            {
                SortMode.ByName => (a, b) =>
                    string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase),
                SortMode.ById => (a, b) =>
                    string.Compare(a.Id, b.Id, StringComparison.Ordinal),
                _ => (a, b) =>
                    string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase)
            };

            list.Sort(comparison);
            return list.ToArray();
        }

        private int FindOriginalIndex(CmsEntityContainer entity)
        {
            for (var i = 0; i < _cms.Entities.Count; i++)
            {
                if (_cms.Entities[i].Id == entity.Id)
                    return i;
            }

            return -1;
        }

        private void CreateNewEntity()
        {
            Undo.RecordObject(_cms, "Create CMS Entity");
            _cms.CreateEntity("New Entity");
            _serializedCms.Update();
            _selectedIndex = _entitiesProperty.arraySize - 1;
        }

        private void ExportJson()
        {
            var path = EditorUtility.SaveFilePanel("Export CMS to JSON", "", "cms_data", "json");
            if (string.IsNullOrEmpty(path))
                return;

            var json = _cms.ExportToJson();
            File.WriteAllText(path, json);
            Log.Info(LogCategory.Editor, $"[CmsEditor] Exported CMS data to: {path}");
        }

        private void ImportJson()
        {
            var path = EditorUtility.OpenFilePanel("Import CMS from JSON", "", "json");
            if (string.IsNullOrEmpty(path))
                return;

            var json = File.ReadAllText(path);

            Undo.RecordObject(_cms, "Import CMS JSON");
            _cms.ImportFromJson(json);

            _serializedCms.Update();
            _selectedIndex = -1;

            Log.Info(LogCategory.Editor, $"[CmsEditor] Imported CMS data from: {path}");
        }
    }
}
