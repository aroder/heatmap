using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    public struct HeatPoint
    {
        public int X;
        public int Y;
        public byte Intensity;
        public HeatPoint(int iX, int iY, byte bIntensity)
        {
            X = iX;
            Y = iY;
            Intensity = bIntensity;
        }
    }
}
