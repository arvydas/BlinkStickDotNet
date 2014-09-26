using System;
using BlinkStickDotNet;
using System.Collections.Generic;

namespace SetRandomColor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Set random color.\r\n");

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
					Random r = new Random ();
					device.SetColor ((byte)r.Next(), (byte)r.Next(), (byte)r.Next());
				}
			}

			Console.WriteLine ("\r\nPress Enter to exit...");
			Console.ReadLine ();
		}
	}
}
