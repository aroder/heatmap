using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    [Serializable]
    public class HeatPointList : List<HeatPoint>
    {
        public HeatPointList()
        {
        }
        public HeatPointList(IEnumerable<HeatPoint> heatPointArray)
        {
            this.AddRange(heatPointArray);
        }

        public double GetMaxLongitude()
        {
            this.Sort((h1, h2) => h1.Longitude.CompareTo(h2.Longitude));
            return this.Last().Longitude;
        }
        public double GetMaxLatitude()
        {
            this.Sort((h1, h2) => h1.Latitude.CompareTo(h2.Latitude));
            return this.Last().Latitude;
        }
        public double GetMinLongitude()
        {
            this.Sort((h1, h2) => h1.Longitude.CompareTo(h2.Longitude));
            return this.First().Longitude;
        }
        public double GetMinLatitude()
        {
            this.Sort((h1, h2) => h1.Latitude.CompareTo(h2.Latitude));
            return this.First().Latitude;
        }
    }
}
