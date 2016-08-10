namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Base class for animations.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.IAnimation" />
    public abstract class AnimationBase : IAnimation
    {
        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public abstract void Start(IBlinkStickColorProcessor processor);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public abstract IAnimation Clone();
    }
}
