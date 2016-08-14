using System;
using HidSharp;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Adapter for the HID device.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Usb.IUsbDevice" />
    public class HidDeviceAdapter : IUsbDevice
    {
        private HidDevice _device;
        private UsbMonitor _monitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidDeviceAdapter"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public HidDeviceAdapter(HidDevice device, UsbMonitor monitor)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if(monitor == null)
            {
                throw new ArgumentNullException(nameof(monitor));
            }

            _device = device;
            _monitor = monitor;
            _monitor.Disconnected += OnSomeDeviceDisconnected;
            _monitor.Connected += OnSomeDeviceConnected; 
        }


        /// <summary>
        /// Called when some USB device is disconnected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void OnSomeDeviceDisconnected(object sender, DeviceModifiedArgs eventArgs)
        {
            if (eventArgs != null &&
                eventArgs.Device.Manufacturer == Manufacturer &&
                eventArgs.Device.ProductName == ProductName &&
                eventArgs.Device.ProductVersion == ProductVersion &&
                eventArgs.Device.SerialNumber == SerialNumber)
            {
                this.Disconnect?.Invoke(sender, new DeviceModifiedArgs(this));
            }
        }

        /// <summary>
        /// Called when some USB device is connected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event arguments.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnSomeDeviceConnected(object sender, DeviceModifiedArgs eventArgs)
        {
            if (eventArgs != null &&
                eventArgs.Device.Manufacturer == Manufacturer &&
                eventArgs.Device.ProductName == ProductName &&
                eventArgs.Device.ProductVersion == ProductVersion &&
                eventArgs.Device.SerialNumber == SerialNumber)
            {
                this.Reconnect?.Invoke(sender, new DeviceModifiedArgs(this));
            }
        }

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
        /// Occurs when the device is disconnected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> Disconnect;

        /// <summary>
        /// Occurs when the device is reconnected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> Reconnect;

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