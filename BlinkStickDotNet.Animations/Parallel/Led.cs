using System.Drawing;

namespace BlinkStickDotNet.Animations.Parallel
{
    public class Led
    {
        public Led(uint ledNr, Color color)
        {
            this.LedNr = ledNr;
            this.Color = color;
        }

        public uint LedNr { get; set; }

        public Color Color { get; set; }

    }
}
