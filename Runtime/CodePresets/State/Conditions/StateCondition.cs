using Libraries.DNV.CodePresets.Rules.Interfaces;
using Libraries.DNV.CodePresets.State.Interfaces;

namespace Libraries.DNV.CodePresets.State.Conditions
{
    public class StateCondition<TState> : ICondition where TState : class, IState
    {
        private readonly IStateHistory _stateHistory;

        public bool IsExecuted => _stateHistory.CurrentState is TState;

        public StateCondition(IStateHistory stateHistory)
        {
            _stateHistory = stateHistory;
        }

        public void Reset()
        {
        }
    }

    public class NotInStateCondition<TState> : ICondition where TState : class, IState
    {
        private readonly IStateHistory _stateHistory;

        public bool IsExecuted => _stateHistory.CurrentState is not TState;

        public NotInStateCondition(IStateHistory stateHistory)
        {
            _stateHistory = stateHistory;
        }

        public void Reset()
        {
        }
    }
}
