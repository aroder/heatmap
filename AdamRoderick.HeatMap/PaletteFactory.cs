using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace AdamRoderick.HeatMap
{
    public enum PALLETES
    {
        DEFAULT = 0
        ,SECOND = 1
    }
    public class PaletteFactory
    {

        #region Singleton
        private PaletteFactory()
        {
        }

        private static PaletteFactory _instance;
        public static PaletteFactory Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new PaletteFactory();
                }
                return _instance;
            }
        }
        #endregion

        public Bitmap GetPaletteBitmap(PALLETES paletteSelection)
        {
            string filename = string.Empty;
            Bitmap retval = null;
            switch (paletteSelection)
            {
                case PALLETES.DEFAULT:
                    filename = "AdamRoderick.HeatMap.HeatMapPalette.bmp";
                    break;
                case PALLETES.SECOND:
                    filename = "AdamRoderick.HeatMap.HeatMapPalette2.bmp";
                    break;

            }
            Stream s = this.GetType().Assembly.GetManifestResourceStream(filename);
            if (null == s) throw new Exception("Could not find file " + filename);
            retval = new Bitmap(s);
            return retval;
        }
    }
}
