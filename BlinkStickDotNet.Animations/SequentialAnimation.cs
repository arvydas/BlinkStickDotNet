using BlinkStickDotNet.Animations.Processors;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// This animation queue will run its animations sequential.
    /// </summary>
    public class SequentialAnimation : AnimatorBase, IAnimation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialAnimation"/> class.
        /// </summary>
        /// <param name="owner">The owner. Used for chaining.</param>
        public SequentialAnimation(IAnimationQueue owner = null) : base(owner)
        {
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(ILedProcessor processor)
        {
            Animations.ForEach(a => a.Start(processor));
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(IColorProcessor processor)
        {
            Animations.ForEach(a => a.Start(processor));
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public IAnimation Clone()
        {
            var animation = new SequentialAnimation(Owner);
            animation.Animations.AddRange(Animations);
            return animation;
        }
    }
}