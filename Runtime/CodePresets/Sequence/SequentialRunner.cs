using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Sequence
{
    [Serializable]
    public class SequentialRunner : IAsyncSequenceRunner
    {
        [SerializeReference][SubclassSelector]
        private ISequence[] _sequences;
        
        public async UniTask Play(CancellationToken token)
        {
            foreach (var sequence in _sequences)
            {
                await sequence.Play(token);
            }
        }
    }
}