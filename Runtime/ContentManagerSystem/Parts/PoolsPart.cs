using System;
using System.Collections.Generic;
using Libraries.DNV.CodePresets.Pools.Config;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Scripting.APIUpdating;

namespace Libraries.DNV.ContentManagerSystem.Parts
{
    [Serializable]
    [MovedFrom(false, sourceNamespace: "Libraries.DNV.CodePresets.Pools.Config")]
    public sealed class PoolsPart : CmsEntityPart
    {
        [SerializeReference] [SubclassSelector]
        [FormerlySerializedAs("_pollsConfigs")]
        private List<PoolConfig> _poolConfigs = new List<PoolConfig>();

        public override string PartName => "Pools";

        public IReadOnlyList<PoolConfig> PoolConfigs => _poolConfigs;
    }
}
