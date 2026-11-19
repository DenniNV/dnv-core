using UnityEngine;

namespace Libraries.DNV.MVC.Core
{
    /// <summary>
    /// What UIRoot can give any factory. Controller-specific data belongs to the factory itself,
    /// so a new field here reaches every factory without changing their signatures.
    /// </summary>
    public readonly struct ControllerFactoryContext
    {
        public Transform ViewParent { get; }

        public ControllerFactoryContext(Transform viewParent)
        {
            ViewParent = viewParent;
        }
    }
}
