using System;
using System.Collections.Generic;
using Libraries.DNV.CodePresets.Factory;
using Libraries.DNV.DependencyInjection.Interfaces;
using Libraries.DNV.Logging;

namespace Libraries.DNV.DependencyInjection
{
    [Serializable]
    public abstract class Resolver : IResolver
    {
        private readonly Dictionary<DIKey, DIItem> _baseTypes = new();
        private readonly Dictionary<DIKey, DIItem> _interfaces = new();

        public void RegistrationInstance<TItem>(TItem[] instances) where TItem : class
        {
            RegistrationInstance(instances, null);
        }

        public void RegistrationInstance<TItem>(TItem[] instances, string tag) where TItem : class
        {
            foreach (var item in instances)
            {
                RegistrationSingleton(item, tag);
            }
        }

        public void RegistrationInstance<TItem>(TItem instance) where TItem : class
        {
            RegistrationSingleton(instance, null);
        }

        public void RegistrationInstance<TItem>(TItem instance, string tag) where TItem : class
        {
            RegistrationSingleton(instance, tag);
        }

        // The key is always the declared TItem: what you register under is exactly what you resolve by.
        private void RegistrationSingleton<TItem>(TItem instance, string tag) where TItem : class
        {
            var key = new DIKey(tag, typeof(TItem));

            Log.Info(LogCategory.DependencyInjection, $"Registering {key.Type}");

            var items = typeof(TItem).IsInterface ? _interfaces : _baseTypes;

            if (!items.TryAdd(key, new DISingletonItem<TItem>(instance)))
                throw AlreadyRegistered<TItem>(tag);

            Construct(instance);
        }

        public void Registration<TItem>(IStaticFactory<TItem> staticFactory) where TItem : class
        {
            Registration(staticFactory, null);
        }

        public void Registration<TItem>(IStaticFactory<TItem> staticFactory, string tag) where TItem : class
        {
            var diKey = new DIKey(tag, typeof(TItem));

            if (!_baseTypes.TryAdd(diKey, new DIDynamicItem<TItem>(staticFactory, tag)))
                throw AlreadyRegistered<TItem>(tag);
        }

        /// <summary>
        /// Resolves only from this scope. Lets an installer use what an earlier installer of the same context
        /// registered, since <see cref="Injector"/> sees the scope only after every installer has run.
        /// </summary>
        public TItem Get<TItem>(string tag = null) where TItem : class
        {
            if (Resolve(out TItem item, tag))
                return item;

            throw new InvalidOperationException(
                $"{typeof(TItem).Name} (tag: {tag ?? "none"}) is not registered in {GetType().Name} yet — check installer order");
        }

        bool IResolver.Resolve<TItem>(out TItem item, string tag) where TItem : class
        {
            return Resolve(out item, tag);
        }

        private InvalidOperationException AlreadyRegistered<TItem>(string tag)
        {
            return new InvalidOperationException(
                $"{typeof(TItem).Name} (tag: {tag ?? "none"}) is already registered in {GetType().Name}");
        }

        private void Construct<TItem>(TItem item)
        {
            if (item is IDIConstructor constructor)
                constructor.Construct();
        }

        private void LazyConstruct<TItem>(TItem item)
        {
            if (item is IInjectionInitializer initializer)
                initializer.Initialize();
        }

        private bool Resolve<TItem>(out TItem instance, string tag)
        {
            var isInterface = typeof(TItem).IsInterface;
            var diKey = new DIKey(tag, typeof(TItem));

            if (isInterface)
            {
                if (_interfaces.TryGetValue(diKey, out var item))
                {
                    var result = item.Instance;
                    instance = (TItem)result;
                    return true;
                }
            }
            else
            {
                if (_baseTypes.TryGetValue(diKey, out var item))
                {
                    var result = item.Instance;
                    instance = (TItem)result;
                    return true;
                }
            }
           

            instance = default;
            return false;
        }

        
        [Serializable]
        public class Entry : Resolver { }

        [Serializable]
        public class Project : Resolver { }

        [Serializable]
        public class Unit : Resolver { }

        [Serializable]
        public class Game : Resolver { }

        [Serializable]
        public class UI : Resolver { }

        #region BaseDIItem

        private abstract class DIItem
        {
            public string Tag { get; }

            public object Instance => GetInstance();

            protected DIItem(string tag)
            {
                Tag = tag;
            }

            protected abstract object GetInstance();
        }

        private class DISingletonItem<TType> : DIItem where TType : class
        {
            private readonly TType _instance;

            public DISingletonItem(TType instance, string tag = null) : base(tag)
            {
                _instance = instance;
            }

            protected override object GetInstance()
            {
                return _instance;
            }
        }

        private class DIDynamicItem<TType> : DIItem where TType : class
        {
            private readonly IStaticFactory<TType> _staticFactory;

            public DIDynamicItem(IStaticFactory<TType> staticFactory, string tag = null) : base(tag)
            {
                _staticFactory = staticFactory;
            }

            protected override object GetInstance()
            {
                var instance = _staticFactory.Create();

                if (instance is IDIConstructor constructor)
                    constructor.Construct();

                return instance;
            }
        }

        private readonly struct DIKey
        {
            public string Tag { get; }
            public Type Type { get; }

            public DIKey(string tag, Type type)
            {
                Tag = tag;
                Type = type;
            }

            public override bool Equals(object obj)
            {
                if (obj is DIKey other)
                    return Tag == other.Tag && Type == other.Type;

                return false;
            }

            public override int GetHashCode()
            {
                if (ReferenceEquals(Tag, null))
                    return Type.GetHashCode();

                return Type.GetHashCode() ^ Tag.GetHashCode();
            }
        }

        #endregion
    }
}