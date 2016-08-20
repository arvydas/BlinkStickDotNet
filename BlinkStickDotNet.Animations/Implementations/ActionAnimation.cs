using BlinkStickDotNet.Animations.Processors;
using System;

namespace BlinkStickDotNet.Animations.Implementations
{
    /// <summary>
    /// Wraps an action for a parallel processor.
    /// </summary>
    public class ActionAnimation : IAnimation
    {
        Action<ILedProcessor> _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionAnimation"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public ActionAnimation(Action<ILedProcessor> action)
        {
            _action = action;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(IColorProcessor processor)
        {
            Start(new LedProcessor(processor));
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public void Start(ILedProcessor processor)
        {
            _action(processor);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public IAnimation Clone()
        {
            return new ActionAnimation(_action);
        }
    }
}