using Libraries.DNV.Core;
using LazySquirrelLabs.MinMaxRangeAttribute;
using UnityEngine;

namespace Libraries.DNV.Navigation
{
    [CreateAssetMenu(fileName = nameof(NavigationBrainSettings), menuName = ConfigPath.Unit + nameof(NavigationBrainSettings))]
    public class NavigationBrainSettings : ScriptableObject
    {
        [Range(0.1f, 1f)]
        [SerializeField] private float _breakDistance;

        [MinMaxRange(1f, 15f)]
        [SerializeField] private Vector2 _range;

        [MinMaxRange(0.1f, 3f)]
        [SerializeField] private Vector2 _delayTime;

        public Vector2 DelayTime => _delayTime;
        public Vector2 Range => _range;
        public float BreakDistance => _breakDistance;

    }
}