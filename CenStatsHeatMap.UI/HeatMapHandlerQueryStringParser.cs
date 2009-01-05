using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using AdamRoderick.HeatMap;
using System.Collections.Specialized;
using System.Drawing;

namespace CenStatsHeatMap.UI
{
    public  class HeatMapHandlerQueryStringParser
    {
        private NameValueCollection _queryString;
        public HeatMapHandlerQueryStringParser(NameValueCollection queryString)
        {
            if (null == queryString) throw new ArgumentException("queryString cannot be null");
            _queryString = queryString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset">true to offset the points by the PointRadius value
        /// Useful if the window size is different from the map size
        /// </param>
        /// <returns></returns>
        public List<HeatPoint> GetHeatPointList(bool offsetPointLocations)
        {
            List<HeatPoint> retval = new List<HeatPoint>();
            if (null == _queryString[Constants.QueryString.HeatPointData.Key]) return retval;

            string[] pointLocations = _queryString[Constants.QueryString.HeatPointData.Key].Split(Constants.QueryString.HeatPointData.LocationSplitChar);
            foreach (string loc in pointLocations)
            {
                string location = loc;
                int pointRadus = this.GetHeatPointRadius();
                int pointCount = 1;
                if (-1 < location.IndexOf(Constants.QueryString.HeatPointData.PointCountSplitChar[0]))
                {
                    string[] parts = location.Split(Constants.QueryString.HeatPointData.PointCountSplitChar);
                    int newPointCount;
                    int.TryParse(parts[1], out newPointCount);
                    if (newPointCount > pointCount) pointCount = newPointCount;
                    location = parts[0];                    
                }
                string[] locationParts = location.Split(Constants.QueryString.HeatPointData.XYSplitChar);
                // create the locations, but offset them by the size of the point radius
                int x = int.Parse(locationParts[0]) + pointRadus;
                int y = int.Parse(locationParts[1]) + pointRadus;

                for (int i = 0; i < pointCount; i++)
                {
                    retval.Add(new HeatPoint(x, y, Constants.HeatMapSettingsDefaults.HeatPointIntensity));
                }
            }
            return retval;
        }
        /// <summary>
        /// Gets the HeatPointRadius from the query string, if it exists
        /// </summary>
        /// <returns>int representing the HeatPointRadius.  Default value if does not exist in the query string</returns>
        public int GetHeatPointRadius()
        {
            int retval = Constants.HeatMapSettingsDefaults.PointRadius;
            if (null == _queryString[Constants.QueryString.HeatMapSettings.PointRadiusKey]) return retval;
            int.TryParse(_queryString[Constants.QueryString.HeatMapSettings.PointRadiusKey], out retval);
            return retval;
        }
        public Size GetHeatMapSize()
        {
            Size retval = Constants.HeatMapSettingsDefaults.Size;
            if (null == _queryString[Constants.QueryString.HeatMapSettings.SizeKey])
            {
                // increase the map size by twice the radius of the points
                int pointRadius = this.GetHeatPointRadius();
                retval = new Size(retval.Width + pointRadius * 2, retval.Height + pointRadius * 2);
                return retval;
            }
            try
            {
                string[] parts =
                    _queryString[Constants.QueryString.HeatMapSettings.SizeKey].Split(
                        Constants.QueryString.HeatMapSettings.SizeSplitChar);
                var w = int.Parse(parts[0]);
                var h = int.Parse(parts[1]);
                retval = new Size(w, h);

            } catch (Exception ex)
            {
                Util.Log(ex);
            }
            return retval;
        }
        public Size GetHeatMapWindowSize()
        {
            Size retval = Constants.HeatMapSettingsDefaults.WindowSize;
            if (null == _queryString[Constants.QueryString.HeatMapSettings.WindowSizeKey]) return retval;
            try
            {
                string[] parts =
                    _queryString[Constants.QueryString.HeatMapSettings.WindowSizeKey].Split(
                        Constants.QueryString.HeatMapSettings.WindowSizeSplitChar);

                var w = int.Parse(parts[0]);
                var h = int.Parse(parts[1]);
                retval = new Size(w, h);
            } catch (Exception ex)
            {
                Util.Log(ex);
            }
            return retval;
        }
        public Point GetWindowTopLeftCornerPoint()
        {
            Point retval = Constants.HeatMapSettingsDefaults.WindowTopLeftCornerPoint;
            if (null == _queryString[Constants.QueryString.HeatMapSettings.WindowTopLeftCornerPointKey])
            {
                // position the top left corner of the window relative 
                //to the heat map's top left corner accounting for the point radius
                int pointRadius = this.GetHeatPointRadius();
                retval = new Point(retval.X + pointRadius, retval.Y + pointRadius);

                return retval;
            }
            try
            {
                string[] parts =
                    _queryString[Constants.QueryString.HeatMapSettings.WindowTopLeftCornerPointKey].Split(
                        Constants.QueryString.HeatMapSettings.WindowTopLeftCornerPointSplitChar);
                var x = int.Parse(parts[0]);
                var y = int.Parse(parts[1]);
                retval = new Point(x, y);
            } catch (Exception ex)
            {
                Util.Log(ex);
            }
            return retval;
        }
        public bool GetShowTileBorder()
        {
            bool retval = false;
            if (null == _queryString[Constants.QueryString.HeatMapSettings.ShowTileBorderKey]) return retval;
            try
            {
                retval = bool.Parse(_queryString[Constants.QueryString.HeatMapSettings.ShowTileBorderKey]);
            } catch (Exception ex)
            {
                Util.Log(ex);
            }
            return retval;
        }
    }
}
