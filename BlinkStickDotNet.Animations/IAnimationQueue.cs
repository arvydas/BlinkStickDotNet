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
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        void Start(IBlinkStickColorProcessor processor);

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}