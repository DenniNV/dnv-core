using System;
using Libraries.DNV.CodePresets.Pools.Interface;
using Random = UnityEngine.Random;

namespace Libraries.DNV.CodePresets.Pools.Selectors
{
    [Serializable]
    public sealed class RandomSelector : ISourceSelector
    {
        public int Select(int count)
        {
            return Random.Range(0, count);
        }
    }
}
