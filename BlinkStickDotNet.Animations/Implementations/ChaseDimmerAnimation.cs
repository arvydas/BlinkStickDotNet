using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace BlinkStickDotNet.Animations.Implementations
{
    public class ChaseDimmerAnimation : AnimationBase
    {
        private int _duration;
        private int _spins;
        private Color[] _colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaseAnimation" /> class.
        /// </summary>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public ChaseDimmerAnimation(int duration, params Color[] colors) : this(duration, 1, colors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaseAnimation" /> class.
        /// </summary>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public ChaseDimmerAnimation(int duration, int spins, params Color[] colors)
        {
            if (colors.Length < 0)
            {
                throw new ArgumentNullException(nameof(colors));
            }

            _duration = duration;
            _colors = colors;
            _spins = Math.Max(1, spins);
        }


        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            int offset = 0;
            int nr = 0;

            var rounds = (int)processor.NrOfLeds * _spins;
            var darkenAmount = 1d / (rounds - 1);
            var colours = _colors;

            while (true)
            {
                processor.ProcessColors(offset, _colors.Darken(darkenAmount * nr));
                offset = (offset + 1) % _colors.Length;
                nr++;

                Thread.Sleep(_duration / rounds);

                //check if chase is completed
                if (nr == rounds)
                {
                    break;
                }
            }

            processor.ProcessColors(Color.Black);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public override IAnimation Clone()
        {
            return new ChaseDimmerAnimation(_duration, _spins, _colors);
        }
    }
}