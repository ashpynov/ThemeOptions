using System;
using System.Linq;

namespace ThemeOptions.Tools
{
    public class DisplayAspectRatio
    {
        private struct AspectRatio
        {
            public double Width;
            public double Height;
            public AspectRatio(int width, int height)
            {
                Width = width;
                Height = height;
            }

            public string Name => $"dsp{Width}{Height}";
            public double Ratio => Width / Height;
        }
        private static readonly AspectRatio[] commonRatios =
        {
            new AspectRatio(4, 3),
            new AspectRatio(16, 9),
            new AspectRatio(16, 10),
            new AspectRatio(21, 9),
            new AspectRatio(19, 10),
            new AspectRatio(32, 9)
        };

        public static string GetAspectRatio(double width, double height)
        {
            double ratio = width / height;
            return commonRatios.OrderBy(c => Math.Abs(ratio - c.Ratio)).First().Name;
        }
    }
}