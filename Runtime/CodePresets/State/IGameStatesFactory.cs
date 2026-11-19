using System.Collections.Generic;
using Libraries.DNV.CodePresets.State.Interfaces;

namespace Libraries.DNV.CodePresets.State
{
    /// <summary>
    /// Builds the states of the global GameStateMachine. Chosen on EntryPoint via [SerializeReference],
    /// so a game supplies its own flow without touching the library. Must include <see cref="InitializeState"/>.
    /// </summary>
    public interface IGameStatesFactory
    {
        IEnumerable<IState> Create(IStateRunner gameStateMachine);
    }
}
