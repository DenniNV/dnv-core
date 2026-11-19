using UnityEditor;
using UnityEngine;

namespace Libraries.Attributes.Conditional.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalDrawAttribute), true)]
    public class ConditionalDrawDrawer : PropertyDrawer
    {
        private const float ErrorHeight = 34f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var conditional = (ConditionalDrawAttribute)attribute;

            if(!ConditionResolver.TryEvaluate(property, conditional, out var passed, out var error))
            {
                DrawError(position, property, label, error);
                return;
            }

            if(passed)
            {
                DrawProperty(position, property, label);
                return;
            }

            if(conditional.Mode == ConditionalMode.Hide)
                return;

            using(new EditorGUI.DisabledScope(true))
                DrawProperty(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var conditional = (ConditionalDrawAttribute)attribute;

            if(!ConditionResolver.TryEvaluate(property, conditional, out var passed, out _))
                return ErrorHeight + EditorGUIUtility.standardVerticalSpacing + GetInnerHeight(property, label);

            if(!passed && conditional.Mode == ConditionalMode.Hide)
                return -EditorGUIUtility.standardVerticalSpacing;

            return GetInnerHeight(property, label);
        }

        private void DrawError(Rect position, SerializedProperty property, GUIContent label, string error)
        {
            var errorRect = new Rect(position.x, position.y, position.width, ErrorHeight);
            EditorGUI.HelpBox(errorRect, error, MessageType.Error);

            var fieldRect = new Rect(position)
            {
                y = errorRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                height = position.height - ErrorHeight - EditorGUIUtility.standardVerticalSpacing
            };

            DrawProperty(fieldRect, property, label);
        }

        private void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            var inner = InnerDrawerProvider.Resolve(property, fieldInfo);

            if(inner == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            inner.OnGUI(position, property, label);
        }

        private float GetInnerHeight(SerializedProperty property, GUIContent label)
        {
            var inner = InnerDrawerProvider.Resolve(property, fieldInfo);

            return inner == null
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : inner.GetPropertyHeight(property, label);
        }
    }
}
