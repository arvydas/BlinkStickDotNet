using System;
using BlinkStickDotNet;
using System.Collections.Generic;

namespace GetInfo
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Information about BlinkSticks.\r\n");

			BlinkStick[] devices = BlinkStick.FindAll();

			if (devices.Length == 0) {
				Console.WriteLine ("Could not find any BlinkStick devices...");
				return;
			}

			//Iterate through all of them
			foreach (BlinkStick device in devices)
			{
				//Open the device
				if (device.OpenDevice ()) 
				{
					Console.WriteLine (String.Format ("Device {0} opened successfully", device.Serial));

					byte cr;
					byte cg;
					byte cb;

					device.GetColor(out cr, out cg, out cb);

					Console.WriteLine (String.Format ("    Device color: #{0:X2}{1:X2}{2:X2}", cr, cg, cb));
					Console.WriteLine ("    Serial:       " + device.Serial);
					Console.WriteLine ("    Manufacturer: " + device.ManufacturerName);
					Console.WriteLine ("    Product Name: " + device.ProductName);
					Console.WriteLine ("    InfoBlock1:   " + device.InfoBlock1);
					Console.WriteLine ("    InfoBlock2:   " + device.InfoBlock2);				}
			}

			Console.WriteLine ("\r\nPress Enter to exit...");
			Console.ReadLine ();
		}
	}
}
