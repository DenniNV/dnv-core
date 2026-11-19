using System;
using System.Collections.Generic;
using System.Threading;
using Libraries.DNV.CodePresets.State.Interfaces;
using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.CodePresets.State
{
    public abstract class BaseStateMachine : IStateMachine
    {
        private readonly IStateSwitcher _stateSwitcher = new StateSwitcher();

        public IState CurrentState => _stateSwitcher.CurrentState;
        public IState PreviousState => _stateSwitcher.PreviousState;
        public CancellationToken CurrentStateToken => _stateSwitcher.CurrentStateToken;

        public event Action<StateChange> StateChanged
        {
            add => _stateSwitcher.StateChanged += value;
            remove => _stateSwitcher.StateChanged -= value;
        }

        public void SwitchTo<TState>() where TState : class, IState
        {
            _stateSwitcher.SwitchTo<TState>();
        }

        void IStateRegistry.Registration(IEnumerable<IState> states)
        {
            _stateSwitcher.Registration(states);
        }

        void IExecutable.Execute()
        {
            _stateSwitcher.Execute();
        }

        public void LateExecute()
        {
        }
    }
}
