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
        /// <param name="interval">The interval.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="colors">The colors.</param>
        public static void QueuePulse(this IAnimationQueue queue, int interval, double amount, params Color[] colors)
        {
            var animation = new PulseAnimation(interval, amount, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a dim.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="colors">The colors.</param>
        public static void QueueDim(this IAnimationQueue queue, int interval, double amount, params Color[] colors)
        {
            var animation = new DimAnimation(interval, amount, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a chase.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="colors">The colors.</param>
        public static void QueueChase(this IAnimationQueue queue, int interval, params Color[] colors)
        {
            var animation = new ChaseAnimation(interval, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues an off.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="durationMilliseconds">The duration milliseconds.</param>
        public static void QueueOff(this IAnimationQueue queue, int durationMilliseconds)
        {
            var animation = new ColorPattern(durationMilliseconds, Color.Black);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a color.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="durationMilliseconds">The duration milliseconds.</param>
        /// <param name="colors">The colors.</param>
        public static void QueueColor(this IAnimationQueue queue, int durationMilliseconds, params Color[] colors)
        {
            var animation = new ColorPattern(durationMilliseconds, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Queues a wait.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        public static void QueueWait(this IAnimationQueue queue, int millisecondsTimeout)
        {
            var animation = new Wait(millisecondsTimeout);
            queue.Queue(animation);
        }
    }
}