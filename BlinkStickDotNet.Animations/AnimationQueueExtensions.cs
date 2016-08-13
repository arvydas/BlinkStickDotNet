using BlinkStickDotNet.Animations.Implementations;
using System;
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
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            var animation = new PulseAnimation(duration, colors);
            queue.Queue(animation);
        }

        public static void PulseInverted(this IAnimationQueue queue, int duration, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

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
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

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
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

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
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

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
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

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
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

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
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

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
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            if (animations == null)
                throw new ArgumentNullException(nameof(animations));

            foreach (var animation in animations.ToList())
            {
                queue.Queue(animation);
            }
        }

        /// <summary>
        /// Queues one or more repeats of the last animation.
        /// </summary>
        /// <param name="nrOfTimes">The nr of times.</param>
        public static void Repeat(this IAnimationQueue queue, int nrOfTimes = 1)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            if (queue.FirstOrDefault() == null)
                throw new Exception("Can't repeat. No animations queued.");

            if (nrOfTimes > 0)
            {
                var animation = queue.LastOrDefault();
                if (animation != null)
                {
                    for (int i = 0; i < nrOfTimes; i++)
                    {
                        queue.Queue(animation.Clone());
                    }
                }
            }
        }

        /// <summary>
        /// Queues a repeat of the current queue.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="nrOfTimes">The nr of times.</param>
        public static void RepeatQueue(this IAnimationQueue queue, int nrOfTimes = 1)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            if (queue.FirstOrDefault() == null)
                throw new Exception("Can't repeat. No animations queued.");

            if (nrOfTimes > 0)
            {
                var list = queue.ToList();

                if (list.Count > 0)
                {
                    for (int i = 0; i < nrOfTimes; i++)
                    {
                        list.ForEach(a => queue.Queue(a.Clone()));
                    }
                }
            }
        }

        /// <summary>
        /// Loops the queue.
        /// </summary>
        /// <param name="queue"></param>
        public static void Loop(this IAnimationQueue queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            if (queue.FirstOrDefault() == null)
                throw new Exception("Can't loop. No animations queued.");

            queue.Queue(new LoopAnimation());
        }

        /// <summary>
        /// Chases the dimmer animation.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="spins">The spins.</param>
        /// <param name="colors">The colors.</param>
        public static void ChaseDimmer(this IAnimationQueue queue, int duration, int spins, params Color[] colors) 
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            var animation = new ChaseDimmerAnimation(duration, spins, colors);
            queue.Queue(animation);
        }

        /// <summary>
        /// Chases the dimmer animation.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        public static void ChaseDimmer(this IAnimationQueue queue, int duration, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            queue.ChaseDimmer(duration, 1, colors);
        }
    }
}