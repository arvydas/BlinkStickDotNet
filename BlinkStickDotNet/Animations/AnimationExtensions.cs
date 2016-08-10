using System;
using System.Threading;

namespace BlinkStickDotNet
{
    /// <summary>
    /// Seperates the animation from the BlinkStick implementation.
    /// </summary>
    public static class AnimationExtensions
    {
        #region Animation Control

        public static void Stop(this BlinkStick stick)
        {
            if(stick == null)
            {
                throw new ArgumentNullException(nameof(stick));
            }

            stick.AnimationState.Stopped = true;
        }

        public static void Enable(this BlinkStick stick)
        {
            if (stick == null)
            {
                throw new ArgumentNullException(nameof(stick));
            }

            stick.AnimationState.Stopped = false;
        }
        
        #endregion

        #region Blink Animation

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Blink(this BlinkStick stick, byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int delay = 500)
        {
            if (stick == null)
            {
                throw new ArgumentNullException(nameof(stick));
            }

            for (int i = 0; i < repeats; i++)
            {
                stick.InternalSetColor(channel, index, r, g, b);

                if (!stick.WaitThread(delay))
                    return;

                stick.InternalSetColor(channel, index, 0, 0, 0);

                if (!stick.WaitThread(delay))
                    return;
            }
        }

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Blink(this BlinkStick stick, byte channel, byte index, RgbColor color, int repeats = 1, int delay = 500)
        {
            stick.Blink(channel, index, color.R, color.G, color.B, repeats, delay);
        }

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Blink(this BlinkStick stick, byte channel, byte index, string color, int repeats = 1, int delay = 500)
        {
            stick.Blink(channel, index, RgbColor.FromString(color), repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Blink(this BlinkStick stick, byte r, byte g, byte b, int repeats = 1, int delay = 500)
        {
            stick.Blink(0, 0, r, g, b, repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Blink(this BlinkStick stick, RgbColor color, int repeats = 1, int delay = 500)
        {
            stick.Blink(0, 0, color, repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Blink(this BlinkStick stick, string color, int repeats = 1, int delay = 500)
        {
            stick.Blink(0, 0, color, repeats, delay);
        }

        #endregion

        #region Morph Animation

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Morph(this BlinkStick stick, byte channel, byte index, byte r, byte g, byte b, int duration = 1000, int steps = 50)
        {
            if (stick == null)
            {
                throw new ArgumentNullException(nameof(stick));
            }

            if (stick.AnimationState.Stopped)
                return;

            byte cr, cg, cb;
            stick.GetColor(index, out cr, out cg, out cb);

            for (int i = 1; i <= steps; i++)
            {
                stick.InternalSetColor(channel, index,
                    (byte)(1.0 * cr + (r - cr) / 1.0 / steps * i),
                    (byte)(1.0 * cg + (g - cg) / 1.0 / steps * i),
                    (byte)(1.0 * cb + (b - cb) / 1.0 / steps * i));

                if (!stick.WaitThread(duration / steps))
                    return;
            }
        }

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Morph(this BlinkStick stick, byte channel, byte index, RgbColor color, int duration = 1000, int steps = 50)
        {
            stick.Morph(channel, index, color.R, color.G, color.B, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Morph(this BlinkStick stick, byte channel, byte index, string color, int duration = 1000, int steps = 50)
        {
            stick.Morph(channel, index, RgbColor.FromString(color), duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Morph(this BlinkStick stick, byte r, byte g, byte b, int duration = 1000, int steps = 50)
        {
            stick.Morph(0, 0, r, g, b, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Morph(this BlinkStick stick, RgbColor color, int duration = 1000, int steps = 50)
        {
            stick.Morph(0, 0, color, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Morph(this BlinkStick stick, string color, int duration = 1000, int steps = 50)
        {
            stick.Morph(0, 0, color, duration, steps);
        }

        #endregion

        #region Pulse Animation

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">The repeats.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Pulse(this BlinkStick stick, byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50)
        {
            if (stick == null)
            {
                throw new ArgumentNullException(nameof(stick));
            }

            stick.InternalSetColor(channel, index, 0, 0, 0);

            if (stick.SetColorDelay > 0)
            {
                if (!stick.WaitThread(stick.SetColorDelay))
                    return;
            }

            for (int i = 0; i < repeats; i++)
            {
                if (stick.AnimationState.Stopped)
                    break;

                stick.Morph(channel, index, r, g, b, duration, steps);

                if (stick.AnimationState.Stopped)
                    break;

                stick.Morph(channel, index, 0, 0, 0, duration, steps);
            }
        }

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="repeats">The repeats.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Pulse(this BlinkStick stick, byte channel, byte index, RgbColor color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            stick.Pulse(channel, index, color.R, color.G, color.B, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">The repeats.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Pulse(this BlinkStick stick, byte channel, byte index, string color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            stick.Pulse(channel, index, RgbColor.FromString(color), repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">The repeats.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Pulse(this BlinkStick stick, byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50)
        {
            stick.Pulse(0, 0, r, g, b, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">The repeats.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Pulse(this BlinkStick stick, RgbColor color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            stick.Pulse(0, 0, color, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">The repeats.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        /// <remarks>Executes the animation on the current thread. Might block the thread.</remarks>
        public static void Pulse(this BlinkStick stick, string color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            stick.Pulse(0, 0, color, repeats, duration, steps);
        }
     
        #endregion
    }
}
