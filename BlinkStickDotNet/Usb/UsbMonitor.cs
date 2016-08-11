using HidSharp;
using LibUsbDotNet.DeviceNotify;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Monitors changes on the USB ports regarding connected devices. Hides implementation details.
    /// </summary>
    public class UsbMonitor
    {
        private int? _vendorId;
        private int? _productId;

        private List<IUsbDevice> _trackedDevices;
        private IDeviceNotifier _usbDeviceNotifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbMonitor"/> class.
        /// </summary>
        /// <param name="vendorId">The vendor identifier.</param>
        /// <param name="productId">The product identifier.</param>
        public UsbMonitor(int? vendorId = null, int? productId = null)
        {
            _vendorId = vendorId;
            _productId = productId;

            _usbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();

            Start();
        }

        /// <summary>
        /// Occurs when a usb device is connected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> Connected;

        /// <summary>
        /// Occurs when a usb device is disconnected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> Disconnected;

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
        /// Raises the connected event.
        /// </summary>
        /// <param name="device">Device which has been connected.</param>
        protected void OnConnected(IUsbDevice device)
        {
            Connected?.Invoke(this, new DeviceModifiedArgs(device));
        }

        /// <summary>
        /// Raises the disconnected event.
        /// </summary>
        /// <param name="device">Device which has been disconnected.</param>
        protected void OnDisconnected(IUsbDevice device)
        {
            Disconnected?.Invoke(this, new DeviceModifiedArgs(device));
        }

        /// <summary>
        /// Raises the usb device changed event.
        /// </summary>
        protected void OnUsbDevicesChanged()
        {
            UsbDevicesChanged?.Invoke(this, new EventArgs());

            var scannedDevices = GetDevices().ToList();

            //signal disconnected devices
            _trackedDevices
                .Where(d => scannedDevices.FirstOrDefault(d2 => d2.SerialNumber == d.SerialNumber) == null)
                .ToList()
                .ForEach(d => OnDisconnected(d));

            //signal newly connected devices
            scannedDevices
                .Where(d => this._trackedDevices.FirstOrDefault(d2 => d2.SerialNumber == d.SerialNumber) == null)
                .ToList()
                .ForEach(d => OnConnected(d));

            //register devices to class
            _trackedDevices = scannedDevices;
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
        /// Start monitoring for added/removed devices.
        /// </summary>
		public void Start()
        {
            //Get the list of already connected devices
            _trackedDevices = this.GetDevices().ToList();

            if (_usbDeviceNotifier != null)
            {
                _usbDeviceNotifier.Enabled = true;
                _usbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;
            }

            Monitoring = true;
        }

        /// <summary>
        /// Stop monitoring for added/removed devices.
        /// </summary>
		public void Stop()
        {
            if (_usbDeviceNotifier != null)
            {
                _usbDeviceNotifier.Enabled = false;  // Disable the device notifier
                _usbDeviceNotifier.OnDeviceNotify -= OnDeviceNotifyEvent;
            }

            Monitoring = false;
        }

        /// <summary>
        /// Gets the devices that match the specified options.
        /// </summary>
        /// <param name="serial">The serial (optional).</param>
        /// <returns>The devices.</returns>
        public IEnumerable<IUsbDevice> GetDevices(string serial = null)
        {
            var loader = new HidDeviceLoader();
            var devices = loader
                .GetDevices(_vendorId, _productId, serialNumber: serial)
                .Select(d => new HidDeviceAdapter(d, this));

            return devices;
        }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <param name="vendorId">The vendor identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="serial">The serial.</param>
        /// <returns>The devices.</returns>
        public static IEnumerable<IUsbDevice> GetAllDevices(int? vendorId = null, int? productId = null, string serial = null)
        {
            return new UsbMonitor(vendorId, productId).GetDevices(serial);
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
            return GetAllDevices(vendorId, productId, serial).FirstOrDefault();
        }
    }
}