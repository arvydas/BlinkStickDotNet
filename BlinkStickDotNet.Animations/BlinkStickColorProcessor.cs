using System;
using System.Collections.Generic;
using System.Drawing;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Wraps the BlinkStick into a color processor.
    /// </summary>
    /// <seealso cref="BlinkStickDotNet.Animations.IBlinkStickColorProcessor" />
    public class BlinkStickColorProcessor : IBlinkStickColorProcessor
    {
        private Color[] _currentColors;
        private BlinkStick _stick;

        /// <summary>
        /// Gets the nr of leds.
        /// </summary>
        /// <value>
        /// The nr of leds.
        /// </value>
        public uint NrOfLeds { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkStickColorProcessor" /> class.
        /// </summary>
        /// <param name="stick">The stick.</param>
        /// <param name="nrOfLeds">The nr of leds.</param>
        public BlinkStickColorProcessor(BlinkStick stick, uint nrOfLeds = 1)
        {
            _stick = stick;

            NrOfLeds = nrOfLeds;

            if (NrOfLeds > 1)
            {
                _stick.SetMode(2);
            }
        }

        /// <summary>
        /// Turns the stick off.
        /// </summary>
        public void Off()
        {
            ProcessColors(Color.Black);
        }

        /// <summary>
        /// Processes the colors. Uses round robin with an offset to set the led colors.
        /// Makes it easier to build chaser patterns.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="colors"></param>
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
        public void ProcessColors(int offset, Color[] colors)
        {
            var currentColors = new List<Color>();
            var bytes = new List<byte>();

            for (int l = 0; l < NrOfLeds; l++)
            {
                var colorIndex = (offset + l) % colors.Length;
                var color = colors[colorIndex];

                //format: GRB - don't ask ;-)
                bytes.Add(color.G);
                bytes.Add(color.R);
                bytes.Add(color.B);

                currentColors.Add(color);
            }

            _stick.SetColors(0, bytes.ToArray());
            _currentColors = currentColors.ToArray();
        }

        /// <summary>
        /// Processes the colors. Uses round robin to set the led colors.
        /// </summary>
        /// <param name="colors">The colors.</param>
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
        public void ProcessColors(params Color[] colors)
        {
            ProcessColors(0, colors);
        }

        /// <summary>
        /// Gets the last used current colors.
        /// </summary>
        /// <returns>
        /// The colors.
        /// </returns>
        public Color[] GetCurrentColors()
        {
            if(_currentColors == null || _currentColors.Length == 0)
            {
                var colors = new List<Color>();
                var bytes = new byte[3 * NrOfLeds];
                this._stick.GetColors(out bytes);

                for(var i = 0;i< NrOfLeds; i++)
                {
                    //format: GRB - don't ask ;-)
                    var g = bytes[i + 0];
                    var r = bytes[i + 1];
                    var b = bytes[i + 2];

                    colors.Add(Color.FromArgb(r, g, b)); 
                }

                return colors.ToArray();
            }

            return _currentColors;
        }
    }
}