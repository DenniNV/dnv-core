using System;
using System.Collections.Generic;
using System.Threading;
using Libraries.DNV.CodePresets.State.Interfaces;
using Libraries.DNV.Logging;

namespace Libraries.DNV.CodePresets.State
{
    /// <summary>
    /// Synchronous transitions: Release of the old state and Activate of the new one happen within SwitchTo, in order.
    /// Each state gets its own token, cancelled before Release. A transition requested from Activate, Release or Execute
    /// is queued and run by the outermost call, so the machine is settled when SwitchTo returns.
    /// An exception in Release is logged and the transition goes on — broken cleanup must not freeze the game.
    /// An exception in Activate reaches the caller: the machine stays in the new state and the queue is dropped.
    /// Systems outside the machine follow it through <see cref="IStateHistory"/>: CurrentState, CurrentStateToken, StateChanged.
    /// </summary>
    public class StateSwitcher : IStateSwitcher
    {
        private readonly Dictionary<Type, IState> _registeredStates = new();
        private readonly Queue<IState> _pendingTransitions = new();

        private CancellationTokenSource _stateTokenSource;
        private bool _isBusy;

        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }
        public CancellationToken CurrentStateToken { get; private set; }

        public event Action<StateChange> StateChanged;

        public void Registration(IEnumerable<IState> states)
        {
            foreach (var state in states)
            {
                if(_registeredStates.TryAdd(state.GetType(), state) == false)
                {
                    throw new InvalidOperationException($"{state.GetType().Name} is already registered in the state machine");
                }
            }
        }

        public void SwitchTo<TState>() where TState : class, IState
        {
            _pendingTransitions.Enqueue(GetState<TState>());

            if(_isBusy)
            {
                return;
            }

            RunPendingTransitions();
        }

        public void Execute()
        {
            if(CurrentState is null || _isBusy)
            {
                return;
            }

            _isBusy = true;

            try
            {
                CurrentState.Execute();
            }
            catch
            {
                _pendingTransitions.Clear();
                throw;
            }
            finally
            {
                _isBusy = false;
            }

            RunPendingTransitions();
        }

        private void RunPendingTransitions()
        {
            if(_pendingTransitions.Count == 0)
            {
                return;
            }

            _isBusy = true;

            try
            {
                while (_pendingTransitions.Count > 0)
                {
                    Transition(_pendingTransitions.Dequeue());
                }
            }
            catch
            {
                _pendingTransitions.Clear();
                throw;
            }
            finally
            {
                _isBusy = false;
            }
        }

        private void Transition(IState newState)
        {
            if(CurrentState == newState)
            {
                throw new InvalidOperationException($"State machine is already in {newState.GetType().Name}");
            }

            var previous = CurrentState;

            ReleaseCurrent();

            _stateTokenSource = new CancellationTokenSource();
            CurrentStateToken = _stateTokenSource.Token;
            CurrentState = newState;

            try
            {
                newState.Activate(CurrentStateToken);
            }
            finally
            {
                NotifyStateChanged(new StateChange(previous, newState, CurrentStateToken));
            }
        }

        private void NotifyStateChanged(StateChange change)
        {
            var handlers = StateChanged;

            if(handlers is null)
            {
                return;
            }

            foreach (var handler in handlers.GetInvocationList())
            {
                try
                {
                    ((Action<StateChange>)handler)(change);
                }
                catch (Exception exception)
                {
                    var previousName = change.Previous?.GetType().Name ?? "none";
                    Log.Error(LogCategory.State, $"A StateChanged subscriber threw on {previousName} → {change.Current.GetType().Name}: {exception}");
                }
            }
        }

        private void ReleaseCurrent()
        {
            var state = CurrentState;

            if(state is null)
            {
                return;
            }

            try
            {
                CancelStateToken();
            }
            catch (Exception exception)
            {
                LogReleaseFailure(state, exception);
            }

            try
            {
                state.Release();
            }
            catch (Exception exception)
            {
                LogReleaseFailure(state, exception);
            }

            PreviousState = state;
        }

        private void CancelStateToken()
        {
            var source = _stateTokenSource;
            _stateTokenSource = null;

            if(source is null)
            {
                return;
            }

            try
            {
                source.Cancel();
            }
            finally
            {
                source.Dispose();
            }
        }

        private static void LogReleaseFailure(IState state, Exception exception)
        {
            Log.Error(LogCategory.State, $"{state.GetType().Name} failed while being released, the transition goes on: {exception}");
        }

        private TState GetState<TState>() where TState : class, IState
        {
            if(_registeredStates.TryGetValue(typeof(TState), out var state))
            {
                return (TState)state;
            }

            throw new InvalidOperationException($"{typeof(TState).Name} is not registered in the state machine");
        }
    }
}
