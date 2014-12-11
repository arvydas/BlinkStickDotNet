using System;
using BlinkStickDotNet;
using System.Threading;

namespace IndexedColors
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Set indexed color. \r\nThis example requires BlinkStick Pro with 8 smart pixels connected to R channel.\r\n");

			BlinkStick device = BlinkStick.FindFirst ();

			if (device != null) {
				if (device.OpenDevice ()) {
					//Set mode to WS2812. Read more about modes here:
					//http://www.blinkstick.com/help/tutorials/blinkstick-pro-modes

					device.SetMode (2);
					Thread.Sleep (100);

					int numberOfLeds = 8;

					for (byte i = 0; i < numberOfLeds; i++) {
						device.SetColor (0, i, "#ff0000");

						Thread.Sleep (500);
					}

				} else {
					Console.WriteLine ("Could not open the device");
				}
			} else {
				Console.WriteLine ("BlinkStick not found");
			}
		}
	}
}
