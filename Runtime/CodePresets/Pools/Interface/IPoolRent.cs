using UnityEngine;

namespace Libraries.DNV.CodePresets.Pools.Interface
{
    public interface IPoolRent<out TItem> where TItem : class
    {
        TItem Rent(Vector3 position, Quaternion rotation);
    }
}
