using System.Threading;
using Libraries.DNV.CodePresets.State.Interfaces;

namespace Libraries.DNV.CodePresets.State
{
    /// <summary>
    /// The scene is loaded and built. A game replaces or extends this through its IGameStatesFactory.
    /// </summary>
    public sealed class InGameState : IState
    {
        public void Activate(CancellationToken stateToken)
        {
        }

        public void Release()
        {
        }

        public void Execute()
        {
        }
    }
}
