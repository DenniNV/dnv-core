using System.Threading;
using Cysharp.Threading.Tasks;

namespace Libraries.DNV.CodePresets
{
    public interface IAsyncBuilder
    {
        public UniTask Build(CancellationToken token);
    }
}