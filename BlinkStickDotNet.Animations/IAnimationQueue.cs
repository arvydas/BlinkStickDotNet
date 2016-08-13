using System.Collections.Generic;

namespace BlinkStickDotNet.Animations
{
    public interface IAnimationQueue : IEnumerable<IAnimation>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }

        /// <summary>
        /// Queues the specified animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        void Queue(IAnimation animation);
        
        /// <summary>
        /// Connects the specified stick.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="nrOfLeds">The nr of leds.</param>
        void Connect(BlinkStick stick, uint nrOfLeds);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Starts the animation.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="turnOff">if set to <c>true</c> if the stick should be turned off.</param>
        void Stop(bool turnOff = false);
    }
}