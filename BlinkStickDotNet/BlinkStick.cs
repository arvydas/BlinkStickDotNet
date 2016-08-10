using BlinkStickDotNet.Meta;
using BlinkStickDotNet.Usb;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace BlinkStickDotNet
{
    /// <summary>
    /// BlinkStick class is designed to control regular BlinkStick devices.<para/>
    /// Code examples on how you can use this class are available
    /// <a href="https://github.com/arvydas/BlinkStickDotNet/wiki">wiki</a>.
    /// </summary>
	public class BlinkStick : IDisposable
    {
        public const int VendorId = 0x20A0;
        public const int ProductId = 0x41E5;

        private int? _mode;
        private bool _disposed = false;
        private bool _stopped = false;

        private IUsbDevice _device;
        private IUsbStream _stream;
        private BlinkStickMetaData _meta;

        #region Events

        public event SendColorEventHandler SendColor;

        private bool OnSendColor(byte channel, byte index, byte r, byte g, byte b)
        {
            if (SendColor != null)
            {
                var args = new SendColorEventArgs(channel, index, r, g, b);

                SendColor(this, args);

                return args.SendToDevice;
            }

            return true;
        }

        public event ReceiveColorEventHandler ReceiveColor;

        private bool OnReceiveColor(byte index, out byte r, out byte g, out byte b)
        {
            if (ReceiveColor != null)
            {
                var args = new ReceiveColorEventArgs(index);

                ReceiveColor(this, args);

                r = args.R;
                g = args.G;
                b = args.B;

                return true;
            }

            r = g = b = 0;

            return false;
        }

        #endregion

        #region Device Properties

        /// <summary>
        /// Gets the meta.
        /// </summary>
        /// <value>
        /// The meta.
        /// </value>
        public BlinkStickMetaData Meta
        {
            get
            {
                if (_device != null && _meta == null)
                {
                    _meta = new BlinkStickMetaData(_device, this);
                }

                return _meta;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BlinkStickDotNet.BlinkStick"/> is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected
        {
            get { return _stream != null; }
        }

        /// <summary>
        /// Returns the serial number of BlinkStick.<para/>
        /// <pre>
        /// BSnnnnnn-1.0
        /// ||  |    | |- Software minor version
        /// ||  |    |--- Software major version
        /// ||  |-------- Denotes sequential number
        /// ||----------- Denotes BlinkStick device
        /// </pre>
        /// Software version defines the capabilities of the device
        /// </summary>
        /// <value>The serial.</value>
        [Obsolete("Please use Meta.Serial.")]
        public string Serial
        {
            get { return Meta?.Serial; }
        }

        /// <summary>
        /// Gets the major version number from serial number.
        /// </summary>
        /// <value>Returns the major version number.</value>
        [Obsolete("Please use Meta.VersionMajor.")]
        public int VersionMajor
        {
            get { return (Meta?.VersionMajor).GetValueOrDefault(); }
        }

        /// <summary>
        /// Gets the minor version number from serial number.
        /// </summary>
        /// <value>Returns the version minor.</value>
        [Obsolete("Please use Meta.VersionMinor.")]
        public int VersionMinor
        {
            get { return (Meta?.VersionMinor).GetValueOrDefault(); }
        }

        /// <summary>
        /// Gets the device type
        /// </summary>
        /// <value>Returns the device type.</value>
        [Obsolete("Please use Meta.BlinkStickDevice.")]
        public BlinkStickDeviceEnum BlinkStickDevice
        {
            get { return (Meta?.BlinkStickDevice).GetValueOrDefault(); }
        }

        /// <summary>
        /// Gets the name of the manufacturer.
        /// </summary>
        /// <value>Returns the name of the manufacturer.</value>
        [Obsolete("Please use Meta.ManufacturerName.")]
        public string ManufacturerName
        {
            get { return Meta?.Manufacturer; }
        }

        /// <summary>
        /// Gets the product name of the device.
        /// </summary>
        /// <value>Returns the name of the product.</value>
        [Obsolete("Please use Meta.ProductName.")]
        public string ProductName
        {
            get { return Meta?.ProductName; }
        }

        /// <summary>
        /// Gets or sets the data of the device (InfoBlock1).
        /// </summary>
        /// <value>String value of InfoBlock1.</value>
        [Obsolete("Please use Meta.InfoBlock1.")]
        public string InfoBlock1
        {
            get { return Meta?.InfoBlock1; }
            set
            {
                if (Meta != null)
                {
                    Meta.InfoBlock1 = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the data of the device (InfoBlock2).
        /// </summary>
        /// <value>String value of InfoBlock2.</value>
        [Obsolete("Please use Meta.InfoBlock2.")]
        public string InfoBlock2
        {
            get { return Meta?.InfoBlock2; }
            set
            {
                if (Meta != null)
                {
                    Meta.InfoBlock2 = value;
                }
            }
        }

        public int SetColorDelay { get; set; }

        /// <summary>
        /// Gets or sets the mode of BlinkStick device.
        /// </summary>
        /// <value>The mode to set or get.</value>
        public int Mode
        {
            get
            {
                if (_mode == null)
                {
                    _mode = GetMode();
                }

                return _mode.GetValueOrDefault();
            }
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    SetMode((byte)_mode);
                }
            }
        }
        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Initializes a new instance of the BlinkStick class.
        /// </summary>
        public BlinkStick()
        {
            SetColorDelay = 0;
        }

        public BlinkStick(IUsbDevice device)
        {
            this._device = device;
        }

        /// <summary>
        /// Disposes of the device and closes the connection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes any connected devices.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    CloseDevice();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Destroys instance and frees device resources (if not freed already)
        /// </summary>
        ~BlinkStick()
        {
            Dispose(false);
        }
        #endregion

        #region Device Open/Close functions

        /// <summary>
        /// Attempts to connect to a BlinkStick device.
        /// 
        /// After a successful connection, a DeviceAttached event will normally be sent.
        /// </summary>
        /// <returns>True if a Blinkstick device is connected, False otherwise.</returns>
        public bool OpenDevice(IUsbDevice aDevice = null)
        {
            if (aDevice != null)
            {
                //Todo: what to do with the previously opened device?
                if (_device != null)
                {
                    CloseDevice();
                }

                _device = aDevice;
            }
            else if (_device == null)
            {
                _device = UsbMonitor.GetFirstDevice(VendorId, ProductId);
            }

            if (_device == null)
            {
                return false;
            }

            return OpenCurrentDevice();
        }

        /// <summary>
        /// Opens the current device.
        /// </summary>
        /// <returns>
        ///   <c>true</c>, if current device was opened, <c>false</c> otherwise.
        /// </returns>
        private bool OpenCurrentDevice()
        {
            //Todo: looks like the device always opens.

            _meta = null;
            _device.TryOpen(out _stream);

            return true;
        }

        /// <summary>
        /// Closes the connection to the device.
        /// </summary>
        public void CloseDevice()
        {
            _stream.Close();
            _stream = null;
            _device = null;
            _meta = null;
        }
        #endregion

        #region Helper functions for InfoBlocks

        /// <summary>
        /// Sets the info block.
        /// </summary>
        /// <param name="id">2 - InfoBlock1, 3 - InfoBlock2</param>
        /// <param name="data">Maximum 32 bytes of data</param>
        internal void SetInfoBlock(byte id, string data)
        {
            SetInfoBlock(id, Encoding.ASCII.GetBytes(data));
        }

        internal bool GetInfoBlock(byte id, out string data)
        {
            byte[] dataBytes;
            bool result = GetInfoBlock(id, out dataBytes);

            if (result)
            {
                for (int i = 1; i < dataBytes.Length; i++)
                {
                    if (dataBytes[i] == 0)
                    {
                        Array.Resize(ref dataBytes, i);
                        break;
                    }
                }

                data = Encoding.ASCII.GetString(dataBytes, 1, dataBytes.Length - 1);
            }
            else
            {
                data = "";
            }

            return result;
        }

        internal void SetInfoBlock(byte id, byte[] data)
        {
            if (id == 2 || id == 3)
            {
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


                for (int i = 32; i > 0; i--)
                {
                    data[i] = data[i - 1];
                }

                data[0] = id;

                SetFeature(data);
            }
            else
            {
                throw new Exception("Invalid info block id");
            }
        }

        /// <summary>
        /// Gets the info block.
        /// </summary>
        /// <returns><c>true</c>, if info block was received, <c>false</c> otherwise.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="data">Data.</param>
        public bool GetInfoBlock(byte id, out byte[] data)
        {
            if (id == 2 || id == 3)
            {
                data = new byte[33];
                data[0] = id;

                if (Connected)
                {
                    int attempt = 0;
                    while (attempt < 5)
                    {
                        attempt++;
                        try
                        {
                            _stream.GetFeature(data, 0, data.Length);
                            break;
                        }
                        catch (System.IO.IOException e)
                        {
                            if (e.InnerException is System.ComponentModel.Win32Exception)
                            {
                                System.ComponentModel.Win32Exception win32Exception = e.InnerException as System.ComponentModel.Win32Exception;

                                if (win32Exception != null && win32Exception.NativeErrorCode == 0)
                                    return true;
                            }

                            if (attempt == 5)
                                throw;

                            if (!WaitThread(20))
                                return false;
                        }
                    }

                    return true;
                }
                else
                {
                    data = new byte[0];
                    return false;
                }
            }
            else
            {
                throw new Exception("Invalid info block id");
            }
        }
        #endregion

        #region Color manipulation functions

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public void SetColor(byte r, byte g, byte b)
        {
            if (!OnSendColor(0, 0, r, g, b))
                return;

            if (Connected)
            {
                if (Meta.RequiresSoftwareColorPatch)
                {
                    byte cr, cg, cb;
                    if (GetColor(out cr, out cg, out cb))
                    {
                        if (r == cg && g == cr && b == cb)
                        {
                            if (cr > 0)
                            {
                                SetFeature(new byte[4] { 1, (byte)(cr - 1), cg, cb });
                            }
                            else if (cg > 0)
                            {
                                SetFeature(new byte[4] { 1, cr, (byte)(cg - 1), cb });
                            }
                        }
                    }
                }

                SetFeature(new byte[4] { 1, r, g, b });
            }
        }

        /// <summary>
        /// Gets the color of the led.
        /// </summary>
        /// <returns><c>true</c>, if led color was received, <c>false</c> otherwise.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public bool GetColor(out byte r, out byte g, out byte b)
        {
            if (OnReceiveColor(0, out r, out g, out b))
            {
                return true;
            }

            byte[] report = new byte[33];
            report[0] = 1;

            if (Connected)
            {
                int attempt = 0;
                while (attempt < 5)
                {
                    attempt++;
                    try
                    {
                        _stream.GetFeature(report, 0, 33);
                        break;
                    }
                    catch
                    {
                        if (attempt == 5)
                            throw;

                        if (!WaitThread(20))
                            return false;
                    }
                }

                r = report[1];
                g = report[2];
                b = report[3];

                return true;
            }

            r = g = b = 0;

            return false;
        }

        /// <summary>
        /// Turn BlinkStick off.
        /// </summary>
        public void TurnOff()
        {
            SetColor(0, 0, 0);
        }

        #endregion

        #region Color manipulation functions for BlinkStick Pro

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public void SetColor(byte channel, byte index, byte r, byte g, byte b)
        {
            if (!OnSendColor(channel, index, r, g, b))
            {
                return;
            }

            if (Connected)
            {
                SetFeature(new byte[6] { 5, channel, index, r, g, b });
            }
        }

        /// <summary>
        /// Send a packet of data to LEDs
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="reportData">Report data must be a byte array in the following format: [g0, r0, b0, g1, r1, b1, g2, r2, b2 ...]</param>
        public void SetColors(byte channel, byte[] colorData)
        {
            int max_leds = 64;
            byte reportId = 9;

            //Automatically determine the correct report id to send the data to
            if (colorData.Length <= 8 * 3)
            {
                max_leds = 8;
                reportId = 6;
            }
            else if (colorData.Length <= 16 * 3)
            {
                max_leds = 16;
                reportId = 7;
            }
            else if (colorData.Length <= 32 * 3)
            {
                max_leds = 32;
                reportId = 8;
            }
            else if (colorData.Length <= 64 * 3)
            {
                max_leds = 64;
                reportId = 9;
            }
            else if (colorData.Length <= 128 * 3)
            {
                max_leds = 64;
                reportId = 10;
            }

            byte[] data = new byte[max_leds * 3 + 2];
            data[0] = reportId;
            data[1] = channel; // chanel index

            for (int i = 0; i < Math.Min(colorData.Length, data.Length - 2); i++)
            {
                data[i + 2] = colorData[i];
            }

            for (int i = colorData.Length + 2; i < data.Length; i++)
            {
                data[i] = 0;
            }

            SetFeature(data);

            if (reportId == 10)
            {
                for (int i = 0; i < Math.Min(data.Length - 2, colorData.Length - 64 * 3); i++)
                {
                    data[i + 2] = colorData[64 * 3 + i];
                }

                for (int i = colorData.Length + 2 - 64 * 3; i < data.Length; i++)
                {
                    data[i] = 0;
                }

                data[0] = (byte)(reportId + 1);

                SetFeature(data);
            }
        }

        /// <summary>
        /// Gets led data.
        /// </summary>
        /// <returns><c>true</c>, if led data was received, <c>false</c> otherwise.</returns>
        /// <param name="data">LED data as an array of colors [g0, r0, b0, g1, r1, b1 ...]</param>
        public bool GetColors(out byte[] colorData)
        {
            if (Connected)
            {
                byte[] data = new byte[3 * 8 * 8 + 2];
                data[0] = 9;
                _stream.GetFeature(data, 0, data.Length);

                colorData = new byte[3 * 8 * 8];
                Array.Copy(data, 2, colorData, 0, colorData.Length);

                return true;
            }

            colorData = new byte[0];
            return false;
        }

        /// <summary>
        /// Gets the color of the led.
        /// </summary>
        /// <returns><c>true</c>, if led color was received, <c>false</c> otherwise.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public bool GetColor(byte index, out byte r, out byte g, out byte b)
        {
            if (OnReceiveColor(index, out r, out g, out b))
            {
                return true;
            }

            if (index == 0)
            {
                return this.GetColor(out r, out g, out b);
            }

            byte[] colors;
            this.GetColors(out colors);

            if (colors.Length >= (index + 1) * 3)
            {
                r = colors[index * 3 + 1];
                g = colors[index * 3];
                b = colors[index * 3 + 2];

                return true;
            }

            r = g = b = 0;
            return false;
        }

        #endregion

        #region BlinkStick Pro mode selection

        /// <summary>
        /// Sets the mode for BlinkStick Pro.
        /// </summary>
        /// <param name="mode">0 - Normal, 1 - Inverse, 2 - WS2812</param>
        public void SetMode(byte mode)
        {
            if (Connected)
            {
                SetFeature(new byte[2] { 4, mode });
            }
        }

        /// <summary>
        /// Gets the mode on BlinkStick Pro.
        /// </summary>
        /// <param name="mode">0 - Normal, 1 - Inverse, 2 - WS2812, 3 - WS2812 mirror</param>
        public int GetMode()
        {
            if (Connected)
            {
                byte[] data = new byte[2];
                data[0] = 4;
                GetFeature(data);
                return data[1];
            }

            return -1;
        }

        #endregion

        #region BlinkStick Flex features

        public void SetLedCount(byte count)
        {
            if (Connected)
            {
                SetFeature(new byte[2] { 0x81, count });
            }
        }

        /// <summary>
        /// Gets the led count.
        /// </summary>
        /// <returns>The number of leds. <c>-1</c> when no device is connected.</returns>
        public int GetLedCount()
        {
            if (Connected)
            {
                byte[] data = new byte[2];
                data[0] = 0x81;
                _stream.GetFeature(data, 0, data.Length);
                return data[1];
            }
            return -1;
        }

        #endregion

        #region Animation Control

        public void Stop()
        {
            this._stopped = true;
        }

        public void Enable()
        {
            this._stopped = false;
        }

        #endregion

        #region Blink Animation

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public void Blink(byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int delay = 500)
        {
            for (int i = 0; i < repeats; i++)
            {
                this.InternalSetColor(channel, index, r, g, b);

                if (!WaitThread(delay))
                    return;

                this.InternalSetColor(channel, index, 0, 0, 0);

                if (!WaitThread(delay))
                    return;
            }
        }

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public void Blink(byte channel, byte index, RgbColor color, int repeats = 1, int delay = 500)
        {
            this.Blink(channel, index, color.R, color.G, color.B, repeats, delay);
        }

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public void Blink(byte channel, byte index, string color, int repeats = 1, int delay = 500)
        {
            this.Blink(channel, index, RgbColor.FromString(color), repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public void Blink(byte r, byte g, byte b, int repeats = 1, int delay = 500)
        {
            this.Blink(0, 0, r, g, b, repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public void Blink(RgbColor color, int repeats = 1, int delay = 500)
        {
            this.Blink(0, 0, color, repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public void Blink(string color, int repeats = 1, int delay = 500)
        {
            this.Blink(0, 0, color, repeats, delay);
        }

        #endregion

        #region Morph Animation

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Morph(byte channel, byte index, byte r, byte g, byte b, int duration = 1000, int steps = 50)
        {
            if (_stopped)
                return;

            byte cr, cg, cb;
            GetColor(index, out cr, out cg, out cb);

            for (int i = 1; i <= steps; i++)
            {
                this.InternalSetColor(channel, index,
                    (byte)(1.0 * cr + (r - cr) / 1.0 / steps * i),
                    (byte)(1.0 * cg + (g - cg) / 1.0 / steps * i),
                    (byte)(1.0 * cb + (b - cb) / 1.0 / steps * i));

                if (!WaitThread(duration / steps))
                    return;
            }
        }

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Morph(byte channel, byte index, RgbColor color, int duration = 1000, int steps = 50)
        {
            this.Morph(channel, index, color.R, color.G, color.B, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Morph(byte channel, byte index, string color, int duration = 1000, int steps = 50)
        {
            this.Morph(channel, index, RgbColor.FromString(color), duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Morph(byte r, byte g, byte b, int duration = 1000, int steps = 50)
        {
            this.Morph(0, 0, r, g, b, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Morph(RgbColor color, int duration = 1000, int steps = 50)
        {
            this.Morph(0, 0, color, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Morph(string color, int duration = 1000, int steps = 50)
        {
            this.Morph(0, 0, color, duration, steps);
        }
        #endregion

        #region Pulse Animation
        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Pulse(byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50)
        {
            this.InternalSetColor(channel, index, 0, 0, 0);

            if (this.SetColorDelay > 0)
            {
                if (!WaitThread(this.SetColorDelay))
                    return;
            }

            for (int i = 0; i < repeats; i++)
            {
                if (this._stopped)
                    break;

                this.Morph(channel, index, r, g, b, duration, steps);

                if (this._stopped)
                    break;

                this.Morph(channel, index, 0, 0, 0, duration, steps);
            }
        }

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Pulse(byte channel, byte index, RgbColor color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            this.Pulse(channel, index, color.R, color.G, color.B, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Pulse(byte channel, byte index, string color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            this.Pulse(channel, index, RgbColor.FromString(color), repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Pulse(byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50)
        {
            this.Pulse(0, 0, r, g, b, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Pulse(RgbColor color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            this.Pulse(0, 0, color, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public void Pulse(string color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            this.Pulse(0, 0, color, repeats, duration, steps);
        }
        #endregion

        #region Static Functions to initialize BlinkSticks

        /// <summary>
        /// Find all BlinkStick devices.
        /// </summary>
        /// <returns>An array of BlinkStick devices</returns>
        public static BlinkStick[] FindAll()
        {
            var devices = UsbMonitor.GetDevices(VendorId, ProductId);
            return devices.Select(d => new BlinkStick(d)).ToArray();
        }

        /// <summary>
        /// Find first BlinkStick.
        /// </summary>
        /// <returns>The BlinkStick device if found, otherwise <c>null</c>.</returns>
        public static BlinkStick FindFirst()
        {
            var device = UsbMonitor.GetFirstDevice(VendorId, ProductId);
            return device == null ? null : new BlinkStick(device);
        }

        /// <summary>
        /// Finds BlinkStick by serial number.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <returns>The BlinkStick device if found, otherwise <c>null</c>.</returns>
        public static BlinkStick FindBySerial(string serial)
        {
            if (String.IsNullOrEmpty(serial))
            {
                throw new ArgumentNullException(nameof(serial));
            }

            var device = UsbMonitor.GetFirstDevice(VendorId, ProductId, serial);
            return device == null ? null : new BlinkStick(device);
        }

        #endregion

        #region Misc helper functions

        /// <summary>
        /// Automatically sets the color of the device using either BlinkStick or BlinkStick Pro API
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        private void InternalSetColor(byte channel, byte index, byte r, byte g, byte b)
        {
            if (channel == 0 && index == 0)
            {
                this.SetColor(r, g, b);
            }
            else
            {
                this.SetColor(channel, index, r, g, b);
            }
        }

        private void SetFeature(byte[] buffer)
        {
            int attempt = 0;
            while (attempt < 5)
            {
                attempt++;
                try
                {
                    _stream.SetFeature(buffer);
                    break;
                }
                catch (System.IO.IOException e)
                {
                    if (e.InnerException is System.ComponentModel.Win32Exception)
                    {
                        System.ComponentModel.Win32Exception win32Exception = e.InnerException as System.ComponentModel.Win32Exception;

                        if (win32Exception != null && win32Exception.NativeErrorCode == 0)
                            return;
                    }

                    if (attempt == 5)
                        throw;

                    if (!WaitThread(20))
                        return;
                }
            }
        }

        private void GetFeature(byte[] buffer)
        {
            int attempt = 0;
            while (attempt < 5)
            {
                attempt++;
                try
                {
                    _stream.GetFeature(buffer);
                    break;
                }
                catch (System.IO.IOException e)
                {
                    if (e.InnerException is System.ComponentModel.Win32Exception)
                    {
                        System.ComponentModel.Win32Exception win32Exception = e.InnerException as System.ComponentModel.Win32Exception;

                        if (win32Exception != null && win32Exception.NativeErrorCode == 0)
                            return;
                    }

                    if (attempt == 5)
                        throw;

                    if (!WaitThread(20))
                        return;
                }
            }
        }

        public bool WaitThread(long milliseconds)
        {
            DateTime nextRetry = DateTime.Now + new TimeSpan(TimeSpan.TicksPerMillisecond * milliseconds);

            while (nextRetry > DateTime.Now)
            {
                if (this._stopped)
                    return false;

                Thread.Sleep(20);
            }

            return true;
        }

        #endregion
    }
}