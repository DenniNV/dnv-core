using System.Collections.Generic;
using System.Linq;
using Libraries.DNV.Core;
using UnityEngine;

namespace Libraries.DNV.Logging
{
    [CreateAssetMenu(fileName = "LoggerSettings", menuName = MenuPath.Root + "Logger Settings")]
    public sealed class LoggerSettings : ScriptableObject
    {
        public const string ResourcePath = "Settings/LoggerSettings";
        public const string AssetPath = "Assets/Resources/" + ResourcePath + ".asset";

        [SerializeField]
        private bool _enabled = true;

        [SerializeField]
        private List<LogCategoryColor> _categories = new();

        private Dictionary<int, Color> _colorsById;

        private static LoggerSettings _instance;

        /// <summary>
        /// Never null: without the asset the logger falls back to defaults and warns once,
        /// instead of silently dropping every Info/Warning.
        /// </summary>
        public static LoggerSettings Instance => _instance != null ? _instance : _instance = LoadOrCreateDefault();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlayModeEnter()
        {
            _instance = null;
        }

        public bool Enabled => _enabled;

        public IReadOnlyList<LogCategoryColor> Categories => _categories;

        public IEnumerable<LogCategoryColor> GameCategories => _categories.Where(entry => entry.IsGameCategory);

        public void SetEnabled(bool value)
        {
            _enabled = value;
        }

        public Color GetColor(LogCategory category)
        {
            _colorsById ??= BuildColorMap();
            return _colorsById.TryGetValue(category.Id, out var color) ? color : Color.white;
        }

        public void InvalidateColorCache()
        {
            _colorsById = null;
        }

        private static LoggerSettings LoadOrCreateDefault()
        {
            var settings = Resources.Load<LoggerSettings>(ResourcePath);

            if (settings != null)
                return settings;

            Debug.LogWarning($"[Logger] {AssetPath} not found — logging with default settings.");

            var defaults = CreateInstance<LoggerSettings>();
            defaults.hideFlags = HideFlags.DontSave;
            return defaults;
        }

        private Dictionary<int, Color> BuildColorMap()
        {
            var map = new Dictionary<int, Color>(_categories.Count);
            foreach (var entry in _categories)
            {
                map[entry.Id] = entry.Color;
            }

            return map;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Keeps every library category present with its current name, drops unknown library ids and duplicates,
        /// leaves game categories as they are. Returns true when the asset has to be saved.
        /// </summary>
        public bool SyncWithCategories()
        {
            var changed = _categories.RemoveAll(entry => !entry.IsGameCategory && LogCategory.Library.All(c => c.Id != entry.Id)) > 0;

            var visited = new HashSet<int>();
            changed |= _categories.RemoveAll(entry => !visited.Add(entry.Id)) > 0;

            foreach (var category in LogCategory.Library)
            {
                var entry = _categories.FirstOrDefault(e => e.Id == category.Id);

                if (entry == null)
                {
                    _categories.Add(new LogCategoryColor(category, Color.white));
                    changed = true;
                }
                else if (entry.Name != category.Name)
                {
                    entry.SetName(category.Name);
                    changed = true;
                }
            }

            if (!changed)
                return false;

            SortById();
            InvalidateColorCache();
            return true;
        }

        public void AddGameCategory(string name)
        {
            var nextId = _categories.Where(e => e.IsGameCategory).Select(e => e.Id + 1).DefaultIfEmpty(LogCategory.FirstGameId).Max();
            _categories.Add(new LogCategoryColor(new LogCategory(nextId, name), Color.white));
            SortById();
            InvalidateColorCache();
        }

        public void RemoveGameCategory(int id)
        {
            _categories.RemoveAll(entry => entry.IsGameCategory && entry.Id == id);
            InvalidateColorCache();
        }

        private void SortById()
        {
            _categories.Sort((left, right) => left.Id.CompareTo(right.Id));
        }
#endif
    }
}
