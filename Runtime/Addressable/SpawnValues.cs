using System;
using UnityEngine;

namespace Libraries.DNV.Addressable
{
    [Serializable]
    public class SpawnValues
    {
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private Quaternion _rotation;
        [SerializeField]
        private Transform _parent;

        public Vector3 Position => _position;
        public Quaternion Rotation => _rotation;
        public Transform Parent => _parent;
 
        public SpawnValues(Vector3 position, Quaternion rotation, Transform parent)
        {
            _position = position;
            _rotation = rotation;
            _parent = parent;
        }

        public static SpawnValues Default => new SpawnValues(Vector3.zero, Quaternion.identity, null);
    }
}