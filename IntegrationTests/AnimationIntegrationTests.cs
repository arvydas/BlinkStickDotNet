using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlinkStickDotNet.IntegrationTests
{
    /// <summary>
    /// Integration tests for animations. The animations block the thread,
    /// so they can be tested synchronously.
    /// </summary>
    [TestClass]
    public class AnimationIntegrationTests
    {
        [TestMethod]
        public void BlinkStick_Animation_BlinkRed5Times()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();

            stick.OpenDevice();
            stick.Blink("red", 5);
        }

        [TestMethod]
        public void BlinkStick_Animation_MorphFromRedToBlueIn2Seconds()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();

            stick.OpenDevice();
            stick.SetColor("red");
            stick.Morph("blue", 2000);
        }

        [TestMethod]
        public void BlinkStick_Animation_PusleBlue5TimesIn5Seconds()
        {
            var stick = BlinkStickIntegrationTests.EnsureBlinkStick();

            stick.OpenDevice();
            stick.SetColor("black");
            stick.Pulse("blue", 5, 1000);
        }
    }
}