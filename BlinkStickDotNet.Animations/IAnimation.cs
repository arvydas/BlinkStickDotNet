namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Indicates the object implements an animation.
    /// </summary>
    public interface IAnimation
    {
        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        void Start(IBlinkStickColorProcessor processor);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The clone.</returns>
        IAnimation Clone();
    }
}