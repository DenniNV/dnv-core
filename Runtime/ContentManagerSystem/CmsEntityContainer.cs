using System;
using System.Collections.Generic;
using System.Linq;
using Libraries.DNV.Logging;
using UnityEngine;

namespace Libraries.DNV.ContentManagerSystem
{
    [Serializable]
    public class CmsEntityContainer
    {
        [SerializeField] private string _id;
        [SerializeField] private string _name;

        [SerializeReference] [SubclassSelector]
        private List<CmsEntityPart> _parts = new();

        public string Id => _id;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public IReadOnlyList<CmsEntityPart> Parts => _parts;

        public CmsEntityContainer()
        {
            _id = Guid.NewGuid().ToString();
            _name = string.Empty;
        }

        public CmsEntityContainer(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Log.Warning(LogCategory.Cms, "[CmsEntityContainer] Name cannot be null or empty. Using default.");
                name = "Unnamed";
            }

            _id = Guid.NewGuid().ToString();
            _name = name;
        }

        public void AddPart(CmsEntityPart part)
        {
            if (part == null)
            {
                Log.Warning(LogCategory.Cms, "[CmsEntityContainer] Cannot add null part.");
                return;
            }

            _parts.Add(part);
        }

        public void RemovePart(int index)
        {
            if (index < 0 || index >= _parts.Count)
            {
                Log.Warning(LogCategory.Cms, $"[CmsEntityContainer] Index {index} out of range (count: {_parts.Count}).");
                return;
            }

            _parts.RemoveAt(index);
        }

        public T GetPart<T>() where T : CmsEntityPart
        {
            return _parts.OfType<T>().FirstOrDefault();
        }

        public bool HasPart<T>() where T : CmsEntityPart
        {
            return _parts.OfType<T>().Any();
        }
    }
}
