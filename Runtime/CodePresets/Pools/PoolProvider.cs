using System;
using System.Collections.Generic;
using Libraries.DNV.CodePresets.Pools.Interface;
using Libraries.DNV.ExtendedUpdate;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Libraries.DNV.CodePresets.Pools
{
    public sealed class PoolProvider : IPoolProvider, IExecutable
    {
        private readonly Dictionary<string, IPool> _allPools = new Dictionary<string, IPool>(StringComparer.Ordinal);
        private readonly List<IPool> _executedPools = new List<IPool>();

        private Transform _root;

        public Transform Root => _root != null ? _root : CreateRoot();

        public IPoolRent<TItem> Get<TItem>(string id) where TItem : class
        {
            if(!_allPools.TryGetValue(id, out var pool))
            {
                var registered = _allPools.Count == 0 ? "(none)" : string.Join(", ", _allPools.Keys);
                throw new KeyNotFoundException($"Pool '{id}' is not registered. Registered pools: {registered}.");
            }

            if(pool is IPoolRent<TItem> rent)
            {
                return rent;
            }

            throw new InvalidCastException($"Pool '{id}' does not provide '{typeof(TItem).Name}'.");
        }

        public bool Contains(string id)
        {
            return _allPools.ContainsKey(id);
        }

        public void Add(IPool pool)
        {
            if(!_allPools.TryAdd(pool.Id, pool))
            {
                throw new ArgumentException($"Pool '{pool.Id}' is already registered.");
            }

            _executedPools.Add(pool);
        }

        public void Remove(IPool pool)
        {
            if(!_allPools.Remove(pool.Id))
            {
                return;
            }

            _executedPools.Remove(pool);
        }

        public void Execute()
        {
            for (var i = _executedPools.Count - 1; i >= 0; i--)
            {
                _executedPools[i].Execute();
            }
        }

        private Transform CreateRoot()
        {
            var holder = new GameObject("Pools");
            Object.DontDestroyOnLoad(holder);
            _root = holder.transform;

            return _root;
        }
    }
}
