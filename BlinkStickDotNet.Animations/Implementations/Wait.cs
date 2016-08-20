using BlinkStickDotNet.Animations.Processors;
using System.Threading;
using System;

namespace BlinkStickDotNet.Animations.Implementations
{
    /// <summary>
    /// Waits for the specified amount of time and continues.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.AnimationBase" />
    public class Wait : IAnimation
    {
        private uint _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wait"/> class.
        /// </summary>
        /// <param name="duration">The duration in ms timeout.</param>
        public Wait(uint duration)
        {
            _duration = duration;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(IColorProcessor processor)
        {
            if (_duration > 0)
            {
                Thread.Sleep((int)_duration);
            }
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(ILedProcessor processor)
        {
            if (_duration > 0)
            {
                Thread.Sleep((int)_duration);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public IAnimation Clone()
        {
            return new Wait(_duration);
        }
    }
}