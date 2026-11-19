using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.CodePresets.Pools;
using Libraries.DNV.CodePresets.Pools.Interface;
using NUnit.Framework;

namespace Libraries.DNV.Tests.Pools
{
    public sealed class PoolProviderTests
    {
        private PoolProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new PoolProvider();
        }

        [Test]
        public void Contains_FollowsAddAndRemove()
        {
            var pool = new StubPool("bullet");

            Assert.IsFalse(_provider.Contains("bullet"));

            _provider.Add(pool);
            Assert.IsTrue(_provider.Contains("bullet"));

            _provider.Remove(pool);
            Assert.IsFalse(_provider.Contains("bullet"));
        }

        [Test]
        public void Add_SameIdTwice_Throws()
        {
            _provider.Add(new StubPool("bullet"));

            Assert.Throws<ArgumentException>(() => _provider.Add(new StubPool("bullet")));
        }

        [Test]
        public void Get_UnregisteredId_ThrowsAndListsRegisteredPools()
        {
            _provider.Add(new StubPool("bullet"));

            var exception = Assert.Throws<KeyNotFoundException>(() => _provider.Get<object>("missing"));

            StringAssert.Contains("bullet", exception.Message);
        }

        [Test]
        public void Execute_RunsEveryAddedPoolAndSkipsRemoved()
        {
            var kept = new StubPool("kept");
            var removed = new StubPool("removed");
            _provider.Add(kept);
            _provider.Add(removed);
            _provider.Remove(removed);

            _provider.Execute();

            Assert.AreEqual(1, kept.Executions);
            Assert.AreEqual(0, removed.Executions);
        }

        private sealed class StubPool : IPool
        {
            public StubPool(string id)
            {
                Id = id;
            }

            public string Id { get; }
            public int Executions { get; private set; }

            public UniTask Warmup(CancellationToken token)
            {
                return UniTask.CompletedTask;
            }

            public void Execute()
            {
                Executions++;
            }

            public void Dispose()
            {
            }
        }
    }
}
