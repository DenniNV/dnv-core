using System;
using UnityEngine;

namespace Libraries.DNV.Extensions
{
    public static class VectorExtensions
    {
        public static float DistanceByVectorComponent(float self, float target)
        {
            return Math.Abs(self - target);
        }

        public static Vector3 ClampOnlyX(this Vector3 self, float minValue, float maxValue)
        {
            return self.WithX(Mathf.Clamp(self.x, minValue, maxValue));
        }

        public static Vector3 ClampOnlyZ(this Vector3 self, float minValue, float maxValue)
        {
            return self.WithZ(Mathf.Clamp(self.z, minValue, maxValue));
        }

        public static float CustomMagnitude(this Vector3 self)
        {
            return (self.x + self.y + self.z) / 3f;
        }

        public static Vector3 WithX(this Vector3 self, float x)
        {
            return new Vector3(x, self.y, self.z);
        }

        public static Vector3 WithY(this Vector3 self, float y)
        {
            return new Vector3(self.x, y, self.z);
        }

        public static Vector3 WithZ(this Vector3 self, float z)
        {
            return new Vector3(self.x, self.y, z);
        }

        public static bool IsZero(this Vector3 self)
        {
            return self == Vector3.zero;
        }

        public static Vector3 Random(this Vector3 self, Vector2 range)
        {
            return new Vector3(
                UnityEngine.Random.Range(range.x, range.y),
                UnityEngine.Random.Range(range.x, range.y),
                UnityEngine.Random.Range(range.x, range.y));
        }
    }
}
