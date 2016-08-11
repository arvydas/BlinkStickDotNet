using System.Drawing;

namespace BlinkStickDotNet.Animations.Implementations
{
    public class BrightenAnimation : AnimationBase
    {
        private int _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrightenAnimation"/> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public BrightenAnimation(int duration)
        {
            _duration = duration;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public override IAnimation Clone()
        {
            return new BrightenAnimation(_duration);
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            MorphAnimation.Morph(processor, _duration, Color.White);
        }
    }
}
