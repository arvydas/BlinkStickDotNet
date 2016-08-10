using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlinkStickDotNet.Usb
{
    public interface IUsbStream
    {
        void Close();
        void GetFeature(byte[] data, int v, int length);
        void SetFeature(byte[] buffer);
        void GetFeature(byte[] buffer);
    }
}
