using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Libraries.DNV.ContentManagerSystem
{
    /// <summary>
    /// Custom ContractResolver that serializes ONLY fields matching Unity's
    /// serialization rules: private [SerializeField]/[SerializeReference]
    /// and public fields without [NonSerialized]. Public properties are excluded
    /// to avoid duplication with their backing fields.
    /// </summary>
    public class UnityContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = new List<JsonProperty>();

            var fields = GetAllFields(type);
            foreach (var field in fields)
            {
                if (!IsUnitySerializedField(field))
                    continue;

                var jsonProperty = base.CreateProperty(field, MemberSerialization.Fields);
                jsonProperty.Writable = true;
                jsonProperty.Readable = true;
                jsonProperty.PropertyName = field.Name;
                jsonProperty.DeclaringType = type;
                properties.Add(jsonProperty);
            }

            return properties;
        }

        private static bool IsUnitySerializedField(FieldInfo field)
        {
            if (field.IsDefined(typeof(NonSerializedAttribute), false))
                return false;

            if (field.IsDefined(typeof(SerializeField), false) ||
                field.IsDefined(typeof(SerializeReference), false))
                return true;

            return field.IsPublic;
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public |
                                       BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            var currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                foreach (var field in currentType.GetFields(flags))
                {
                    yield return field;
                }

                currentType = currentType.BaseType;
            }
        }
    }
}
