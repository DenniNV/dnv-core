using System.Linq;
using Libraries.DNV.ContentManagerSystem;

namespace Libraries.DNV.Editor.CodeGeneration
{
    /// <summary>
    /// Entity name → entity GUID. Not a PartIdsGenerator: it reads the entities themselves, not their parts.
    /// </summary>
    public sealed class CmsIdsGenerator : ICmsCodeGenerator
    {
        private const string ClassName = "CmsIds";

        public void Generate(CmsDatabase database)
        {
            var constants = database.Entities
                .Where(entity => string.IsNullOrEmpty(entity.Id) == false)
                .Select(entity => new GeneratedConstant(entity.Name, entity.Id));

            GeneratedCodeWriter.WriteConstants(ClassName, constants);
        }
    }
}
