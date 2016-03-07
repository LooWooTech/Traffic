using ESRI.ArcGIS.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoowooTech.Traffic.Common
{
    public static class DisplayHelper
    {
        public static IRgbColor GetRGBColor(int Red, int Green, int Blue, byte Alpha = 255)
        {
            IRgbColor color = new RgbColorClass();
            color.Red = Red;
            color.Green = Green;
            color.Blue = Blue;
            color.Transparency = Alpha;
            return color;
        }

        public static IRgbColor GetSelectRGBColor()
        {
            int temp;
            return GetRGBColor(int.TryParse(System.Configuration.ConfigurationManager.AppSettings["SELECTRED"],out temp)?temp:0, int.TryParse(System.Configuration.ConfigurationManager.AppSettings["SELECTGREEN"],out temp)?temp:0, int.TryParse(System.Configuration.ConfigurationManager.AppSettings["SELECTBLUE"],out temp)?temp:0);
        }

        public static ISimpleLineSymbol GetSimpleLineSymbol(IRgbColor rgbColor,double width)
        {
            var simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Color = rgbColor;
            simpleLineSymbol.Width = width;
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            return simpleLineSymbol;
        }

    }
}
