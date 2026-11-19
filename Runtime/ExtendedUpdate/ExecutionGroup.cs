using System.Collections.Generic;

namespace Libraries.DNV.ExtendedUpdate
{
    public abstract class ExecutionGroup : IExecutable, ILateExecutable, IFixedExecutable
    {
        private readonly List<IExecutable> _executables = new List<IExecutable>();
        private readonly List<ILateExecutable> _lateExecutables = new List<ILateExecutable>();
        private readonly List<IFixedExecutable> _fixedExecutables = new List<IFixedExecutable>();

        private bool _isInPause;

        public void Pause()
        {
            _isInPause = true;
        }

        public void Resume()
        {
            _isInPause = false;
        }

        public void Add(IExecutionTarget target)
        {
            AddTo(_executables, target);
            AddTo(_lateExecutables, target);
            AddTo(_fixedExecutables, target);
        }

        public void Remove(IExecutionTarget target)
        {
            RemoveFrom(_executables, target);
            RemoveFrom(_lateExecutables, target);
            RemoveFrom(_fixedExecutables, target);
        }

        public void Execute()
        {
            if(_isInPause)
            {
                return;
            }

            for (var i = _executables.Count - 1; i >= 0; i--)
            {
                _executables[i].Execute();
            }
        }

        public void LateExecute()
        {
            if(_isInPause)
            {
                return;
            }

            for (var i = _lateExecutables.Count - 1; i >= 0; i--)
            {
                _lateExecutables[i].LateExecute();
            }
        }

        public void FixedExecute()
        {
            if(_isInPause)
            {
                return;
            }

            for (var i = _fixedExecutables.Count - 1; i >= 0; i--)
            {
                _fixedExecutables[i].FixedExecute();
            }
        }

        private static void AddTo<TPhase>(List<TPhase> phase, IExecutionTarget target) where TPhase : class, IExecutionTarget
        {
            if(target is not TPhase typed || phase.Contains(typed))
            {
                return;
            }

            phase.Add(typed);
        }

        private static void RemoveFrom<TPhase>(List<TPhase> phase, IExecutionTarget target) where TPhase : class, IExecutionTarget
        {
            if(target is not TPhase typed)
            {
                return;
            }

            phase.Remove(typed);
        }
    }
}
