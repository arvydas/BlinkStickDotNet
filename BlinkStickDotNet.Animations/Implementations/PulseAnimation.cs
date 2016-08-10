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
        private int _interval;
        private double _amount;
        private Color[] _colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulseAnimation"/> class.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="colors">The colors.</param>
        public PulseAnimation(int interval, double amount, params Color[] colors)
        {
            _interval = interval;
            _amount = amount;
            _colors = colors;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            var iteration = 1;
            var localAmount = 0d;

            while (true)
            {
                if (localAmount <= 1)
                {
                    var c = _colors.Darken(localAmount);
                    processor.ProcessColors(c);
                }

                iteration++;
                localAmount = iteration * (_amount / 4);

                if (localAmount > 1)
                {
                    break;
                }

                Thread.Sleep(_interval / 4);
            }

            while (true)
            {
                if (iteration == 0)
                {
                    break;
                }

                if (localAmount <= 1)
                {
                    var c = _colors.Darken(localAmount);
                    processor.ProcessColors(c);
                }

                iteration--;
                localAmount = iteration * (_amount / 4);

                Thread.Sleep(_interval / 4);
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
            return new DimAnimation(_interval, _amount, _colors);
        }
    }
}