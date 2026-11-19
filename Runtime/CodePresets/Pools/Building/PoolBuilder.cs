using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.CodePresets.Pools.Config;
using Libraries.DNV.CodePresets.Pools.Interface;
using Libraries.DNV.ContentManagerSystem;
using Libraries.DNV.ContentManagerSystem.Parts;
using Libraries.DNV.DependencyInjection;
using Libraries.DNV.DependencyInjection.Context;
using Libraries.DNV.Logging;

namespace Libraries.DNV.CodePresets.Pools.Building
{
    /// <summary>
    /// Builds every pool of the CMS once, in ProjectContext: all pools are global. A second PoolBuilder (e.g. left in a scene
    /// context by a project made before 0.3.2) skips the pools that already exist and warns.
    /// </summary>
    [Serializable]
    public class PoolBuilder : IBaseBuilder
    {
        private readonly List<IPool> _built = new List<IPool>();

        // Реестр держится ссылкой: на Dispose контекст уже может быть снят с инжектора.
        private PoolProvider _provider;
        private PoolExecutionGroup _group;
        private bool _isInGroup;

        public async UniTask Build(CancellationToken token)
        {
            _provider = Injector.Get<PoolProvider>();
            _group = Injector.Get<PoolExecutionGroup>();

            var alreadyBuilt = new List<string>();

            foreach(var entity in Injector.Get<Cms>().Entities)
            {
                var part = entity.GetPart<PoolsPart>();

                if(part == null)
                {
                    continue;
                }

                await BuildPools(part, alreadyBuilt, token);
            }

            AddProviderToGroup();
            Report(alreadyBuilt);
        }

        public void Dispose()
        {
            if(_provider == null)
            {
                return;
            }

            foreach(var pool in _built)
            {
                _provider.Remove(pool);
                pool.Dispose();
            }

            if(_isInGroup)
            {
                _group.Remove(_provider);
            }

            _isInGroup = false;
            _built.Clear();
        }

        private async UniTask BuildPools(PoolsPart part, List<string> alreadyBuilt, CancellationToken token)
        {
            foreach(var config in part.PoolConfigs)
            {
                if(config == null)
                {
                    continue;
                }

                if(_provider.Contains(config.Id))
                {
                    alreadyBuilt.Add(config.Id);
                    continue;
                }

                var pool = config.Create(_provider.Root);
                await pool.Warmup(token);

                _provider.Add(pool);
                _built.Add(pool);
            }
        }

        private void AddProviderToGroup()
        {
            if(_built.Count == 0)
            {
                return;
            }

            _group.Add(_provider);
            _isInGroup = true;
        }

        private void Report(List<string> alreadyBuilt)
        {
            Log.Info(LogCategory.Pool, $"PoolBuilder built {_built.Count} pool(s).");

            if(alreadyBuilt.Count == 0)
            {
                return;
            }

            Log.Warning(LogCategory.Pool,
                $"PoolBuilder skipped pool(s) that already exist: {string.Join(", ", alreadyBuilt)}. " +
                "Pools are global and built once in ProjectContext — remove this PoolBuilder from the other context.");
        }
    }
}
