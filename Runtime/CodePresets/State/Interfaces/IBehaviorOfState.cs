using System.Threading;
using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.CodePresets.State.Interfaces
{
    /// <summary>
    /// Logic of a <see cref="BaseState"/>. Run and Resolve are synchronous; anything that outlives Run
    /// (awaits, delays) must use <paramref name="stateToken"/>, which is cancelled when the state is exited.
    /// </summary>
    public interface IBehaviorOfState : IExecutable
    {
        void Run(CancellationToken stateToken);
        void Resolve();
    }
}
