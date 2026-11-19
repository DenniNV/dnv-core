using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.CodePresets.Pools.Config;
using Libraries.DNV.CodePresets.Pools.Interface;
using Libraries.DNV.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Libraries.DNV.CodePresets.Pools
{
    public sealed class Pool<TItem> : IPool<TItem> where TItem : Component, IPoolable
    {
        private const int WarmupBatchSize = 8;

        private readonly ItemGroup[] _itemGroups;
        private readonly ISourceSelector _selector;
        private readonly List<PoolItem> _active = new List<PoolItem>();
        private readonly Transform _root;
        private readonly int _maxCount;
        private readonly int _initialCount;

        private bool _isDisposed;

        public string Id { get; }

        public Pool(string id, PoolCapacity capacity, IReadOnlyList<SegmentDefinition<TItem>> definitions,
            ISourceSelector selector, Transform parent)
        {
            if(definitions == null || definitions.Count == 0)
            {
                throw new ArgumentException($"Pool '{id}' has no segments.", nameof(definitions));
            }

            Id = id;
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _maxCount = Mathf.Max(1, capacity.MaxCount);
            _initialCount = Mathf.Clamp(capacity.InitialCount, 0, _maxCount);

            if(capacity.InitialCount > _maxCount)
            {
                Log.Warning(LogCategory.Pool,
                    $"Pool '{id}': InitialCount {capacity.InitialCount} exceeds MaxCount {_maxCount}. Clamped to {_maxCount}.");
            }

            _root = new GameObject($"Pool [{id}]").transform;
            _root.SetParent(parent, false);

            _itemGroups = new ItemGroup[definitions.Count];

            for(var i = 0; i < definitions.Count; i++)
            {
                _itemGroups[i] = new ItemGroup(definitions[i], _root, i);
            }
        }

        public async UniTask Warmup(CancellationToken token)
        {
            foreach(var segment in _itemGroups)
            {
                await segment.Preload(token);
            }

            var created = 0;

            foreach(var segment in _itemGroups)
            {
                for(var i = 0; i < _initialCount; i++)
                {
                    segment.Warmup();
                    created++;

                    // Крупный пул иначе инстанцируется целиком за один кадр и даёт спайк на загрузке.
                    if(created % WarmupBatchSize == 0)
                    {
                        await UniTask.Yield(token);
                    }
                }
            }

            Log.Info(LogCategory.Pool,
                $"Pool '{Id}' warmed up: {_itemGroups.Length} segment(s), {created} item(s).");
        }

        public TItem Rent(Vector3 position, Quaternion rotation)
        {
            if(_isDisposed)
            {
                Log.Warning(LogCategory.Pool, $"Pool '{Id}' is disposed. Rent returned null.");
                return null;
            }

            var slot = Take(SelectSegment());

            slot.Value.transform.SetPositionAndRotation(position, rotation);
            slot.Value.OnRent();
            slot.State.OnRent(slot.Value);
            _active.Add(slot);

            return slot.Value;
        }

        public void Return(TItem item)
        {
            if(item == null)
            {
                Log.Warning(LogCategory.Pool, $"Pool '{Id}': Return called with null.");
                return;
            }

            for(var i = 0; i < _active.Count; i++)
            {
                if(!ReferenceEquals(_active[i].Value, item))
                {
                    continue;
                }

                var slot = _active[i];
                _active.RemoveAt(i);
                ReturnToSegment(slot);
                return;
            }

            Log.Warning(LogCategory.Pool, $"Pool '{Id}': '{item.name}' is not rented from this pool.");
        }

        public void Execute()
        {
            if(_isDisposed)
            {
                return;
            }

            var deltaTime = Time.deltaTime;

            for(var i = _active.Count - 1; i >= 0; i--)
            {
                var poolItem = _active[i];

                if(!poolItem.State.ShouldReturn(poolItem.Value, deltaTime))
                {
                    continue;
                }

                _active.RemoveAt(i);
                ReturnToSegment(poolItem);
            }
        }

        public void Dispose()
        {
            if(_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach(var poolItem in _active)
            {
                poolItem.Value.OnReturn();
            }

            _active.Clear();
            
            if(_root != null)
            {
                Object.Destroy(_root.gameObject);
            }

            foreach(var segment in _itemGroups)
            {
                segment.Dispose();
            }

            Log.Info(LogCategory.Pool, $"Pool '{Id}' disposed.");
        }

        private int SelectSegment()
        {
            var index = _selector.Select(_itemGroups.Length);

            if(index >= 0 && index < _itemGroups.Length)
            {
                return index;
            }

            Log.Warning(LogCategory.Pool,
                $"Pool '{Id}': selector returned out of range index {index}. Falling back to 0.");

            return 0;
        }

        private PoolItem Take(int index)
        {
            var segment = _itemGroups[index];

            if(segment.TryDequeue(out var slot))
            {
                return slot;
            }

            if(segment.Count < _maxCount)
            {
                return segment.CreateItem();
            }

            Log.Warning(LogCategory.Pool,
                $"Pool '{Id}' segment {index} hit MaxCount ({_maxCount}). Reusing the oldest active item.");

            return TakeOldestActive(index);
        }

        private PoolItem TakeOldestActive(int segmentIndex)
        {
            // _active хранит слоты всех сегментов вперемешку, но порядок вставки и есть порядок возраста.
            for(var i = 0; i < _active.Count; i++)
            {
                if(_active[i].SegmentIndex != segmentIndex)
                {
                    continue;
                }

                var slot = _active[i];
                _active.RemoveAt(i);
                slot.Value.OnReturn();
                return slot;
            }

            throw new InvalidOperationException($"Pool '{Id}' segment {segmentIndex} reached MaxCount but has no active items.");
        }

        private void ReturnToSegment(PoolItem poolItem)
        {
            poolItem.Value.OnReturn();
            _itemGroups[poolItem.SegmentIndex].Enqueue(poolItem);
        }

        private readonly struct PoolItem
        {
            public TItem Value { get; }
            public IReturnState<TItem> State { get; }
            public int SegmentIndex { get; }

            public PoolItem(TItem value, IReturnState<TItem> state, int segmentIndex)
            {
                Value = value;
                State = state;
                SegmentIndex = segmentIndex;
            }
        }

        private sealed class ItemGroup
        {
            private readonly IPoolSource<TItem> _source;
            private readonly IReturnRule<TItem> _rule;
            private readonly Queue<PoolItem> _free = new Queue<PoolItem>();
            
            private readonly Transform _root;
            private readonly int _index;

            public int Count { get; private set; }

            public ItemGroup(SegmentDefinition<TItem> definition, Transform root, int index)
            {
                _source = definition.Source ?? throw new ArgumentNullException(nameof(definition.Source));
                _rule = definition.Rule ?? throw new ArgumentNullException(nameof(definition.Rule));
                _root = root;
                _index = index;
            }

            public UniTask Preload(CancellationToken token)
            {
                return _source.Preload(token);
            }

            public PoolItem CreateItem()
            {
                Count++;
                return new PoolItem(_source.Create(_root), _rule.CreateState(), _index);
            }

            public void Warmup()
            {
                var slot = CreateItem();

                // Свежесозданный объект обязан оказаться в том же покое, что и возвращённый.
                slot.Value.OnReturn();
                _free.Enqueue(slot);
            }

            public bool TryDequeue(out PoolItem poolItem)
            {
                return _free.TryDequeue(out poolItem);
            }

            public void Enqueue(PoolItem poolItem)
            {
                _free.Enqueue(poolItem);
            }

            public void Dispose()
            {
                _free.Clear();
                _source.Release();
            }
        }
    }
}
