using System;
using System.Collections.Generic;
using Libraries.DNV.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Libraries.DNV.Editor
{
    public class CopyComponentEditor : EditorWindow
    {
        private SerializedObject _serializedObject;

        [SerializeField] private GameObject _current;

        [SerializeField] private GameObject _target;

        [Header("Also copy name, position, rotation and scale")]
        [SerializeField]
        private bool _isCopyAll;

        [Header("Copy the selected object only, without children")]
        [SerializeField]
        private bool _onlyRoot;

        private readonly List<DuplicateComponent> _duplicates = new List<DuplicateComponent>();

        [MenuItem(MenuPath.Tools + nameof(CopyComponentEditor))]
        private static void OpenWindow()
        {
            GetWindow<CopyComponentEditor>().Show();
        }

        // OnEnable, not the menu handler: an open window is restored after a domain reload without the menu call.
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
        }

        private void OnGUI()
        {
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_current"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_target"));

            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_isCopyAll"));
            EditorGUILayout.PropertyField(_serializedObject.FindProperty("_onlyRoot"));

            _serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Copy"))
            {
                CopyComponent();
                PastComponents();
            }
        }

        private void RemoveComponentsOnTargetObject(Component[] components)
        {
            foreach (var component in components)
            {
                if (component is Transform)
                {
                    continue;
                }

                try
                {
                    DestroyImmediate(component, true);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void RemoveComponents(GameObject target)
        {
            var components = target.GetComponents<Component>();
            RemoveComponentsOnTargetObject(components);
        }

        private void CopyComponent()
        {
            _duplicates.Clear();

            if (_onlyRoot)
            {
                var copy = new DuplicateComponent(_current.name,
                    _current.transform.position,
                    _current.transform.localPosition,
                    _current.transform.rotation,
                    _current.transform.localScale,
                    _current.GetComponents<Component>());
                _duplicates.Add(copy);

                return;
            }

            var children = _current.GetComponentsInChildren<Transform>();
            var childrenLength = children.Length;

            for (int i = 0; i < childrenLength; i++)
            {
                var child = children[i];

                var copy = new DuplicateComponent(child.name,
                    child.position,
                    child.localPosition,
                    child.rotation,
                    child.localScale,
                    child.GetComponents<Component>());

                _duplicates.Add(copy);
            }
        }

        private void PastComponents()
        {
            if (_onlyRoot)
            {
                if (_isCopyAll)
                {
                    PastAll(_target.transform, _duplicates[0]);
                }
                else
                {
                    RemoveComponents(_target.gameObject);
                    PastComponents(_duplicates[0].Components, _target.gameObject);
                }

                return;
            }

            var children = _target.GetComponentsInChildren<Transform>();
            var childrenLength = children.Length;

            if (childrenLength < _duplicates.Count)
            {
                throw new Exception($"The number of objects does not match the original " +
                                    $"{_duplicates.Count} current children {childrenLength} target children");
            }

            for (int i = 0; i < childrenLength; i++)
            {
                var child = children[i];
                var copy = _duplicates[i];

                if (_isCopyAll)
                {
                    PastAll(child, copy);
                }
                else
                {
                    RemoveComponents(child.gameObject);
                    PastComponents(copy.Components, child.gameObject);
                }
            }
        }

        private void PastComponents(Component[] components, GameObject target)
        {
            foreach (var component in components)
            {
                ComponentUtility.CopyComponent(component);
                ComponentUtility.PasteComponentAsNew(target);
            }
        }

        private void PastAll(Transform child, DuplicateComponent duplicate)
        {
            RemoveComponents(child.gameObject);
            PastComponents(duplicate.Components, child.gameObject);

            child.name = duplicate.Name;
            child.position = duplicate.Position;
            child.localPosition = duplicate.LocalPosition;
            child.rotation = duplicate.Rotation;
            child.localScale = duplicate.Size;
        }

        private struct DuplicateComponent
        {
            public string Name { get; }
            public Vector3 Position { get; }
            public Vector3 LocalPosition { get; }
            public Quaternion Rotation { get; }
            public Vector3 Size { get; }
            public Component[] Components { get; }

            public DuplicateComponent(string name, Vector3 position, Vector3 localPosition, Quaternion rotation,
                Vector3 size, Component[] components)
            {
                Name = name;
                Position = position;
                LocalPosition = localPosition;
                Rotation = rotation;
                Size = size;
                Components = components;
            }
        }
    }
}