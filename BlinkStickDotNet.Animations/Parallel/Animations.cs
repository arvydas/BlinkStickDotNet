using System;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BlinkStickDotNet.Animations.Parallel
{
    public static class Animations
    {
        public static void Chase(IParallelProcessor processor, uint duration, int direction = 1)
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
        /// Animates an inverted pulse.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        public static void PulseInverted(IParallelProcessor processor, uint duration, Color[] colors)
        {
            duration = duration / 2;

            //set to black
            Morph(processor, 1, colors);

            //morph to color
            Morph(processor, duration, Color.Black.PadBlack(processor.Leds.Length));

            //morph to black
            Morph(processor, duration, colors);
        }

        /// <summary>
        /// Animated a pulse.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        public static void Pulse(IParallelProcessor processor, uint duration, Color[] colors)
        {
            duration = duration / 2;

            //set to black
            Morph(processor, 1, Color.Black.PadBlack(processor.Leds.Length));

            //morph to color
            Morph(processor, duration, colors);

            //morph to black
            Morph(processor, duration, new Color[] { Color.Black });
        }

        /// <summary>
        /// Animates a darken.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="percentage">The percentage.</param>
        public static void Darken(IParallelProcessor processor, uint duration, double percentage)
        {
            var colors = processor.Leds.Select(c => c.Color.Darken(percentage)).ToArray();
            Morph(processor, duration, colors);
        }

        /// <summary>
        /// Animates a morphs to the specified colors.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="destinationColors">The destination colors.</param>
        public static void Morph(IParallelProcessor processor, uint duration, Color[] destinationColors)
        {
            var leds = processor.Leds.OrderBy(l => l.LedNr).ToArray();
            var colors = processor.Leds.Select(l => l.Color).ToArray();
            var nrOfColors = leds.Length;

            colors = colors.CloneArray(nrOfColors);
            destinationColors = destinationColors.CloneArray(nrOfColors);

            if (duration > 1)
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

                        leds[c].Color = Color.FromArgb(r, g, b);
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
    }
}