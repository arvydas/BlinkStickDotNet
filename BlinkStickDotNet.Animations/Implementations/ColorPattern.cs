using System;
using System.Drawing;

namespace BlinkStickDotNet.Animations.Implementations
{
    /// <summary>
    /// Shows the given color pattern for the specified amount of time.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.Wait" />
    public class ColorPattern : Wait
    {
        private Color[] _colors;
        private int _duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> class.
        /// </summary>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public ColorPattern(int duration, params Color[] colors) : base(duration)
        {
            if (colors.Length < 0)
            {
                throw new ArgumentNullException(nameof(colors));
            }

            this._duration = duration;
            this._colors = colors;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void Start(IBlinkStickColorProcessor processor)
        {
            processor.ProcessColors(_colors);
            base.Start(processor);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>
        /// The clone.
        /// </returns>
        public override IAnimation Clone()
        {
            return new ColorPattern(_duration, _colors);
        }
    }
}
