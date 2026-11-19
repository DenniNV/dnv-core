using Libraries.DNV.ExtendedUpdate;

namespace Libraries.DNV.DependencyInjection
{
    public class ProjectExecutionGroup : ExecutionGroup{}

    /// The decide phase, <see cref="ExecutionOrder.Brain"/>: owners of state machines (the only ones calling SwitchTo) run here.
    public class BrainExecutionGroup : ExecutionGroup{}

    public class PoolExecutionGroup : ExecutionGroup{}
}
