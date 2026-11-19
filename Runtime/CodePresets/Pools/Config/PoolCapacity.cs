using System;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Pools.Config
{
    [Serializable]
    public class PoolCapacity
    {
        [SerializeField] [Min(0)]
        private int _initialCount = 8;

        [SerializeField] [Min(1)]
        private int _maxCount = 32;

        public int InitialCount => _initialCount;
        public int MaxCount => _maxCount;
    }
}
