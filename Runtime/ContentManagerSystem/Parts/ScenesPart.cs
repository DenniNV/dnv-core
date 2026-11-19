using System;
using System.Collections.Generic;
using Libraries.DNV.LevelLoader;
using UnityEngine;

namespace Libraries.DNV.ContentManagerSystem.Parts
{
    /// <summary>
    /// Game scenes of the project and the one loaded after Entry. Scene names become constants in SceneIds.
    /// </summary>
    [Serializable]
    public sealed class ScenesPart : CmsEntityPart
    {
        [SerializeField]
        private string _startScene;

        [SerializeField]
        private List<ScenePreset> _scenes = new List<ScenePreset>();

        public override string PartName => "Scenes";

        public string StartScene => _startScene;

        public IReadOnlyList<ScenePreset> Scenes => _scenes;
    }
}
