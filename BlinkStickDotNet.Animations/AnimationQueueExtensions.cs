using System.Drawing;

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

        /// <summary>
        /// Queues a dim.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public static void Dim(this IAnimationQueue queue, int duration, params Color[] colors)
        {
            var animation = new DimAnimation(duration, colors);
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
    }
}