using Libraries.DNV.CodePresets.Modules;
using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.Navigation
{
    public interface INavigationBrain : INavigationAgentHandler, INavigationRegenerator, IRunnable, IExecutable
    {
        public bool IsPathCompleted { get; }
        
    }
    
}