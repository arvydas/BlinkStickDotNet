using System;

namespace BlinkStickDotNet.Animations.Implementations
{
    public sealed class LoopAnimation : IAnimation
    {
        public IAnimation Clone()
        {
            return new LoopAnimation();
        }

        public void Start(IBlinkStickColorProcessor processor)
        {
            throw new NotImplementedException("Looping should be implemented and handles by the IAnimationQueue.");
        }
    }
}
