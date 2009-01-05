using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AdamRoderick.HeatMap;
using System.Drawing.Imaging;

namespace CenStatsHeatMap.UI.images
{
    public class HeatMapTileGenerator : IHttpHandler
    {
        //private HeatmapSettings _settings;
        private const string DATA_CACHE_TOKEN = "dataCacheToken";
        private const string TOP_LEFT_LAT = "topLeftLat";
        private const string TOP_LEFT_LNG = "topLeftLng";
        private const string BOTTOM_RIGHT_LAT = "bottomRightLat";
        private const string BOTTOM_RIGHT_LNG = "bottomRightLng";
        private const string DEGREES_PER_PIXEL_LAT = "degreesPerPixelLat";
        private const string DEGREES_PER_PIXEL_LNG = "degreesPerPixelLng";

        private const string IMAGE_TYPE = "image/png";
        private const string SMILEY_IMAGE_PATH = "~/images/Smiley.png";

        public void ProcessRequest(HttpContext context)
        {

            //if (0 == tile.HeatMap.HeatPoints.Count) return;

            context.Response.Clear();
            context.Response.ContentType = IMAGE_TYPE;
            try
            {
                var outStream = new MemoryStream();
                HeatPoint p1 = new HeatPoint(50, 50, 255);
                HeatPoint p2 = new HeatPoint(60, 60, 255);
                List<HeatPoint> list = new List<HeatPoint>();
                list.Add(p1);
                list.Add(p2);
                var hm = new HeatMap(list, 100, 100);
                hm.GetBitmap().Save(outStream, ImageFormat.Png);

                outStream.WriteTo(context.Response.OutputStream);
            }
            catch (Exception ex)
            {
                Error(context);
            }
        }

        private void Error(HttpContext context)
        {
            context.Response.WriteFile(SMILEY_IMAGE_PATH);
            context.Response.End();
        }


        //protected HeatmapSettings GetHeatMapSettings(HttpContext context)
        //{
        //    HeatmapSettings retval = null;
        //    if (string.IsNullOrEmpty(context.Request.QueryString[DATA_CACHE_TOKEN])) Error(context);
        //    Guid token = new Guid(context.Request.QueryString[DATA_CACHE_TOKEN]);

        //    retval = HeatmapSettingsCache.Instance.Retrieve(token).Settings;
        //    return retval;
        //}

        protected IEnumerable<HeatPoint> GetHeatPointList(HttpContext context)
        {
            IList<HeatPoint> retval = new List<HeatPoint>();
            if (string.IsNullOrEmpty(context.Request.QueryString["data"])) Error(context);

            return retval;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
