using System;
using BlinkStickDotNet;

namespace MorphTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Morph test for BlinkStick.\r\n");

			BlinkStick device = BlinkStick.FindFirst ();
			if (device != null && device.OpenDevice ()) {
				device.Morph ("red");
				device.Morph ("green");
				device.Morph ("blue");
			}
		}
	}
}
