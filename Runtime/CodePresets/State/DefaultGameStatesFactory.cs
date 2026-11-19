using System;
using System.Collections.Generic;
using Libraries.DNV.CodePresets.State.Interfaces;
using Libraries.DNV.ContentManagerSystem;
using Libraries.DNV.ContentManagerSystem.Parts;
using Libraries.DNV.Core;
using Libraries.DNV.DependencyInjection;
using Libraries.DNV.LevelLoader;

namespace Libraries.DNV.CodePresets.State
{
    /// <summary>
    /// Base flow: Initialize → load the start scene from <see cref="ScenesPart"/> → InGame.
    /// A game factory can call this one and add its own states to the result.
    /// </summary>
    [Serializable]
    public sealed class DefaultGameStatesFactory : IGameStatesFactory
    {
        public IEnumerable<IState> Create(IStateRunner gameStateMachine)
        {
            var uiRoot = Injector.Get<UIRoot>();
            var sceneLoader = Injector.Get<ISceneLoader>();
            var startScene = Injector.Get<Cms>().GetFirstPart<ScenesPart>().StartScene;

            return new IState[]
            {
                new InitializeState(gameStateMachine, uiRoot),
                new LoadGameSceneState(gameStateMachine, sceneLoader, uiRoot, startScene),
                new InGameState(),
            };
        }
    }
}
