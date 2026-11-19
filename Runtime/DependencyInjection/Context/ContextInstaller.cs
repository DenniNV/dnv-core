using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.DependencyInjection.Context.Interface;
using Libraries.DNV.DependencyInjection.Interfaces;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection.Context
{
    [Serializable]
    public class ContextInstaller : IContextInstaller
    {
        [SerializeReference]
        [SubclassSelector]
        private Resolver _resolver;

        [SerializeReference] [SubclassSelector]
        private List<IInstaller> _installer = new List<IInstaller>();

        private Context _context;
        
        public UniTask Install(CancellationToken token)
        {
            _context = new Context(_resolver, _installer).Install();
            return UniTask.CompletedTask;
        }
        
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}