using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Pulse animation.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.AnimationBase" />
    internal class PulseAnimation : AnimationBase
    {
        private int _duration;
        private Color[] _colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulseAnimation" /> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        public PulseAnimation(int duration, params Color[] colors)
        {
            _duration = duration;
            _colors = colors;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            var hz = 100;
            var steps = ((double)(_duration / 2) / 1000) * hz;
            var wait = _duration / steps;
            var amount = 1 / steps;
            var localAmount = 1d;

            while (true)
            {
                var c = _colors.Darken(localAmount);
                processor.ProcessColors(c);

                if (localAmount <= 0)
                {
                    break;
                }

                localAmount -= amount;

                Thread.Sleep((int)wait);
            }

            while (true)
            {
                var c = _colors.Darken(localAmount);
                processor.ProcessColors(c);

                if (localAmount >= 1)
                {
                    break;
                }

                localAmount += amount;

                Thread.Sleep((int)wait);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public override IAnimation Clone()
        {
            return new PulseAnimation(_duration, _colors);
        }
    }
}