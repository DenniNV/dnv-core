using System.Collections.Generic;
using System.Linq;
using Libraries.DNV.ContentManagerSystem.Parts;

namespace Libraries.DNV.Editor.CodeGeneration
{
    public sealed class PoolIdsGenerator : PartIdsGenerator<PoolsPart>
    {
        protected override string ClassName => "PoolIds";

        protected override IEnumerable<string> CollectIds(PoolsPart part)
        {
            return part.PoolConfigs?.Where(pool => pool != null).Select(pool => pool.Id);
        }
    }
}
