using Libraries.DNV.ContentManagerSystem;

namespace Libraries.DNV.Editor.CodeGeneration
{
    /// <summary>
    /// Any non-abstract implementation is found via TypeCache and run by "Regenerate IDs" in the CMS Editor,
    /// so a project adds a generator just by declaring the class. Requires a parameterless constructor.
    /// </summary>
    public interface ICmsCodeGenerator
    {
        void Generate(CmsDatabase database);
    }
}
