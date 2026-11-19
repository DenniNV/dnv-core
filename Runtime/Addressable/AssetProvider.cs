using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Libraries.DNV.Addressable
{
    public sealed class AssetProvider : IAssetProvider
    {
        private readonly AssetCache _cache;
        private readonly InstanceRegistry _instances;

        public AssetProvider()
        {
            _cache = new AssetCache();
            _instances = new InstanceRegistry(_cache);
        }

        public UniTask<T> LoadAsync<T>(AssetReferenceT<T> reference, CancellationToken token) where T : Object
        {
            if (ReferenceEquals(reference, null))
                throw new ArgumentNullException(nameof(reference));

            var key = AssetKey.Of<T>(reference.AssetGUID);
            LogCall(nameof(LoadAsync), key);

            return _cache.LoadAsync(key, reference.LoadAssetAsync, token);
        }

        public void Release<T>(AssetReferenceT<T> reference) where T : Object
        {
            if (ReferenceEquals(reference, null))
                throw new ArgumentNullException(nameof(reference));

            var key = AssetKey.Of<T>(reference.AssetGUID);
            LogCall(nameof(Release), key);

            _cache.Release(key);
        }

        public UniTask<T> LoadPrefabAsync<T>(CustomReferences<T> reference, CancellationToken token)
            where T : Component
        {
            if (ReferenceEquals(reference, null))
                throw new ArgumentNullException(nameof(reference));

            var key = AssetKey.Of<T>(reference.AssetGUID);
            LogCall(nameof(LoadPrefabAsync), key);

            return _cache.LoadAsync(key, reference.LoadAssetAsync, token);
        }

        public void ReleasePrefab<T>(CustomReferences<T> reference) where T : Component
        {
            if (ReferenceEquals(reference, null))
                throw new ArgumentNullException(nameof(reference));

            var key = AssetKey.Of<T>(reference.AssetGUID);
            LogCall(nameof(ReleasePrefab), key);

            _cache.Release(key);
        }

        public UniTask<T> InstantiateAsync<T>(CustomReferences<T> reference, SpawnValues values, CancellationToken token)
            where T : Component
        {
            if (ReferenceEquals(reference, null))
                throw new ArgumentNullException(nameof(reference));

            LogCall(nameof(InstantiateAsync), AssetKey.Of<T>(reference.AssetGUID));

            return _instances.InstantiateAsync(reference, values, token);
        }

        public void ReleaseAndDestroy<T>(T instance) where T : Component
        {
            Log.Info(LogCategory.Assets, $"[AssetProvider] {nameof(ReleaseAndDestroy)}<{typeof(T).Name}>");

            _instances.ReleaseInstance(instance);
        }

        public void ReleaseAndDestroyAll<T>(CustomReferences<T> reference) where T : Component
        {
            if (ReferenceEquals(reference, null))
                throw new ArgumentNullException(nameof(reference));

            LogCall(nameof(ReleaseAndDestroyAll), AssetKey.Of<T>(reference.AssetGUID));

            _instances.ReleaseAll(reference);
        }

        public void Dispose()
        {
            Log.Info(LogCategory.Assets, "[AssetProvider] Dispose — final memory report below");

            _instances.Dispose();
            _cache.Dispose();
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        private static void LogCall(string operation, AssetKey key)
        {
            Log.Info(LogCategory.Assets, $"[AssetProvider] {operation} {key}");
        }
    }
}
