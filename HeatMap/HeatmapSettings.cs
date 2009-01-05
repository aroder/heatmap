using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AdamRoderick.HeatMap
{
    [Serializable]
    [DataContract(Name = "HeatmapSettings")]
    public class HeatmapSettings
    {
        public static int DEFAULT_SENSITIVITY = 100;
        public static int DEFAULT_ZOOM = 8;
        public static int DEFAULT_TRANSPARENCY = 50;
        public static int DEFAULT_SIZE = 100;
        public static int DEFAULT_INTENSITY = 100;
        public static bool DEFAULT_BORDER = false;
        public static double DEFAULT_DEGREES_PER_PIXEL = 1.0;

        #region Properties

        //private double _degreesPerPixelLat = DEFAULT_DEGREES_PER_PIXEL;
        //public double DegreesPerPixelLat
        //{
        //    get
        //    {
        //        return _degreesPerPixelLat;
        //    }
        //    set
        //    {
        //        _degreesPerPixelLat = value;
        //    }
        //}

        //private double _degreesPerPixelLng = DEFAULT_DEGREES_PER_PIXEL;
        //public double DegreesPerPixelLng
        //{
        //    get
        //    {
        //        return _degreesPerPixelLng;
        //    }
        //    set
        //    {
        //        _degreesPerPixelLng = value;
        //    }
        //}

        public Bounds Bounds { get; set; }

        private bool _border = DEFAULT_BORDER;
        [DataMember]
        public bool Border
        {
            get { return _border; }
            set { _border = value; }
        }

        [DataMember]
        public PALLETES Palette { get; set; }

        //sensitivity is between 0 and 255, but the public property is percent based (0 to 100)
        private byte _sensitivity = (byte)(DEFAULT_SENSITIVITY * 255 / 100);
        /// <summary>
        /// Percent value.  100% shows all heat values, the lower the percent, the less of an area is colored.  For example, 10% would show only the "hottest" 10% of the heat map
        /// </summary>
        [DataMember]
        public int Sensitivity
        {
            get
            {
                return (int)Math.Ceiling(_sensitivity * 100.0 / 255.0);
            }
            set
            {
                if (0 > value || 100 < value) throw new ArgumentOutOfRangeException("Sensitivity must be between 0 and 100");
                _sensitivity = (byte)(value * 255 / 100);
            }
        }

        private int _zoom = DEFAULT_ZOOM;
        [DataMember]
        public int Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                if (1 <= value || 21 >= value) _zoom = value;
            }

        }

        //transparency is between 0 and 255, but the public property is percent based (0 to 100)
        private byte _transparency = (byte)(DEFAULT_TRANSPARENCY * 255 / 100);
        /// <summary>
        /// Percent value.  100% is fully transparent, 0% is invisible
        /// </summary>
        [DataMember]
        public int Transparency
        {
            get
            {
                return (int)Math.Ceiling(_transparency * 100.0 / 255.0);
            }
            set
            {
                if (0 > value || 100 < value) throw new ArgumentOutOfRangeException("Transparency must be between 0 and 100");
                _transparency = (byte)(value * 255 / 100);
            }
        }

        private int _size = DEFAULT_SIZE;
        /// <summary>
        /// Pixel value.  How large the heat radius around each point should be
        /// </summary>
        [DataMember]
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        private int _intensity = DEFAULT_INTENSITY * 255 / 100;
        /// <summary>
        /// Percent value.  A higher value will result in each individual point being "hotter"
        /// </summary>
        [DataMember]
        public int Intensity
        {
            get
            {
                return (int)Math.Ceiling(_intensity * 100.0 / 255.0);
            }
            set
            {
                if (0 > value || 100 < value) throw new ArgumentOutOfRangeException("Intensity must be between 0 and 100");
                _intensity = (byte)(value * 255 / 100);
            }
        }

        public HeatPoint TopLeftPoint { get; set; }

        #endregion

        public HeatmapSettings()
        {
        }

        //public static HeatmapSettings DeepClone(HeatmapSettings source)
        //{
        //    MemoryStream m = new MemoryStream();
        //    BinaryFormatter b = new BinaryFormatter();
        //    b.Serialize(m, source);
        //    m.Position = 0;
        //    return b.Deserialize(m) as HeatmapSettings;
        //}
    }
}
