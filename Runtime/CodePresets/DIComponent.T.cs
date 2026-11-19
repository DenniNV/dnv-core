using UnityEngine;

namespace Libraries.DNV.CodePresets
{
    public abstract class AutoComponent<TComponent> : AutoComponent where TComponent : Component
    {
        private TComponent _cachedComponent;

        public Vector3 WorldPosition => transform.position;
        public Vector3 WorldEulerAngles => transform.rotation.eulerAngles;
        public Vector3 Forward => transform.forward;
        public Vector3 UpDirection => transform.up;

        protected TComponent Component => _cachedComponent 
            ? _cachedComponent : (_cachedComponent = GetComponent<TComponent>());

        public virtual void Activate()
        {
        }

        public virtual void Release()
        {
        }
    }
}
