using System.Collections.Generic;
using Libraries.DNV.CodePresets.State;
using Libraries.DNV.CodePresets.State.Interfaces;
using Libraries.DNV.ExtendedUpdate;
using NUnit.Framework;

namespace Libraries.DNV.Tests.State
{
    public sealed class StateMachineTests
    {
        private List<string> _journal;
        private StateMachine _machine;
        private StateA _a;
        private StateB _b;

        [SetUp]
        public void SetUp()
        {
            _journal = new List<string>();
            _a = new StateA(_journal);
            _b = new StateB(_journal);
            _machine = new StateMachine();
            ((IStateRegistry)_machine).Registration(new IState[] { _a, _b });
        }

        [Test]
        public void Machines_AreIndependent()
        {
            var other = new StateMachine();
            var otherA = new StateA(_journal);
            ((IStateRegistry)other).Registration(new IState[] { otherA });

            _machine.SwitchTo<StateA>();
            other.SwitchTo<StateA>();
            _machine.SwitchTo<StateB>();

            Assert.AreSame(_b, _machine.CurrentState);
            Assert.AreSame(otherA, other.CurrentState);
        }

        [Test]
        public void StateChanged_IsForwardedAndCanBeUnsubscribed()
        {
            var changes = 0;
            void OnChanged(StateChange change) => changes++;

            _machine.StateChanged += OnChanged;
            _machine.SwitchTo<StateA>();
            _machine.StateChanged -= OnChanged;
            _machine.SwitchTo<StateB>();

            Assert.AreEqual(1, changes);
        }

        [Test]
        public void Execute_RunsTheCurrentState()
        {
            _machine.SwitchTo<StateA>();
            ((IExecutable)_machine).Execute();

            CollectionAssert.Contains(_journal, "A.Execute");
        }

        [Test]
        public void ReadSide_IsEnoughForSystemsOutsideTheMachine()
        {
            IStateHistory readOnly = _machine;
            _machine.SwitchTo<StateA>();

            Assert.AreSame(_a, readOnly.CurrentState);
            Assert.AreEqual(_a.Token, readOnly.CurrentStateToken);
        }
    }
}
