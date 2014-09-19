#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick.HID library.
//
// BlinkStick.HID library is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) any 
// later version.
//		
// BlinkStick.HID library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick.HID library. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using HidSharp;

namespace BlinkStick.Hid
{
    /// <summary>
    /// Main class to access Blinkstick HID devices.
    /// </summary>
	public class BlinkstickHid : IDisposable
    {
        protected const int VendorId = 0x20A0;
        protected const int ProductId = 0x41E5;

        private HidDevice device;
        private HidStream stream;

        private bool disposed = false;

        protected bool connectedToDriver = false;

        /// <summary>
        /// Gets a value indicating whether this <see cref="BlinkStick.Hid.BlinkstickHid"/> is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public Boolean Connected {
            get {
                return connectedToDriver;
            }
        }

        /// <summary>
        /// Gets the serial number of BlinkStick.
        /// </summary>
        /// <value>The serial.</value>
        public String Serial {
            get {
                return device.SerialNumber;
            }
        }

        /// <summary>
        /// Gets the name of the manufacturer.
        /// </summary>
        /// <value>The name of the manufacturer.</value>
        public String ManufacturerName {
            get {
                return device.Manufacturer;
            }
        }

        /// <summary>
        /// Gets the product name of the device.
        /// </summary>
        /// <value>The name of the product.</value>
        public String ProductName {
            get {
                return device.ProductName;
            }
        }

        private String _InfoBlock1;
        /// <summary>
        /// Gets or sets the name of the device (InfoBlock1).
        /// </summary>
        /// <value>The name.</value>
        public String InfoBlock1 {
            get {
                if (_InfoBlock1 == null) {
                    GetInfoBlock (2, out _InfoBlock1);
                }

                return _InfoBlock1;
            }
            set {
                if (_InfoBlock1 != value)
                {
                    _InfoBlock1 = value;
                    SetInfoBlock(2, _InfoBlock1);
                }
            }
        }

        private String _InfoBlock2;
        /// <summary>
        /// Gets or sets the data of the device (InfoBlock2).
        /// </summary>
        /// <value>The data.</value>
        public String InfoBlock2 {
            get {
                if (_InfoBlock2 == null) {
                    GetInfoBlock (3, out _InfoBlock2);
                }

                return _InfoBlock2;
            }
            set {
                if (_InfoBlock2 != value)
                {
                    _InfoBlock2 = value;
                    SetInfoBlock(3, _InfoBlock2);
                }
            }
        }

        /// <summary>
        /// Sets the info block.
        /// </summary>
        /// <param name="id">2 - InfoBlock1, 3 - InfoBlock2</param>
        /// <param name="data">Maximum 32 bytes of data</param>
        private void SetInfoBlock (byte id, string data)
        {
            SetInfoBlock(id, Encoding.ASCII.GetBytes(data));
        }

