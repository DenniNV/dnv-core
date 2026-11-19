using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Addressable;
using UnityEngine;

namespace Libraries.DNV.ContentManagerSystem.Parts
{
    [Serializable]
    public abstract class CustomAddressable<TPart> : CmsEntityPart where TPart : Component
    {
        [SerializeField] private CustomReferences<TPart> _customReferences;

        public override string PartName =>  $"Addressable Custom Part {typeof(TPart).Name}";

        public async UniTask<TPart> InstantiateAsync(IAssetProvider provider, SpawnValues spawnValues,  CancellationToken token)
        {
            return await provider.InstantiateAsync(_customReferences, spawnValues, token);
        }

        public void Release(IAssetProvider provider)
        {
            provider.ReleaseAndDestroyAll(_customReferences);
        }
    }
}
