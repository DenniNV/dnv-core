using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.CodePresets.Pools.Interface
{
    public interface IPool : IExecutable, IDisposable
    {
        string Id { get; }
        UniTask Warmup(CancellationToken token);
    }

    public interface IPool<TItem> : IPool, IPoolRent<TItem> where TItem : class
    {
        void Return(TItem item);
    }
}
