using System.Collections.Generic;

namespace Libraries.DNV.CodePresets.State.Interfaces
{
    public interface IStateRegistry
    {
        void Registration(IEnumerable<IState> states);
    }
}
