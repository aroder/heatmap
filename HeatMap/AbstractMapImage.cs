using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    public abstract class AbstractMapImage
    {
        public Bounds Bounds { get; set; }

        public double DegreesPerPixelLat
        {
            get { return (this.Bounds.TopLeftLat - this.Bounds.BottomRightLat) / this.Bounds.Height; }
        }
        public double DegreesPerPixelLng
        {
            get { return (this.Bounds.BottomRightLng - this.Bounds.TopLeftLng) / this.Bounds.Width; }
        }
        public int PixelsPerDegreeLat
        {
            get { return (int)(this.Bounds.Height / (this.Bounds.TopLeftLat - this.Bounds.BottomRightLat)); }
        }
        public int PixelsPerDegreeLng
        {
            get { return (int)(this.Bounds.Width / (this.Bounds.BottomRightLng - this.Bounds.TopLeftLng)); }
        }
    }
}
