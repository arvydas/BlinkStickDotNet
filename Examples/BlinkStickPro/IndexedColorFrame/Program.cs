using System;
using BlinkStickDotNet;
using System.Threading;

namespace IndexedColorFrame
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Set indexed color frame. \r\nThis example requires BlinkStick Pro with 8 smart pixels connected to R channel.\r\n");

			BlinkStick device = BlinkStick.FindFirst ();

			if (device != null) {
				if (device.OpenDevice ()) {
					//Set mode to WS2812. Read more about modes here:
					//http://www.blinkstick.com/help/tutorials/blinkstick-pro-modes

					device.SetMode (2);
					Thread.Sleep (100);

					byte[] data = new byte[3*8] 
						{0, 0, 255,    //GRB for led0
						 0, 128, 0,    //GRB for led1
						 128, 0, 0,    //...
						 128, 255, 0,
						 0, 255, 128,
						 128, 0, 128,
						 0, 128, 255,
						 128, 0, 0    //GRB for led7
					    };


					device.SetColors (0, data);

				} else {
					Console.WriteLine ("Could not open the device");
				}
			} else {
				Console.WriteLine ("BlinkStick not found");
			}
		}
	}
}
