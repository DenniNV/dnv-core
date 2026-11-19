using System;
using System.Collections.Generic;
using System.Threading;
using Libraries.DNV.CodePresets.State;
using Libraries.DNV.CodePresets.State.Interfaces;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Libraries.DNV.Tests.State
{
    public sealed class StateSwitcherTests
    {
        private List<string> _journal;
        private StateSwitcher _switcher;
        private StateA _a;
        private StateB _b;
        private StateC _c;

        [SetUp]
        public void SetUp()
        {
            _journal = new List<string>();
            _a = new StateA(_journal);
            _b = new StateB(_journal);
            _c = new StateC(_journal);
            _switcher = new StateSwitcher();
            _switcher.Registration(new IState[] { _a, _b, _c });
        }

        [Test]
        public void SwitchTo_FromNothing_ActivatesState()
        {
            _switcher.SwitchTo<StateA>();

            Assert.AreSame(_a, _switcher.CurrentState);
            Assert.IsNull(_switcher.PreviousState);
            Assert.AreEqual(new[] { "A.Activate" }, _journal);
        }

        [Test]
        public void SwitchTo_AnotherState_ReleasesOldBeforeActivatingNew()
        {
            _switcher.SwitchTo<StateA>();
            _switcher.SwitchTo<StateB>();

            Assert.AreSame(_b, _switcher.CurrentState);
            Assert.AreSame(_a, _switcher.PreviousState);
            Assert.AreEqual(new[] { "A.Activate", "A.Release", "B.Activate" }, _journal);
        }

        [Test]
        public void SwitchTo_AnotherState_CancelsOldTokenOnlyAndBeforeRelease()
        {
            var cancelledInRelease = false;
            _a.OnRelease = () => cancelledInRelease = _a.Token.IsCancellationRequested;

            _switcher.SwitchTo<StateA>();
            _switcher.SwitchTo<StateB>();

            Assert.IsTrue(cancelledInRelease, "token must be cancelled before Release so Release can rely on it");
            Assert.IsTrue(_a.Token.IsCancellationRequested);
            Assert.IsFalse(_b.Token.IsCancellationRequested);
        }

        [Test]
        public void SwitchTo_RequestedFromActivate_RunsAfterActivateReturns()
        {
            _a.OnActivate = () => _switcher.SwitchTo<StateB>();

            _switcher.SwitchTo<StateA>();

            Assert.AreSame(_b, _switcher.CurrentState);
            Assert.AreEqual(new[] { "A.Activate", "A.Release", "B.Activate" }, _journal);
        }

        [Test]
        public void SwitchTo_RequestedFromActivate_StateFinishesActivateBeforeItIsReleased()
        {
            _a.OnActivate = () =>
            {
                _switcher.SwitchTo<StateB>();
                _journal.Add("A.ActivateTail");
            };

            _switcher.SwitchTo<StateA>();

            Assert.AreEqual(new[] { "A.Activate", "A.ActivateTail", "A.Release", "B.Activate" }, _journal);
        }

        [Test]
        public void SwitchTo_RequestedFromRelease_RunsAfterCurrentTransition()
        {
            _a.OnRelease = () => _switcher.SwitchTo<StateC>();

            _switcher.SwitchTo<StateA>();
            _switcher.SwitchTo<StateB>();

            Assert.AreSame(_c, _switcher.CurrentState);
            Assert.AreEqual(new[] { "A.Activate", "A.Release", "B.Activate", "B.Release", "C.Activate" }, _journal);
        }

        [Test]
        public void SwitchTo_ChainOfRequests_RunsInRequestOrder()
        {
            _a.OnActivate = () =>
            {
                _switcher.SwitchTo<StateB>();
                _switcher.SwitchTo<StateC>();
            };

            _switcher.SwitchTo<StateA>();

            Assert.AreEqual(new[] { "A.Activate", "A.Release", "B.Activate", "B.Release", "C.Activate" }, _journal);
        }

        [Test]
        public void Execute_OnlyRunsCurrentState()
        {
            _switcher.SwitchTo<StateA>();
            _switcher.Execute();

            Assert.AreEqual(new[] { "A.Activate", "A.Execute" }, _journal);
        }

        [Test]
        public void Execute_WithoutState_DoesNothing()
        {
            Assert.DoesNotThrow(() => _switcher.Execute());
        }

        [Test]
        public void SwitchTo_RequestedFromExecute_RunsAfterExecuteReturns()
        {
            _a.OnExecute = () =>
            {
                _switcher.SwitchTo<StateB>();
                _journal.Add("A.ExecuteTail");
            };

            _switcher.SwitchTo<StateA>();
            _switcher.Execute();

            Assert.AreEqual(new[] { "A.Activate", "A.Execute", "A.ExecuteTail", "A.Release", "B.Activate" }, _journal);
        }

        [Test]
        public void SwitchTo_ToCurrentState_Throws()
        {
            _switcher.SwitchTo<StateA>();

            Assert.Throws<InvalidOperationException>(() => _switcher.SwitchTo<StateA>());
            Assert.AreSame(_a, _switcher.CurrentState);
        }

        [Test]
        public void SwitchTo_UnregisteredState_Throws()
        {
            var switcher = new StateSwitcher();

            Assert.Throws<InvalidOperationException>(() => switcher.SwitchTo<StateA>());
        }

        [Test]
        public void Registration_SameTypeTwice_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => _switcher.Registration(new IState[] { new StateA(_journal) }));
        }

        [Test]
        public void SwitchTo_ReleaseThrows_IsLoggedAndTransitionContinues()
        {
            _a.OnRelease = () => throw new InvalidOperationException("broken cleanup");
            _switcher.SwitchTo<StateA>();

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("StateA failed while being released.*broken cleanup"));
            _switcher.SwitchTo<StateB>();

            Assert.AreSame(_b, _switcher.CurrentState);
            Assert.AreSame(_a, _switcher.PreviousState);
            Assert.AreEqual("B.Activate", _journal[^1]);
        }

        [Test]
        public void SwitchTo_ActivateThrows_ReachesCallerKeepsNewStateAndDropsQueue()
        {
            _b.OnActivate = () =>
            {
                _switcher.SwitchTo<StateC>();
                throw new InvalidOperationException("broken activate");
            };
            _switcher.SwitchTo<StateA>();

            Assert.Throws<InvalidOperationException>(() => _switcher.SwitchTo<StateB>());

            Assert.AreSame(_b, _switcher.CurrentState);
            CollectionAssert.DoesNotContain(_journal, "C.Activate");

            _switcher.SwitchTo<StateA>();
            Assert.AreSame(_a, _switcher.CurrentState, "the machine must stay usable after a failed Activate");
        }

        [Test]
        public void SwitchTo_ExecuteThrows_ReachesCallerAndDropsQueue()
        {
            _a.OnExecute = () =>
            {
                _switcher.SwitchTo<StateB>();
                throw new InvalidOperationException("broken execute");
            };
            _switcher.SwitchTo<StateA>();

            Assert.Throws<InvalidOperationException>(() => _switcher.Execute());

            Assert.AreSame(_a, _switcher.CurrentState);
            CollectionAssert.DoesNotContain(_journal, "B.Activate");
        }
    }
}
