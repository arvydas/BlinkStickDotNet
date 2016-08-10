using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Dims from the specified color to off.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.AnimationBase" />
    internal class DimAnimation : AnimationBase
    {
        private int _duration;
        private Color[] _colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DimAnimation"/> class.
        /// </summary>
        /// <param name="duration">The interval.</param>
        /// <param name="colors">The colors.</param>
        public DimAnimation(int duration, params Color[] colors)
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
            var hz = 50;
            var steps = ((double)_duration / 1000) * hz;
            var wait = _duration / steps;
            var amount = 1 / steps;
            var localAmount = 0d;

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
            return new DimAnimation(_duration, _colors);
        }
    }
}