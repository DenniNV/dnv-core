using Libraries.DNV.DebugPhysics;
using UnityEngine;
using Debug = Libraries.DNV.DebugPhysics.Physics;

namespace Libraries.DNV.Casters
{
    public class DebugCaster
    {
        public void DebugSphereCast(Ray ray, float radius, float distance, LayerMask mask, RayHitColor color)
        {
            Debug.SphereCast(ray, radius, distance, mask, PreviewCondition.Editor, 0f, color.HitColor,
                color.NoHitColor, drawType: CastDrawType.Minimal);
        }

        public void DebugSphereCast(Vector3 origin, float radius, Vector3 direction, float distance, LayerMask mask,
            RayHitColor color)
        {
            Debug.SphereCast(origin, radius, direction, distance, mask, PreviewCondition.Editor, 0f, color.HitColor,
                color.NoHitColor, drawType: CastDrawType.Minimal);
        }

        public void DebugRayCast(Ray ray, float distance, LayerMask mask, RayHitColor color)
        {
            Debug.Raycast(ray, distance, mask, PreviewCondition.Editor, 0f, color.HitColor,
                color.NoHitColor);
        }

        public void DebugRayNonAlloc(Ray ray, float distance, LayerMask mask, int bufferSize)
        {
            var hits = new RaycastHit[bufferSize];

            Debug.RaycastNonAlloc(ray, hits, distance, mask, PreviewCondition.Editor, 0f, Color.green, Color.red);
        }

        public void DebugRayCast(Ray ray, float distance)
        {
            Debug.Raycast(ray, distance, PreviewCondition.Editor, 0f, Color.green,
                Color.red);
        }

        public void DebugSphereCast(Ray ray, float radius, float distance, RayHitColor color)
        {
            Debug.SphereCast(ray, radius, distance, PreviewCondition.Editor, 0f, color.HitColor,
                color.NoHitColor, drawType: CastDrawType.Minimal);
        }

        public void DebugSphereNonAllocCast(Vector3 origin, float radius, Vector3 direction, float distance, LayerMask mask, int bufferSize,
            float showTime)
        {
            var nonAlloc = new RaycastHit[bufferSize];

            Debug.SphereCastNonAlloc(origin, radius,direction, nonAlloc, distance, mask, PreviewCondition.Editor,
                showTime, Color.green, Color.red, drawType: CastDrawType.Minimal);
        }

        public void DebugBoxCast(Ray ray, Vector3 boxSize, Quaternion rotation, float distance, float showTime,
            LayerMask mask)
        {
            Debug.BoxCast(ray.origin, boxSize, ray.direction, rotation, distance, mask, PreviewCondition.Editor,
                showTime, Color.green, Color.red, drawType: CastDrawType.Minimal);
        }


        public void DebugOverlapSphere(Vector3 origin, float radius, float showTime, int bufferSize, LayerMask mask,
            RayHitColor color)
        {
            var result = new Collider[bufferSize];
            
            Debug.OverlapSphereNonAlloc(origin, radius, result, mask, QueryTriggerInteraction.Collide,
                PreviewCondition.Editor,
                showTime, color.HitColor, color.NoHitColor, false);
        }

        public void DebugCapsuleCast(Vector3 firstPoint, Vector3 secondPoint, Vector3 direction, float radius, float distance, LayerMask mask,
            RayHitColor color, float showTime)
        {
            Debug.CapsuleCast(firstPoint, secondPoint, radius, direction, distance, mask, PreviewCondition.Editor, showTime,
                color.HitColor, color.NoHitColor);
        }

        public void DebugOverlapCapsule(Vector3 firstPoint, Vector3 secondPoint, int bufferSize, float radius,
           LayerMask mask,
            RayHitColor color, float showTime)
        {
            var result = new Collider[bufferSize];

            Debug.OverlapCapsuleNonAlloc(firstPoint, secondPoint, radius, result, mask, PreviewCondition.Editor, showTime,
                color.HitColor, color.NoHitColor);
        }

        public void DebugCheckSphere(Vector3 origin, float radius, LayerMask mask, RayHitColor color, float showTime)
        {
            Debug.CheckSphere(origin, radius, mask, PreviewCondition.Editor, showTime, color.HitColor,
                color.NoHitColor);
        }
    }
}