        private Boolean GetInfoBlock (byte id, out string data)
        {
            byte[] dataBytes;
            Boolean result = GetInfoBlock (id, out dataBytes);

            if (result) {
                for (int i = 1; i < dataBytes.Length; i++) {
                    if (dataBytes [i] == 0) {
                        Array.Resize (ref dataBytes, i);
                        break;
                    }
                }

                data = Encoding.ASCII.GetString (dataBytes, 1, dataBytes.Length - 1);
            } else {
                data = "";
            }

            return result;
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format</param>
        public void SetLedColor(String color)
        {
            if (!IsValidColor(color))
                throw new Exception("Color value is invalid");

            SetLedColor(
                Convert.ToByte(color.Substring(1, 2), 16),
                Convert.ToByte(color.Substring(3, 2), 16),
                Convert.ToByte(color.Substring(5, 2), 16));
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public void SetLedColor(byte channel, byte index, byte r, byte g, byte b)
        {
            if (connectedToDriver)
            {
                byte [] data = new byte[6];
                data[0] = 5;
                data[1] = channel;
                data[2] = index;
                data[3] = r;
                data[4] = g;
                data[5] = b;

                stream.SetFeature(data);
            }
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format</param>
        public void SetLedColor(byte channel, byte index, string color)
        {
            if (!IsValidColor(color))
                throw new Exception("Color value is invalid");

            SetLedColor(
                channel,
                index,
                Convert.ToByte(color.Substring(1, 2), 16),
                Convert.ToByte(color.Substring(3, 2), 16),
                Convert.ToByte(color.Substring(5, 2), 16));
        }

        /// <summary>
        /// Sets the mode for BlinkStick Pro.
        /// </summary>
        /// <param name="mode">0 - Normal, 1 - Inverse, 2 - WS2812</param>
        public void SetLedMode(byte mode)
        {
            if (connectedToDriver)
            {
                byte [] data = new byte[2];
                data[0] = 4;
                data[1] = mode;
                stream.SetFeature(data);
            }
        }

        /// <summary>
        /// Send a packet of data to LEDs
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="reportData">Report data must be a byte array in the following format: [g0, r0, b0, g1, r1, b1, g2, r2, b2 ...]</param>
        public void SetLedColors(byte channel, byte[] reportData)
        {
            int max_leds = 64;
            byte reportId = 9;

            //Automatically determine the correct report id to send the data to
            if (reportData.Length <= 8 * 3)
            {
                max_leds = 8;
                reportId = 6;
            }
            else if (reportData.Length <= 16 * 3)
            {
                max_leds = 16;
                reportId = 7;
            }
            else if (reportData.Length <= 32 * 3)
            {
                max_leds = 32;
                reportId = 8;
            }
            else if (reportData.Length <= 64 * 3)
            {
                max_leds = 64;
                reportId = 9;
            }
            else if (reportData.Length <= 128 * 3)
            {
                max_leds = 64;
                reportId = 10;
            }

            byte [] data = new byte[max_leds * 3 + 2];
            data[0] = reportId;
            data[1] = channel; // chanel index

            for (int i = 0; i < Math.Min(reportData.Length, data.Length - 2); i++)
            {
                data[i + 2] = reportData[i];
            }

            for (int i = reportData.Length + 2; i < data.Length; i++)
            {
                data[i] = 0;
            }

            stream.SetFeature(data);

            if (reportId == 10)
            {
                //System.Threading.Thread.Sleep(1);

                for (int i = 0; i < Math.Min(data.Length - 2, reportData.Length - 64 * 3); i++)
                {
                    data[i + 2] = reportData[64 * 3 + i];
                } 

                for (int i = reportData.Length + 2 - 64 * 3; i < data.Length; i++)
                {
                    data[i] = 0;
                }

                data[0] = (byte)(reportId + 1);

                stream.SetFeature(data);
            }
        } 

        /// <summary>
        /// Determines if is valid color format of the specified string (#rrggbb).
        /// </summary>
        /// <returns><c>true</c> if is valid color the specified color; otherwise, <c>false</c>.</returns>
        /// <param name="color">Color.</param>
        public static Boolean IsValidColor (String color)
        {
            return Regex.IsMatch(color, "^#[A-Fa-f0-9]{6}$");
        }

        /// <summary>
        /// Occurs when BlinkStick device is attached.
        /// </summary>
        public event EventHandler DeviceAttached;

        /// <summary>
        /// Occurs when a BlinkStick device is removed.
        /// </summary>
        public event EventHandler DeviceRemoved;

        /// <summary>
        /// Initializes a new instance of the BlinkstickHid class.
        /// </summary>
        public BlinkstickHid()
        {
        }

        /// <summary>
        /// Attempts to connect to a BlinkStick device.
        /// 
        /// After a successful connection, a DeviceAttached event will normally be sent.
        /// </summary>
        /// <returns>True if a Blinkstick device is connected, False otherwise.</returns>
        public bool OpenDevice ()
		{
			if (this.device == null) {
                HidDeviceLoader loader = new HidDeviceLoader();
                HidDevice adevice = loader.GetDevices(VendorId, ProductId).FirstOrDefault();
				return OpenDevice (adevice);
			} else {
				return OpenCurrentDevice();
			}
        }

        /// <summary>
        /// Opens the device.
        /// </summary>
        /// <returns><c>true</c>, if device was opened, <c>false</c> otherwise.</returns>
        /// <param name="adevice">Pass the parameter of HidDevice to open it directly</param>
        public bool OpenDevice(HidDevice adevice)
        {
            if (adevice != null)
            {
                this.device = adevice;

                return OpenCurrentDevice();
            }

            return false;
        }

        /// <summary>
        /// Opens the current device.
        /// </summary>
        /// <returns><c>true</c>, if current device was opened, <c>false</c> otherwise.</returns>
		private bool OpenCurrentDevice()
		{
            connectedToDriver = true;
            device.TryOpen(out stream);

            //!!!device.Inserted += DeviceAttachedHandler;
            //!!!device.Removed += DeviceRemovedHandler;

			return true;
		}

        /// <summary>
        /// Find all BlinkStick devices.
        /// </summary>
        /// <returns>The devices.</returns>
        public static BlinkstickHid[] AllDevices ()
		{
            List<BlinkstickHid> result = new List<BlinkstickHid>();

            HidDeviceLoader loader = new HidDeviceLoader();
            foreach (HidDevice adevice in loader.GetDevices(VendorId, ProductId).ToArray())
            {
                BlinkstickHid hid = new BlinkstickHid();
                hid.device = adevice;
                result.Add(hid);
            }

            return result.ToArray();      
        }

        /// <summary>
        /// Closes the connection to the device.
        /// </summary>
        public void CloseDevice()
        {
            stream.Close();
            device = null;
            connectedToDriver = false;
        }

        /// <summary>
        /// Closes the connection to the device.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

		private void DeviceAttachedHandler()
        {
            if (DeviceAttached != null)
                DeviceAttached(this, EventArgs.Empty);
        }

        private void DeviceRemovedHandler()
        {
            if (DeviceRemoved != null)
                DeviceRemoved(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public void SetLedColor(byte r, byte g, byte b)
        {
            if (connectedToDriver)
            {
                byte [] data = new byte[4];
                data[0] = 1;
                data[1] = r;
                data[2] = g;
                data[3] = b;

                stream.SetFeature(data);
            }
        }

        /// <summary>
        /// Gets the color of the led.
        /// </summary>
        /// <returns><c>true</c>, if led color was gotten, <c>false</c> otherwise.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
		public Boolean GetLedColor (out byte r, out byte g, out byte b)
		{
            byte[] report = new byte[33]; 
            report[0] = 1;

            if (connectedToDriver) {
                stream.GetFeature(report, 0, 33);

				r = report [1];
				g = report [2];
				b = report [3];

				return true;
			} else {
				r = 0;
				g = 0;
				b = 0;

				return false;
			}
		}

        /// <summary>
        /// Gets the led data.
        /// </summary>
        /// <returns><c>true</c>, if led data was gotten, <c>false</c> otherwise.</returns>
        /// <param name="data">Data.</param>
        public Boolean GetLedData (out byte[] data)
        {
            if (connectedToDriver)
            {
                data = new byte[3 * 8 * 8 + 1];
                data[0] = 9;
                stream.GetFeature(data, 0, data.Length);
                return true;
            }
            else
            {
                data = new byte[0];
                return false;
            }

        }

        protected void SetInfoBlock (byte id, byte[] data)
		{
			if (id == 2 || id == 3) {
				if (data.Length > 32)
				{
		            Array.Resize(ref data, 32);
				}
				else if (data.Length < 32)
				{
					int size = data.Length;

		            Array.Resize(ref data, 32);

					//pad with zeros
					for (int i = size; i < 32; i++)
					{
						data[i] = 0;
					}
				}

                Array.Resize(ref data, 33);


                for (int i = 32; i >0; i--)
                {
                    data[i] = data[i-1];
				}

                data[0] = id;

                stream.SetFeature(data);
			} else {
				throw new Exception("Invalid info block id");
			}
		}

        /// <summary>
        /// Gets the info block.
        /// </summary>
        /// <returns><c>true</c>, if info block was gotten, <c>false</c> otherwise.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="data">Data.</param>
		public Boolean GetInfoBlock (byte id, out byte[] data)
		{
			if (id == 2 || id == 3) {
                data = new byte[33];
                data[0] = id;

                if (connectedToDriver)
				{
                    stream.GetFeature(data, 0, data.Length);
                	return true;
				}
				else
				{
					data = new byte[0];
					return false;
				}
			} else {
				throw new Exception("Invalid info block id");
			}
		}


        /// <summary>
        /// Closes any connected devices.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    CloseDevice();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Destroys instance and frees device resources (if not freed already)
        /// </summary>
        ~BlinkstickHid()
        {
            Dispose(false);
        }

	}
}

