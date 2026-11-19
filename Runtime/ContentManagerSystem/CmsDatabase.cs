using System;
using System.Collections.Generic;
using System.Linq;
using Libraries.DNV.Core;
using Libraries.DNV.Logging;
using Newtonsoft.Json;
using UnityEngine;

namespace Libraries.DNV.ContentManagerSystem
{
    /// <summary>
    /// Authoring storage of CMS data, edited in the CMS Editor. Runtime code never reads it directly —
    /// it gets a <see cref="Cms"/> built on a copy, so Play Mode cannot write into the asset.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(CmsDatabase), menuName = MenuPath.Root + "CMS Database")]
    public sealed class CmsDatabase : ScriptableObject
    {
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ContractResolver = new UnityContractResolver(),
            Converters = { new AssetReferenceJsonConverter() }
        };

        [SerializeField]
        private List<CmsEntityContainer> _entities = new();

        public IReadOnlyList<CmsEntityContainer> Entities => _entities;

        public CmsEntityContainer CreateEntity(string name)
        {
            var entity = new CmsEntityContainer(name);
            _entities.Add(entity);
            MarkDirty();
            return entity;
        }

        public void RemoveEntity(string id)
        {
            var index = _entities.FindIndex(e => e.Id == id);
            if (index < 0)
            {
                Log.Warning(LogCategory.Cms, $"Entity with ID '{id}' not found.");
                return;
            }

            _entities.RemoveAt(index);
            MarkDirty();
        }

        public List<CmsEntityContainer> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
                return new List<CmsEntityContainer>(_entities);

            return _entities
                .Where(e =>
                    (e.Id != null && e.Id.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                    (e.Name != null && e.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public string ExportToJson()
        {
            return JsonConvert.SerializeObject(_entities, JsonSettings);
        }

        public void ImportFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Log.Warning(LogCategory.Cms, "Cannot import empty JSON.");
                return;
            }

            List<CmsEntityContainer> imported;
            try
            {
                imported = JsonConvert.DeserializeObject<List<CmsEntityContainer>>(json, JsonSettings);
            }
            catch (Exception ex)
            {
                Log.Error(LogCategory.Cms, $"Failed to deserialize JSON: {ex.Message}");
                return;
            }

            if (imported == null)
            {
                Log.Warning(LogCategory.Cms, "Deserialized result is null.");
                return;
            }

            ValidateDuplicateIds(imported);
            _entities = imported;
            MarkDirty();
        }

        private static void ValidateDuplicateIds(List<CmsEntityContainer> entities)
        {
            var seen = new HashSet<string>();
            foreach (var entity in entities)
            {
                if (entity.Id == null) continue;
                if (!seen.Add(entity.Id))
                {
                    Log.Warning(LogCategory.Cms, $"Duplicate entity ID detected: '{entity.Id}' (name: '{entity.Name}').");
                }
            }
        }

        private void MarkDirty()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
