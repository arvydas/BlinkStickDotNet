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
	public class BlinkstickHid : IDisposable
    {
        protected const int VendorId = 0x20A0;
        protected const int ProductId = 0x41E5;

        private HidDevice device;
        private HidStream stream;

        private bool disposed = false;

        protected bool connectedToDriver = false;

        public Boolean Connected {
            get {
                return connectedToDriver;
            }
        }

        public String Serial {
            get {
                return device.SerialNumber;
            }
        }

        public String ManufacturerName {
            get {
                return device.Manufacturer;
            }
        }

        /// <summary>
        public String ProductName {
            get {
                return device.ProductName;
            }
        }

        private String _Name;
        public String Name {
            get {
                if (_Name == null) {
                    GetInfoBlock (2, out _Name);
                }

                return _Name;
            }
            set {
                if (_Name != value)
                {
                    _Name = value;
                    SetInfoBlock(2, _Name);
                }
            }
        }

        private String _Data;
        public String Data {
            get {
                if (_Data == null) {
                    GetInfoBlock (3, out _Data);
                }

                return _Data;
            }
            set {
                if (_Data != value)
                {
                    _Data = value;
                    SetInfoBlock(3, _Data);
                }
            }
        }

        public void SetInfoBlock (byte id, string data)
        {
            SetInfoBlock(id, Encoding.ASCII.GetBytes(data));
        }

        public Boolean GetInfoBlock (byte id, out string data)
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
        /// Set LED color
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

        public static Boolean IsValidColor (String color)
        {
            return Regex.IsMatch(color, "^#[A-Fa-f0-9]{6}$");
        }

        /// Occurs when a BlinkStick device is attached.
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

        public bool OpenDevice(HidDevice adevice)
        {
            if (adevice != null)
            {
                this.device = adevice;

                return OpenCurrentDevice();
            }

            return false;
        }

		private bool OpenCurrentDevice()
		{
            connectedToDriver = true;
            device.TryOpen(out stream);

            //!!!device.Inserted += DeviceAttachedHandler;
            //!!!device.Removed += DeviceRemovedHandler;

			return true;
		}

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
            /*
			List<WindowsBlinkstickHid> result = new List<WindowsBlinkstickHid>();
			foreach (HidDevice device in HidDevices.Enumerate(VendorId, ProductId).ToArray<HidDevice>()) {
				WindowsBlinkstickHid hid = new WindowsBlinkstickHid();
				hid.device = device;

				result.Add(hid);
			}
			return result.ToArray();
            */         
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

