using System;

namespace BlinkStickDotNet
{
    /// <summary>
    /// Seperate color implementation for the BlinkStick source by using extension methods.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format</param>
        public static void SetColor(this BlinkStick blinkStick, string color)
        {
            if (blinkStick == null)
            {
                throw new ArgumentNullException(nameof(blinkStick));
            }

            blinkStick.SetColor(RgbColor.FromString(color));
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Color as RgbColor class.</param>
        public static void SetColor(this BlinkStick blinkStick, RgbColor color)
        {
            if (blinkStick == null)
            {
                throw new ArgumentNullException(nameof(blinkStick));
            }

            blinkStick.SetColor(color.R, color.G, color.B);
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="blinkStick">The blink stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        public static void SetColor(this BlinkStick blinkStick, byte channel, byte index, string color)
        {
            if (blinkStick == null)
            {
                throw new ArgumentNullException(nameof(blinkStick));
            }

            blinkStick.SetColor(channel, index, RgbColor.FromString(color));
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="blinkStick">The blink stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void SetColor(this BlinkStick blinkStick, byte channel, byte index, RgbColor color)
        {
            if (blinkStick == null)
            {
                throw new ArgumentNullException(nameof(blinkStick));
            }

            blinkStick.SetColor(channel, index, color.R, color.G, color.B);
        }
    }
}