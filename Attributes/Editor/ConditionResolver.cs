using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Libraries.Attributes.Conditional.Editor
{
    internal static class ConditionResolver
    {
        private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Static |
                                                 BindingFlags.Public | BindingFlags.NonPublic |
                                                 BindingFlags.DeclaredOnly;

        private static readonly Dictionary<(Type Owner, string Member), MemberInfo> Members = new();

        /// <summary>
        /// Evaluates the attribute condition against the object that owns the drawn field.
        /// </summary>
        /// <param name="passed">Whether the field should stay visible and editable, inversion already applied.</param>
        public static bool TryEvaluate(SerializedProperty property, ConditionalDrawAttribute conditional,
            out bool passed, out string error)
        {
            passed = false;
            error = null;

            if(string.IsNullOrEmpty(conditional.MemberName))
            {
                error = "Condition member name is empty.";
                return false;
            }

            var ownerPath = GetOwnerPath(property.propertyPath);

            if(TryEvaluateSerialized(property, ownerPath, conditional, out passed))
                return true;

            return TryEvaluateReflected(property, ownerPath, conditional, out passed, out error);
        }

        private static bool TryEvaluateSerialized(SerializedProperty property, string ownerPath,
            ConditionalDrawAttribute conditional, out bool passed)
        {
            passed = false;

            var path = ownerPath.Length == 0
                ? conditional.MemberName
                : $"{ownerPath}.{conditional.MemberName}";

            var condition = property.serializedObject.FindProperty(path);

            if(condition == null)
                return false;

            if(condition.hasMultipleDifferentValues)
            {
                // Targets disagree, so the field stays visible instead of hiding data behind one of the values.
                passed = true;
                return true;
            }

            if(!TryReadValue(condition, out var value))
                return false;

            passed = Matches(value, conditional.ExpectedValues) != conditional.Inverted;
            return true;
        }

        private static bool TryEvaluateReflected(SerializedProperty property, string ownerPath,
            ConditionalDrawAttribute conditional, out bool passed, out string error)
        {
            passed = false;
            error = null;

            var evaluated = false;

            foreach(var target in property.serializedObject.targetObjects)
            {
                if(!TryGetOwner(target, ownerPath, out var owner) || owner == null)
                    continue;

                if(!TryGetMemberValue(owner, conditional.MemberName, out var value))
                {
                    error = $"'{owner.GetType().Name}' has no field, property or parameterless method " +
                            $"named '{conditional.MemberName}'.";
                    return false;
                }

                passed |= Matches(value, conditional.ExpectedValues) != conditional.Inverted;
                evaluated = true;
            }

            if(!evaluated)
                error = $"Condition member '{conditional.MemberName}' cannot be resolved for '{property.propertyPath}'.";

            return evaluated;
        }

        private static bool TryReadValue(SerializedProperty property, out object value)
        {
            switch(property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    value = property.boolValue;
                    return true;
                case SerializedPropertyType.Enum:
                    value = property.intValue;
                    return true;
                case SerializedPropertyType.Integer:
                    value = property.longValue;
                    return true;
                case SerializedPropertyType.Float:
                    value = property.floatValue;
                    return true;
                case SerializedPropertyType.String:
                    value = property.stringValue;
                    return true;
                case SerializedPropertyType.ObjectReference:
                    value = property.objectReferenceValue;
                    return true;
                default:
                    value = null;
                    return false;
            }
        }

        private static bool Matches(object value, object[] expectedValues)
        {
            if(expectedValues.Length == 0)
                return IsAssigned(value);

            foreach(var expected in expectedValues)
            {
                if(AreEqual(value, expected))
                    return true;
            }

            return false;
        }

        private static bool IsAssigned(object value)
        {
            return value switch
            {
                null => false,
                bool flag => flag,
                Object unityObject => unityObject != null,
                string text => !string.IsNullOrEmpty(text),
                _ => true
            };
        }

        private static bool AreEqual(object value, object expected)
        {
            if(value == null || expected == null)
                return ReferenceEquals(value, expected);

            if(TryConvertToLong(value, out var left) && TryConvertToLong(expected, out var right))
                return left == right;

            return value.Equals(expected);
        }

        private static bool TryConvertToLong(object value, out long result)
        {
            result = 0;

            if(value is not (Enum or sbyte or byte or short or ushort or int or uint or long))
                return false;

            result = Convert.ToInt64(value);
            return true;
        }

        /// <summary>
        /// Path of the object that declares the drawn field: the field's own segment is dropped, and for
        /// collection elements the whole array segment goes with it.
        /// </summary>
        private static string GetOwnerPath(string propertyPath)
        {
            var path = propertyPath;

            if(path.EndsWith("]", StringComparison.Ordinal))
            {
                var arrayIndex = path.LastIndexOf(".Array.data[", StringComparison.Ordinal);

                if(arrayIndex >= 0)
                    path = path.Substring(0, arrayIndex);
            }

            var lastDot = path.LastIndexOf('.');
            return lastDot < 0 ? string.Empty : path.Substring(0, lastDot);
        }

        private static bool TryGetOwner(Object target, string ownerPath, out object owner)
        {
            owner = target;

            if(string.IsNullOrEmpty(ownerPath))
                return true;

            foreach(var segment in ownerPath.Replace(".Array.data[", "[").Split('.'))
            {
                if(owner == null)
                    return false;

                var bracket = segment.IndexOf('[');

                if(bracket < 0)
                {
                    if(!TryGetFieldValue(owner, segment, out owner))
                        return false;

                    continue;
                }

                var name = segment.Substring(0, bracket);
                var index = int.Parse(segment.Substring(bracket + 1, segment.Length - bracket - 2));

                if(!TryGetFieldValue(owner, name, out owner) || !TryGetElement(owner, index, out owner))
                    return false;
            }

            return true;
        }

        private static bool TryGetFieldValue(object owner, string fieldName, out object value)
        {
            for(var type = owner.GetType(); type != null; type = type.BaseType)
            {
                var field = type.GetField(fieldName, MemberFlags);

                if(field == null)
                    continue;

                value = field.GetValue(field.IsStatic ? null : owner);
                return true;
            }

            value = null;
            return false;
        }

        private static bool TryGetElement(object collection, int index, out object element)
        {
            element = null;

            if(collection is IList list)
            {
                if(index < 0 || index >= list.Count)
                    return false;

                element = list[index];
                return true;
            }

            if(collection is not IEnumerable enumerable)
                return false;

            var current = 0;

            foreach(var item in enumerable)
            {
                if(current++ != index)
                    continue;

                element = item;
                return true;
            }

            return false;
        }

        private static bool TryGetMemberValue(object owner, string memberName, out object value)
        {
            value = null;

            switch(GetMember(owner.GetType(), memberName))
            {
                case FieldInfo field:
                    value = field.GetValue(field.IsStatic ? null : owner);
                    return true;
                case PropertyInfo property:
                    value = property.GetValue(property.GetMethod.IsStatic ? null : owner);
                    return true;
                case MethodInfo method:
                    value = method.Invoke(method.IsStatic ? null : owner, null);
                    return true;
                default:
                    return false;
            }
        }

        private static MemberInfo GetMember(Type ownerType, string memberName)
        {
            var key = (ownerType, memberName);

            if(Members.TryGetValue(key, out var cached))
                return cached;

            var member = FindMember(ownerType, memberName);
            Members.Add(key, member);
            return member;
        }

        private static MemberInfo FindMember(Type ownerType, string memberName)
        {
            for(var type = ownerType; type != null; type = type.BaseType)
            {
                var field = type.GetField(memberName, MemberFlags);

                if(field != null)
                    return field;

                var property = type.GetProperty(memberName, MemberFlags);

                if(property is { CanRead: true })
                    return property;

                var method = type.GetMethod(memberName, MemberFlags, null, Type.EmptyTypes, null);

                if(method != null && method.ReturnType != typeof(void))
                    return method;
            }

            return null;
        }
    }
}
