using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Libraries.DNV.ContentManagerSystem
{
    /// <summary>
    /// Runtime access to CMS data. Works on a copy of the <see cref="CmsDatabase"/> asset, so changes made
    /// in Play Mode never reach the asset on disk; later a runtime JSON overlay can be applied to this copy.
    /// </summary>
    public sealed class Cms
    {
        private readonly CmsDatabase _runtimeCopy;
        private readonly Dictionary<string, CmsEntityContainer> _entityById = new();

        public Cms(CmsDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database), "CmsDatabase is not assigned in ProjectInstaller");

            _runtimeCopy = Object.Instantiate(database);
            BuildIndex();
        }

        public IReadOnlyList<CmsEntityContainer> Entities => _runtimeCopy.Entities;

        public CmsEntityContainer GetEntityById(string id)
        {
            if (TryGetEntity(id, out var entity))
                return entity;

            throw new InvalidOperationException($"CMS entity with id '{id}' not found");
        }

        public bool TryGetEntity(string id, out CmsEntityContainer entity)
        {
            if (id != null && _entityById.TryGetValue(id, out entity))
                return true;

            entity = null;
            return false;
        }

        /// <summary>
        /// For parts the library itself needs: it cannot know entity ids, which are generated per game.
        /// </summary>
        public TPart GetFirstPart<TPart>() where TPart : CmsEntityPart
        {
            foreach (var entity in Entities)
            {
                var part = entity.GetPart<TPart>();

                if (part != null)
                    return part;
            }

            throw new InvalidOperationException($"No CMS entity has a {typeof(TPart).Name}");
        }

        public CmsEntityContainer GetEntityByName(string name)
        {
            return Entities.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private void BuildIndex()
        {
            foreach (var entity in Entities)
            {
                if (entity.Id != null)
                    _entityById[entity.Id] = entity;
            }
        }
    }
}
