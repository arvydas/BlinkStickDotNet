using BlinkStickDotNet.Animations.Processors;
using System;

namespace BlinkStickDotNet.Animations.Implementations
{
    public sealed class LoopAnimation : IAnimation
    {
        public IAnimation Clone()
        {
            return new LoopAnimation();
        }

        public void Start(ILedProcessor processor)
        {
            throw new NotImplementedException("Looping should be implemented and handles by the IAnimationQueue.");
        }

        public void Start(IColorProcessor processor)
        {
            throw new NotImplementedException("Looping should be implemented and handles by the IAnimationQueue.");
        }
    }
}
