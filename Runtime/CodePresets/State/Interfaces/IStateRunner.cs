namespace Libraries.DNV.CodePresets.State.Interfaces
{
    public interface IStateRunner
    {
        /// <summary>
        /// Releases the current state and activates <typeparamref name="TState"/>. A request made while a transition
        /// (or a state's Execute) is running is queued and runs right after it, so Activate and Release never nest.
        /// </summary>
        void SwitchTo<TState>() where TState : class, IState;
    }
}
