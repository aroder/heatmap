using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdamRoderick.HeatMap;

namespace CenStatsHeatMap.UI.images
{
    public partial class HeatMapTileOverlay : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HeatmapSettings settings;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["token"]))
                {
                    Guid token = new Guid(Request.QueryString["token"]);
                    settings = HeatmapSettingsCache.Instance.Retrieve(token).Settings;
                }
                else
                {
                    double smallestX = 0, smallestY = 0, largestX = 0, largestY = 0;
                    int largestRange = 0;
                    int multiplier = 10;

                    var HeatPoints = new HeatPointList();
                    string[] dataPoints = Request.QueryString["data"].Split(new char[] { ',' });

                    // create the heatpoints from 
                    for (int i = 0; i < dataPoints.Length; i++)
                    {
                        double lat = double.Parse(dataPoints[i]);
                        double lon = double.Parse(dataPoints[++i]);

                        HeatPoints.Add(new HeatPoint(lat, lon, HeatmapSettings.DEFAULT_INTENSITY));
                    }
                    settings = new HeatmapSettings();
                    settings.Size = MapQueryStringValueToHeatMapPropertyValue("size", settings.Size);
                    settings.Sensitivity = MapQueryStringValueToHeatMapPropertyValue("sensitivity", settings.Sensitivity);
                    settings.Transparency = MapQueryStringValueToHeatMapPropertyValue("transparency", settings.Transparency);
                    settings.Zoom = MapQueryStringValueToHeatMapPropertyValue("zoom", settings.Zoom);

                }
                //var hm = new HeatMap(settings);
                throw new NotImplementedException("not implemented");
                //Respond(hm.GenerateMap());

            }
            catch (Exception ex)
            {
                Bitmap b = new Bitmap(100, 100);
                Graphics Surface = Graphics.FromImage(b);
                Surface.Clear(Color.Black);
                Surface.DrawImage(b, 100, 100);
                Respond(b);
            }

        }
        private void Respond(Bitmap b)
        {
            Response.ContentType = "image/png";
            var outStream = new MemoryStream();
            b.Save(outStream, ImageFormat.Png);
            outStream.WriteTo(Response.OutputStream);

        }
        private int MapQueryStringValueToHeatMapPropertyValue(string queryStringParmName, int defaultValue)
        {
            var retval = defaultValue;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString[queryStringParmName]) &&
                    true == int.TryParse(Request.QueryString[queryStringParmName], out retval))
                {
                    if (0 > retval || 100 < retval)
                    {
                        retval = defaultValue;
                    }

                }
            }
            catch { }

            return retval;
        }
    }
}
