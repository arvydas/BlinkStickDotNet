using BlinkStickDotNet.Usb;
using System;

namespace BlinkStickDotNet.Meta
{
    /// <summary>
    /// Calculates the meta data for the device.
    /// </summary>
    public class BlinkStickMetaData
    {
        private int? _versionMajor;
        private int? _versionMinor;
        private string _infoBlock1;
        private string _infoBlock2;
        private bool? _requiresSoftwareColorPatch;

        IUsbDevice _device;
        BlinkStick _stick;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkStickMetaData"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public BlinkStickMetaData(IUsbDevice device, BlinkStick stick)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _device = device;
            _stick = stick;
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
        public string Serial
        {
            get { return _device?.SerialNumber; }
        }

        /// <summary>
        /// Gets the major version number from serial number.
        /// </summary>
        /// <value>Returns the major version number.</value>
        public int VersionMajor
        {
            get
            {
                if (_versionMajor == null)
                {
                    try
                    {
                        _versionMajor = Convert.ToInt32(this.Serial.Substring(this.Serial.Length - 3, 1));
                    }
                    catch
                    {
                        _versionMajor = 0;
                    }
                }

                return _versionMajor.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Gets the minor version number from serial number.
        /// </summary>
        /// <value>Returns the minor version number.</value>
        public int VersionMinor
        {
            get
            {
                if (_versionMinor == null)
                {
                    try
                    {
                        _versionMinor = Convert.ToInt32(this.Serial.Substring(this.Serial.Length - 1, 1));
                    }
                    catch
                    {
                        _versionMinor = 0;
                    }
                }

                return _versionMinor.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stick needs to patch the color. This is due to a hardware bug in some versions.
        /// </summary>
        /// <value>
        /// <c>true</c> if the software requires a color patch; otherwise, <c>false</c>.
        /// </value>
        public bool RequiresSoftwareColorPatch
        {
            get
            {
                if (_requiresSoftwareColorPatch == null)
                {
                    _requiresSoftwareColorPatch = VersionMajor == 1 && VersionMinor >= 1 && VersionMinor <= 3;
                }

                return _requiresSoftwareColorPatch.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Gets the blink stick device.
        /// </summary>
        /// <value>
        /// The blink stick device.
        /// </value>
        public BlinkStickDeviceEnum BlinkStickDevice
        {
            get
            {
                if (VersionMajor == 1)
                {
                    return BlinkStickDeviceEnum.BlinkStick;
                }
                else if (VersionMajor == 2)
                {
                    return BlinkStickDeviceEnum.BlinkStickPro;
                }
                else if (VersionMajor == 3)
                {
                    if (_device?.ProductVersion == 0x0200)
                    {
                        return BlinkStickDeviceEnum.BlinkStickSquare;
                    }
                    else if (_device?.ProductVersion == 0x0201)
                    {
                        return BlinkStickDeviceEnum.BlinkStickStrip;
                    }
                    else if (_device?.ProductVersion == 0x0202)
                    {
                        return BlinkStickDeviceEnum.BlinkStickNano;
                    }
                    else if (_device?.ProductVersion == 0x0203)
                    {
                        return BlinkStickDeviceEnum.BlinkStickFlex;
                    }
                }

                return BlinkStickDeviceEnum.Unknown;
            }
        }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer
        {
            get { return _device?.Manufacturer; }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName
        {
            get { return _device?.ProductName; }
        }

        /// <summary>
        /// Gets or sets the data of the device (InfoBlock1).
        /// </summary>
        /// <value>String value of InfoBlock1.</value>
        public string InfoBlock1
        {
            get
            {
                if (_infoBlock1 == null)
                {
                    _stick.GetInfoBlock(2, out _infoBlock1);
                }

                return _infoBlock1;
            }
            set
            {
                if (_infoBlock1 != value)
                {
                    _infoBlock1 = value;
                    _stick.SetInfoBlock(2, _infoBlock1);
                }
            }
        }

        /// <summary>
        /// Gets or sets the data of the device (InfoBlock2).
        /// </summary>
        /// <value>String value of InfoBlock2.</value>
        public string InfoBlock2
        {
            get
            {
                if (_infoBlock2 == null)
                {
                    _stick.GetInfoBlock(3, out _infoBlock2);
                }

                return _infoBlock2;
            }
            set
            {
                if (_infoBlock2 != value)
                {
                    _infoBlock2 = value;
                    _stick.SetInfoBlock(3, _infoBlock2);
                }
            }
        }

        /// <summary>
        /// Gets the BlinkStick device type from the serial.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <returns>The device type.</returns>
        public static BlinkStickDeviceEnum BlinkStickDeviceFromSerial(string serial)
        {
            int versionMajor = Convert.ToInt32(serial.Substring(serial.Length - 3, 1));

            if (versionMajor == 1)
            {
                return BlinkStickDeviceEnum.BlinkStick;
            }
            else if (versionMajor == 2)
            {
                return BlinkStickDeviceEnum.BlinkStickPro;
            }
            else if (versionMajor == 3)
            {
                return BlinkStickDeviceEnum.BlinkStickSquare;
            }

            return BlinkStickDeviceEnum.Unknown;
        }
    }
}