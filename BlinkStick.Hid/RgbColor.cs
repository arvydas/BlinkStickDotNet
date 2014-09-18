#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick application.
//
// BlinkStick application is free software: you can redistribute 
// it and/or modify it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//		
// BlinkStick application is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick application. If not, see http://www.gnu.org/licenses/.
#endregion

using System;

namespace BlinkStick.Hid
{
	public class RgbColor
	{
		public Byte R;
		public Byte G;
		public Byte B;

		public RgbColor ()
		{
		}

		public static RgbColor FromRgb(int r, int g, int b)
		{
			RgbColor color = new RgbColor();
			color.R = (byte)r;
			color.G = (byte)g;
			color.B = (byte)b;

			return color;
		}
		
		public static RgbColor FromGdkColor(ushort r, ushort g, ushort b)
		{
			RgbColor color = new RgbColor();
			
			color.R = (byte)(r / 0x100);
			color.G = (byte)(g / 0x100);
			color.B = (byte)(b / 0x100);
			
			return color;
		}

		public static RgbColor FromString (String colorStr)
		{
			RgbColor color = new RgbColor();
			color.R = Convert.ToByte(colorStr.Substring(1, 2), 16);
			color.G = Convert.ToByte(colorStr.Substring(3, 2), 16);
			color.B = Convert.ToByte(colorStr.Substring(5, 2), 16);

			return color;
		}

        public static RgbColor Black ()
        {
            return RgbColor.FromRgb(0, 0, 0);
        }

        public override string ToString()
        {
            return string.Format("{0:x2}{1:x2}{2:x2}", this.R, this.G, this.B);
        }
	}
}

