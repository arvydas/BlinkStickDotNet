using BlinkStickDotNet.Animations.Implementations;
using BlinkStickDotNet.Animations.Processors;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Extend animation queues with extra features. Seperates the queue logic from
    /// the animation object creation logic.
    /// </summary>
    public static class IAnimationQueueExtensions
    {
        private static readonly System.Drawing.Color Black = System.Drawing.Color.Black;

        /// <summary>
        /// Queues a pulse.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public static IAnimationQueue Pulse(this IAnimationQueue queue, uint duration, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            var animation = new ActionAnimation((p) => Pulse(p, duration, colors));

            return queue.Queue(animation);
        }

        /// <summary>
        /// Animated a pulse.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        public static void Pulse(ILedProcessor processor, uint duration, Color[] colors)
        {
            duration = duration / 2;

            //set to black
            Off(processor);

            //morph to color
            Morph(processor, duration, colors);

            //morph to black
            Morph(processor, duration, new Color[] { Black });
        }

        /// <summary>
        /// Queues an inverted pulse.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue PulseInverted(this IAnimationQueue queue, uint duration, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            var animation = new ActionAnimation((p) => PulseInverted(p, duration, colors));

            return queue.Queue(animation);
        }

        /// <summary>
        /// Animates an inverted pulse.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        public static void PulseInverted(ILedProcessor processor, uint duration, Color[] colors)
        {
            duration = duration / 2;

            //set to black
            Morph(processor, 0, colors);

            //morph to color
            Morph(processor, duration, Black.PadBlack(processor.Leds.Length));

            //morph to black
            Morph(processor, duration, colors);
        }

        /// <summary>
        /// Queues a chase.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Chase(this IAnimationQueue queue, uint duration, params Color[] colors)
        {
            return Chase(queue, duration, 1, colors);
        }

        /// <summary>
        /// Queues a dim.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="fractionPercentage">The fraction percentage.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Dim(this IAnimationQueue queue, uint duration, double fractionPercentage = 1)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));


            var animation = new ActionAnimation((p) => Dim(p, duration, fractionPercentage));

            return queue.Queue(animation);
        }

        /// <summary>
        /// Animates a dim.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="percentage">The percentage.</param>
        public static void Dim(ILedProcessor processor, uint duration, double percentage)
        {
            var colors = processor.Leds.Select(c => c.Color.Darken(percentage)).ToArray();
            Morph(processor, duration, colors);
        }

        /// <summary>
        /// Chases the specified processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="direction">The direction.</param>
        public static void Chase(ILedProcessor processor, uint duration, int direction = 1)
        {
            direction = Math.Max(-1, Math.Min(1, direction));

            var wait = duration / processor.Leds.Length;

            var total = processor.Leds.Length;
            for (var x = 0; x < total; x++)
            {
                for (int i = 0; i < total; i++)
                {
                    var led = processor.Leds[i];

                    int l = (int)(led.LedNr + direction + total) % processor.Leds.Length;
                    led.LedNr = (uint)l;
                }

                processor.Process();
                Thread.Sleep((int)wait);
            }
        }

        /// <summary>
        /// Queues a chase.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Chase(this IAnimationQueue queue, uint duration, int direction,  params Color[] colors)
        {
            return queue.Chase(duration, direction, 1, colors);
        }

        /// <summary>
        /// Queues a chase.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Chase(this IAnimationQueue queue, uint duration, int direction, uint spins, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            var animation = new ActionAnimation((p) =>
            {
                if (colors != null && colors.Length > 0)
                {
                    Morph(p, 0, colors);
                }

                var d = duration / spins;
                for (int i = 0; i < spins; i++)
                {
                    Chase(p, d, direction);
                }
            });

            return queue.Queue(animation);
        }

        /// <summary>
        /// Queues an off.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Off(this IAnimationQueue queue, uint duration = 0)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            return queue.Color(duration, Black);
        }

        /// <summary>
        /// Turns the leds off.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public static void Off(ILedProcessor processor)
        {
            foreach(var led in processor.Leds)
            {
                led.Color = Black;
            }

            processor.Process();
        }

        /// <summary>
        /// Queues a color.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Color(this IAnimationQueue queue, uint duration, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            var animation = new ActionAnimation((p) =>
            {
                Morph(p, 0, colors);
                if (duration > 0)
                {
                    Thread.Sleep((int)duration);
                }
            });

            return queue.Queue(animation);
        }

        /// <summary>
        /// Queues a color.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Color(this IAnimationQueue queue, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            return queue.Color(0, colors);
        }

        /// <summary>
        /// Queues a wait.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration in ms.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Wait(this IAnimationQueue queue, uint duration)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            var animation = new Wait(duration);
            return queue.Queue(animation);
        }

        /// <summary>
        /// Queues a morph from the current color(s) to the specified color(s).
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Morph(this IAnimationQueue queue, uint duration, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            //var animation = new MorphAnimation(duration, colors);

            var animation = new ActionAnimation((p) => Morph(p, (uint)duration, colors));
            return queue.Queue(animation);
        }

        /// <summary>
        /// Animates a morphs to the specified colors.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="destinationColors">The destination colors.</param>
        public static void Morph(ILedProcessor processor, uint duration, Color[] destinationColors)
        {
            var leds = processor.Leds.OrderBy(l => l.OrignalLedNr).ToArray();
            var colors = processor.Leds.Select(l => l.Color).ToArray();
            var nrOfColors = leds.Length;

            colors = colors.CloneArray(nrOfColors);
            destinationColors = destinationColors.CloneArray(nrOfColors);

            if (duration > 0)
            {
                var hz = 100;
                var steps = (((double)duration) / 1000) * hz;
                var wait = duration / steps;

                for (int i = 0; i < steps; i++)
                {
                    //calculate each color as a percentage move
                    for (int c = 0; c < colors.Length; c++)
                    {
                        var color = colors[c];
                        var destinationColor = destinationColors[c];

                        var r = (byte)Math.Round(color.R + (destinationColor.R - color.R) / steps * i, MidpointRounding.AwayFromZero);
                        var g = (byte)Math.Round(color.G + (destinationColor.G - color.G) / steps * i, MidpointRounding.AwayFromZero);
                        var b = (byte)Math.Round(color.B + (destinationColor.B - color.B) / steps * i, MidpointRounding.AwayFromZero);

                        leds[c].Color = System.Drawing.Color.FromArgb(r, g, b);
                    }

                    processor.Process();
                    Thread.Sleep((int)wait);
                }
            }

            //end with colors - helps to prevent rounding errors
            for (var i = 0; i < leds.Length; i++)
            {
                leds[i].Color = destinationColors[i];
            }

            processor.Process();
        }

        /// <summary>
        /// Chases the dimmer animation.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="spins">The spins.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue ChaseDimmer(this IAnimationQueue queue, uint duration, uint spins, params Color[] colors)
        {
            return queue.ChaseDimmer(duration, spins, 1, colors);
        }

        /// <summary>
        /// Chases the dimmer animation.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="spins">The spins.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue ChaseDimmer(this IAnimationQueue queue, uint duration, uint spins, int direction, params Color[] colors) 
        {
            return queue
                .BeginParallel()
                    .Chase(duration, direction, spins, colors)
                    .Dim(duration)
                    .End();
        }

        /// <summary>
        /// Chases the dimmer animation.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue ChaseDimmer(this IAnimationQueue queue, uint duration, params Color[] colors)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            return queue.ChaseDimmer(duration, 1, colors);
        }

        /// <summary>
        /// Queues a Set on an event wait handle.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="handle">The handle.</param>
        /// <returns>The queue for chaining.</returns>
        public static IAnimationQueue Set(this IAnimationQueue queue, EventWaitHandle handle)
        {
            return queue.Queue(new SetEventWaitHandle(handle));
        }

        /// <summary>
        /// Queues a loop.
        /// </summary>
        /// <param name="queue">The queue.</param>
        public static void Loop(this IAnimationQueue queue)
        {
            queue.Queue(new LoopAnimation());
        }
    }
}