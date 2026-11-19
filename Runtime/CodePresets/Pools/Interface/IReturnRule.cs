namespace Libraries.DNV.CodePresets.Pools.Interface
{
    public interface IReturnRule<in TItem> where TItem : class
    {
        IReturnState<TItem> CreateState();
    }

    public interface IReturnState<in TItem> where TItem : class
    {
        void OnRent(TItem item);
        bool ShouldReturn(TItem item, float deltaTime);
    }
}
