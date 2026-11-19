using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.CodePresets;
using Libraries.DNV.DependencyInjection.Context.Interface;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection.Context
{
    /// <summary>
    /// Shared body of every context: install the scope, build it, and release it when the object goes away.
    /// Field names are serialized in prefabs and scenes — keep them unchanged.
    /// </summary>
    public abstract class ContextContainer : AutoComponent, IAsyncBuilder
    {
        [SerializeReference] [SubclassSelector]
        private IContextInstaller _contextInstaller;

        [SerializeReference] [SubclassSelector]
        private IContextBuilder _contextBuilder;

        public async UniTask Build(CancellationToken token)
        {
            await _contextInstaller.Install(token);
            await _contextBuilder.Build(token);
        }

        private void OnDestroy()
        {
            _contextInstaller?.Dispose();
            _contextBuilder?.Dispose();
        }
    }
}
