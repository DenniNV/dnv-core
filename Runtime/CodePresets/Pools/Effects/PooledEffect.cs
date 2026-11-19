using Libraries.DNV.CodePresets.Pools.Interface;

namespace Libraries.DNV.CodePresets.Pools.Effects
{
    public abstract class PooledEffect : AutoComponent, IPoolable
    {
        public virtual void OnRent()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnReturn()
        {
            gameObject.SetActive(false);
        }
    }
}
