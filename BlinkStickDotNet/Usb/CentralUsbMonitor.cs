using HidSharp;
using LibUsbDotNet.DeviceNotify;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Internal centralizes USB monitoring.
    /// </summary>
    internal class CentralUsbMonitor
    {
        private static Lazy<CentralUsbMonitor> _instance = new Lazy<CentralUsbMonitor>(() => new CentralUsbMonitor());
        private IDeviceNotifier _usbDeviceNotifier;
        private List<IInternalUsbDevice> _tracked = new List<IInternalUsbDevice>();

        public event EventHandler Updated;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CentralUsbMonitor Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CentralUsbMonitor"/> class.
        /// </summary>
        internal CentralUsbMonitor()
        {
            _usbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
            _usbDeviceNotifier.OnDeviceNotify += (s, e) => UpdateDevices();
            _usbDeviceNotifier.Enabled = true;

            UpdateDevices();
        }

        /// <summary>
        /// Updates the devices.
        /// </summary>
        private void UpdateDevices()
        {
            var loader = new HidDeviceLoader();

            var l = loader.GetDevices().ToList();

            var devices = l.Select(d => new HidDeviceAdapter(d)).AsEnumerable<IInternalUsbDevice>().ToList();

            lock (_tracked)
            {
                bool update = false;

                //handle newly connected devices
                var newDevices = devices
                    .Except(_tracked, UsbDeviceEquality.InternalComparer)
                    .ToList();

                update |= newDevices.Count > 0;
                _tracked.AddRange(newDevices);

                //handle devices that were disconnected
                var disconnected = _tracked
                    .Except(devices, UsbDeviceEquality.InternalComparer)
                    .Where(t => t.IsConnected)
                    .ToList();

                update |= disconnected.Count > 0;
                disconnected.ForEach(d => d.OnDisconnect());

                //handle devices that we (re)connected
                var connected = _tracked
                    .Where(t => !t.IsConnected)
                    .Intersect(devices, UsbDeviceEquality.InternalComparer)
                    .ToList();

                update |= connected.Count > 0;
                connected.ForEach(d => d.OnConnect());

                if (update)
                {
                    this.Updated?.Invoke(null, null);
                }
            }
        }

        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <returns>The devices.</returns>
        public IEnumerable<IUsbDevice> GetDevices()
        {
            return _tracked.AsEnumerable<IUsbDevice>();
        }
    }
}