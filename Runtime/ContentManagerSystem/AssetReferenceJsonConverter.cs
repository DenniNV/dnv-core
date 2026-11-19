using System;
using Libraries.DNV.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Libraries.DNV.ContentManagerSystem
{
    /// <summary>
    /// Custom JsonConverter for Newtonsoft.Json that serializes/deserializes
    /// AssetReference types via their GUID string.
    /// </summary>
    public class AssetReferenceJsonConverter : JsonConverter
    {
        private const string GuidPropertyName = "guid";
        private const string TypePropertyName = "$assetRefType";

        public override bool CanConvert(Type objectType)
        {
            return typeof(AssetReference).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var assetRef = (AssetReference)value;

            writer.WriteStartObject();
            writer.WritePropertyName(TypePropertyName);
            writer.WriteValue(value.GetType().FullName);
            writer.WritePropertyName(GuidPropertyName);
            writer.WriteValue(assetRef.AssetGUID);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jsonObject = JObject.Load(reader);
            var typeName = jsonObject[TypePropertyName]?.Value<string>();
            var guid = jsonObject[GuidPropertyName]?.Value<string>();

            if (string.IsNullOrEmpty(guid))
            {
                Log.Warning(LogCategory.Cms, "[AssetReferenceJsonConverter] Missing GUID in JSON.");
                return null;
            }

            return typeName switch
            {
                "UnityEngine.AddressableAssets.AssetReferenceGameObject" => new AssetReferenceGameObject(guid),
                "UnityEngine.AddressableAssets.AssetReferenceSprite" => new AssetReferenceSprite(guid),
                _ when typeName != null && typeName.Contains("AssetReferenceT") =>
                    CreateGenericAssetReference(typeName, guid),
                _ => new AssetReference(guid)
            };
        }

        private static AssetReference CreateGenericAssetReference(string typeName, string guid)
        {
            if (typeName.Contains("Mesh"))
                return new AssetReferenceT<Mesh>(guid);

            Log.Warning(LogCategory.Cms,
                $"[AssetReferenceJsonConverter] Unknown AssetReference type: {typeName}. Falling back to base AssetReference.");
            return new AssetReference(guid);
        }
    }
}
