using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Libraries.DNV.Addressable
{
    internal sealed class InstanceRegistry
    {
        private readonly AssetCache _cache;
        private readonly Dictionary<Component, AssetKey> _keyByInstance = new(ReferenceComparer.Instance);
        private readonly Dictionary<AssetKey, List<Component>> _instancesByKey = new();

        public InstanceRegistry(AssetCache cache)
        {
            _cache = cache;
        }

        public async UniTask<T> InstantiateAsync<T>(CustomReferences<T> reference, SpawnValues values, CancellationToken token)
            where T : Component
        {
            var key = AssetKey.Of<T>(reference.AssetGUID);
            var asset = await _cache.LoadAsync(key, reference.LoadAssetAsync, token);
            var instance = Object.Instantiate(asset, values.Position, values.Rotation, values.Parent);
            Register(key, instance);

            Log.Info(LogCategory.Assets,
                $"[InstanceRegistry] SPAWN   {instance.name} of {key} — alive instances: {AliveCountOf(key)}", instance);

            return instance;
        }

        public void ReleaseInstance<T>(T instance) where T : Component
        {
            if (ReferenceEquals(instance, null))
            {
                Log.Warning(LogCategory.Assets, "[InstanceRegistry] ReleaseInstance called with null instance.");
                return;
            }

            if (!_keyByInstance.TryGetValue(instance, out var key))
            {
                Log.Warning(LogCategory.Assets, $"[InstanceRegistry] Release of unregistered instance: {NameOf(instance)}");
                return;
            }

            Unregister(instance, key);

            Log.Info(LogCategory.Assets,
                $"[InstanceRegistry] DESPAWN {NameOf(instance)} of {key} — alive instances left: {AliveCountOf(key)}");

            if (instance != null)
                Object.Destroy(instance.gameObject);

            _cache.Release(key);
        }

        public void ReleaseAll<T>(CustomReferences<T> reference) where T : Component
        {
            var key = AssetKey.Of<T>(reference.AssetGUID);

            if (!_instancesByKey.Remove(key, out var list))
            {
                Log.Warning(LogCategory.Assets, $"[InstanceRegistry] ReleaseAll: nothing registered for {key}");
                return;
            }

            Log.Info(LogCategory.Assets, $"[InstanceRegistry] RELEASE ALL {key} — {list.Count} instance(s)");

            foreach (var instance in list)
            {
                if (!ReferenceEquals(instance, null))
                {
                    _keyByInstance.Remove(instance);

                    if (instance != null)
                        Object.Destroy(instance.gameObject);
                }

                _cache.Release(key);
            }
        }

        public void Dispose()
        {
            LogSnapshot("Dispose");

            foreach (var pair in _instancesByKey)
            {
                foreach (var instance in pair.Value)
                {
                    if (instance != null)
                        Object.Destroy(instance.gameObject);

                    _cache.Release(pair.Key);
                }
            }

            _instancesByKey.Clear();
            _keyByInstance.Clear();
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public void LogSnapshot(string reason)
        {
            if (_instancesByKey.Count == 0)
            {
                Log.Info(LogCategory.Assets, $"[InstanceRegistry] {reason}: no live instances — every instance was released by its owner");
                return;
            }

            var report = new StringBuilder();
            report.Append($"[InstanceRegistry] {reason}: instances of {_instancesByKey.Count} asset(s) are still alive — destroyed by Dispose, not by their owners:");

            foreach (var pair in _instancesByKey)
                report.Append($"\n    {pair.Key} x{pair.Value.Count}");

            Log.Warning(LogCategory.Assets, report.ToString());
        }

        private void Register(AssetKey key, Component instance)
        {
            _keyByInstance[instance] = key;

            if (!_instancesByKey.TryGetValue(key, out var list))
            {
                list = new List<Component>();
                _instancesByKey[key] = list;
            }

            list.Add(instance);
        }

        private void Unregister(Component instance, AssetKey key)
        {
            _keyByInstance.Remove(instance);

            if (!_instancesByKey.TryGetValue(key, out var list))
                return;

            list.Remove(instance);
            if (list.Count == 0)
                _instancesByKey.Remove(key);
        }

        private int AliveCountOf(AssetKey key)
        {
            return _instancesByKey.TryGetValue(key, out var list) ? list.Count : 0;
        }

        private static string NameOf(Component instance)
        {
            return instance == null ? "<destroyed>" : instance.name;
        }
        
        private sealed class ReferenceComparer : IEqualityComparer<Component>
        {
            public static readonly ReferenceComparer Instance = new ReferenceComparer();

            public bool Equals(Component left, Component right)
            {
                return ReferenceEquals(left, right);
            }

            public int GetHashCode(Component instance)
            {
                return RuntimeHelpers.GetHashCode(instance);
            }
        }
    }
}
