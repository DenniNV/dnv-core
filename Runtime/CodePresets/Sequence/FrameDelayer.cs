using Libraries.DNV.CodePresets.Sequence;
using UnityEngine;

namespace Libraries.DNV.CodePresets.Sequence
{
    public class FrameDelayer : IDelay
    {
        private readonly int _delayFrames;
        private int _targetDelayFrame;

        public bool IsExecuted => Time.frameCount >= _targetDelayFrame;
        
        public FrameDelayer(int delayFrames)
        {
            _delayFrames = delayFrames;
        }

        public void Reset()
        {
            _targetDelayFrame = Time.frameCount + _delayFrames;
        }
    }
}