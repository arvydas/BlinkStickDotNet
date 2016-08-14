using System;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Indicates the object implements a USB device.
    /// </summary>
    public interface IUsbDevice
    {
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        string Path { get; }

        /// <summary>
        /// Gets the vendor identifier.
        /// </summary>
        /// <value>
        /// The vendor identifier.
        /// </value>
        int VendorId { get; }

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
        /// Gets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        int ProductId { get; }

        /// <summary>
        /// Tries to open the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void TryOpen(out IUsbStream stream);

        /// <summary>
        /// Occurs when the device disconnects.
        /// </summary>
        event EventHandler<DeviceModifiedArgs> Disconnect;

        /// <summary>
        /// Occurs when the device is reconnected.
        /// </summary>
        event EventHandler<DeviceModifiedArgs> Reconnect;

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }
    }
}
