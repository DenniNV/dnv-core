using System.Collections.Generic;
using UnityEngine;

namespace Libraries.DNV.Casters
{
    public class SphereCaster
    {
        private readonly Caster _caster = new();

        public bool TryCast(Vector3 origin, Vector3 direction,
            float radius, float distance, LayerMask mask, RayHitColor color)
        {
            return _caster.TryCast(out var _, origin, direction, radius, distance, mask, color);
        }

        public bool TryCast(out HitInfo<Collider> data, Vector3 origin, Vector3 direction,
            float radius, float distance, LayerMask mask, RayHitColor color)
        {
            if (_caster.TryCast(out var hit, origin, direction, radius, distance, mask, color))
            {
                data = new HitInfo<Collider>(hit.collider, hit.collider, hit.normal, hit.point);
                return true;
            }

            data = default;
            return false;
        }

        public bool TryCast(out HitInfo<Collider> data, Vector3 origin, Vector3 direction,
            float radius, float distance)
        {
            var result = _caster.TryCast(out var hit, origin, direction, radius, distance);

            if (result)
            {
                data = new HitInfo<Collider>(hit.collider, hit.collider, hit.normal, hit.point);
                return true;
            }

            data = default;
            return false;
        }


        public bool TryCast<T>(out HitInfo<T> data, Vector3 origin, Vector3 direction,
            float radius, float distance, LayerMask mask, RayHitColor color) where T : class
        {
            if (_caster.TryCast(out var hit, origin, direction, radius, distance, mask, color))
            {
                if (hit.collider.TryGetComponent<T>(out var component))
                {
                    data = new HitInfo<T>(component, hit.collider, hit.normal, hit.point);
                    return true;
                }
            }

            data = default;
            return false;
        }


        public bool TryCastNonAlloc<T>(out HitInfo<T>[] data, Vector3 origin, Vector3 direction,
            float radius, float distance, LayerMask mask, int bufferSize) where T : class
        {
            var count = _caster.TryOverlapSphereNonAlloc(out var rayHits, origin, direction, radius, distance,
                mask, bufferSize);

            data = new HitInfo<T>[count];

            for (int i = 0; i < count; i++)
            {
                var item = rayHits[i];

                if (item.collider.TryGetComponent(out T component))
                {
                    data[i] = new HitInfo<T>(component, item.collider, item.normal, item.point);
                }
            }

            return count > 0;
        }
        
        public bool TryCheckSphere(Vector3 origin, float radius, LayerMask mask, RayHitColor color)
        {
            return _caster.TryCheckSphere(origin, radius, mask, color);
        }

        #region Caster

        private class Caster
        {
            private readonly DebugCaster _debugCaster = new();

            public bool TryCast(out RaycastHit hit, Vector3 origin, Vector3 direction, float radius,
                float distance, LayerMask layerMask, RayHitColor color)
            {

#if UNITY_EDITOR
                _debugCaster.DebugSphereCast(origin, radius, direction, distance, layerMask, color);
#endif

                return Physics.SphereCast(origin, radius, direction, out hit, distance, layerMask,
                    QueryTriggerInteraction.Collide);
            }

            public bool TryCast(out RaycastHit hit, Vector3 origin, Vector3 direction, float radius, float distance)
            {
// #if UNITY_EDITOR
//                 _debugCaster.DebugSphereCast(ray, radius, distance);
// #endif

                return Physics.SphereCast(origin, radius,direction, out hit, distance);
            }


            public int TryOverlapSphereNonAlloc(out RaycastHit[] rayData, Vector3 origin, Vector3 direction,
                float radius,
                float distance, LayerMask layerMask, int bufferSize)

            {
                var nonAlloc = new RaycastHit[bufferSize];

#if UNITY_EDITOR

                _debugCaster.DebugSphereNonAllocCast(origin, radius, direction, distance, layerMask, bufferSize, 0.33f);
#endif
                var count = Physics.SphereCastNonAlloc(origin, radius, direction, nonAlloc, distance, layerMask);

                rayData = nonAlloc;
                return count;
            }

            public bool TryCheckSphere(Vector3 origin, float radius, LayerMask mask, RayHitColor color)
            {
#if UNITY_EDITOR
                _debugCaster.DebugCheckSphere(origin, radius, mask,color, 0f);
#endif
                return Physics.CheckSphere(origin, radius, mask);
            }
        }

        #endregion


      
    }

    public class OverlapCaster
    {
        private readonly DebugCaster _debugCaster = new DebugCaster();
        
        public bool TryCastSphere(Vector3 position,
            float radius, int bufferSize, LayerMask mask, RayHitColor color)
        {
            var results = new Collider[bufferSize];
            var size = Physics.OverlapSphereNonAlloc(position, radius, results, mask);

#if UNITY_EDITOR
            _debugCaster.DebugOverlapSphere(position, radius, 0.1f, bufferSize, mask, color);
#endif
            
            return size > 0;
            
        }
        
        public bool TryCastSphere<TType>(Vector3 position,
            float radius, int bufferSize, LayerMask mask, RayHitColor color) where TType : class
        {
            return TryCastSphere<TType>(out var hits, position, radius, bufferSize, mask, color);
        }
        
        
        public bool TryCastSphere(out Collider[] result, Vector3 position,
            float radius, int bufferSize, LayerMask mask, RayHitColor color)
        {
            var colliders = new Collider[bufferSize];
            var size = Physics.OverlapSphereNonAlloc(position, radius, colliders, mask);

#if UNITY_EDITOR
            
            _debugCaster.DebugOverlapSphere(position, radius, 0.1f, bufferSize, mask, color);
#endif
            result = new Collider[size];

            for (int i = 0; i < size; i++)
            {
                result[i] = colliders[i];
            }
            
            return size > 0;
        }
        
        public bool TryCastSphere<T>(out HitInfo<T>[] result, Vector3 position,
            float radius, int bufferSize, LayerMask mask, RayHitColor color) where T: class
        {
            var colliders = new Collider[bufferSize];
            var size = Physics.OverlapSphereNonAlloc(position, radius, colliders, mask, QueryTriggerInteraction.Collide);

#if UNITY_EDITOR
            
            _debugCaster.DebugOverlapSphere(position, radius, 0.1f, bufferSize, mask, color);
#endif

            var hits = new List<HitInfo<T>>();

            for (var i = 0; i < size; i++)
            {
                var element = colliders[i];

                if (element.TryGetComponent<T>(out var component))
                {
                    hits.Add(new HitInfo<T>(component, element, Vector3.zero, element.transform.position));
                }
            }

            result = hits.ToArray(); 
            return result.Length > 0;
        }
       
    }
}