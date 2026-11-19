namespace Libraries.DNV.Editor.CodeGeneration
{
    public readonly struct GeneratedConstant : IGeneratedKey
    {
        /// <summary>Source text for the C# identifier, e.g. an entity name "Unit Settings".</summary>
        public string Key { get; }

        public string Value { get; }

        public GeneratedConstant(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
