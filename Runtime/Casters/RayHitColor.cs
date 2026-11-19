using UnityEngine;

namespace Libraries.DNV.Casters
{
    public struct RayHitColor
    {
        public Color HitColor { get; }
        public Color NoHitColor { get; }

        public RayHitColor(Color hitColor, Color noHitColor)
        {
            NoHitColor = noHitColor;
            HitColor = hitColor;
        }

        public static RayHitColor DefaultColor => new(Color.green, Color.red);
    }
}