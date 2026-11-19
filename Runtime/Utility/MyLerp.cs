using UnityEngine;

namespace Libraries.DNV.Utility
{
    public static class MyLerp
    {
        private static Vector3 ExpDecay(Vector3 self, Vector3 target, float decay, float dt)
        {
            return target + (self - target) * Mathf.Exp(-decay * dt);
        }
        
        private static float ExpDecay(float self, float target, float decay, float dt)
        {
            return target + (self - target) * Mathf.Exp(-decay * dt);
        }

        public static Vector3 Lerp(Vector3 self, Vector3 target, float dt, float decay = 1f)
        {
            return ExpDecay(self, target, decay, dt);
        }
        
        public static float Lerp(float self, float target, float dt, float decay = 1f)
        {
            return ExpDecay(self, target, decay, dt);
        }
    }
}