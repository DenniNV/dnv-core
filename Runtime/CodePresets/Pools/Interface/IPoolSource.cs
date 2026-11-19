using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Pools.Interface
{
    public interface IPoolSource<out TItem> where TItem : class
    {
        UniTask Preload(CancellationToken token);
        TItem Create(Transform root);
        void Release();
    }
}
