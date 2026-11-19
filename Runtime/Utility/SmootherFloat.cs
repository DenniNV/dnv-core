using UnityEngine;

namespace Libraries.DNV.Utility
{
    public sealed class SmootherFloat
    {
        private float _value;
        private float _targetValue;
        
        private readonly float _time;

        public float Value => GetSmoothValue();
      
        public SmootherFloat(float targetValue, float time)
        {
            _targetValue = targetValue;
            _time = time;
        }
        
        public void ChangeTargetValue(float targetValue)
        {
            _targetValue = targetValue;
        }

        public void ResetTo(float value)
        {
            _value = value;
        }

        private float GetSmoothValue()
        {
            _value += (_targetValue - _value)
                      * (1 - Mathf.Exp(-Time.deltaTime / _time));

            return _value;
        }
    }
}