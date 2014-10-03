using System;
using System.Threading;
using BlinkStickDotNet;

namespace BlinkTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Blink test for BlinkStick.\r\n");

			BlinkStick device = BlinkStick.FindFirst ();
			if (device != null && device.OpenDevice ()) {
				foreach (string color in new string[] {"red", "green", "blue"})
				{
					Console.WriteLine (color);
					device.Blink(color);
				}
			}
		}
	}
}
