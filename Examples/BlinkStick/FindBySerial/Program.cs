using System;
using BlinkStickDotNet;

namespace FindBySerial
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Find by serial.\r\n");

			String serial = "BS010000-1.1";

			if (BlinkStick.FindBySerial (serial) != null) {
				Console.WriteLine ("BlinkStick found!");
			} else {
				Console.WriteLine ("BlinkStick not found");
			}

			Console.WriteLine ("\r\nPress Enter to exit...");
			Console.ReadLine ();
		}
	}
}
