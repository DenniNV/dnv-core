using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Sequence
{
    [Serializable]
    public class SequenceContainer : ISequence
    {
        [SerializeReference]
        [SubclassSelector]
        private ISequenceAction[] _actions;
        
        public async UniTask Play(CancellationToken token)
        {
            foreach (var action in _actions)
            {
                await action.DoAction(token);
            }
        }
    }
}