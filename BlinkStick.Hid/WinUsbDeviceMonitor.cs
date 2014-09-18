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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace BlinkStick.Hid
{
	public class WinUsbDeviceMonitor
	{
        public event EventHandler DeviceListChanged;
		
		protected void OnDeviceListChanged()
		{
			if (DeviceListChanged != null)
			{
				DeviceListChanged(this, new EventArgs());
			}
		}

		MyForm form;

		public class MyForm : Form
		{
			public WinUsbDeviceMonitor Monitor;

			const int WM_DEVICECHANGE = 0x0219;
			const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device
			const int DBT_DEVICEREMOVECOMPLETE = 0x8004; //device was removed
			const int DBT_DEVNODES_CHANGED = 0x0007; //device changed
			const int DBT_DEVTYP_VOLUME = 0x00000002; // logical volume

			//[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			protected override void WndProc(ref Message m)
			{
				if (m.Msg == WM_DEVICECHANGE
				    && (m.WParam.ToInt32() == DBT_DEVICEARRIVAL
				    || m.WParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE
				    || m.WParam.ToInt32() == DBT_DEVNODES_CHANGED))
				{
					Monitor.OnDeviceListChanged();
				}
				
				base.WndProc(ref m);
			}
		
		}

		public WinUsbDeviceMonitor ()
		{
			form = new MyForm();
			form.Monitor = this;
			form.Visible = true;
			form.Visible = false;
		}
	}
}

