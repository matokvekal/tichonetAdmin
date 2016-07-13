using System;
using System.Globalization;

namespace Business_Logic.Helpers
{
    public static class ColorHelper
    {
        public static string CssToNumeric(this string cssColor)
        {
            return cssColor.Replace("#", "");
        }
        public static Line CssToNumeric(this Line line)
        {
            line.HexColor = line.HexColor.CssToNumeric();
            return line;
        }
    }
}
