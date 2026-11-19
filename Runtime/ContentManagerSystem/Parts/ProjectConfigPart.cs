using System;
using UnityEngine;

namespace Libraries.DNV.ContentManagerSystem.Parts
{
    [Serializable]
    public sealed class ProjectConfigPart : CmsEntityPart
    {
        [SerializeReference][SubclassSelector]
        private ProjectConfig[] _config;

        public override string PartName => "Project Config Part";

        public T GetConfig<T>() where T : ProjectConfig
        {
            foreach (var config in _config)
            {
                if (config is T projectConfig)
                {
                    return projectConfig;
                }
            }

            throw new Exception($"Config not found {typeof(T).Name}");
        }
    }
}
