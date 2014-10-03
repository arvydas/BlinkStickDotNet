using System;
using BlinkStickDotNet;

namespace PulseTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Pulse test for BlinkStick.\r\n");

			BlinkStick device = BlinkStick.FindFirst ();
			if (device != null && device.OpenDevice ()) {
				device.Pulse ("red");
				device.Pulse ("green");
				device.Pulse ("blue");
			}
		}
	}
}
