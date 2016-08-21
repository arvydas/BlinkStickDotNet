using System;

namespace BlinkStickDotNet.Colors
{
    /// <summary>
    /// Defines an HSL color.
    /// </summary>
    /// <remarks>Check http://en.wikipedia.org/wiki/HSL_color_space for more info.</remarks>
    public struct HSLColor
    {
        private double _hue;
        private double _saturation;
        private double _lightness;

        public double Hue
        {
            get { return _hue; }
            set { _hue = value; }
        }
        public double Saturation
        {
            get { return _saturation; }
            set { _saturation = value; }
        }
        public double Lightness
        {
            get { return _lightness; }
            set
            {
                _lightness = value;
                if (_lightness < 0)
                    _lightness = 0;
                if (_lightness > 1)
                    _lightness = 1;
            }
        }
        public HSLColor(double hue, double saturation, double lightness)
        {
            _hue = Math.Min(360, hue);
            _saturation = Math.Min(1, saturation);
            _lightness = Math.Min(1, lightness);
        }
        public HSLColor(RgbColor color)
        {
            _hue = 0;
            _saturation = 1;
            _lightness = 1;
            FromRGB(color);
        }
        public RgbColor Color
        {
            get { return ToRGB(); }
            set { FromRGB(value); }
        }
        void FromRGB(RgbColor cc)
        {
            double r = (double)cc.R / 255d;
            double g = (double)cc.G / 255d;
            double b = (double)cc.B / 255d;

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            // calulate hue according formula given in
            // "Conversion from RGB to HSL or HSV"
            _hue = 0;
            if (min != max)
            {
                if (r == max && g >= b)
                {
                    _hue = 60 * ((g - b) / (max - min)) + 0;
                }
                else
                if (r == max && g < b)
                {
                    _hue = 60 * ((g - b) / (max - min)) + 360;
                }
                else
                if (g == max)
                {
                    _hue = 60 * ((b - r) / (max - min)) + 120;
                }
                else
                if (b == max)
                {
                    _hue = 60 * ((r - g) / (max - min)) + 240;
                }
            }
            // find lightness
            _lightness = (min + max) / 2;

            // find saturation
            if (_lightness == 0 || min == max)
                _saturation = 0;
            else
            if (_lightness > 0 && _lightness <= 0.5)
                _saturation = (max - min) / (2 * _lightness);
            else
            if (_lightness > 0.5)
                _saturation = (max - min) / (2 - 2 * _lightness);
        }
        RgbColor ToRGB()
        {
            // convert to RGB according to
            // "Conversion from HSL to RGB"

            double r = _lightness;
            double g = _lightness;
            double b = _lightness;
            if (_saturation == 0)
                return RgbColor.FromRgb((int)(r * 255), (int)(g * 255), (int)(b * 255));

            double q = 0;
            if (_lightness < 0.5)
                q = _lightness * (1 + _saturation);
            else
                q = _lightness + _saturation - (_lightness * _saturation);
            double p = 2 * _lightness - q;
            double hk = _hue / 360;

            // r,g,b colors
            double[] tc = new double[3] { hk + (1d / 3d), hk, hk - (1d / 3d) };
            double[] colors = new double[3] { 0, 0, 0 };

            for (int color = 0; color < colors.Length; color++)
            {
                if (tc[color] < 0)
                    tc[color] += 1;
                if (tc[color] > 1)
                    tc[color] -= 1;

                if (tc[color] < (1d / 6d))
                    colors[color] = p + ((q - p) * 6 * tc[color]);
                else
                if (tc[color] >= (1d / 6d) && tc[color] < (1d / 2d))
                    colors[color] = q;
                else
                if (tc[color] >= (1d / 2d) && tc[color] < (2d / 3d))
                    colors[color] = p + ((q - p) * 6 * (2d / 3d - tc[color]));
                else
                    colors[color] = p;

                colors[color] *= 255; // convert to value expected by Color
            }
            return RgbColor.FromRgb((int)colors[0], (int)colors[1], (int)colors[2]);
        }

        public static bool operator !=(HSLColor left, HSLColor right)
        {
            return !(left == right);
        }
        public static bool operator ==(HSLColor left, HSLColor right)
        {
            return (left.Hue == right.Hue &&
                    left.Lightness == right.Lightness &&
                    left.Saturation == right.Saturation);
        }
        public override string ToString()
        {
            string s = string.Format("HSL({0:f2}, {1:f2}, {2:f2})", Hue, Saturation, Lightness);
            s += string.Format("RGB({0:x2}{1:x2}{2:x2})", Color.R, Color.G, Color.B);
            return s;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}