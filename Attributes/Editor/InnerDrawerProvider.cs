using System;
using System.Collections.Generic;
using System.Reflection;
using MackySoft.SerializeReferenceExtensions.Editor;
using UnityEditor;
using UnityEngine;

namespace Libraries.Attributes.Conditional.Editor
{
    /// <summary>
    /// Unity draws a single PropertyDrawer per field, so a conditional attribute silently suppresses the drawer
    /// of any other attribute on the same field. This provider resolves the suppressed drawer, letting the
    /// conditional drawer delegate the rendering instead of replacing it.
    /// </summary>
    internal static class InnerDrawerProvider
    {
        private static readonly Dictionary<FieldInfo, PropertyDrawer> AttributeDrawers = new();

        private static readonly FieldInfo AttributeSlot =
            typeof(PropertyDrawer).GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo FieldInfoSlot =
            typeof(PropertyDrawer).GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.NonPublic);

        public static PropertyDrawer Resolve(SerializedProperty property, FieldInfo fieldInfo)
        {
            if(fieldInfo == null)
                return null;

            return ResolveAttributeDrawer(fieldInfo) ?? ResolveTypeDrawer(property, fieldInfo);
        }

        private static PropertyDrawer ResolveAttributeDrawer(FieldInfo fieldInfo)
        {
            if(AttributeDrawers.TryGetValue(fieldInfo, out var cached))
                return cached;

            var drawer = CreateAttributeDrawer(fieldInfo);
            AttributeDrawers.Add(fieldInfo, drawer);
            return drawer;
        }

        private static PropertyDrawer CreateAttributeDrawer(FieldInfo fieldInfo)
        {
            // Binding relies on internals of PropertyDrawer: without them the drawer would read a null attribute.
            if(AttributeSlot == null || FieldInfoSlot == null)
                return null;

            foreach(var candidate in fieldInfo.GetCustomAttributes<PropertyAttribute>(true))
            {
                if(candidate is ConditionalDrawAttribute)
                    continue;

                if(!PropertyDrawerCache.TryGetPropertyDrawer(candidate.GetType(), out var shared))
                    continue;

                // The cached instance is shared by every field of that type, while a bound drawer belongs
                // to a single attribute instance, so each field gets its own copy.
                var drawer = (PropertyDrawer)Activator.CreateInstance(shared.GetType());
                Bind(drawer, candidate, fieldInfo);
                return drawer;
            }

            return null;
        }

        private static PropertyDrawer ResolveTypeDrawer(SerializedProperty property, FieldInfo fieldInfo)
        {
            var valueType = GetValueType(property, fieldInfo);

            if(valueType == null || !PropertyDrawerCache.TryGetPropertyDrawer(valueType, out var drawer))
                return null;

            Bind(drawer, null, fieldInfo);
            return drawer;
        }

        private static Type GetValueType(SerializedProperty property, FieldInfo fieldInfo)
        {
            if(property.propertyType == SerializedPropertyType.ManagedReference)
                return ManagedReferenceUtility.GetType(property.managedReferenceFullTypename);

            var fieldType = fieldInfo.FieldType;

            if(fieldType.IsArray)
                return fieldType.GetElementType();

            if(fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                return fieldType.GetGenericArguments()[0];

            return fieldType;
        }

        private static void Bind(PropertyDrawer drawer, PropertyAttribute attribute, FieldInfo fieldInfo)
        {
            AttributeSlot.SetValue(drawer, attribute);
            FieldInfoSlot.SetValue(drawer, fieldInfo);
        }
    }
}
