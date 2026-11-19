using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Sequence
{
    [Serializable]
    public class ParticlePlayer : ISequenceAction
    {
        [SerializeReference]
        [SubclassSelector]
        private ISequenceAwaiter _awaiter;

        [SerializeField]
        private ParticleSystem _particle;
        
        public async UniTask DoAction(CancellationToken token)
        {
            await _awaiter.Wait(token);
            _particle.Play();
        }
    }
}