using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.Animations.Implementations
{
    /// <summary>
    /// A chase animation is created by offsetting the color for each led. 
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.AnimationBase" />
    public class ChaseAnimation : AnimationBase
    {
        private int _duration;
        private Color[] _colors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaseAnimation" /> class.
        /// </summary>
        /// <param name="duration">The duration in ms.</param>
        /// <param name="colors">The colors.</param>
        public ChaseAnimation(int duration, params Color[] colors)
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
            int offset = 0;
            int nr = 0;

            while (true)
            {
                //check if chase is completed
                if (nr == processor.NrOfLeds)
                {
                    break;
                }

                processor.ProcessColors(offset, _colors);
                offset = (offset + 1) % _colors.Length;
                nr++;

                Thread.Sleep(_duration / processor.NrOfLeds);
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
            return new ChaseAnimation(_duration, _colors);
        }
    }
}