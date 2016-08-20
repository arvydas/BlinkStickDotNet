using System.Drawing;

namespace BlinkStickDotNet.Animations.Processors
{
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

        public uint OrignalLedNr { get; private set; }

        public uint LedNr { get; set; }

        public Color Color { get; set; }
    }
}
