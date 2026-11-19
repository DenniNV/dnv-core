using System.Threading;
using Cysharp.Threading.Tasks;

namespace Libraries.DNV.CodePresets.Sequence
{
    public interface IAsyncSequenceRunner
    {
        UniTask Play(CancellationToken token);
    }
}