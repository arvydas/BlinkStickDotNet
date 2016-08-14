using BlinkStickDotNet.Animations;
using BlinkStickDotNet.Usb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Threading;

namespace BlinkStickDotNet.IntegrationTests
{
    [TestClass]
    public class BlinkStickAnimationQueueIntegrationTests
    {
        [TestMethod]
        public void AnimationQueue_InfiniteLoop_Disconnect()
        {
            AnimationQueue q = null;

            try
            {

                var msg = new MessageBox();
                var finished = new ManualResetEvent(false);

                //make sure a stick is connected
                BlinkStickIntegrationTests.EnsureBlinkStick();

                //use the devie to monitor the connect / disconnect
                var device = UsbMonitor.GetFirstDevice(BlinkStick.VendorId, BlinkStick.ProductId);

                var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
                stick.IgnoreDisconnectErrors = true;

                //create queue
                q = new AnimationQueue(stick);
                q.Color(1, Color.Red);
                q.Morph(1000, Color.Green);
                q.Morph(1000, Color.Blue);
                q.Morph(1000, Color.Red);
                q.Loop();
                q.Connect(stick);
                q.Start();

                Thread.Sleep(2000);

                bool disconnected = false;
                device.Disconnect += (sender, e) =>
                {
                    disconnected = true;
                    msg.Close();
                    finished.Set();
                };

                msg.Show("Please disconnect the stick.");
                finished.WaitOne(5000);

                Thread.Sleep(1000);

                Assert.IsTrue(disconnected, "No disconnect detected.");

                bool reconnected = false;
                device.Reconnect += (sender, e) =>
                {
                    reconnected = true;
                    msg.Close();
                    finished.Set();
                };

                msg.Show("Please reconnnect the same stick.");
                finished.WaitOne(5000);

                Assert.IsTrue(reconnected, "No reconnect detected.");

                Thread.Sleep(2000);

                byte r, g, b;
                var color = stick.GetColor(out r, out g, out b);

                var m = Math.Max(r, Math.Max(g, b));

                //colors should not be null
                Assert.AreNotEqual(m, 0, "There should be some color.");
            }
            finally
            {
                q?.Stop();
                q?.Dispose();
            }
        }
    }
}