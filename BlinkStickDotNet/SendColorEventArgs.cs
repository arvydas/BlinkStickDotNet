using System;

namespace BlinkStickDotNet
{
    public class SendColorEventArgs : EventArgs
    {
        public byte Channel;
        public byte Index;
        public byte R;
        public byte G;
        public byte B;
        public bool SendToDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendColorEventArgs"/> class.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="index">The index.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        public SendColorEventArgs(byte channel, byte index, byte r, byte g, byte b)
        {
            this.Channel = channel;
            this.Index = index;
            this.R = r;
            this.G = g;
            this.B = b;
            this.SendToDevice = true;
        }
    }
}