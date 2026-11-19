using System.Threading;
using Cysharp.Threading.Tasks;

namespace Libraries.DNV.CodePresets.Sequence
{
    public interface ISequenceAwaiter
    {
        public UniTask Wait(CancellationToken token);
    }
}