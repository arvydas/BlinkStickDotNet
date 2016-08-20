using System.Drawing;

namespace BlinkStickDotNet.Animations.Processors
{
    /// <summary>
    /// Indicates the object implements a BlinkStick color processor. The processor processes
    /// the given colors to the leds. Hides implementation details.
    /// </summary>
    public interface IColorProcessor
    {
        /// <summary>
        /// Gets the nr of leds.
        /// </summary>
        /// <value>
        /// The nr of leds.
        /// </value>
        uint NrOfLeds { get; }

        /// <summary>
        /// Processes the colors. Uses round robin to set the led colors.
        /// </summary>
        /// <example>
        /// If there are 4 given colors and 8 leds, the following will happend:
        /// Led0 = Color0, Led1 = Color1, 
        /// Led2 = Color2, Led3 = Color3,
        /// Led4 = Color0, Led5 = Color1,
        /// Led6 = Color2, Led7 = Color3
        /// 
        /// If there is 2 given colors and 2 leds, the following will happen:
        /// Led0 = Color0
        /// </example>
        /// <param name="colors">The colors.</param>
        void ProcessColors(params Color[] colors);

        /// <summary>
        /// Processes the colors. Uses round robin with an offset to set the led colors.
        /// Makes it easier to build chaser patterns.
        /// </summary>
        /// <example>
        /// If there are 4 given colors, 8 leds and offset 1, the following will happend:
        /// Led0 = Color1, Led1 = Color2, 
        /// Led2 = Color3, Led3 = Color0,
        /// Led4 = Color1, Led5 = Color2,
        /// Led6 = Color3, Led7 = Color0
        /// 
        /// If there is 2 given colors, 2 leds and offset 1, the following will happen:
        /// Led0 = Color1
        /// </example>
        void ProcessColors(int offset, params Color[] colors);

        /// <summary>
        /// Gets the last used current colors.
        /// </summary>
        /// <returns>The colors.</returns>
        Color[] GetCurrentColors();
    }
}