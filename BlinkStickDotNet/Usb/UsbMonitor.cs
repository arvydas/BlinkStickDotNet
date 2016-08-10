using System;
using System.Collections.Generic;
using System.Linq;
using LibUsbDotNet.DeviceNotify;
using HidSharp;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Monitors changes on the USB ports regarding connected BlinkSticks. Hides implementation details.
    /// </summary>
    public class UsbMonitor
    {
        /// <summary>
        /// Internal list of tracked devices.
        /// </summary>
        private List<BlinkStick> trackedDevices;

        /// <summary>
        /// USB device monitor for Linux/Mac.
        /// </summary>
        private IDeviceNotifier usbDeviceNotifier;

        /// <summary>
        /// Occurs when BlinkStick is connected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> BlinkStickConnected;

        /// <summary>
        /// Occurs when BlinkStick disconnected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> BlinkStickDisconnected;

        /// <summary>
        /// Occurs when usb devices change.
        /// </summary>
        public event EventHandler UsbDevicesChanged;

        /// <summary>
        /// Gets a value indicating whether this <see cref="BlinkStickDotNet.UsbMonitor"/> is monitoring.
        /// </summary>
        /// <value><c>true</c> if monitoring; otherwise, <c>false</c>.</value>
        public bool Monitoring
        {
            get;
            private set;
        }

        /// <summary>
        /// Raises the BlinkStick connected event.
        /// </summary>
        /// <param name="device">Device which has been connected.</param>
        protected void OnBlinkStickConnected(BlinkStick device)
        {
            BlinkStickConnected?.Invoke(this, new DeviceModifiedArgs(device));
        }

        /// <summary>
        /// Raises the BlinkStick disconnected event.
        /// </summary>
        /// <param name="device">Device which has been disconnected.</param>
        protected void OnBlinkStickDisconnected(BlinkStick device)
        {
            BlinkStickDisconnected?.Invoke(this, new DeviceModifiedArgs(device));
        }

        /// <summary>
        /// Raises the usb device changed event.
        /// </summary>
        protected void OnUsbDevicesChanged()
        {
            UsbDevicesChanged?.Invoke(this, new EventArgs());

            var scannedDevices = new List<BlinkStick>(BlinkStick.FindAll());

            //signal disconnected devices
            trackedDevices
                .Where(d => scannedDevices.FirstOrDefault(d2 => d.Serial == d.Serial) == null)
                .ToList()
                .ForEach(d => OnBlinkStickDisconnected(d));

            //signal newly connected devices
            scannedDevices
                .Where(d => this.trackedDevices.FirstOrDefault(d2 => d2.Serial == d.Serial) == null)
                .ToList()
                .ForEach(d => OnBlinkStickConnected(d));

            //register devices to class
            trackedDevices = scannedDevices;
        }

        public UsbMonitor()
        {
            usbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
            Start();
        }

        /// <summary>
        /// Handles the device list change on Windows.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
		private void HandleDeviceListChanged(object sender, EventArgs e)
        {
            OnUsbDevicesChanged();
        }

        /// <summary>
        /// Handles device list change on Linux/Mac.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
        {
            OnUsbDevicesChanged();
        }

        /// <summary>
        /// Start monitoring for added/removed BlinkStick devices.
        /// </summary>
		public void Start()
        {
            //Get the list of already connected BlinkSticks
            trackedDevices = new List<BlinkStick>(BlinkStick.FindAll());

            if (usbDeviceNotifier != null)
            {
                usbDeviceNotifier.Enabled = true;
                usbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;
            }

            Monitoring = true;
        }

        /// <summary>
        /// Stop monitoring for added/removed BlinkStick devices.
        /// </summary>
		public void Stop()
        {
            if (usbDeviceNotifier != null)
            {
                usbDeviceNotifier.Enabled = false;  // Disable the device notifier
                usbDeviceNotifier.OnDeviceNotify -= OnDeviceNotifyEvent;
            }

            Monitoring = false;
        }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <returns>The devices.</returns>
        public IEnumerable<BlinkStick> GetDevices()
        {
            return trackedDevices.ToList();
        }

        /// <summary>
        /// Gets the devices that match the specified options.
        /// </summary>
        /// <param name="vendorId">The vendor identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="serial">The serial (optional).</param>
        /// <returns>The devices.</returns>
        public static IEnumerable<IUsbDevice> GetDevices(int vendorId, int productId, string serial = null)
        {
            var loader = new HidDeviceLoader();
            var devices = loader
                .GetDevices(vendorId, productId)
                .Where(d => serial == null || serial == "" || d.SerialNumber == serial)
                .Select(d => new HidDeviceAdapter(d));

            return devices;
        }

        /// <summary>
        /// Gets the first device that matches the specified options.
        /// </summary>
        /// <param name="vendorId">The vendor identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="serial">The serial (optional).</param>
        /// <returns>A device or <c>null</c>.</returns>
        public static IUsbDevice GetFirstDevice(int vendorId, int productId, string serial = null)
        {
            return GetDevices(vendorId, productId, serial).FirstOrDefault();
        }
    }
}