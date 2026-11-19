using System.Collections.Generic;
using System.Linq;
using Libraries.DNV.ContentManagerSystem.Parts;

namespace Libraries.DNV.Editor.CodeGeneration
{
    public sealed class SceneIdsGenerator : PartIdsGenerator<ScenesPart>
    {
        protected override string ClassName => "SceneIds";

        protected override IEnumerable<string> CollectIds(ScenesPart part)
        {
            return part.Scenes?.Where(scene => scene != null).Select(scene => scene.Name);
        }
    }
}
