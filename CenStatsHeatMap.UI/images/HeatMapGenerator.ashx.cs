using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using AdamRoderick.HeatMap;
using System.Drawing;
//using Util = AdamRoderick.HeatMap.Util;

namespace CenStatsHeatMap.UI
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class HeatMapHandler : IHttpHandler
    {
        private const string IMAGE_TYPE = "image/png";
        private const string SMILEY_IMAGE_PATH = "~/images/Smiley.png";

        public void ProcessRequest(HttpContext context)
        {
            HeatMapHandlerQueryStringParser parser = new HeatMapHandlerQueryStringParser(context.Request.QueryString);
            context.Response.Clear();
            context.Response.ContentType = IMAGE_TYPE;
            try
            {
                var outStream = new MemoryStream();
                IList<HeatPoint> list = parser.GetHeatPointList(true);
                bool showTileBorder = parser.GetShowTileBorder();
                if (0 == list.Count) Error(context, new Exception("data list was empty in request " + context.Request.Url.Query));
                Size mapSize = parser.GetHeatMapSize();
                Size windowSize = parser.GetHeatMapWindowSize();
                Point windowTopLeftCorner = parser.GetWindowTopLeftCornerPoint();
                if (windowSize.Height + windowTopLeftCorner.Y > mapSize.Height) windowSize.Height = mapSize.Height - windowTopLeftCorner.Y;
                if (windowSize.Width + windowTopLeftCorner.X > mapSize.Width) windowSize.Width = mapSize.Width - windowTopLeftCorner.X;
                var hm = new HeatMap(list, mapSize);
                Bitmap img = hm.GetBitmap();
                img = hm.GetWindow(img, windowSize, windowTopLeftCorner);

                if (showTileBorder) img = hm.AddBorder(img);

                img.Save(outStream, ImageFormat.Png);

                outStream.WriteTo(context.Response.OutputStream);
            }
            catch (Exception ex)
            {
                Error(context, ex);
            }
        }


        private void Error(HttpContext context, Exception ex)
        {
            Util.Log(context.Request.Url.ToString());
            Util.Log(ex);
            context.Response.WriteFile(SMILEY_IMAGE_PATH);
            context.Response.End();
        }

        public bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }

    }
}
