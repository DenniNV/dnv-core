using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.CodePresets.State.Interfaces
{
    public interface IStateMachine : IStateHistory, IStateRegistry, IStateRunner, IExecutable, ILateExecutable
    {
    }
    
}