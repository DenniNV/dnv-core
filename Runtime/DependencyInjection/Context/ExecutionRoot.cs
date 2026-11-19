using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.CodePresets;
using Libraries.DNV.ExtendedUpdate;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection.Context
{
    internal sealed class ExecutionRoot : MonoBehaviour, IAsyncBuilder
    {
        private ExecutionPipeline _pipeline;

        public UniTask Build(CancellationToken token)
        {
            _pipeline = Injector.Get<ExecutionPipeline>();

            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _pipeline = null;
        }

        private void Update()
        {
            _pipeline?.Execute();
        }

        private void LateUpdate()
        {
            _pipeline?.LateExecute();
        }

        private void FixedUpdate()
        {
            _pipeline?.FixedExecute();
        }
    }
}
