using System;
using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Animations.Implementations
{
    /// <summary>
    /// Dims from the specified color to off.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.AnimationBase" />
    public class DimAnimation : AnimationBase
    {
        private int _duration;
        private double _percentageFraction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DimAnimation"/> class.
        /// </summary>
        /// <param name="duration">The interval.</param>
        /// <param name="colors">The colors.</param>
        public DimAnimation(int duration, double percentageFraction = 1)
        {
            _duration = duration;
            _percentageFraction = percentageFraction;
        }

        public static double Fract(double percentageFraction)
        {
            return Math.Max(1, Math.Min(0, percentageFraction));
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            var colors = processor.GetCurrentColors().Darken(_percentageFraction);
            MorphAnimation.Morph(processor, _duration, colors);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public override IAnimation Clone()
        {
            return new DimAnimation(_duration, _percentageFraction);
        }
    }
}