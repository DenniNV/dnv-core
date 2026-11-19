using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.DependencyInjection.Context.Interface;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection.Context
{
    [Serializable]
    public class ContextBuilder : IContextBuilder
    {
        [SerializeReference] [SubclassSelector]
        private List<IBaseBuilder> _baseBuilders = new List<IBaseBuilder>();

        public async UniTask Build(CancellationToken token)
        {
            foreach (var builder in _baseBuilders)
            {
                await builder.Build(token);
            }
        }
        
        public void Dispose()
        {
            foreach (var baseBuilder in _baseBuilders)
            {
                baseBuilder.Dispose();
            }
        }
    }
}