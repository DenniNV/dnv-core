using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Libraries.DNV.DependencyInjection.Context
{
    [Serializable]
    public class ProjectBuilder : IBaseBuilder
    {
        [SerializeField]
        [FormerlySerializedAs("_projectExecutor")]
        private ExecutionRoot _executionRoot;

        public async UniTask Build(CancellationToken token)
        {
            await _executionRoot.Build(token);
        }

        public void Dispose()
        {
            _executionRoot.Dispose();
        }
    }
}
