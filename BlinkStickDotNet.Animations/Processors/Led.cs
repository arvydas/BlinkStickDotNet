using System.Drawing;

namespace BlinkStickDotNet.Animations.Processors
{
    /// <summary>
    /// Manages a (virtual) led.
    /// </summary>
    public class Led
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Led"/> class.
        /// </summary>
        /// <param name="ledNr">The led nr.</param>
        /// <param name="color">The color.</param>
        public Led(uint ledNr, Color color)
        {
            this.OrignalLedNr = ledNr;
            this.LedNr = ledNr;
            this.Color = color;
        }

        /// <summary>
        /// Gets the orignal led nr.
        /// </summary>
        /// <value>
        /// The orignal led nr.
        /// </value>
        public uint OrignalLedNr { get; private set; }

        /// <summary>
        /// Gets or sets the led nr.
        /// </summary>
        /// <value>
        /// The led nr.
        /// </value>
        public uint LedNr { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color { get; set; }
    }
}
