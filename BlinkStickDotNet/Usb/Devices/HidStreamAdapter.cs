using HidSharp;
using System;
using System.Threading;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Adapter for the HID stream.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Usb.IUsbStream" />
    public class HidStreamAdapter : IUsbStream
    {
        private const uint RetriesOnFail = 5;
        private HidStream _hid;

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

            this._hid = hid;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            _hid.Close();
        }

        /// <summary>
        /// Sends a Get Feature setup request.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void GetFeature(byte[] buffer)
        {
            RetryActionOnFail(RetriesOnFail, () => _hid.GetFeature(buffer));
        }

        /// <summary>
        /// Sends a Get Feature setup request.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public void GetFeature(byte[] buffer, int offset, int count)
        {
            RetryActionOnFail(RetriesOnFail, () => _hid.GetFeature(buffer, offset, count));
        }

        /// <summary>
        /// Sends a Set Feature setup request.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void SetFeature(byte[] buffer)
        {
            RetryActionOnFail(RetriesOnFail, () => _hid.SetFeature(buffer));
        }

        /// <summary>
        /// Retries the action on fail.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="action">The action.</param>
        private static void RetryActionOnFail(uint times, Action action)
        {
            int i = 0;

            while (true)
            {
                try
                {
                    action();
                    break;
                }
                catch (Exception)
                {
                    if (i == times)
                    {
                        throw;
                    }

                    i++;

                    Thread.Sleep(2);
                }
            }
        }
    }
}