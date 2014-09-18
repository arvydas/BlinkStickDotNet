#region License
/* Copyright 2012-2013 James F. Bellinger <http://www.zer7.com/software/hidsharp>

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

using System.Threading;
using LibUsbDotNet;

namespace HidSharp.Platform
{
    sealed class HidSelector
    {
        public static readonly HidManager Instance;
        static readonly Thread ManagerThread; 

        static HidSelector ()
		{
			if (Instance != null) {
				return;	
			}

			switch (PlatformDetector.RunningPlatform()) {
				case PlatformDetector.Platform.Linux:
					Instance = new Libusb.LibusbHidManager();
					break;
				case PlatformDetector.Platform.Mac:
					Instance = new MacOS.MacHidManager();
					break;
				case PlatformDetector.Platform.Windows:
					Instance = new Windows.WinHidManager();
					break;
			}
            
			var readyEvent = new ManualResetEvent(false);
            ManagerThread = new Thread(Instance.RunImpl);
            ManagerThread.IsBackground = true;
            ManagerThread.Start(readyEvent);
            readyEvent.WaitOne();

			/*
            foreach (var hidManager in new HidManager[]
                {
                    new Windows.WinHidManager(),
                    //new Linux.LinuxHidManager(), //Disabled, because BlinkStick does not use this
                    new MacOS.MacHidManager(),
                    new Libusb.LibusbHidManager(),
                    new Unsupported.UnsupportedHidManager()
                })
            {
                if (hidManager.IsSupported)
                {
                    var readyEvent = new ManualResetEvent(false);

                    Instance = hidManager;
                    ManagerThread = new Thread(Instance.RunImpl);
                    ManagerThread.IsBackground = true;
                    ManagerThread.Start(readyEvent);
                    readyEvent.WaitOne();

                    break;
                }
            }
            */
        }

		public static void FreeUsbResources ()
		{
			// Free usb resources
			if (PlatformDetector.RunningPlatform() == PlatformDetector.Platform.Linux) {
				UsbDevice.Exit ();
			}
		}
	}
}
