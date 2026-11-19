using System.Threading;

namespace Libraries.DNV.CodePresets.State.Interfaces
{
    /// <summary>
    /// One completed transition. <see cref="Token"/> belongs to <see cref="Current"/> and is cancelled when it is released:
    /// work a system starts for this state binds to it. <see cref="Previous"/> is null on the first transition.
    /// </summary>
    public readonly struct StateChange
    {
        public StateChange(IState previous, IState current, CancellationToken token)
        {
            Previous = previous;
            Current = current;
            Token = token;
        }

        public IState Previous { get; }
        public IState Current { get; }
        public CancellationToken Token { get; }
    }
}
