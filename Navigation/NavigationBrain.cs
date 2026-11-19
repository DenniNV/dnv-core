using System;
using System.Linq;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Libraries.DNV.Navigation
{
    public class NavigationBrain : INavigationBrain
    {
        private readonly NavigationAgent _agent;
        private readonly NavigationBrainSettings _settings;
        private readonly IPathGenerator[] _pathGenerators;
        
        private Path _path;
        private IPathGenerator _currentPathGenerator;
        
        private int _currentPointIndex;
        public bool IsPathCompleted { get; private set; }

        public NavigationBrain(NavigationAgent agent, NavigationBrainSettings settings, params IPathGenerator[] pathGenerators)
        {
            _agent = agent;
            _settings = settings;

            _pathGenerators = pathGenerators;
            _currentPathGenerator = pathGenerators[^1];
        }

        public void Run()
        {
            _agent.AddHandler(this);

            RegeneratePath();
        }

        public void Stop()
        {
            _agent.RemoveHandler(this);
        }

        public void Execute()
        {
            if (IsPathCompleted)
            {
                return;
            }

            if (_currentPointIndex < _path.vectorPath.Count)
            {
                var point = _path.vectorPath[_currentPointIndex];
                var direction = (point - _agent.transform.position).normalized;

                if (Vector3.Distance(point, _agent.transform.position) < _settings.BreakDistance)
                {
                    _currentPointIndex++;
                }
                
            }
            else
            {
                IsPathCompleted = true;
            }
        }

        public void RegeneratePath()
        {
            _path = _currentPathGenerator.Generate();
            _currentPointIndex = 0;
            IsPathCompleted = false;
        }

        public void ChangePathGeneratorTo<TGenerator>() where TGenerator : IPathGenerator
        {
            var element = _pathGenerators.FirstOrDefault(x => x is TGenerator);
            _currentPathGenerator = element ??
                                    throw new NullReferenceException(
                                        $"The path generator {typeof(TGenerator).Name} could not be found.");
        }

        public void PathIsNotAchievable()
        {
            RegeneratePath();
        }

        public void PathBeingReachedHandler()
        {
            
        }
    }

    public interface IPathGenerator
    {
        public Path Generate();
    }

    public class PathGeneratorByForward : IPathGenerator
    {
        private readonly NavigationAgent _agent;
        private readonly NavigationBrainSettings _navigationBrainSettings;
        private readonly bool _reverse;

        public PathGeneratorByForward(NavigationAgent agent, NavigationBrainSettings navigationBrainSettings, bool reverse = false)
        {
            _agent = agent;
            _navigationBrainSettings = navigationBrainSettings;
            _reverse = reverse;
        }

        public Path Generate()
        {
            return _agent.GeneratePathBy(GeneratePoint());
        }

        private Vector3 GeneratePoint()
        {
            var forward = _agent.transform.forward;
            var distance = Random.Range(_navigationBrainSettings.Range.x, _navigationBrainSettings.Range.y);
            
            if (_reverse)
            {
                forward *= -1;
            }
            
            return _agent.transform.position + forward * distance;
        }
    }

    public class PathGeneratorInCircle : IPathGenerator
    {
        private readonly NavigationAgent _agent;
        private readonly NavigationBrainSettings _navigationBrainSettings;
        private readonly Vector3 _position;

        public PathGeneratorInCircle(NavigationAgent agent, NavigationBrainSettings navigationBrainSettings, Vector3 position)
        {
            _agent = agent;
            _navigationBrainSettings = navigationBrainSettings;
            _position = position;
        }

        public Path Generate()
        {
            var insideUnitCircle = Random.insideUnitCircle *
                                   Random.Range(_navigationBrainSettings.Range.x, _navigationBrainSettings.Range.y);
            var position = _position + new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
            return _agent.GeneratePathBy(position);
        }
    }

    public class PathGeneratorByAgentBehind : IPathGenerator
    {
        private readonly PathGeneratorByForward _pathGeneratorByForward;

        public PathGeneratorByAgentBehind(NavigationAgent agent, NavigationBrainSettings navigationBrainSettings)
        {
            _pathGeneratorByForward = new PathGeneratorByForward(agent, navigationBrainSettings, true);
        }

        public Path Generate()
        {
           return _pathGeneratorByForward.Generate();
        }
    }
    
    public class PathGeneratorAroundAgent : IPathGenerator
    {
        private readonly NavigationAgent _agent;
        private readonly NavigationBrainSettings _navigationBrainSettings;

        public PathGeneratorAroundAgent(NavigationAgent agent, NavigationBrainSettings navigationBrainSettings)
        {
            _agent = agent;
            _navigationBrainSettings = navigationBrainSettings;
        }

        public Path Generate()
        {
            var insideUnitCircle = Random.insideUnitCircle *
                                   Random.Range(_navigationBrainSettings.Range.x, _navigationBrainSettings.Range.y);
            var position = _agent.transform.position + new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
            return _agent.GeneratePathBy(position);
        }
    }

    public class PathGeneratorByTransform : IPathGenerator
    {
        private readonly NavigationAgent _agent;
        private readonly Transform _transform;

        public PathGeneratorByTransform(NavigationAgent agent, Transform transform)
        {
            _agent = agent;
            _transform = transform;
        }

        public Path Generate()
        {
            var position =_transform.position;
            return _agent.GeneratePathBy(position);
        }
    }
}