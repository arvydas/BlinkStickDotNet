#region License
/* Copyright 2012 James F. Bellinger <http://www.zer7.com/software/hidsharp>

   Permission to use, copy, modify, and/or distribute this software for any
   purpose with or without fee is hereby granted, provided that the above
   copyright notice and this permission notice appear in all copies.

   THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
   WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
   MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
   ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
   WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
   ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
   OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE. */
#endregion

using System;
using System.Collections.Generic;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace HidSharp.Platform.Libusb
{
	class LibusbHidManager : HidManager
	{
		protected override object[] Refresh ()
		{
			List<UsbRegistry> result = new List<UsbRegistry>();

			foreach (UsbRegistry device in UsbDevice.AllDevices) {
				result.Add(device);
			}

			return result.ToArray();
		}

		protected override bool TryCreateDevice (object key, out HidDevice device, out object creationState)
		{
			creationState = null;
            
			var hidDevice = new LibusbHidDevice((UsbRegistry)key);
            
			if (!hidDevice.GetInfo()) { 
				device = null; 
				return false; 
			}
            
			device = hidDevice; 
			return true;
		}

		protected override void CompleteDevice (object key, HidDevice device, object creationState)
		{

		}

		public override bool IsSupported {
			get {
				return true;
			}
		}
	}
}

