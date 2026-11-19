using System;
using System.Collections.Generic;
using System.Threading;
using Libraries.DNV.CodePresets.State.Interfaces;

namespace Libraries.DNV.Tests.State
{
    internal abstract class RecordingState : IState
    {
        private readonly List<string> _journal;
        private readonly string _name;

        protected RecordingState(List<string> journal, string name)
        {
            _journal = journal;
            _name = name;
        }

        public CancellationToken Token { get; private set; }
        public Action OnActivate { get; set; }
        public Action OnRelease { get; set; }
        public Action OnExecute { get; set; }

        public void Activate(CancellationToken stateToken)
        {
            Token = stateToken;
            _journal.Add(_name + ".Activate");
            OnActivate?.Invoke();
        }

        public void Release()
        {
            _journal.Add(_name + ".Release");
            OnRelease?.Invoke();
        }

        public void Execute()
        {
            _journal.Add(_name + ".Execute");
            OnExecute?.Invoke();
        }
    }

    internal sealed class StateA : RecordingState
    {
        public StateA(List<string> journal) : base(journal, "A") { }
    }

    internal sealed class StateB : RecordingState
    {
        public StateB(List<string> journal) : base(journal, "B") { }
    }

    internal sealed class StateC : RecordingState
    {
        public StateC(List<string> journal) : base(journal, "C") { }
    }
}
