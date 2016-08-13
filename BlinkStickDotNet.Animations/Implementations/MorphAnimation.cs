using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Animations.Implementations
{
    public class MorphAnimation : AnimationBase
    {
        private Color[] _colors;
        private int _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> class.
        /// </summary>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public MorphAnimation(int duration, params Color[] colors)
        {
            if (colors.Length < 0)
            {
                throw new ArgumentNullException(nameof(colors));
            }

            _duration = duration;
            _colors = colors;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            Morph(processor, _duration, _colors); 
        }

        /// <summary>
        /// Morphes to the colors.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="destinationColors">The destination colors.</param>
        public static void Morph(IBlinkStickColorProcessor processor, int duration, params Color[] destinationColors)
        {
            if (duration > 1)
            {
                var hz = 100;
                var steps = (((double)duration) / 1000) * hz;
                var wait = duration / steps;

                var colors = processor.GetCurrentColors();
                var nrOfColors = Math.Max(colors.Length, destinationColors.Length);

                colors = colors.CloneArray(nrOfColors);
                destinationColors = destinationColors.CloneArray(nrOfColors);

                var newColors = new List<Color>();

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

                        newColors.Add(Color.FromArgb(r, g, b));
                    }

                    processor.ProcessColors(newColors.ToArray());
                    Thread.Sleep((int)wait);
                    newColors.Clear();
                }
            }

            //end with colors - helps to prevent rounding errors
            processor.ProcessColors(destinationColors);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public override IAnimation Clone()
        {
            return new MorphAnimation(_duration, _colors);
        }
    }
}
