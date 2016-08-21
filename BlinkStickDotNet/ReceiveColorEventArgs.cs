using System;

namespace BlinkStickDotNet
{
    public class ReceiveColorEventArgs : EventArgs
    {
        public byte Index;

        //Todo: why are these not connected?
        public byte R;
        public byte G;
        public byte B;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveColorEventArgs"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public ReceiveColorEventArgs(byte index)
        {
            this.Index = index;
        }
    }
}
