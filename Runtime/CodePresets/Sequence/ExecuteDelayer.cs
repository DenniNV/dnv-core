using System;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Sequence
{
    public class ExecuteDelayer
    {
        private readonly float _delay;
        private float _currentDelay;

        private bool IsCompleted => _currentDelay <= 0;

        private event Action Completed;

        public ExecuteDelayer(float delay)
        {
            _delay = delay;
            Reset();
        }

        public void Reset()
        {
            _currentDelay = _delay;
        }

        public void Execute()
        {
            _currentDelay -= Time.deltaTime;

            if (IsCompleted)
            {
                Completed?.Invoke();
                Reset();
            }
        }

        public void AddCompletedHandler(Action action)
        {
            Completed += action;
        }
        
        public void RemoveCompletedHandler(Action action)
        {
            Completed -= action;
        }
    }
}