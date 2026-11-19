namespace Libraries.DNV.CodePresets.Modules
{
    public interface IModuleProvider<in TItem> where TItem : class
    {
        TModule Get<TModule>() where TModule : class, TItem;
    }
}
