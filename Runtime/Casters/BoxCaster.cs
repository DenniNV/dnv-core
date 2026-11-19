using UnityEngine;

namespace Libraries.DNV.Casters
{
    public class BoxCaster
    {
        private readonly Caster _caster = new(new RayCast());

        public bool TryCast(Vector3 origin, Vector3 direction, Vector3 boxSize, Quaternion rotation,
            float distance, LayerMask mask)
        {
            return _caster.TryCast(out var _, origin, boxSize, direction, rotation, distance, mask);
        }

        public bool TryCast(out HitInfo<Collider> data, Vector3 origin, Vector3 direction, Vector3 boxSize,
            Quaternion rotation,
            float distance, LayerMask mask)
        {
            if (_caster.TryCast(out var hit, origin, boxSize, direction, rotation, distance, mask))
            {
                data = new HitInfo<Collider>(hit.collider, hit.collider, hit.normal, hit.point);
                return true;
            }

            data = default;
            return false;
        }


        #region Caster

        private class Caster
        {
            private readonly RayCast _rayCast;
            private readonly DebugCaster _debugCaster;

            public Caster(RayCast rayCast)
            {
                _rayCast = rayCast;
                _debugCaster = new DebugCaster();
            }

            public bool TryCast(out RaycastHit hit, Vector3 origin, Vector3 boxSize, Vector3 direction,
                Quaternion rotation, float distance, LayerMask layerMask)
            {
                var ray = _rayCast.Cast(origin, direction);

#if UNITY_EDITOR
                _debugCaster.DebugBoxCast(ray, boxSize, rotation, distance, 0f, layerMask);
#endif

                // Physics.SphereCast()
                return Physics.BoxCast(origin, boxSize, direction, out hit, rotation, distance, layerMask,
                    QueryTriggerInteraction.Collide);
            }

            public bool TryCast(out RaycastHit hit, Vector3 origin, Vector3 direction, float radius, float distance)
            {
                var ray = _rayCast.Cast(origin, direction);
// #if UNITY_EDITOR
//                 _debugCaster.DebugSphereCast(ray, radius, distance);
// #endif

                return Physics.SphereCast(ray, radius, out hit, distance);
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
        }

        #endregion
    }
}