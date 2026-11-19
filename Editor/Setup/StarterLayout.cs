namespace Libraries.DNV.Editor.Setup
{
    /// <summary>
    /// Where the starter project lives inside Assets. Mirrors the package folder Samples~/Starter one to one.
    /// </summary>
    internal static class StarterLayout
    {
        public const string SamplesFolder = "Samples~/Starter";
        public const string AssetsRoot = "Assets";
        public const string MetaExtension = ".meta";

        public const string EntryScene = "Assets/Scenes/Entry.unity";
        public const string GameScene = "Assets/Scenes/Game.unity";
        public const string AddressableRoot = "Assets/Addressable";
        public const string CmsDatabase = AddressableRoot + "/EntryPoint/CmsDatabase.asset";

        public const string LegacyAddressableRoot = "Assets/Adressable";

        public static readonly string[] AddressableFolders =
        {
            AddressableRoot + "/EntryPoint",
            AddressableRoot + "/UI",
        };

        /// <summary>
        /// Top-level folders of Samples~/Starter. In the host project they are edited in Assets and copied
        /// back to the package by StarterTemplateSync, so the package holds the only tracked copy.
        /// </summary>
        public static readonly string[] MirroredFolders = { "Scenes", "Addressable" };
    }
}
