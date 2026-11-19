using System;
using Libraries.DNV.CodePresets.Pools.Interface;

namespace Libraries.DNV.CodePresets.Pools.Rules
{
    [Serializable]
    public sealed class ManualRule : IReturnRule<IPoolable>
    {
        public IReturnState<IPoolable> CreateState()
        {
            return State.Shared;
        }

        private sealed class State : IReturnState<IPoolable>
        {
            public static readonly State Shared = new State();

            public void OnRent(IPoolable item)
            {
            }

            public bool ShouldReturn(IPoolable item, float deltaTime)
            {
                return false;
            }
        }
    }
}
