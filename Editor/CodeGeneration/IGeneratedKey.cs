namespace Libraries.DNV.Editor.CodeGeneration
{
    /// <summary>
    /// Source text a generated member's identifier is made from, e.g. "Unit Settings" → UnitSettings.
    /// </summary>
    public interface IGeneratedKey
    {
        string Key { get; }
    }
}
