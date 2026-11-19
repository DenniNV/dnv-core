using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Libraries.DNV.DependencyInjection.Context.Interface
{
    public interface IContextInstaller : IDisposable
    {
        public UniTask Install(CancellationToken cancellationToken = default);
    }
}