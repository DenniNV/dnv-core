using System.Threading;
using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.CodePresets.State.Interfaces
{
    /// <summary>
    /// State of a state machine. A transition completes within the call, in the same frame: the machine is never
    /// "between" states. <paramref name="stateToken"/> is cancelled when the state is released: long-running work
    /// started in Activate must be bound to it, and a continuation that asks for a transition must check it first.
    /// Release is synchronous — cleanup that has to be awaited belongs to the next state or to an owner outside the machine.
    /// </summary>
    public interface IState : IExecutable
    {
        void Activate(CancellationToken stateToken);
        void Release();
    }
}
