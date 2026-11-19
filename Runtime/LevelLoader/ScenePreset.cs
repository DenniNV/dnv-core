using System;
using UnityEngine;

namespace Libraries.DNV.LevelLoader
{
    [Serializable]
    public sealed class ScenePreset
    {
        [field: SerializeField]
        public string Name { get; private set; }
    }
}
