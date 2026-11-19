using System;
using System.Collections.Generic;

namespace Libraries.DNV.CodePresets.Modules
{
    public class ModuleRegistry<TItem> : IModuleProvider<TItem> where TItem : class
    {
        private readonly IReadOnlyList<TItem> _modules;
        private readonly Dictionary<Type, TItem> _cache = new();

        public ModuleRegistry(IReadOnlyList<TItem> modules)
        {
            if (modules == null || modules.Count == 0)
            {
                throw new ArgumentException(
                    $"Module set of {typeof(TItem).Name} is null or empty.", nameof(modules));
            }

            _modules = modules;
        }

        public TModule Get<TModule>() where TModule : class, TItem
        {
            var type = typeof(TModule);

            if (_cache.TryGetValue(type, out var cached))
            {
                return (TModule)cached;
            }

            for (var i = _modules.Count - 1; i >= 0; i--)
            {
                if (_modules[i] is TModule found)
                {
                    _cache[type] = found;
                    return found;
                }
            }

            throw new InvalidOperationException(
                $"Module of type {type.Name} is not registered in {typeof(TItem).Name} set.");
        }
    }
}
