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
using LibUsbDotNet.DeviceNotify;

namespace BlinkStick.Hid
{
	public class UsbMonitor
	{
		public event EventHandler UsbDeviceAdded;
		
		protected void OnUsbDeviceAdded()
		{
			if (UsbDeviceAdded != null)
			{
				UsbDeviceAdded(this, new EventArgs());
			}
		}

		private WinUsbDeviceMonitor winUsbDeviceMonitor;
        public IDeviceNotifier UsbDeviceNotifier;

		public Boolean Monitoring {
			get;
			private set;
		}

		public UsbMonitor (IntPtr mainWindowHandle)
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

		void HandleDeviceListChanged (object sender, EventArgs e)
		{
			OnUsbDeviceAdded();
		}

		public void Start ()
		{
            if (UsbDeviceNotifier != null) {
				UsbDeviceNotifier.Enabled = true;
			}

            Monitoring = true;
		}

		public void Stop ()
		{
            if (UsbDeviceNotifier != null) {
				UsbDeviceNotifier.Enabled = false;  // Disable the device notifier

				UsbDeviceNotifier.OnDeviceNotify -= OnDeviceNotifyEvent;
			}

			Monitoring = false;
		}

		private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
        {
			OnUsbDeviceAdded();
        }

		~UsbMonitor ()
		{
		}
	}
}

