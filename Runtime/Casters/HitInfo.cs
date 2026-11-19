using UnityEngine;

namespace Libraries.DNV.Casters
{
    public struct HitInfo<T> where T : class
    {
        public T Component { get; }
        public Collider Collider { get; }
        public Vector3 Normal { get; }
        public Vector3 Point { get; }
        public bool IsHasInfo { get; }

        public HitInfo(T component, Collider collider, Vector3 normal, Vector3 point, bool isHasInfo = true)
        {
            Component = component;
            Collider = collider;
            Normal = normal;
            Point = point;
            IsHasInfo = isHasInfo;
        }

        public override string ToString()
        {
            return $"({Component} Component, {Collider} Collider, {Normal} Normal, {Point} Point, {IsHasInfo} IsHasInfo)";
        }
    }
}