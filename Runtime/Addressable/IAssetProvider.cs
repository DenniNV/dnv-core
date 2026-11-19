using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Libraries.DNV.Addressable
{
    public interface IAssetProvider : IDisposable
    {
        UniTask<T> LoadAsync<T>(AssetReferenceT<T> reference, CancellationToken token) where T : Object;
        void Release<T>(AssetReferenceT<T> reference) where T : Object;

        UniTask<T> LoadPrefabAsync<T>(CustomReferences<T> reference, CancellationToken token) where T : Component;
        void ReleasePrefab<T>(CustomReferences<T> reference) where T : Component;

        UniTask<T> InstantiateAsync<T>(CustomReferences<T> reference, SpawnValues values, CancellationToken token)
            where T : Component;
        void ReleaseAndDestroy<T>(T instance) where T : Component;
        void ReleaseAndDestroyAll<T>(CustomReferences<T> reference) where T : Component;
    }
}
