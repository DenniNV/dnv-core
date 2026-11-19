using System;
using Libraries.DNV.CodePresets.Pools.Interface;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Pools.Rules
{
    [Serializable]
    public sealed class DurationRule : IReturnRule<IPoolable>
    {
        [SerializeField] [Min(0f)]
        private float _duration = 1f;

        public IReturnState<IPoolable> CreateState()
        {
            return new State(_duration);
        }

        private sealed class State : IReturnState<IPoolable>
        {
            private readonly float _duration;

            private float _elapsed;

            public State(float duration)
            {
                _duration = duration;
            }

            public void OnRent(IPoolable item)
            {
                _elapsed = 0f;
            }

            public bool ShouldReturn(IPoolable item, float deltaTime)
            {
                _elapsed += deltaTime;

                return _elapsed >= _duration;
            }
        }
    }
}
