using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Common
{
    public static class DisplayHelper
    {
        public static IRgbColor GetRGBColor(int Red, int Green, int Blue)
        {
            IRgbColor color = new RgbColorClass();
            color.Red = Red;
            color.Green = Green;
            color.Blue = Blue;
            return color;
        }
    }
}
