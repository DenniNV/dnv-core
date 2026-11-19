using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Libraries.DNV.CodePresets.State;
using Libraries.DNV.CodePresets.State.Interfaces;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Libraries.DNV.Tests.State
{
    public sealed class StateChangedTests
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
            _switcher.StateChanged += change => _journal.Add($"Changed:{Name(change.Previous)}->{Name(change.Current)}");
        }

        [Test]
        public void StateChanged_FirstTransition_HasNoPrevious()
        {
            _switcher.SwitchTo<StateA>();

            Assert.AreEqual(new[] { "A.Activate", "Changed:none->A" }, _journal);
        }

        [Test]
        public void StateChanged_IsRaisedAfterReleaseAndActivate()
        {
            _switcher.SwitchTo<StateA>();
            _switcher.SwitchTo<StateB>();

            Assert.AreEqual(new[] { "A.Activate", "Changed:none->A", "A.Release", "B.Activate", "Changed:A->B" }, _journal);
        }

        [Test]
        public void StateChanged_CarriesTokenOfNewStateOnly()
        {
            StateChange received = default;
            _switcher.StateChanged += change => received = change;

            _switcher.SwitchTo<StateA>();
            var tokenOfA = received.Token;
            _switcher.SwitchTo<StateB>();

            Assert.AreEqual(_b.Token, received.Token);
            Assert.IsFalse(received.Token.IsCancellationRequested);
            Assert.IsTrue(tokenOfA.IsCancellationRequested, "the token of the released state must be cancelled");
        }

        [Test]
        public void CurrentStateToken_FollowsCurrentState()
        {
            Assert.AreEqual(CancellationToken.None, _switcher.CurrentStateToken);

            _switcher.SwitchTo<StateA>();
            var tokenOfA = _switcher.CurrentStateToken;
            _switcher.SwitchTo<StateB>();

            Assert.AreEqual(_a.Token, tokenOfA);
            Assert.AreEqual(_b.Token, _switcher.CurrentStateToken);
            Assert.IsTrue(tokenOfA.IsCancellationRequested);
            Assert.IsFalse(_switcher.CurrentStateToken.IsCancellationRequested);
        }

        [Test]
        public void CurrentStateToken_DuringRelease_IsTheCancelledTokenOfTheReleasedState()
        {
            var cancelledInRelease = false;
            var stillCurrent = false;
            _a.OnRelease = () =>
            {
                cancelledInRelease = _switcher.CurrentStateToken.IsCancellationRequested;
                stillCurrent = _switcher.CurrentState == _a;
            };

            _switcher.SwitchTo<StateA>();
            _switcher.SwitchTo<StateB>();

            Assert.IsTrue(cancelledInRelease);
            Assert.IsTrue(stillCurrent);
        }

        [Test]
        public void StateChanged_SubscriberSeesSettledCurrentState()
        {
            IState seen = null;
            _switcher.StateChanged += change => seen = _switcher.CurrentState;

            _switcher.SwitchTo<StateA>();

            Assert.AreSame(_a, seen);
        }

        [Test]
        public void StateChanged_ChainOfRequests_RaisesAnEventForEveryState()
        {
            _a.OnActivate = () =>
            {
                _switcher.SwitchTo<StateB>();
                _switcher.SwitchTo<StateC>();
            };

            _switcher.SwitchTo<StateA>();

            Assert.AreEqual(new[]
            {
                "A.Activate", "Changed:none->A",
                "A.Release", "B.Activate", "Changed:A->B",
                "B.Release", "C.Activate", "Changed:B->C",
            }, _journal);
        }

        [Test]
        public void StateChanged_RequestFromSubscriber_IsQueuedAfterTheCurrentEvent()
        {
            var otherSubscriberSaw = string.Empty;
            _switcher.StateChanged += change =>
            {
                if(change.Current == _a)
                {
                    _switcher.SwitchTo<StateB>();
                }
            };
            _switcher.StateChanged += change => otherSubscriberSaw += Name(change.Current);

            _switcher.SwitchTo<StateA>();

            Assert.AreSame(_b, _switcher.CurrentState);
            Assert.AreEqual("AB", otherSubscriberSaw, "the second subscriber must get A before the queued transition to B runs");
        }

        [Test]
        public void StateChanged_SubscriberThrows_IsLoggedAndOthersAndTransitionAreNotAffected()
        {
            var reached = false;
            _switcher.StateChanged += _ => throw new InvalidOperationException("broken subscriber");
            _switcher.StateChanged += _ => reached = true;

            LogAssert.Expect(LogType.Error, new Regex("StateChanged subscriber threw on none .* StateA.*broken subscriber", RegexOptions.Singleline));
            _switcher.SwitchTo<StateA>();

            Assert.IsTrue(reached);
            Assert.AreSame(_a, _switcher.CurrentState);
        }

        [Test]
        public void StateChanged_ActivateThrows_IsStillRaisedBecauseCurrentStateChanged()
        {
            _a.OnActivate = () => throw new InvalidOperationException("broken activate");

            Assert.Throws<InvalidOperationException>(() => _switcher.SwitchTo<StateA>());

            Assert.AreSame(_a, _switcher.CurrentState);
            CollectionAssert.Contains(_journal, "Changed:none->A");
        }

        [Test]
        public void StateChanged_Unsubscribed_IsNotCalled()
        {
            var calls = 0;
            Action<StateChange> handler = _ => calls++;
            _switcher.StateChanged += handler;
            _switcher.StateChanged -= handler;

            _switcher.SwitchTo<StateA>();

            Assert.AreEqual(0, calls);
        }

        private static string Name(IState state)
        {
            return state switch
            {
                null => "none",
                StateA => "A",
                StateB => "B",
                StateC => "C",
                _ => state.GetType().Name,
            };
        }
    }
}
