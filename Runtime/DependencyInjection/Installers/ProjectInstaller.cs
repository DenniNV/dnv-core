using System;
using System.Collections.Generic;
using Libraries.DNV.CodePresets.Pools;
using Libraries.DNV.CodePresets.Pools.Interface;
using Libraries.DNV.CodePresets.State;
using Libraries.DNV.ContentManagerSystem;
using Libraries.DNV.DependencyInjection.Interfaces;
using Libraries.DNV.ExtendedUpdate;
using Libraries.DNV.LevelLoader;
using UnityEngine;

namespace Libraries.DNV.DependencyInjection.Installers
{
    [Serializable]
    public class ProjectInstaller : IInstaller
    {
        [SerializeField] private CmsDatabase _cmsDatabase;

        public void Bind(Resolver container)
        {
            BindCms(container);
            BindPoolRegistry(container);
            BindServices(container);

            var gsm = BindGameStateMachine(container);

            BindExecutors(container, gsm);
        }

        private void BindCms(Resolver container)
        {
            container.RegistrationInstance(new Cms(_cmsDatabase));
        }

        private void BindServices(Resolver resolver)
        {
            resolver.RegistrationInstance<ISceneLoader>(new SceneLoader());
        }

        private void BindExecutors(Resolver resolver, GameStateMachine gsm)
        {
            var projectGroup = new ProjectExecutionGroup();
            var brainGroup = new BrainExecutionGroup();
            var poolGroup = new PoolExecutionGroup();
            var pipeline = new ExecutionPipeline();

            pipeline.Add(projectGroup, ExecutionOrder.Project);
            pipeline.Add(brainGroup, ExecutionOrder.Brain);
            pipeline.Add(poolGroup, ExecutionOrder.Pool);
            projectGroup.Add(gsm);

            resolver.RegistrationInstance(projectGroup);
            resolver.RegistrationInstance(brainGroup);
            resolver.RegistrationInstance(poolGroup);
            resolver.RegistrationInstance(pipeline);
        }

        private GameStateMachine BindGameStateMachine(Resolver resolver)
        {
            var gameStateMachine = new GameStateMachine();
            resolver.RegistrationInstance(gameStateMachine);
            
            return  gameStateMachine;
        }

        private void BindPoolRegistry(Resolver resolver)
        {
            var registry = new PoolProvider();

            resolver.RegistrationInstance(registry);
            resolver.RegistrationInstance<IPoolProvider>(registry);
        }
    }
}