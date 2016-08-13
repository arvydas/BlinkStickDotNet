using BlinkStickDotNet.Animations.Implementations;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Extend animation queues with extra features. Seperates the queue logic from
    /// the animation object creation logic.
    /// </summary>
    public static class AnimationQueueExtensions
    {
        /// <summary>
        /// Queues a pulse.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public static void Pulse(this IAnimationQueue queue, int duration, params Color[] colors)
        {
            var animation = new PulseAnimation(duration, colors);
            queue.Queue(animation);
        }

        public static void PulseInverted(this IAnimationQueue queue, int duration, params Color[] colors)
        {
            var animation = new PulseInvertedAnimation(duration, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a dim.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="fractionPercentage">The fraction percentage.</param>
        public static void Dim(this IAnimationQueue queue, int duration, double fractionPercentage = 1)
        {
            var animation = new DimAnimation(duration, fractionPercentage);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a chase.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public static void Chase(this IAnimationQueue queue, int duration, params Color[] colors)
        {
            var animation = new ChaseAnimation(duration, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues an off.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        public static void Off(this IAnimationQueue queue, int duration)
        {
            var animation = new ColorPattern(duration, System.Drawing.Color.Black);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a color.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public static void Color(this IAnimationQueue queue, int duration, params Color[] colors)
        {
            var animation = new ColorPattern(duration, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a wait.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        public static void Wait(this IAnimationQueue queue, int duration)
        {
            var animation = new Wait(duration);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a morph from the current color(s) to the specified color(s).
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        public static void Morph(this IAnimationQueue queue, int duration, params Color[] colors)
        {
            var animation = new MorphAnimation(duration, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues the specified animations.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="animations">The animations.</param>
        public static void Queue(this IAnimationQueue queue, IEnumerable<IAnimation> animations)
        {
            foreach (var animation in animations.ToList())
            {
                queue.Queue(animations);
            }
        }
    }
}