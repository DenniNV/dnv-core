using System;
using System.Threading;

namespace Libraries.DNV.CodePresets.State.Interfaces
{
    /// <summary>
    /// The read side of a machine — what systems living outside of it get. <see cref="CurrentState"/> answers "while in the state",
    /// <see cref="StateChanged"/> answers "just entered / just left".
    /// </summary>
    public interface IStateHistory
    {
        IState CurrentState { get; }
        IState PreviousState { get; }

        /// <summary>
        /// Token of <see cref="CurrentState"/>; it is already cancelled while the state is being released.
        /// <see cref="CancellationToken.None"/> before the first transition.
        /// </summary>
        CancellationToken CurrentStateToken { get; }

        /// <summary>
        /// Raised synchronously inside the transition, after the new state's Activate returned — also when Activate threw,
        /// because <see cref="CurrentState"/> has changed anyway. Every state of a queued chain gets its own event.
        /// A throwing subscriber is logged and does not affect the others or the transition.
        /// </summary>
        event Action<StateChange> StateChanged;
    }
}
