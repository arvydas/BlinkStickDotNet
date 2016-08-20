using BlinkStickDotNet.Usb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace BlinkStickDotNet.IntegrationTests
{
    [TestClass]
    public class BlinkStickIntegrationTests
    {
        /// <summary>
        /// Tests finding the first connected BlinkStick.
        /// </summary>
        [TestMethod]
        public void BlinkStick_FindFirst()
        {
            var stick = EnsureBlinkStick();
            Assert.IsNotNull(stick, "No BlinkStick device found.");
        }

        /// <summary>
        /// Tests the opening of a BlinkStick device.
        /// </summary>
        [TestMethod]
        public void BlinkStick_OpenDevice_Empty()
        {
            EnsureDevice();

            var stick = new BlinkStick();
            stick.OpenDevice();

            Assert.IsTrue(stick.Connected, "Stick is not connected.");
        }

        /// <summary>
        /// Ensures the device. When no device is present the use will be asked to connect one.
        /// </summary>
        public static void EnsureDevice()
        {
            BlinkStick stick = BlinkStick.FindFirst();
            if (stick == null)
            {
                var finished = new ManualResetEvent(false);
                var msg = new MessageBox("Please connect a BlinkStick.");
                var connectDetected = false;

                var monitor = new UsbMonitor(BlinkStick.VendorId, BlinkStick.ProductId);
                monitor.Connected += (s, e) =>
                {
                    connectDetected = true;
                    msg.Close();
                    finished.Set();
                };

                msg.Show();
                finished.WaitOne(5000);

                Assert.IsTrue(connectDetected, "No connect detected.");
            }
        }

        /// <summary>
        /// Ensures that a blink stick is present.
        /// </summary>
        /// <returns>The stick.</returns>
        public static BlinkStick EnsureBlinkStick()
        {
            BlinkStick stick = BlinkStick.FindFirst();
            if (stick == null)
            {
                EnsureDevice();
                stick = BlinkStick.FindFirst();
            }

            Assert.IsNotNull(stick, "No BlinkStick device found.");

            return stick;
        }
    }
}
