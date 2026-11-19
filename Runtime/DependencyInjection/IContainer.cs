using Libraries.DNV.DependencyInjection.Interfaces;

namespace Libraries.DNV.DependencyInjection
{
    internal interface IContainer
    {
        TItem Resolve<TItem>() where TItem : class;
        TItem Resolve<TItem>(string tag) where TItem : class;

        void Add(IResolver resolver);
        void Remove(IResolver resolver);
    }
}