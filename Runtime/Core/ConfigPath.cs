namespace Libraries.DNV.Core
{
    /// <summary>
    /// Roots of "Create" asset menu entries. Game-specific categories belong to the game.
    /// </summary>
    public static class ConfigPath
    {
        private const string Separator = "/";
        private const string GlobalPath = "Data/";
        private const string New = nameof(New) + Space;
        private const string Space = " ";

        public const string Unit = GlobalPath + nameof(Unit) + Separator;
        public const string GlobalData = GlobalPath + nameof(GlobalData) + Separator + New;
        public const string ProjectData = GlobalPath + nameof(ProjectData) + Separator + New;
        public const string ServiceData = GlobalPath + nameof(ServiceData) + Separator + New;
        public const string PoolData = GlobalPath + nameof(PoolData) + Separator + New;
        public const string ContextData = GlobalPath + nameof(ContextData) + Separator;
        public const string GameSequence = GlobalPath + nameof(GameSequence) + Separator;
    }
}
