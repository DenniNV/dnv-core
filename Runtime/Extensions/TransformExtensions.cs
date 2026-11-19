using System.Collections.Generic;
using UnityEngine;

namespace Libraries.DNV.Extensions
{
    public static class TransformExtensions
    {
        public static void ActivateAll(this Transform self)
        {
            for (int i = 0, count = self.childCount; i < count; i++)
            {
                var child = self.GetChild(i).gameObject;

                if(!child.activeSelf)
                {
                    child.SetActive(true);
                }
            }
        }

        public static Transform FindNearPoint(this Transform target, List<Transform> objects)
        {
            var nearPoint = objects[0];

            if(nearPoint == null)
            {
                return null;
            }

            for (var i = 1; i < objects.Count; i++)
            {
                var point = objects[i];

                if(point == null)
                {
                    continue;
                }

                var distanceNext = (target.position - point.position).sqrMagnitude;
                var distanceCurrent = (target.position - nearPoint.position).sqrMagnitude;

                if(distanceCurrent > distanceNext)
                {
                    nearPoint = point;
                }
            }

            return nearPoint;
        }
    }
}
