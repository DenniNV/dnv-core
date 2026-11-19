using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection.Context
{
    [Serializable]
    public class SceneBuilder : IBaseBuilder
    {
        [SerializeField] 
        private UnitContext[] _contexts;
        
        public void Dispose()
        {
        }

        public async UniTask Build(CancellationToken token)
        {
            foreach (var context in _contexts)
            {
                await context.Build(token);
            }
        }
    }
}