using BlinkStickDotNet.Animations.Implementations;
using BlinkStickDotNet.Animations.Processors;
using System.Collections.Generic;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// This animation queue will run its animations sequential.
    /// </summary>
    public class SequentialAnimation : AnimatorBase, IAnimation
    {
        private bool loopHasBeenQueued = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequentialAnimation"/> class.
        /// </summary>
        /// <param name="owner">The owner. Used for chaining.</param>
        public SequentialAnimation(IAnimationQueue owner = null) : base(owner)
        {
        }

        /// <summary>
        /// Gets the animations.
        /// </summary>
        /// <returns>The animations</returns>
        public IEnumerable<IAnimation> GetAnimations()
        {
            return this.Animations.AsReadOnly();
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(ILedProcessor processor)
        {
            foreach(var animation in Animations)
            {
                if(animation is LoopAnimation)
                {
                    if (!loopHasBeenQueued)
                    {
                        loopHasBeenQueued = true;
                        Owner?.Queue(animation);
                    }
                    break;
                }

                animation.Start(processor);
            }
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(IColorProcessor processor)
        {
            foreach (var animation in Animations)
            {
                if (animation is LoopAnimation)
                {
                    if (!loopHasBeenQueued)
                    {
                        loopHasBeenQueued = true;
                        Owner?.Queue(animation);
                    }

                    break;
                }

                animation.Start(processor);
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
            var animation = new SequentialAnimation(Owner);
            animation.Animations.AddRange(Animations);
            return animation;
        }
    }
}