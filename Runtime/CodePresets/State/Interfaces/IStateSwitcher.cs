using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.CodePresets.State.Interfaces
{
    public interface IStateSwitcher : IStateRegistry, IStateHistory, IStateRunner, IExecutable
    {
    }
}
