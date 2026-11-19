namespace Libraries.DNV.DependencyInjection
{
    /// <summary>
    /// Public entry point for resolving dependencies. Adding or removing whole scopes
    /// is deliberately not here — only library contexts do that, via <see cref="ResolverRegistry"/>.
    /// </summary>
    public static class Injector
    {
        public static TItem Get<TItem>() where TItem : class
        {
            return ResolverRegistry.Container.Resolve<TItem>();
        }

        public static TItem Get<TItem>(string tag) where TItem : class
        {
            return ResolverRegistry.Container.Resolve<TItem>(tag);
        }
    }
}
