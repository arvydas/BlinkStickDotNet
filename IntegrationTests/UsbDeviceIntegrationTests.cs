using BlinkStickDotNet.Usb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlinkStickDotNet.IntegrationTests
{
    [TestClass]
    public class UsbDeviceIntegrationTests
    {
        [TestMethod]
        public void UsbDevice_First_Reconnect()
        {
            var msg = new MessageBox();
            var finished = new ManualResetEvent(false);

            //! use blinkstick device -- nobody wants to disconnect their printer ;)
            var device = UsbMonitor.GetFirstDevice(BlinkStick.VendorId, BlinkStick.ProductId);
            if(device == null)
            {
                var monitor = new UsbMonitor();
                monitor.Connected += (s, e) => 
                {
                    device = e.Device;

                    msg.Close();
                    finished.Set();
                };

                monitor.Start();

                msg.Show("Please connect a BlinkStick device.");
                finished.WaitOne(5000);
            }
                
            Assert.IsNotNull(device, "A stick should have been connected.");

            bool reconnect = false;
            device.Reconnect += (s, e) =>
            {
                reconnect = true;
                msg.Close();
                finished.Set();
            };

            msg.Show("Please reconnect the BlinkStick device.");
            finished.WaitOne(15000);

            Assert.IsTrue(reconnect, " A stick should have been reconnected.");
        }
    }
}
