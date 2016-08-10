using System;

namespace BlinkStickDotNet.Usb
{
    /// <summary>
    /// Device modified arguments.
    /// </summary>
    public class DeviceModifiedArgs : EventArgs
    {
        /// <summary>
        /// The device which has been modified.
        /// </summary>
        public BlinkStick Device { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkStickDotNet.DeviceModifiedArgs"/> class.
        /// </summary>
        /// <param name="device">Device passed as an argument</param>
        public DeviceModifiedArgs(BlinkStick device)
        {
            this.Device = device;
        }
    }
}
