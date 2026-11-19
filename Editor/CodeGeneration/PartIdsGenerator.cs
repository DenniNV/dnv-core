using System;
using System.Collections.Generic;
using System.Linq;
using Libraries.DNV.ContentManagerSystem;
using Libraries.DNV.Logging;

namespace Libraries.DNV.Editor.CodeGeneration
{
    /// <summary>
    /// Collects string ids from every <typeparamref name="TPart"/> across all CMS entities
    /// into a class of constants where each constant's name and value are the id itself.
    /// </summary>
    public abstract class PartIdsGenerator<TPart> : ICmsCodeGenerator where TPart : CmsEntityPart
    {
        protected abstract string ClassName { get; }

        protected abstract IEnumerable<string> CollectIds(TPart part);

        public void Generate(CmsDatabase database)
        {
            var constants = CollectUniqueIds(database).Select(id => new GeneratedConstant(id, id));

            GeneratedCodeWriter.WriteConstants(ClassName, constants);
        }

        private IEnumerable<string> CollectUniqueIds(CmsDatabase database)
        {
            var ids = new SortedSet<string>(StringComparer.Ordinal);

            foreach(var entity in database.Entities)
            {
                var part = entity.GetPart<TPart>();

                if(part == null)
                {
                    continue;
                }

                AddIds(ids, part, entity.Name);
            }

            return ids;
        }

        private void AddIds(SortedSet<string> ids, TPart part, string entityName)
        {
            foreach(var id in CollectIds(part) ?? Enumerable.Empty<string>())
            {
                if(string.IsNullOrEmpty(id))
                {
                    continue;
                }

                if(ids.Add(id) == false)
                {
                    Log.Warning(LogCategory.Editor, $"[{ClassName}] Duplicate id '{id}' in entity '{entityName}'.");
                }
            }
        }
    }
}
