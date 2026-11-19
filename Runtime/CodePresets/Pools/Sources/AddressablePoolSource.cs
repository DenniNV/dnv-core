using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Addressable;
using Libraries.DNV.CodePresets.Pools.Interface;
using Libraries.DNV.DependencyInjection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Libraries.DNV.CodePresets.Pools.Sources
{
    [Serializable]
    public abstract class AddressablePoolSource<TItem> : IPoolSource<TItem>
        where TItem : Component, IPoolable
    {
        [SerializeField]
        private CustomReferences<TItem> _reference;

        private TItem _prefab;

        public async UniTask Preload(CancellationToken token)
        {
            _prefab = await Injector.Get<IAssetProvider>().LoadPrefabAsync(_reference, token);
        }

        public TItem Create(Transform root)
        {
            return _prefab == null ? throw new InvalidOperationException($"{GetType().Name} is used before Preload.") : Object.Instantiate(_prefab, root);
        }

        public void Release()
        {
            if(_prefab == null)
            {
                return;
            }

            _prefab = null;
            Injector.Get<IAssetProvider>().ReleasePrefab(_reference);
        }
    }
}
