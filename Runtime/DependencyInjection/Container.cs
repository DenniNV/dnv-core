using System;
using System.Collections.Generic;
using Libraries.DNV.DependencyInjection.Interfaces;

namespace Libraries.DNV.DependencyInjection
{
    /// <summary>
    /// Resolvers are tracked by reference: two contexts with resolvers of the same type
    /// (e.g. two units) live side by side and are told apart by tag.
    /// </summary>
    internal sealed class Container : IContainer
    {
        private readonly List<IResolver> _resolvers = new List<IResolver>();

        public void Add(IResolver resolver)
        {
            if(_resolvers.Contains(resolver))
            {
                throw new InvalidOperationException($"{resolver.GetType().Name} is already added — was the context built twice?");
            }

            _resolvers.Add(resolver);
        }

        public void Remove(IResolver resolver)
        {
            _resolvers.Remove(resolver);
        }

        public TItem Resolve<TItem>() where TItem : class
        {
            return Resolve<TItem>(null);
        }

        public TItem Resolve<TItem>(string tag) where TItem : class
        {
            foreach(var resolver in _resolvers)
            {
                if(resolver.Resolve<TItem>(out var item, tag))
                {
                    return item;
                }
            }

            throw new InvalidOperationException(
                $"Dependencies cannot be resolved because this type {typeof(TItem)} (tag: {tag ?? "none"}) is not registered");
        }
    }
}
