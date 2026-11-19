using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Addressable;
using Libraries.DNV.CodePresets;
using Libraries.DNV.DependencyInjection;
using UnityEngine;

namespace Libraries.DNV.Core
{
    public class ProjectDisposer : MonoBehaviour, IDisposable , IAsyncBuilder
    {
        private IAssetProvider _assetProvider;
        private bool _disposed;

        public UniTask Build(CancellationToken token)
        {
            _assetProvider = Injector.Get<IAssetProvider>();
            return  UniTask.CompletedTask;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed || _assetProvider == null)
                return;
            _disposed = true;

            _assetProvider.Dispose();
        }
    }
}