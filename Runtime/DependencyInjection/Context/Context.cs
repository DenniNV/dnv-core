using System;
using System.Collections.Generic;
using Libraries.DNV.DependencyInjection.Interfaces;

namespace Libraries.DNV.DependencyInjection.Context
{
    public class Context : IDisposable
    {
        private readonly Resolver _resolver;
        private readonly IEnumerable<IInstaller> _installers;

        public Context(Resolver resolver, IEnumerable<IInstaller> installers)
        {
            _resolver = resolver;
            _installers = installers;
        }

        public Context Install()
        {
            foreach (var installer in _installers)
            {
                installer.Bind(_resolver);
            }

            ResolverRegistry.Add(_resolver);
            return this;
        }
        

        public void Dispose()
        {
            ResolverRegistry.Remove(_resolver);
        }
    }
    
}