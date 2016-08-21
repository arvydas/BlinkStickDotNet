using System;
using HidSharp;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Adapter for the HID device.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Usb.IUsbDevice" />
    public class HidDeviceAdapter : IInternalUsbDevice
    {
        private HidDevice _device;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidDeviceAdapter"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public HidDeviceAdapter(HidDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _device = device;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer
        {
            get { return this._device.Manufacturer; }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName
        {
            get { return this._device.ProductName; }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>
        /// The product version.
        /// </value>
        public int ProductVersion
        {
            get { return this._device.ProductVersion; }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        public string SerialNumber
        {
            get { return this._device.SerialNumber; }
        }

        /// <summary>
        /// Gets the vendor identifier.
        /// </summary>
        /// <value>
        /// The vendor identifier.
        /// </value>
        public int VendorId
        {
            get { return this._device.VendorID; }
        }

        /// <summary>
        /// Gets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public int ProductId
        {
            get { return this._device.ProductID; }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path
        {
            get { return this._device.DevicePath; }
        }

        /// <summary>
        /// Occurs when the device is disconnected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> Disconnect;

        /// <summary>
        /// Called when the device disconnects.
        /// </summary>
        void IInternalUsbDevice.OnDisconnect()
        {
            this.IsConnected = false; 
            this.Disconnect?.Invoke(null, new DeviceModifiedArgs(this));
        }

        /// <summary>
        /// Occurs when the device is reconnected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> Reconnect;

        /// <summary>
        /// Called when the device connects.
        /// </summary>
        void IInternalUsbDevice.OnConnect()
        {
            this.IsConnected = true;
            this.Reconnect?.Invoke(null, new DeviceModifiedArgs(this));
        }

        /// <summary>
        /// Tries to make a connection to the HID device.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void TryOpen(out IUsbStream stream)
        {
            HidStream hid = null;

            this._device.TryOpen(out hid);

            if (hid == null)
            {
                stream = null;
                return;
            }

            stream = new HidStreamAdapter(hid);
        }
    }
}