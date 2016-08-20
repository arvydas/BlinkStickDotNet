using System.Collections.Generic;
using BlinkStickDotNet.Animations.Processors;

namespace BlinkStickDotNet.Animations.Implementations
{
    /// <summary>
    /// Creates a sequence of animations that is executed.
    /// </summary>
    public class SequentialAnimation : AnimationQueueBase, IAnimation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialAnimation"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public SequentialAnimation(IAnimationQueue owner) : base(owner)
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