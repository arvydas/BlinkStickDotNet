using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlinkStickDotNet.Usb;
using System.Threading;

namespace BlinkStickDotNet.IntegrationTests
{
    /// <summary>
    /// This intergration test can only be done by connecting and disconnecting the sticks.
    /// They are created for debugging purposes and should not be run in production.
    /// </summary>
    [TestClass]
    public class UsbMonitorIntegrationTest
    {
        [TestMethod]
        public void UsbMonitor_ConnectDeviceEvent_ConnectDetected()
        {
            var msg = new MessageBox("Please connect a device. If one is connected it, please reconnect.");
            var finished = new ManualResetEvent(false);

            IUsbDevice device = null;
            var monitor = new UsbMonitor();
            monitor.Connected += (sender, e) =>
            {
                device = e.Device;
                msg.Close();
                finished.Set();
            };

            monitor.Start();

            msg.Show();
            finished.WaitOne(5000);

            Assert.IsNotNull(device, "No connect detected.");
        }

        [TestMethod]
        public void UsbMonitor_DisconnectDeviceEvent_DisconnectDetected()
        {
            var msg = new MessageBox("Please disconnect a device.");
            var finished = new ManualResetEvent(false);

            IUsbDevice device = null;
            var monitor = new UsbMonitor();
            monitor.Disconnected += (sender, e) =>
            {
                device = e.Device;
                msg.Close();
                finished.Set();
            };

            monitor.Start();

            msg.Show();
            finished.WaitOne(5000);

            Assert.IsNotNull(device, "No disconnect detected.");
        }

        [TestMethod]
        public void UsbMonitor_ConnectBlinkStick_ConnectDetected()
        {
            var msg = new MessageBox();
            var finished = new ManualResetEvent(false);

            IUsbDevice device = null;
            var monitor = new UsbMonitor(BlinkStick.VendorId, BlinkStick.ProductId);
            monitor.Connected += (sender, e) =>
            {
                device = e.Device;
                msg.Close();
                finished.Set();
            };

            monitor.Start();

            msg.Show("Please connect a BlinkStick device. If one is connected it, please reconnect.");
            finished.WaitOne(5000);

            Assert.IsNotNull(device, "No connect detected.");
        }

        [TestMethod]
        public void UsbMonitor_DisconnectBlinkStick_DisconnectDetected()
        {
            var msg = new MessageBox("Please disconnect a BlinkStick device.");
            var finished = new ManualResetEvent(false);

            IUsbDevice device = null;
            var monitor = new UsbMonitor(BlinkStick.VendorId, BlinkStick.ProductId);
            monitor.Disconnected += (sender, e) =>
            {
                device = e.Device;
                msg.Close();
                finished.Set();
            };

            monitor.Start();

            msg.Show();
            finished.WaitOne(5000);

            Assert.IsNotNull(device, "No disconnect detected.");
        }

        [TestMethod]
        public void UsbMonitor_BlinkStickDevice_Reconnect()
        {
            var msg = new MessageBox();
            var finished = new ManualResetEvent(false);

            IUsbDevice device = null;
            var monitor = new UsbMonitor(BlinkStick.VendorId, BlinkStick.ProductId);
            monitor.Connected += (sender, e) =>
            {
                device = e.Device;
                msg.Close();
                finished.Set();
            };

            monitor.Start();

            msg.Show("Please connect a BlinkStick device. If one is connected, please reconnect it.");
            finished.WaitOne(5000);

            Assert.IsNotNull(device, "No device detected.");

            bool reconnect = false;

            device.Reconnect += (sender, e) =>
            {
                reconnect = true;
                msg.Close();
                finished.Set();
            };

            msg.Show("Please reconnect the BlinkStick.");
            finished.WaitOne(5000);

            Assert.IsTrue(reconnect, "No reconnect detected.");
        }

    }
}
