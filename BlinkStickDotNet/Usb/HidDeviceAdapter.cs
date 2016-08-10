using System;
using HidSharp;

namespace BlinkStickDotNet.Usb
{
    internal class HidDeviceAdapter : IUsbDevice
    {
        private HidDevice device;

        public HidDeviceAdapter(HidDevice device)
        {
            this.device = device;
        }

        public string Manufacturer
        {
            get { return this.device.Manufacturer; }
        }

        public string ProductName
        {
            get { return this.device.ProductName; }
        }

        public int ProductVersion
        {
            get { return this.device.ProductVersion; }
        }

        public string SerialNumber
        {
            get { return this.device.SerialNumber; }
        }

        public void TryOpen(out IUsbStream stream)
        {
            HidStream hid = null;

            this.device.TryOpen(out hid);

            if(hid == null)
            {
                stream = null;
                return;
            }

            stream = new HidStreamAdapter(hid);
        }
    }
}
