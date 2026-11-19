using System.Threading;
using Cysharp.Threading.Tasks;

namespace Libraries.DNV.CodePresets.Sequence
{
    public interface ISequence
    {
        public UniTask Play(CancellationToken token);
    }
}