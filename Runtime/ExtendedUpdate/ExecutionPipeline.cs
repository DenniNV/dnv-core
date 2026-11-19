using System;
using System.Collections.Generic;

namespace Libraries.DNV.ExtendedUpdate
{
    /// <summary>
    /// Every execution group of the project, run in <see cref="ExecutionOrder"/>.
    /// The library adds its own groups; a game adds its groups from its installer.
    /// </summary>
    public sealed class ExecutionPipeline
    {
        private readonly List<OrderedGroup> _groups = new List<OrderedGroup>();

        public void Add(ExecutionGroup group, int order)
        {
            if(_groups.Exists(x => x.Group == group))
            {
                throw new InvalidOperationException($"{group.GetType().Name} is already in the pipeline");
            }

            // Groups with equal order keep the order they were added in.
            var index = _groups.FindIndex(x => x.Order > order);
            var entry = new OrderedGroup(group, order);

            if(index < 0)
            {
                _groups.Add(entry);
                return;
            }

            _groups.Insert(index, entry);
        }

        public void Execute()
        {
            foreach(var entry in _groups)
            {
                entry.Group.Execute();
            }
        }

        public void LateExecute()
        {
            foreach(var entry in _groups)
            {
                entry.Group.LateExecute();
            }
        }

        public void FixedExecute()
        {
            foreach(var entry in _groups)
            {
                entry.Group.FixedExecute();
            }
        }

        private readonly struct OrderedGroup
        {
            public ExecutionGroup Group { get; }
            public int Order { get; }

            public OrderedGroup(ExecutionGroup group, int order)
            {
                Group = group;
                Order = order;
            }
        }
    }
}
