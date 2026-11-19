using System;
using System.Collections.Generic;
using Libraries.DNV.CodePresets.Pools.Effects;
using Libraries.DNV.CodePresets.Pools.Interface;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Pools.Config
{
    [Serializable]
    public sealed class EffectPoolConfig : PoolConfig
    {
        [SerializeField]
        private List<EffectSegmentConfig> _segments = new List<EffectSegmentConfig>();

        [SerializeReference] [SubclassSelector]
        private ISourceSelector _selector;

        public override IPool Create(Transform root)
        {
            var definitions = new SegmentDefinition<PooledEffect>[_segments.Count];

            for(var i = 0; i < _segments.Count; i++)
            {
                definitions[i] = _segments[i].ToDefinition();
            }

            return new Pool<PooledEffect>(Id, Capacity, definitions, _selector, root);
        }
    }
}
