using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    //with help from http://groups.google.com/group/Google-Maps-API/browse_thread/thread/278923f0aad4811a
    public class CoordinateConverter
    {
        public const int OFFSET = 268435456;
        public const double RADIUS = OFFSET/Math.PI;
        public static int LonToX(double lon)
        {
            return (int)Math.Round(OFFSET + RADIUS * lon * Math.PI / 180);
        }
        public static int LatToY(double lat)
        {
            return
                (int)Math.Round(OFFSET - RADIUS*Math.Log((1 + Math.Sin(lat*Math.PI/180))/(1 - Math.Sin(lat*Math.PI/180)))/2);
        }
        public static double YToLat(int y)
        {
            return (Math.PI / 2 - 2 * Math.Atan(Math.Exp((y - OFFSET) / RADIUS)))*180/Math.PI;
        }
        public static double XToLon(int x)
        {
            return ((x - OFFSET)/RADIUS)*180/Math.PI;
        }
        public static double AdjustLonByPixels(double lon, int delta, int zoom)
        {
            return XToLon(LonToX(lon) + (delta << (21 - zoom)));
        }
        public static double AdjustLatByPixels(double lat, int delta, int zoom)
        {
            return YToLat(LatToY(lat) + (delta << (21 - zoom)));
        }

    }
}
