using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Libraries.DNV.Extensions;
using Libraries.DNV.CodePresets.Rules.Interfaces;
using Libraries.DNV.Logging;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Libraries.DNV.CodePresets.Sequence
{
    [Serializable]
    public class AsyncDelay : ISequenceAwaiter
    {
        [SerializeField]
        private float _time;
        
        public async UniTask Wait(CancellationToken token)
        {
            var seconds = TimeSpan.FromSeconds(_time);
            
            Log.Info(LogCategory.Core, $"{seconds.Seconds}, {seconds.Milliseconds}");
            await UniTask.Delay(seconds, cancellationToken: token);
        }
    }

    public class Delay : IDelay
    {
        private readonly float _targetDelay;
        private float _currentValue;
        
        public bool IsExecuted => Execute();
        
        public float Value => _currentValue;
        public float NormalizedValue => (_currentValue / _targetDelay).Normalize();

        public Delay(float targetDelay)
        {
            _targetDelay = targetDelay;
        }
        
        private bool Execute()
        {
            if (_currentValue >= _targetDelay)
            {
                return true;
            }
            
            _currentValue += Time.deltaTime;
            return false;
        }

        public void Reset()
        {
            _currentValue = 0;
        }
    }
    
    
    public class TransitionDelay : ICondition
    {
        private readonly IDelay _delay;

        public bool IsExecuted => _delay.IsExecuted;
        
        public TransitionDelay(IDelay delay)
        {
            _delay = delay;
        }
        
        public void Reset()
        {
            _delay.Reset();
        }
    }
    

    public class NavigationBreakDelay: ICondition
    {
        private float _targetDelay;
        private float _currentValue;

        private readonly float _minimalDelay;
        private readonly float _maximalDelay;

        public bool IsExecuted => Wait();

        public NavigationBreakDelay(float minimalDelay, float maximalDelay)
        {
            _minimalDelay = minimalDelay;
            _maximalDelay = maximalDelay;
        }
        
        private bool Wait()
        {
            if (_currentValue >= _targetDelay)
            {
                return true;
            }
            
            _currentValue += Time.deltaTime;
            return false;
        }
        
        public void Reset()
        {
            _currentValue = 0;
            _targetDelay = Random.Range(_minimalDelay, _maximalDelay);
        }
    }
}