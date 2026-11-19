namespace Libraries.DNV.CodePresets.Pools.Interface
{
    public interface IPoolProvider
    {
        IPoolRent<TItem> Get<TItem>(string id) where TItem : class;
    }
}
