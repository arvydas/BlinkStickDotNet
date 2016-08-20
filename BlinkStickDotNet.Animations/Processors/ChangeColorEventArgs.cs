using System;
using System.Drawing;

namespace BlinkStickDotNet.Animations.Processors
{
    public class ChangeColorEventArgs : EventArgs
    {
        public Color[] Colors { get; private set; }
        public uint Offset { get; private set; }

        public ChangeColorEventArgs(Color[] colors , uint offset)
        {
            this.Colors = colors;
            this.Offset = offset;
        }
    }
}
