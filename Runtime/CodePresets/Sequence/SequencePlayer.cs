using System;
using Cysharp.Threading.Tasks;
using Libraries.DNV.CodePresets.Rules.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Libraries.DNV.CodePresets.Sequence
{
    public class SequencePlayer : MonoBehaviour
    {
        [SerializeReference]
        [SubclassSelector]
        private IAsyncSequenceRunner _asyncSequenceRunner;

        [SerializeReference]
        [SubclassSelector]
        private ICondition[] _rules;


        public void Update()
        {
            foreach (var rule in _rules)
            {
                if (!rule.IsExecuted)
                {
                    return;
                }
            }

            _asyncSequenceRunner.Play(destroyCancellationToken).Forget();
        }
    }

    [Serializable]
    public class F1 : ICondition
    {
        public bool IsExecuted => Keyboard.current.f1Key.wasPressedThisFrame;
        
        public void Reset()
        {
            
        }
    }
    
}