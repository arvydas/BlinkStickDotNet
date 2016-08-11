using HidSharp;
using System;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Adapter for the HID stream.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Usb.IUsbStream" />
    public class HidStreamAdapter : IUsbStream
    {
        private HidStream hid;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidStreamAdapter"/> class.
        /// </summary>
        /// <param name="hid">The hid.</param>
        public HidStreamAdapter(HidStream hid)
        {
            if (hid == null)
            {
                throw new ArgumentNullException(nameof(hid));
            }

            this.hid = hid;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            hid.Close();
        }

        /// <summary>
        /// Sends a Get Feature setup request.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void GetFeature(byte[] buffer)
        {
            hid.GetFeature(buffer);
        }

        /// <summary>
        /// Sends a Get Feature setup request.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public void GetFeature(byte[] buffer, int offset, int count)
        {
            hid.GetFeature(buffer, offset, count);
        }

        /// <summary>
        /// Sends a Set Feature setup request.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void SetFeature(byte[] buffer)
        {
            hid.SetFeature(buffer);
        }
    }
}