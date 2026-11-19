using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Sequence
{
    [Serializable]
    public class SimultaneousRunner : IAsyncSequenceRunner
    {
        [SerializeReference][SubclassSelector]
        private ISequence[] _sequences;
        
        public async UniTask Play(CancellationToken token)
        {
            var result = _sequences.Select(x => x.Play(token));
            await UniTask.WhenAll(result);
        }
    }
}