using System.Collections.Generic;
using Libraries.DNV.ExtendedUpdate;
using NUnit.Framework;

namespace Libraries.DNV.Tests.Execution
{
    public sealed class ExecutionPipelineTests
    {
        private List<string> _journal;
        private ExecutionPipeline _pipeline;

        [SetUp]
        public void SetUp()
        {
            _journal = new List<string>();
            _pipeline = new ExecutionPipeline();
        }

        [Test]
        public void Orders_FollowDecideThenAct()
        {
            Assert.Less(ExecutionOrder.Project, ExecutionOrder.Brain);
            Assert.Less(ExecutionOrder.Brain, ExecutionOrder.Default);
            Assert.Less(ExecutionOrder.Default, ExecutionOrder.Pool);
        }

        [Test]
        public void Execute_BrainGroupRunsBeforeDefaultEvenIfAddedLater()
        {
            AddGroup("system", ExecutionOrder.Default);
            AddGroup("brain", ExecutionOrder.Brain);
            AddGroup("pool", ExecutionOrder.Pool);
            AddGroup("project", ExecutionOrder.Project);

            _pipeline.Execute();

            Assert.AreEqual(new[] { "project", "brain", "system", "pool" }, _journal);
        }

        [Test]
        public void Execute_GroupsWithEqualOrder_KeepTheOrderTheyWereAddedIn()
        {
            AddGroup("first", ExecutionOrder.Default);
            AddGroup("second", ExecutionOrder.Default);

            _pipeline.Execute();

            Assert.AreEqual(new[] { "first", "second" }, _journal);
        }

        private void AddGroup(string name, int order)
        {
            var group = new RecordingGroup();
            group.Add(new RecordingExecutable(_journal, name));
            _pipeline.Add(group, order);
        }

        private sealed class RecordingGroup : ExecutionGroup
        {
        }

        private sealed class RecordingExecutable : IExecutable
        {
            private readonly List<string> _journal;
            private readonly string _name;

            public RecordingExecutable(List<string> journal, string name)
            {
                _journal = journal;
                _name = name;
            }

            public void Execute()
            {
                _journal.Add(_name);
            }
        }
    }
}
