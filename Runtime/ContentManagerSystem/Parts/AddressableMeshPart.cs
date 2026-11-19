using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Libraries.DNV.ContentManagerSystem.Parts
{
    [Serializable]
    public class AddressableMeshPart : CmsEntityPart
    {
        [SerializeField] private AssetReferenceT<Mesh> _meshReference;

        public override string PartName => "Addressable Mesh";
        public AssetReferenceT<Mesh> MeshReference => _meshReference;

        public async UniTask<Mesh> LoadAsync(IAssetProvider provider, CancellationToken token)
        {
            return await provider.LoadAsync<Mesh>(_meshReference, token);
        }

        public void Release(IAssetProvider provider)
        {
            provider.Release(_meshReference);
        }
    }
}
