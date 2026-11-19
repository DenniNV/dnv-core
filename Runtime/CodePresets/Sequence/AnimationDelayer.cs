using Libraries.DNV.CodePresets.Sequence;

namespace Libraries.DNV.CodePresets.Sequence
{
    public class AnimationDelayer : IDelay
    {
        private readonly FrameDelayer _frameDelayer;

        public bool IsExecuted => _frameDelayer.IsExecuted;

        public AnimationDelayer(int delayFrame)
        {
            _frameDelayer = new FrameDelayer(delayFrame * 2);
        }
        
        public void Reset()
        {
            _frameDelayer.Reset();
        }
    }
}