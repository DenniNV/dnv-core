using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Libraries.DNV.ContentManagerSystem.Parts
{
    [Serializable]
    public class AddressablePrefabPart : CmsEntityPart
    {
        [SerializeField] private AssetReferenceGameObject _prefabReference;

        public override string PartName => "Addressable Prefab";
        public AssetReferenceGameObject PrefabReference => _prefabReference;

        public async UniTask<GameObject> LoadAsync(IAssetProvider provider, CancellationToken token)
        {
            return await provider.LoadAsync<GameObject>(_prefabReference, token);
        }

        public void Release(IAssetProvider provider)
        {
            provider.Release(_prefabReference);
        }
    }
}
