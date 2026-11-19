namespace Libraries.DNV.DependencyInjection.Interfaces
{
    public interface IResolver
    {
        bool Resolve<TItem>(out TItem item, string tag) where TItem : class;
    }
}