using System.Threading;

namespace Libraries.DNV.CodePresets.State.Interfaces
{
    public abstract class BaseState : IState
    {
        private readonly IBehaviorOfState _behaviorOfState;

        protected BaseState(IBehaviorOfState behaviorOfState)
        {
            _behaviorOfState = behaviorOfState;
        }

        public void Execute()
        {
            _behaviorOfState.Execute();
        }

        public void Activate(CancellationToken stateToken)
        {
            _behaviorOfState.Run(stateToken);
        }

        public void Release()
        {
            _behaviorOfState.Resolve();
        }
    }
}
