using System;
using Libraries.DNV.CodePresets.Pools.Effects;
using Libraries.DNV.CodePresets.Pools.Interface;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Pools.Config
{
    [Serializable]
    public sealed class EffectSegmentConfig
    {
        [SerializeReference] [SubclassSelector]
        private IPoolSource<PooledEffect> _source;

        [SerializeReference] [SubclassSelector]
        private IReturnRule<PooledEffect> _rule;

        public SegmentDefinition<PooledEffect> ToDefinition()
        {
            return new SegmentDefinition<PooledEffect>(_source, _rule);
        }
    }
}
