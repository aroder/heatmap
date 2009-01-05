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
    }
    internal class PaletteSelector
    {
        #region Singleton
        private PaletteSelector()
        {
        }

        private static PaletteSelector _instance;
        public static PaletteSelector Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new PaletteSelector();
                }
                return _instance;
            }
        }
        #endregion

        internal Bitmap GetPaletteBitmap(PALLETES paletteSelection)
        {
            string filename = string.Empty;
            Bitmap retval = null;
            switch (paletteSelection)
            {
                case PALLETES.DEFAULT:
                    filename = "AdamRoderick.HeatMap.HeatmapPalette.bmp";
                    break;

            }
            Stream s = this.GetType().Assembly.GetManifestResourceStream(filename);
            if (null == s) throw new Exception("Could not find file " + filename);
            retval = new Bitmap(s);
            return retval;
        }
    }
}
