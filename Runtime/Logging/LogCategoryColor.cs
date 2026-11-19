using System;
using UnityEngine;

namespace Libraries.DNV.Logging
{
    [Serializable]
    public class LogCategoryColor
    {
        // Field name kept from the enum era: the id was serialized as "_category".
        [SerializeField]
        private int _category;

        [SerializeField]
        private string _name;

        [SerializeField]
        private Color _color = Color.white;

        public LogCategoryColor()
        {
        }

        public LogCategoryColor(LogCategory category, Color color)
        {
            _category = category.Id;
            _name = category.Name;
            _color = color;
        }

        public int Id => _category;
        public string Name => _name;
        public LogCategory Category => new(_category, _name);
        public Color Color => _color;
        public bool IsGameCategory => _category >= LogCategory.FirstGameId;

        public void SetColor(Color color)
        {
            _color = color;
        }

        public void SetName(string name)
        {
            _name = name;
        }
    }
}
