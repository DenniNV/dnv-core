using System;
using System.Collections.Generic;
using Libraries.DNV.CodePresets;
using Pathfinding;
using UnityEngine;

namespace Libraries.DNV.Navigation
{
    [RequireComponent(typeof(Seeker))]
    public class NavigationAgent : AutoComponent<Seeker>
    {
        private readonly List<INavigationAgentHandler> _navigationHandlers = new List<INavigationAgentHandler>();

        public Path GeneratePathBy(Vector3 target)
        {
            return Component.StartPath(transform.position, target, CompletedHandler);
        }
        
        private void CompletedHandler(Path path)
        {
            if (path.error)
            {
                CallNavigationError();
                throw new Exception($"Path exception {path.errorLog}");
            }

            CallNavigationCompleted();
        }
        
        public void AddHandler(INavigationAgentHandler agentHandler)
        {
            _navigationHandlers.Add(agentHandler);
        }

        public void RemoveHandler(INavigationAgentHandler agentHandler)
        {
            _navigationHandlers.Remove(agentHandler);
        }

        private void CallNavigationCompleted()
        {
            foreach (var navigationHandler in _navigationHandlers)
            {
                navigationHandler.PathBeingReachedHandler();
            }
        }
        
        private void CallNavigationError()
        {
            foreach (var navigationHandler in _navigationHandlers)
            {
                navigationHandler.PathIsNotAchievable();
            }
        }
    }
}