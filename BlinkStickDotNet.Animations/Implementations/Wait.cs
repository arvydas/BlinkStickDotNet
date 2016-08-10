using System.Threading;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Waits for the specified amount of time and continues.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.AnimationBase" />
    public class Wait : AnimationBase
    {
        private int _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wait"/> class.
        /// </summary>
        /// <param name="duration">The duration in ms timeout.</param>
        public Wait(int duration)
        {
            _duration = duration;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            Thread.Sleep(_duration);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public override IAnimation Clone()
        {
            return new Wait(_duration);
        }
    }
}