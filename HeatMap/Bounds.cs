using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    [DebuggerDisplay("TopLeft: ({TopLeftLat}, {TopLeftLng}), BottomRight: ({BottomRightLat}, {BottomRightLng}), Width: {Width}, Height: {Height}")]
    public class Bounds
    {
        public Bounds()
        {
        }
        public Bounds(Bounds bounds)
        {
            this.TopLeftLat = bounds.TopLeftLat;
            this.TopLeftLng = bounds.TopLeftLng;
            this.BottomRightLat = bounds.BottomRightLat;
            this.BottomRightLng = bounds.BottomRightLng;
        }

        public double TopLeftLat { get; set; }
        public double TopLeftLng { get; set; }
        public double BottomRightLat { get; set; }
        public double BottomRightLng { get; set; }

        public bool Contains(HeatPoint heatPoint)
        {
            return heatPoint.Latitude <= TopLeftLat
                   && heatPoint.Latitude >= BottomRightLat
                   && heatPoint.Longitude >= TopLeftLng
                   && heatPoint.Longitude <= BottomRightLng;
        }

        public double Width
        {
            get { return BottomRightLng - TopLeftLng; }
        }
        public double Height
        {
            get { return TopLeftLat - BottomRightLat;  }
        }
    }
}
