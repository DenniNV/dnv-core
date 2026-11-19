using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Libraries.DNV.ContentManagerSystem.Parts
{
    [Serializable]
    public sealed class AddressableSpritePart : CmsEntityPart
    {
        [SerializeField]
        private AssetReferenceSprite _spriteReference;

        public override string PartName => "Addressable Sprite";
        public AssetReferenceSprite SpriteReference => _spriteReference;

        public async UniTask<Sprite> LoadAsync(IAssetProvider provider, CancellationToken token)
        {
            return await provider.LoadAsync<Sprite>(_spriteReference, token);
        }

        public void Release(IAssetProvider provider)
        {
            provider.Release(_spriteReference);
        }
    }
}
