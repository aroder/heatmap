using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AdamRoderick.HeatMap
{
    public class Tile : AbstractMapImage
    {
        public static int TILE_HEIGHT = 256; //in pixels
        public static int TILE_WIDTH = 256; //in pixels
        public Tile(HeatPointList heatPoints, HeatmapSettings settings, Bounds bounds)
        {
            Bounds = bounds;

            Bounds heatMapBounds = new Bounds(bounds);
            heatMapBounds.TopLeftLat += settings.Size / 2 * this.DegreesPerPixelLat;
            heatMapBounds.TopLeftLng -= settings.Size / 2 * this.DegreesPerPixelLng;
            heatMapBounds.BottomRightLat -= settings.Size / 2 * this.DegreesPerPixelLat;
            heatMapBounds.BottomRightLng += settings.Size / 2 * this.DegreesPerPixelLng;

            HeatPointList pointsWithinBounds = new HeatPointList();
            foreach (HeatPoint hp in heatPoints)
            {
                if (heatMapBounds.Contains(hp)) pointsWithinBounds.Add(hp);
            }

            HeatMap = new HeatMap(pointsWithinBounds, settings, heatMapBounds);
        }



        public HeatMap HeatMap { get; set; }
        private HeatPoint _upperLeftCorner;
        public HeatPoint UpperLeftCorner
        {
            get
            {
                if (null == _upperLeftCorner) _upperLeftCorner = new HeatPoint(Bounds.TopLeftLat, Bounds.TopLeftLng);
                return _upperLeftCorner;
                //return new HeatPoint(HeatPoints.GetMaxLatitude(), HeatPoints.GetMinLongitude());
            }
        }


        public Bitmap GenerateTile()
        {
            var heatMap = HeatMap.GenerateMap();
            var tileMap = new Bitmap(TILE_WIDTH, TILE_HEIGHT);
            int offsetX;
            int offsetY;
            offsetX = (int)Math.Round((this.Bounds.TopLeftLng - this.HeatMap.Bounds.TopLeftLng) / this.HeatMap.DegreesPerPixelLng);
            offsetY = (int)Math.Round((this.HeatMap.Bounds.TopLeftLat - this.Bounds.TopLeftLat) / this.HeatMap.DegreesPerPixelLat);

            bool breaker = false;
            for (int i = 0; i < heatMap.Width; i++)
            {
                for (int j = 0; j < heatMap.Height; j++)
                {
                    int x = i + offsetX;
                    int y = j + offsetY;
                    if (i < tileMap.Width && j < tileMap.Height) tileMap.SetPixel(i, j, heatMap.GetPixel(x, y));
                    else breaker = true;

                    if (breaker) break;
                }
                if (breaker) break;
            }

            return tileMap;
            //return heatMap;
        }

        public Bitmap JustGetHeatmap()
        {
            return HeatMap.GenerateMap();

        }




    }
}
