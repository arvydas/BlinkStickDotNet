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
            _duration = duration;
            _colors = colors;
        }

        /// <summary>
        /// Gets the current colors.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <returns>The current colors.</returns>
        private Color[] GetCurrentColors(IBlinkStickColorProcessor processor)
        {
            var currentColors = processor.GetCurrentColors();
            var nrOfColors = Math.Max(currentColors.Length, _colors.Length);
            return currentColors.CloneArray(nrOfColors);
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            var hz = 100;
            var steps = (((double)_duration) / 1000) * hz;
            var wait = _duration / steps;

            var colors = GetCurrentColors(processor);
            var destinationColors = _colors.CloneArray(colors.Length);
            var newColors = new List<Color>();

            for (int i = 0; i < steps; i++)
            {
                //calculate each color as a percentage move
                for (int c = 0; c < colors.Length; c++)
                {
                    var color = colors[c];
                    var destinationColor = destinationColors[c];

                    var r = (byte)(color.R + (destinationColor.R - color.R) / steps * i);
                    var g = (byte)(color.G + (destinationColor.G - color.G) / steps * i);
                    var b = (byte)(color.B + (destinationColor.B - color.B) / steps * i);

                    newColors.Add(Color.FromArgb(r, g, b));
                }

                processor.ProcessColors(newColors.ToArray());
                Thread.Sleep((int)wait);
                newColors.Clear();
            }

            //end with colors - helps to prevent rounding errors
            processor.ProcessColors(_colors);
        }

        public override IAnimation Clone()
        {
            return new MorphAnimation(_duration, _colors);
        }

    }
}
