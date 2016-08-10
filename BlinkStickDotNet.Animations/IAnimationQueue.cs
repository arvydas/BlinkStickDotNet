namespace BlinkStickDotNet.Animations
{
    public interface IAnimationQueue
    {
        /// <summary>
        /// Gets a value indicating whether this instance is looping.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is looping; otherwise, <c>false</c>.
        /// </value>
        bool IsLooping { get; }

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
        /// Queues a repeat of the last animation.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        void Repeat(int nrOfTimes = 1);

        /// <summary>
        /// Queues a repeat of the current queue.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        void RepeatQueue(int nrOfTimes = 1);

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