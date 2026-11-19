using Libraries.DNV.CodePresets.Pools.Interface;

namespace Libraries.DNV.CodePresets.Pools
{
    public readonly struct SegmentDefinition<TItem> where TItem : class
    {
        public IPoolSource<TItem> Source { get; }
        public IReturnRule<TItem> Rule { get; }

        public SegmentDefinition(IPoolSource<TItem> source, IReturnRule<TItem> rule)
        {
            Source = source;
            Rule = rule;
        }
    }
}
