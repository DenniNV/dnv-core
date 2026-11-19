using UnityEngine;

namespace Libraries.DNV.Extensions
{
    public static class FloatExtensions
    {
        public static float CustomRound(this float value)
        {
            return value switch
            {
                0 => 0,
                > 0 => 1,
                _ => -1
            };
        }

        public static float Normalize(this float value)
        {
            return Mathf.Clamp(value, -1f, 1f);
        }

        public static float Clamp(this float value)
        {
            return Mathf.Clamp01(value);
        }

        public static float ClampByZero(this float value)
        {
            return value < 0 ? 0f : value;
        }
    }
}
