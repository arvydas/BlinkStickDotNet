using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace BlinkStickDotNet.Animations
{
    /// <summary>
    /// Extends the .Net colors with new features.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts a color to an HTML hexadecimal color string.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The #hexadecimal string.</returns>
        public static string ToHtmlString(this Color color)
        {
            if (color == null)
            {
                throw new ArgumentNullException(nameof(color));
            }

            return "#" +
                ConvertByteToHex(color.R) +
                ConvertByteToHex(color.G) +
                ConvertByteToHex(color.B);
        }

        /// <summary>
        /// Converts the byte to hexadecimal.
        /// </summary>
        /// <param name="aByte">The byte.</param>
        /// <returns>The string.</returns>
        private static string ConvertByteToHex(byte aByte)
        {
            var hex = aByte.ToString("X");
            if (hex.Length == 1)
            {
                hex = "0" + hex;
            }

            return hex;
        }

        /// <summary>
        /// Makes the percentage fraction between 0 and 1.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns>The percentage.</returns>
        private static double MakePercentageFraction(double amount)
        {
            return Math.Min(1, Math.Max(0, amount));
        }

        /// <summary>
        /// Darkens the color the specified percentage (between 0 and 1).
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns>The darkened color.</returns>
        public static Color Darken(this Color color, double percentage)
        {
            if (color == null)
            {
                throw new ArgumentNullException(nameof(color));
            }

            percentage = MakePercentageFraction(percentage);

            var r = color.R * (1 - percentage);
            var g = color.G * (1 - percentage);
            var b = color.B * (1 - percentage);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        /// <summary>
        /// Darkens the colors specified percentage (between 0 and 1).
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns>An array with the darkened colors.</returns>
        public static Color[] Darken(this Color[] colors, double percentage)
        {
            percentage = MakePercentageFraction(percentage);

            var list = new List<Color>();

            foreach (var color in colors)
            {
                var color2 = color.Darken(percentage);
                list.Add(color2);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Lightens the color the specified percentage (between 0 and 1).
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="percentage">The amount.</param>
        /// <returns>The lightened color.</returns>
        public static Color Lighten(this Color color, double percentage)
        {
            if (color == null)
            {
                throw new ArgumentNullException(nameof(color));
            }

            percentage = MakePercentageFraction(percentage);

            var r = color.R + (255 - color.R) * percentage;
            var g = color.G + (255 - color.G) * percentage;
            var b = color.B + (255 - color.B) * percentage;

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

        /// <summary>
        /// Lightens the colors specified percentage (between 0 and 1).
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="percentage">The amount.</param>
        /// <returns></returns>
        public static Color[] Lighten(this Color[] colors, double percentage)
        {
            percentage = MakePercentageFraction(percentage);

            var list = new List<Color>();

            foreach (var color in colors)
            {
                var color2 = color.Lighten(percentage);
                list.Add(color2);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Creates a color array of the specified size and pads it with black values.
        /// The current color and the specified colors will be added first.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="size">The amount.</param>
        /// <param name="colors">The colors.</param>
        /// <returns>An array with the colors.</returns>
        public static Color[] PadBlack(this Color color, int size, params Color[] colors)
        {
            var list = new List<Color>();

            list.Add(color);
            list.AddRange(colors);

            while (list.Count < size)
            {
                list.Add(Color.Black);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Creates the color from HTML string.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The color.</returns>
        public static Color CreateColorFromHtmlString(string color)
        {
            if (String.IsNullOrEmpty(color))
            {
                throw new ArgumentNullException(nameof(color));
            }

            if (color.StartsWith("#"))
            {
                color = color.Substring(1);
            }

            if (TestHexString(color))
            {
                return ColorTranslator.FromHtml("#" + color);
            }

            return ColorTranslator.FromHtml(color);
        }

        /// <summary>
        /// Clones the array. When the size is smaller it will return the first part 
        /// of the original array. When the size is bigger it will add new elements from the 
        /// beginning to the array.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static Color[] CloneArray(this Color[] colors, int size)
        {
            if (colors.Length == size)
            {
                return new List<Color>(colors).ToArray();
            }

            var newColors = new List<Color>(size);
            for (int i = 0; i < size; i++)
            {
                if (colors.Length == 0)
                {
                    newColors.Add(Color.Black);
                    continue;
                }

                var z = i % colors.Length;
                newColors.Add(colors[z]);
            }

            return newColors.ToArray();
        }

        /// <summary>
        /// Tests the hexadecimal string.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <returns><c>true</c> if the string is hexadecimal.</returns>
        private static bool TestHexString(string test)
        {
            return test.ToCharArray().All(c => "0123456789abcdefABCDEF".Contains(c));
        }
    }
}