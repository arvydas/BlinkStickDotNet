using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Animations.Implementations
{
    /// <summary>
    /// Pulse animation.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.AnimationBase" />
    public class PulseInvertedAnimation : AnimationBase
    {
        private int _duration;
        private Color[] _colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulseAnimation" /> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="colors">The colors.</param>
        public PulseInvertedAnimation(int duration, params Color[] colors)
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
            var duration = _duration / 2;

            var colors = _colors;
            if (_colors.Length == 0)
            {
                colors = processor.GetCurrentColors();
            }

            //set to color
            processor.ProcessColors(colors);

            //morph to black
            MorphAnimation.Morph(processor, duration, Color.Black);

            //morph to color
            MorphAnimation.Morph(processor, duration, colors);
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