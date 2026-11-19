using System.Threading;
using Cysharp.Threading.Tasks;

namespace Libraries.DNV.CodePresets.Sequence
{
    public interface ISequenceAction
    {
        public UniTask DoAction(CancellationToken token);
    }
}