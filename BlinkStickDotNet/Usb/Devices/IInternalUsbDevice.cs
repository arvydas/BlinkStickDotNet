namespace BlinkStickDotNet.Usb
{

    /// <summary>
    /// Internal interface for USB devices.  
    /// Needed for signaling connect and disconnect events.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Usb.IUsbDevice" />
    internal interface IInternalUsbDevice : IUsbDevice
    {
        /// <summary>
        /// Called when the USB device is connected.
        /// </summary>
        void OnConnect();

        /// <summary>
        /// Called when the USB device is disconnected.
        /// </summary>
        void OnDisconnect();
    }
}
