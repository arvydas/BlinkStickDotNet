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
    public class BlinkStickColorIntegrationTests
    {
        [TestMethod]
        public void BlinkStick_NamePurpleHtmlRgb_Color()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.OpenDevice();

            var color = RgbColor.FromString("#F260F7");
            stick.SetColor(color);
            Thread.Sleep(1000);

            byte r, g, b;

            stick.GetColor(out r, out g, out b);

            Assert.AreEqual(r, 242, "Red should be 242.");
            Assert.AreEqual(g, 96, "Green should be 96.");
            Assert.AreEqual(b, 247, "Blue should be 247.");

        }

        [TestMethod]
        public void BlinkStick_Color_Red()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.OpenDevice();

            stick.SetColor(255, 0, 0);
            Thread.Sleep(1000);

            byte r, g, b;

            stick.GetColor(out r, out g, out b);

            Assert.AreEqual(r, 255, "Red should be 255.");
            Assert.AreEqual(g, 0, "Green should be 0.");
            Assert.AreEqual(b, 0, "Blue should be 0.");
        }

        [TestMethod]
        public void BlinkStick_Color_Green()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.OpenDevice();

            stick.SetColor(0, 255, 0);
            Thread.Sleep(1000);

            byte r, g, b;

            stick.GetColor(out r, out g, out b);

            Assert.AreEqual(r, 0, "Red should be 0.");
            Assert.AreEqual(g, 255, "Green should be 255.");
            Assert.AreEqual(b, 0, "Blue should be 0.");
        }

        [TestMethod]
        public void BlinkStick_Color_Blue()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.OpenDevice();

            stick.SetColor(0, 0, 255);
            Thread.Sleep(1000);

            byte r, g, b;

            stick.GetColor(out r, out g, out b);

            Assert.AreEqual(r, 0, "Red should be 0.");
            Assert.AreEqual(g, 0, "Green should be 0.");
            Assert.AreEqual(b, 255, "Blue should be 255.");
        }

        [TestMethod]
        public void BlinkStick_Color_White()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.OpenDevice();

            stick.SetColor(255, 255, 255);
            Thread.Sleep(1000);

            byte r, g, b;

            stick.GetColor(out r, out g, out b);

            Assert.AreEqual(r, 255, "Red should be 255.");
            Assert.AreEqual(g, 255, "Green should be 255.");
            Assert.AreEqual(b, 255, "Blue should be 255.");
        }


        [TestMethod]
        public void BlinkStick_Color_Black()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();
            stick.OpenDevice();

            stick.SetColor(0, 0, 0);
            Thread.Sleep(1000);

            byte r, g, b;

            stick.GetColor(out r, out g, out b);

            Assert.AreEqual(r, 0, "Red should be 0.");
            Assert.AreEqual(g, 0, "Green should be 0.");
            Assert.AreEqual(b, 0, "Blue should be 0.");
        }
    }
}
