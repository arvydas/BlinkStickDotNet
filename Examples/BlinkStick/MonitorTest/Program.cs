﻿using System;
using BlinkStickDotNet;
using System.Windows.Forms;
using System.Collections.Generic;
using BlinkStickDotNet.Usb;

namespace MonitorTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Monitor BlinkSticks inserted and removed");

			UsbMonitor monitor = new UsbMonitor();

			//Attach to connected event
			monitor.Connected += (object sender, DeviceModifiedArgs e) => {
				Console.WriteLine("BlinkStick " + e.Device.SerialNumber + " connected!");
			};

			//Attach to disconnected event
			monitor.Disconnected += (object sender, DeviceModifiedArgs e) => {
				Console.WriteLine("BlinkStick " + e.Device.SerialNumber + " disconnected...");
			};

			List<BlinkStick> devices = new List<BlinkStick> (BlinkStick.FindAll());

			//List BlinkSticks already connected
			foreach (BlinkStick device in devices)
			{
				Console.WriteLine("BlinkStick " + device.Meta.Serial + " already connected");
			}

			//Start monitoring
			monitor.Start ();

			Console.WriteLine ("Monitoring for BlinkStick devices... Press any key to exit.");

            //Start application event loop. Alternatively you can run main form:
            //   Application.Run ([Your form]);
            while (true)
            {
                //Process messages
                Application.DoEvents();

                //Exit if key is pressed
                if (Console.KeyAvailable)
                    break;
            }

			//Stop monitoring
			monitor.Stop ();
		}
	}
}
