using UnityEngine;

namespace Libraries.DNV.Casters
{
    public class RayCast
    {
        private readonly DebugCaster _debugCaster = new();

        public Ray Cast(Vector3 origin, Vector3 direction)
        {
            return new Ray(origin, direction);
        }

        public bool TryCast<T>(out HitInfo<T> info, Vector3 origin, Vector3 direction, float distance, LayerMask mask)
            where T : Component
        {
            var ray = Cast(origin, direction);

#if UNITY_EDITOR

            _debugCaster.DebugRayCast(ray, distance, mask, RayHitColor.DefaultColor);
#endif

            if (Physics.Raycast(origin, direction, out var hit, distance, mask))
            {
                if (hit.collider.TryGetComponent<T>(out var component))
                {
                    info = new HitInfo<T>(component, hit.collider, hit.normal, hit.point);
                    return true;
                }
            }


            info = default;
            return false;
        }


        public bool TryCast(out HitInfo<Collider> info, Vector3 origin, Vector3 direction,
            float radius, float distance, LayerMask mask)
        {
            var ray = Cast(origin, direction);

#if UNITY_EDITOR

            _debugCaster.DebugRayCast(ray, distance, mask, RayHitColor.DefaultColor);
#endif

            if (Physics.Raycast(origin, direction, out var hit, distance, mask))
            {
                info = new HitInfo<Collider>(hit.collider, hit.collider, hit.normal, hit.point);
                return true;
            }


            info = default;
            return false;
        }


        public bool TryCast(out RaycastHit info, Vector3 origin, Vector3 direction, float distance, LayerMask mask)
        {
            var ray = Cast(origin, direction);

#if UNITY_EDITOR

            _debugCaster.DebugRayCast(ray, distance, mask, RayHitColor.DefaultColor);
#endif

            return Physics.Raycast(origin, direction, out info, distance, mask);
        }

        public bool TryCast(out RaycastHit info, Vector3 origin, Vector3 direction, float distance)
        {
            var ray = Cast(origin, direction);

#if UNITY_EDITOR

            _debugCaster.DebugRayCast(ray, distance);
#endif

            return Physics.Raycast(origin, direction, out info, distance);
        }


        public int TryCastNonAlloc(out RaycastHit[] hits, Vector3 origin, Vector3 direction, float distance,
            int bufferSize,
            LayerMask mask)
        {
            var ray = Cast(origin, direction);

            hits = new RaycastHit[bufferSize];
#if UNITY_EDITOR

            _debugCaster.DebugRayNonAlloc(ray, distance, mask, bufferSize);
#endif

            return Physics.RaycastNonAlloc(ray, hits, distance, mask, QueryTriggerInteraction.UseGlobal);
        }
    }
}