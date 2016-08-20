namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Indicates the object implements a queue with animations.
    /// </summary>
    public interface IAnimationQueue
    {
        /// <summary>
        /// Queues the specified animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <returns>Queue for chaining.</returns>
        IAnimationQueue Queue(IAnimation animation);

        /// <summary>
        /// Begins a parallel chain.
        /// </summary>
        /// <returns>The parallel queue for chaining.</returns>
        IAnimationQueue BeginParallel();

        /// <summary>
        /// Begins a sequencial chain.
        /// </summary>
        /// <returns>The sequencial queue for chaining.</returns>
        IAnimationQueue BeginSequencial();

        /// <summary>
        /// Returns the owner animation queue.
        /// </summary>
        /// <returns>The owner queue for chaining.</returns>
        IAnimationQueue End();

        /// <summary>
        /// Pops the specified nr of items.
        /// </summary>
        /// <param name="nrOfItems">The nr of items.</param>
        /// <returns>Queue for chaining.</returns>
        IAnimationQueue Pop(uint nrOfItems = 1);

        /// Queues one or more repeats of the last animation.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        /// <returns>Queue for chaining.</returns>
        IAnimationQueue Repeat(uint nrOfTimes = 1);

        /// <summary>
        /// Queues a repeat of the current queue.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        /// <returns>
        /// Queue for chaining.
        /// </returns>
        IAnimationQueue RepeatAll(uint nrOfTimes = 1);
    }
}