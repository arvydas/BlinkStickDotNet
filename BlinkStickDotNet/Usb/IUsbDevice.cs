using HidSharp;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Indicates the object implements a USB device.
    /// </summary>
    public interface IUsbDevice
    {
        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        string Manufacturer { get; }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        string ProductName { get; }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>
        /// The product version.
        /// </value>
        int ProductVersion { get; }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        string SerialNumber { get;}

        /// <summary>
        /// Tries to open the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void TryOpen(out IUsbStream stream);
    }
}
