using System;
using Libraries.DNV.CodePresets.Pools.Effects;
using Libraries.DNV.CodePresets.Pools.Interface;
using Libraries.DNV.Logging;

namespace Libraries.DNV.CodePresets.Pools.Rules
{
    [Serializable]
    public sealed class ParticleFinishedRule : IReturnRule<PooledEffect>
    {
        public IReturnState<PooledEffect> CreateState()
        {
            return new State();
        }

        private sealed class State : IReturnState<PooledEffect>
        {
            private IPlayback _playback;

            public void OnRent(PooledEffect item)
            {
                // Каст один раз на аренду, а не каждый кадр в ShouldReturn.
                _playback = item as IPlayback;

                if(_playback == null)
                {
                    Log.Warning(LogCategory.Pool,
                        $"'{item.name}' has no {nameof(IPlayback)}, so {nameof(ParticleFinishedRule)} " +
                        "cannot judge it and returns it at once. Use a duration rule instead.");
                }
            }

            public bool ShouldReturn(PooledEffect item, float deltaTime)
            {
                return _playback is not { IsPlaying: true };
            }
        }
    }
}
