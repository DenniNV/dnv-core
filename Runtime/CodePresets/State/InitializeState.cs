using System.Threading;
using Libraries.DNV.CodePresets.State.Interfaces;
using Libraries.DNV.Core;
using Libraries.DNV.Logging;
using Libraries.DNV.MVC.Transition;

namespace Libraries.DNV.CodePresets.State
{
    public sealed class InitializeState : IState
    {
        private readonly IStateRunner _stateRunner;
        private readonly UIRoot _uiRoot;

        public InitializeState(IStateRunner stateRunner, UIRoot uiRoot)
        {
            _stateRunner = stateRunner;
            _uiRoot = uiRoot;
        }

        public void Activate(CancellationToken stateToken)
        {
            Log.Info(LogCategory.State, $"{nameof(InitializeState)} Activate");

            _uiRoot.Get<TransitionController>().Show();

            _stateRunner.SwitchTo<LoadGameSceneState>();
        }

        public void Release()
        {
        }

        public void Execute()
        {
        }
    }
}
