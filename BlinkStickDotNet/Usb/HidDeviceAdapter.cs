using System;
using HidSharp;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Adapter for the HID device.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Usb.IUsbDevice" />
    internal class HidDeviceAdapter : IUsbDevice
    {
        private HidDevice device;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidDeviceAdapter"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public HidDeviceAdapter(HidDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            this.device = device;
        }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer
        {
            get { return this.device.Manufacturer; }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName
        {
            get { return this.device.ProductName; }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>
        /// The product version.
        /// </value>
        public int ProductVersion
        {
            get { return this.device.ProductVersion; }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        public string SerialNumber
        {
            get { return this.device.SerialNumber; }
        }

        /// <summary>
        /// Tries to make a connection to the HID device.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void TryOpen(out IUsbStream stream)
        {
            HidStream hid = null;

            this.device.TryOpen(out hid);

            if (hid == null)
            {
                stream = null;
                return;
            }

            stream = new HidStreamAdapter(hid);
        }
    }
}