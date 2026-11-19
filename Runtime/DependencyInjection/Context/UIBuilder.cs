using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Core;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection.Context
{
    [Serializable]
    public class UIBuilder : IBaseBuilder
    {
        [SerializeField]
        private UIRoot _uiRoot;
        
        public async UniTask Build(CancellationToken token)
        {
            await _uiRoot.Build(token);
        }
        
        public void Dispose()
        {
            _uiRoot.Dispose();
        }
    }
}