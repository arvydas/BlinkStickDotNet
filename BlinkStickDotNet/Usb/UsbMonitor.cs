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
        private List<IUsbDevice> _trackedDevices = new List<IUsbDevice>();
        Func<IUsbDevice, bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsbMonitor" /> class.
        /// </summary>
        /// <param name="vendorId">The vendor identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="serial">The serial.</param>
        public UsbMonitor(int? vendorId = null, int? productId = null, string serial = null)
        {
            _predicate = CreateUsbSearchPredicate(vendorId, productId, serial);
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
        /// Gets a value indicating whether this <see cref="UsbMonitor"/> is monitoring.
        /// </summary>
        /// <value>
        ///   <c>true</c> if monitoring; otherwise, <c>false</c>.
        /// </value>
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
            var args = new DeviceModifiedArgs(device);

            UsbDevicesChanged?.Invoke(this, args);
            Connected?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the disconnected event.
        /// </summary>
        /// <param name="device">Device which has been disconnected.</param>
        protected void OnDisconnected(IUsbDevice device)
        {
            var args = new DeviceModifiedArgs(device);

            UsbDevicesChanged?.Invoke(this, args);
            Disconnected?.Invoke(this, args);
        }

        /// <summary>
        /// Start monitoring for added/removed devices.
        /// </summary>
		public void Start()
        {
            Monitoring = true;

            CentralUsbMonitor.Instance.Updated += OnDevicesUpdate;

            OnDevicesUpdate();
        }

        /// <summary>
        /// Called when the devices are update.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnDevicesUpdate(object sender = null, EventArgs eventArgs = null)
        {
            if (Monitoring)
            {
                var newDevices = CentralUsbMonitor.Instance.GetDevices()
                    .Where(_predicate)
                    .Except(_trackedDevices, UsbDeviceEquality.Comparer)
                    .ToList();

                newDevices.ForEach(d =>
                {
                    d.Disconnect += (s, e) => OnDisconnected(e.Device);
                    d.Reconnect += (s, e) => OnConnected(e.Device);

                    if (d.IsConnected)
                    {
                        OnConnected(d);
                    }
                    else
                    {
                        OnDisconnected(d);
                    }
                });

                lock (_trackedDevices)
                {
                    if (Monitoring)
                    {
                        _trackedDevices.AddRange(newDevices);
                    }
                }
            }
        }

        /// <summary>
        /// Stop monitoring for added/removed devices.
        /// </summary>
        public void Stop()
        {
            Monitoring = false;

            CentralUsbMonitor.Instance.Updated -= OnDevicesUpdate;

            lock (_trackedDevices)
            {
                _trackedDevices.ForEach(d =>
                {
                    d.Disconnect -= (s, e) => OnDisconnected(e.Device);
                    d.Reconnect -= (s, e) => OnConnected(e.Device);
                });

                _trackedDevices.Clear();
            }
        }

        /// <summary>
        /// Gets the devices that match the specified options.
        /// </summary>
        /// <param name="serial">The serial (optional).</param>
        /// <returns>The devices.</returns>
        public IEnumerable<IUsbDevice> GetDevices(string serial = null)
        {
            return this._trackedDevices.Where(d => serial == null || d.SerialNumber == serial).ToList();
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
            var predicate = CreateUsbSearchPredicate(vendorId, productId, serial);
            return CentralUsbMonitor.Instance.GetDevices().Where(predicate);
        }

        /// <summary>
        /// Creates the UB search predicate.
        /// </summary>
        /// <param name="vendorId">The vendor identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="serial">The serial.</param>
        /// <returns>The predicate.</returns>
        private static Func<IUsbDevice, bool> CreateUsbSearchPredicate(int? vendorId, int? productId, string serial)
        {
            return (d) =>
            {
                return
                    d != null &&
                    (vendorId == null || d.VendorId == vendorId.Value) &&
                    (productId == null || d.ProductId == productId.Value) &&
                    (serial == null || d.SerialNumber == serial);
            };
        }

        /// <summary>
        /// Gets the first device that matches the specified options.
        /// </summary>
        /// <param name="vendorId">The vendor identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="serial">The serial (optional).</param>
        /// <returns>A device or <c>null</c>.</returns>
        public static IUsbDevice GetFirstDevice(int? vendorId = null, int? productId = null, string serial = null)
        {
            return GetAllDevices(vendorId, productId, serial).FirstOrDefault();
        }
    }
}