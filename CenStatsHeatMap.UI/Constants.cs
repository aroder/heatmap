using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace CenStatsHeatMap.UI
{
    public class Constants
    {
        public static class QueryString
        {
            public static class HeatPointData
            {
                public const string Key = "data";
                public static char[] LocationSplitChar = new[] { ',' };
                public static char[] PointCountSplitChar = new[] { ':' };
                public static char[] XYSplitChar = new[] { 'x' };
            }
            public static class HeatMapSettings
            {
                public const string SizeKey = "size";
                public static char[] SizeSplitChar = new[] { 'x' };
                public const string WindowSizeKey = "w";
                public static char[] WindowSizeSplitChar = new char[] { 'x' };
                public const string PointRadiusKey = "radius";
                public const string WindowTopLeftCornerPointKey = "wp";
                public static char[] WindowTopLeftCornerPointSplitChar = new [] {'x'};
                public const string ShowTileBorderKey = "tb";
            }
        }
        public static class HeatMapSettingsDefaults
        {
            public static Size Size = new Size(256, 256);
            public static Size WindowSize = new Size(256, 256);
            public const int PointRadius = 10;
            public static byte HeatPointIntensity = 255;
            public static Point WindowTopLeftCornerPoint = new Point(0,0);
            public static bool ShowTileBorder = false;
        }

    }
}
