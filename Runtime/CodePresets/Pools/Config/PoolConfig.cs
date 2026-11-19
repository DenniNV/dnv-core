using System;
using Libraries.DNV.CodePresets.Pools.Interface;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Pools.Config
{
    [Serializable]
    public abstract class PoolConfig
    {
        [SerializeField]
        private string _id;

        [SerializeField]
        private PoolCapacity _capacity;

        public string Id => _id;

        protected PoolCapacity Capacity => _capacity;

        public abstract IPool Create(Transform root);
    }
}
