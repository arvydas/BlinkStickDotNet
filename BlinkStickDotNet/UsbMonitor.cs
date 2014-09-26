#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick.HID library.
//
// BlinkStick.HID library is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) any 
// later version.
//		
// BlinkStick.HID library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick.HID library. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.Collections.Generic;
using LibUsbDotNet.DeviceNotify;

namespace BlinkStickDotNet
{
	public class UsbMonitor
	{
        /// <summary>
        /// Occurs when BlinkStick is connected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> BlinkStickConnected;

        /// <summary>
        /// Raises the BlinkStick connected event.
        /// </summary>
        /// <param name="device">Device which has been connected.</param>
        protected void OnBlinkStickConnected(BlinkStick device)
        {
            if (BlinkStickConnected != null)
            {
                BlinkStickConnected(this, new DeviceModifiedArgs(device));
            }
        }

        /// <summary>
        /// Occurs when BlinkStick disconnected.
        /// </summary>
        public event EventHandler<DeviceModifiedArgs> BlinkStickDisconnected;

        /// <summary>
        /// Raises the BlinkStick disconnected event.
        /// </summary>
        /// <param name="device">Device which has been disconnected.</param>
        protected void OnBlinkStickDisconnected(BlinkStick device)
        {
            if (BlinkStickDisconnected != null)
            {
                BlinkStickDisconnected(this, new DeviceModifiedArgs(device));
            }
        }

        /// <summary>
        /// Occurs when usb devices change.
        /// </summary>
		public event EventHandler UsbDevicesChanged;
		
        /// <summary>
        /// Raises the usb device changed event.
        /// </summary>
		protected void OnUsbDevicesChanged()
		{
			if (UsbDevicesChanged != null)
			{
				UsbDevicesChanged(this, new EventArgs());
			}

            List<BlinkStick> newDevices = new List<BlinkStick>();

            List<BlinkStick> scannedDevices = new List<BlinkStick>(BlinkStick.FindAll());

            foreach (BlinkStick newDevice in scannedDevices)
            {
                Boolean found = false;

                for (int i = devices.Count - 1; i >= 0; i--)
                {
                    if (devices[i].Serial == newDevice.Serial)
                    {
                        devices.RemoveAt(i);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    OnBlinkStickConnected(newDevice);
                }
            }

            foreach (BlinkStick device in devices)
            {
                OnBlinkStickDisconnected(device);
            }

            devices = scannedDevices;
		}

        /// <summary>
        /// Internal list of tracked devices.
        /// </summary>
        List<BlinkStick> devices;

        /// <summary>
        /// USB device monitor for Windows.
        /// </summary>
		private WinUsbDeviceMonitor winUsbDeviceMonitor;

        /// <summary>
        /// USB device monitor for Linux/Mac.
        /// </summary>
        public IDeviceNotifier UsbDeviceNotifier;

        /// <summary>
        /// Gets a value indicating whether this <see cref="BlinkStickDotNet.UsbMonitor"/> is monitoring.
        /// </summary>
        /// <value><c>true</c> if monitoring; otherwise, <c>false</c>.</value>
		public Boolean Monitoring {
			get;
			private set;
		}

		public UsbMonitor ()
		{
            switch (HidSharp.PlatformDetector.RunningPlatform())
            {
                case HidSharp.PlatformDetector.Platform.Windows:
                    winUsbDeviceMonitor = new WinUsbDeviceMonitor();
                    winUsbDeviceMonitor.DeviceListChanged += HandleDeviceListChanged;
                    break;
                case HidSharp.PlatformDetector.Platform.Linux:
                    UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
                    UsbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;
                    break;
            }
		}

        /// <summary>
        /// Handles the device list change on Windows.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event args</param>
		private void HandleDeviceListChanged (object sender, EventArgs e)
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
		public void Start ()
		{
            //Get the list of already connected BlinkSticks
            devices = new List<BlinkStick>(BlinkStick.FindAll());

            if (UsbDeviceNotifier != null)
            {
                UsbDeviceNotifier.Enabled = true;
            }
            else if (winUsbDeviceMonitor != null)
            {
                winUsbDeviceMonitor.Enabled = true;
            }

            Monitoring = true;
		}

        /// <summary>
        /// Stop monitoring for added/removed BlinkStick devices.
        /// </summary>
		public void Stop ()
		{
            if (UsbDeviceNotifier != null) {
				UsbDeviceNotifier.Enabled = false;  // Disable the device notifier

				UsbDeviceNotifier.OnDeviceNotify -= OnDeviceNotifyEvent;
			}
            else if (winUsbDeviceMonitor != null)
            {
                winUsbDeviceMonitor.Enabled = false;
            }

			Monitoring = false;
		}

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="BlinkStickDotNet.UsbMonitor"/> is reclaimed by garbage collection.
        /// </summary>
		~UsbMonitor ()
		{
		}
	}

    /// <summary>
    /// Device modified arguments.
    /// </summary>
    public class DeviceModifiedArgs : EventArgs
    {
        /// <summary>
        /// The device which has been modified.
        /// </summary>
        public BlinkStick Device;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkStickDotNet.DeviceModifiedArgs"/> class.
        /// </summary>
        /// <param name="device">Device passed as an argument</param>
        public DeviceModifiedArgs(BlinkStick device)
        {
            this.Device = device;
        }
    }
}

