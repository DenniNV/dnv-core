namespace Libraries.DNV.ExtendedUpdate
{
    /// <summary>
    /// Order of execution groups in <see cref="ExecutionPipeline"/>: lower runs first.
    /// Decide, then act: <see cref="Brain"/> holds whatever changes the state of machines, so the systems in
    /// <see cref="Default"/> read a settled state within one frame. Game groups normally use <see cref="Default"/>.
    /// </summary>
    public static class ExecutionOrder
    {
        public const int Project = 0;
        public const int Brain = 250;
        public const int Default = 500;
        public const int Pool = 1000;
    }
}
