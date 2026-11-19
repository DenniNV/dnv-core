using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.DependencyInjection.Context;

namespace Libraries.DNV.CodePresets.Factory
{
    public interface IAsyncFactory<TItems, in TContext> where TContext : struct where TItems : class
    {
        public UniTask<TItems> Create(TContext context, CancellationToken cancellationToken);
    }
}