using System.Collections.Generic;
using UnityEngine;

namespace Libraries.DNV.Casters
{
    public class CapsuleCaster
    {
        private readonly Caster _caster = new Caster();
        
        public bool TryCast<TType>(out HitInfo<TType> data, Vector3 origin, 
            Vector3 direction, float radius, float height, float distance, float size, LayerMask mask,  RayHitColor color) where TType : class
        {
            if (_caster.TryCast(out var hit, origin, direction, radius, height, distance, size, mask, color))
            {
                if (hit.collider.TryGetComponent<TType>(out var component))
                {
                    data = new HitInfo<TType>(component, hit.collider, hit.normal, hit.point);
                    return true;
                }
            }

            data = default;
            return false;
        }
        
        public bool TryCast(out HitInfo<Collider> data, Vector3 origin, 
            Vector3 direction, float radius, float height, float distance, float size, LayerMask mask,  RayHitColor color)
        {
            if (_caster.TryCast(out var hit, origin, direction, radius, height, distance, size, mask, color))
            {
                data = new HitInfo<Collider>(hit.collider, hit.collider, hit.normal, hit.point);
                return true;
            }

            data = default;
            return false;
        }


        public bool TryCastOverlap<T>(out HitInfo<T>[] result, Vector3 origin,
            float radius, float height, float size, int bufferSize, LayerMask mask, RayHitColor color) where T : class
        {
            return _caster.TryCastOverlap<T>(out result, origin, radius, height, size, bufferSize, mask, color);
        }

        private class Caster
        {
            private readonly DebugCaster _debugCaster = new();
             
            public bool TryCast(out RaycastHit hit, Vector3 origin, Vector3 direction, float radius, float height, float distance, float size, LayerMask mask,
                RayHitColor color)
            {
                var firstPoint = origin + Vector3.up * (height - radius);
                var secondPoint = origin + Vector3.up * radius; 

                var newRadius = radius * size;
                
#if UNITY_EDITOR
                _debugCaster.DebugCapsuleCast(firstPoint, secondPoint, direction, newRadius, distance, mask, color, 0);
#endif

                return Physics.CapsuleCast(firstPoint, secondPoint, newRadius, direction, out hit, distance, mask,
                    QueryTriggerInteraction.UseGlobal);
            }
            
            
            public bool TryCastOverlap<T>(out HitInfo<T>[] result, Vector3 origin,
                float radius, float height, float size, int bufferSize, LayerMask mask, RayHitColor color) where T : class
            {
                var colliders = new Collider[bufferSize];
                radius *= size;

                var firstPoint = origin + Vector3.up * ((height - radius) * size);
                var secondPoint = origin + Vector3.up * radius; 
                
            
                var count = Physics.OverlapCapsuleNonAlloc(firstPoint, secondPoint, radius, colliders, mask, QueryTriggerInteraction.UseGlobal);
            
#if UNITY_EDITOR
            
                _debugCaster.DebugOverlapCapsule(firstPoint, secondPoint, bufferSize, radius, mask, color, 0.1f);
#endif
                var hits = new List<HitInfo<T>>();

                for (var i = 0; i < count; i++)
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
}