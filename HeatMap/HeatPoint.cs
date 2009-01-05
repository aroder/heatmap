using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace AdamRoderick.HeatMap
{
    [Serializable]
    [DataContract(Name = "HeatPoint")]
    [DebuggerDisplay("{Latitude}, {Longitude}, Intensity = {Intensity}")]
    public class HeatPoint
    {

        [DataMember]
        public double Latitude { get; private set; }

        [DataMember]
        public double Longitude { get; private set; }

        //intensity is between 0 and 255, but the public property is percent based (0-100)
        internal byte _intensity; //default 50%
        /// <summary>
        /// Percent value.  100% is high intensity.  The higher the intensity, the "hotter" each point will be.  At low intensity, it will take many points to result in a very hot are.  Default is 50%
        /// </summary>
        public int Intensity
        {
            get
            {
                return (int)Math.Ceiling(_intensity * 100.0 / 255.0);
            }
            set
            {
                if (0 > value || 100 < value) throw new ArgumentException("Intensity must be between 0 and 100");
                _intensity = (byte)(value * 255 / 100);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lon">Longitude</param>
        /// <param name="lat">Latitude</param>
        /// <param name="intensity">Percent value representing how "hot" this point is.  100% makes each point very "hot."</param>
        public HeatPoint(double lat, double lon, int intensity)
        {
            this.Longitude = lon;
            this.Latitude = lat;
            Intensity = intensity;
        }
        public HeatPoint(double lat, double lon) : this(lat, lon, HeatmapSettings.DEFAULT_INTENSITY)
        {
        }
        public HeatPoint() : this(0,0,HeatmapSettings.DEFAULT_INTENSITY){}

        public static int GetPixelWidth(HeatPoint h1, HeatPoint h2)
        {
            return GetPixelWidth(h1, h2, 1);
        }
        public static int GetPixelWidth(HeatPoint h1, HeatPoint h2, double scale)
        {
            return (int)( scale * GetDistance(h1.Latitude, h1.Latitude, h2.Longitude, h1.Longitude));
        }
        public static int GetPixelHeight(HeatPoint h1, HeatPoint h2)
        {
            return GetPixelHeight(h1, h2, 1);
        }
        public static int GetPixelHeight(HeatPoint h1, HeatPoint h2, double scale)
        {
            return (int)( scale * GetDistance(h1.Latitude, h2.Latitude, h2.Longitude, h2.Longitude));
        }

        private static double GetDistance(double lat1, double lat2, double lon1, double lon2)
        {
            double distance = 3963.0 *
                             Math.Acos(Math.Sin(lat1 / 57.2958) * Math.Sin(lat2 / 57.2958) +
                                       Math.Cos(lat1 / 57.2958) * Math.Cos(lat2 / 57.2958) *
                                       Math.Cos(lon2 / 57.2958 - lon1 / 57.2958));
            return distance;
        }


    }

}
