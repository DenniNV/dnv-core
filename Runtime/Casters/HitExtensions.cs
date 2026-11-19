using System.Collections.Generic;
using Libraries.DNV.Casters;
using UnityEngine;

namespace Libraries.DNV.Casters
{
    public static class HitExtensions 
    {
        public static HitInfo<TType>[] CompartmentsByDotSide<TType>(this HitInfo<TType>[] hits,
            Transform targetTransform, 
            float dotValue) where TType : class
        {
            var resultHits = new List<HitInfo<TType>>();
			
            foreach (var hit in hits)
            {
                var direction = (hit.Point - targetTransform.transform.position).normalized;
                // direction.y = 0;
                if (Vector3.Dot(direction, targetTransform.transform.forward) > dotValue)
                {
                    resultHits.Add(hit);
                }
            }

            return resultHits.ToArray();
        }


        public static HitInfo<TType>[] CompartmentsByDistance<TType>(this HitInfo<TType>[] hits, 
            Transform targetTransform, float minDistance, float maxDistance) where TType : class
        {
            var resultHits = new List<HitInfo<TType>>();

            foreach (var hit in hits)
            {
                var distance = (hit.Point - targetTransform.position).magnitude;
                if (distance >= minDistance && distance <= maxDistance)
                {
                    resultHits.Add(hit);
                }
            }

            return resultHits.ToArray();
        }

        public static bool TryFindNeighbors<TType>(this HitInfo<TType>[] hits,
            out HitInfo<TType> result, Transform ownerTransform) where TType : class
        {
            if (hits.Length < 1)
            {
                result = default;
                return false;
            }

            var minDistance = Vector3.Distance(ownerTransform.position, hits[^1].Point);
            result = hits[^1];

            foreach (var hit in hits)
            {
                var distance = Vector3.Distance(ownerTransform.position, hit.Point);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = hit;
                }
            }
            
            return true;
        }
    }
}