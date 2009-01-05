using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace AdamRoderick.HeatMap
{
    public class HeatMap : AbstractMapImage
    {

        public HeatmapSettings Settings { get; set; }

        public HeatMap(HeatPointList heatPoints, HeatmapSettings settings)
        {
            Settings = settings;
            HeatPoints = heatPoints;
        }
        public HeatMap(HeatPointList heatPoints, HeatmapSettings settings, Bounds bounds)
    {
            Settings = settings;
            HeatPoints = heatPoints;
            Bounds = bounds;
    }
        private HeatPoint _upperLeftCorner;
        public HeatPoint UpperLeftCorner
        {
            get
            {
                if (null == _upperLeftCorner && null != Bounds)
                {
                    _upperLeftCorner = new HeatPoint(Bounds.TopLeftLat, Bounds.TopLeftLng);
                }
                else if (null == _upperLeftCorner && 0 < HeatPoints.Count)
                {
                    _upperLeftCorner = new HeatPoint(HeatPoints.GetMaxLatitude(), HeatPoints.GetMinLongitude());
                }
                return _upperLeftCorner;
            }
        }

        private HeatPointList _heatPoints;
        public HeatPointList HeatPoints
        {
            get
            {
                if (null == _heatPoints)
                {
                    _heatPoints = new HeatPointList();
                }
                return _heatPoints;
            }
            set
            {
                _heatPoints = value;
            }
        }


        public Bitmap Palette
        {
            get
            {
                return PaletteSelector.Instance.GetPaletteBitmap(Settings.Palette);
            }
        }

        public Bitmap GenerateMap()
        {

            //create the map based on the size  
//            HeatPoint topleft = new HeatPoint(HeatPoints.GetMaxLatitude(), HeatPoints.GetMinLongitude());
//            HeatPoint bottomright = new HeatPoint(HeatPoints.GetMinLatitude(), HeatPoints.GetMaxLongitude());
//            Bitmap heatMap = new Bitmap(HeatPoint.GetPixelWidth(topleft, bottomright), HeatPoint.GetPixelHeight(topleft, bottomright));
            //int width, height;
            //width = (int) (this.Bounds.Width/this.PixelsPerDegreeLng);
            //height = (int) ((Bounds.TopLeftLat - Bounds.BottomRightLat)/this.PixelsPerDegreeLat);
            Bitmap heatMap = new Bitmap(
                 (int) (this.Bounds.Width / this.PixelsPerDegreeLng)
                , (int) (this.Bounds.Height / this.PixelsPerDegreeLat)
            );

            heatMap = this.CreateIntensityMask(heatMap);
            heatMap = this.Colorize(heatMap);
            if (Settings.Border) heatMap = this.AddBorder(heatMap);

            return heatMap;
        }
        protected Bitmap GetIntensityHeatMap(Bitmap map)
        {
            return this.CreateIntensityMask(map);
        }
        protected Bitmap GetColorizedHeatMap(Bitmap map)
        {
            return this.Colorize(GetIntensityHeatMap(map));
        }
        protected Bitmap CreateIntensityMask(Bitmap surface)
        {

            //Create new graphics surface from the memory bitmap
            var drawSurface = Graphics.FromImage(surface);

            //Set bg color to white so the pixels can be correctly colorized
            drawSurface.Clear(Color.White);

            //Traverse heat point data and draw masks for each heat point
            foreach (HeatPoint p in HeatPoints)
            {
                DrawHeatPoint(drawSurface, p, Settings.Size);
            }


            /////////////////////////
            /// //This works, but the DrawHeatPoint does not.  Might be that the brush is off?  The points are getting populated.

            //////// Create solid brush.
            //////SolidBrush blueBrush = new SolidBrush(Color.Blue);

            //////// Create points that define polygon.
            //////Point point1 = new Point(5, 5);
            //////Point point2 = new Point(10, 25);
            //////Point point3 = new Point(20, 5);
            //////Point point4 = new Point(25, 5);
            //////Point point5 = new Point(30, 10);
            //////Point point6 = new Point(35, 20);
            //////Point point7 = new Point(25, 25);
            //////Point[] curvePoints = { point1, point2, point3, point4, point5, point6, point7 };

            //////// Draw polygon to screen.
            //////drawSurface.FillPolygon(blueBrush, curvePoints);

            ///////////////////////

            return surface;
        }
        private void DrawHeatPoint(Graphics Canvas, HeatPoint HeatPoint, int Radius)
        {
            // Create points generic list of points to hold circumference points
            List<Point> CircumferencePointsList = new List<Point>();
            // Create an empty point to predefine the point struct used in the circumference loop
            Point CircumferencePoint;
            // Create an empty array that will be populated with points from the generic list
            Point[] CircumferencePointsArray;
            // Calculate ratio to scale byte intensity range from 0-255 to 0-1
            float fRatio = 1F / Byte.MaxValue;
            // Precalulate half of byte max value
            byte bHalf = Byte.MaxValue / 2;
            // Flip intensity on it's center value from low-high to high-low
            int iIntensity = (byte)(HeatPoint.Intensity - ((HeatPoint.Intensity - bHalf) * 2));
            // Store scaled and flipped intensity value for use with gradient center location
            float fIntensity = iIntensity * fRatio;
            // Loop through all angles of a circle
            // Define loop variable as a double to prevent casting in each iteration
            // Iterate through loop on 10 degree deltas, this can change to improve performance
            for (double i = 0; i <= 360; i += 10)
            {
                // Replace last iteration point with new empty point struct
                CircumferencePoint = new Point();
                // Plot new point on the circumference of a circle of the defined radius
                // Using the point coordinates, radius, and angle
                // Calculate the position of this iterations point on the circle
                CircumferencePoint.X = Convert.ToInt32(((HeatPoint.Longitude - this.Bounds.TopLeftLng) / this.DegreesPerPixelLng) + Radius * Math.Cos(ConvertDegreesToRadians(i)));
                CircumferencePoint.Y = Convert.ToInt32(((this.Bounds.TopLeftLat - HeatPoint.Latitude) / this.DegreesPerPixelLat) + Radius * Math.Sin(ConvertDegreesToRadians(i)));
                //CircumferencePoint.X = Convert.ToInt32(HeatPoint.GetPixelWidth(this.UpperLeftCorner, HeatPoint) + Radius * Math.Cos(ConvertDegreesToRadians(i)));
                //CircumferencePoint.Y = Convert.ToInt32(HeatPoint.GetPixelHeight(this.UpperLeftCorner, HeatPoint) + Radius * Math.Sin(ConvertDegreesToRadians(i)));

                // Add newly plotted circumference point to generic point list
                CircumferencePointsList.Add(CircumferencePoint);
            }
            // Populate empty points system array from generic points array list
            // Do this to satisfy the datatype of the PathGradientBrush and FillPolygon methods
            CircumferencePointsArray = CircumferencePointsList.ToArray();
            // Create new PathGradientBrush to create a radial gradient using the circumference points
            PathGradientBrush GradientShaper = new PathGradientBrush(CircumferencePointsArray);
            // Create new color blend to tell the PathGradientBrush what colors to use and where to put them
            ColorBlend GradientSpecifications = new ColorBlend(3);
            // Define positions of gradient colors, use intesity to adjust the middle color to
            // show more mask or less mask
            GradientSpecifications.Positions = new float[] { 0, fIntensity, 1 };
            // Define gradient colors and their alpha values, adjust alpha of gradient colors to match intensity
            GradientSpecifications.Colors = new Color[] { Color.FromArgb(0, Color.White), Color.FromArgb(HeatPoint.Intensity, Color.Black), Color.FromArgb(HeatPoint.Intensity, Color.Black) };
            //GradientSpecifications.Colors = new Color[] { Color.FromArgb(0, Color.Black), Color.FromArgb(HeatPoint.Intensity, Color.Black), Color.FromArgb(HeatPoint.Intensity, Color.Black) };
            // Pass off color blend to PathGradientBrush to instruct it how to generate the gradient
            GradientShaper.InterpolationColors = GradientSpecifications;
            // Draw polygon (circle) using our point array and gradient brush

            //SolidBrush blueBrush = new SolidBrush(Color.Blue);
            //Canvas.FillPolygon(blueBrush, CircumferencePointsArray);

            Canvas.FillPolygon(GradientShaper, CircumferencePointsArray);
        }
        protected Bitmap Colorize(Bitmap Mask)
        {
            // Create new bitmap to act as a work surface for the colorization process
            Bitmap Output = new Bitmap(Mask.Width, Mask.Height, PixelFormat.Format32bppArgb);
            // Create a graphics object from our memory bitmap so we can draw on it and clear it's drawing surface
            Graphics Surface = Graphics.FromImage(Output);
            Surface.Clear(Color.Transparent);
            // Build an array of color mappings to remap our greyscale mask to full color
            // Accept an alpha byte to specify the transparancy of the output image
            ColorMap[] Colors = CreatePaletteIndex((byte)(Settings.Transparency * 255 / 100));
            // Create new image attributes class to handle the color remappings
            // Inject our color map array to instruct the image attributes class how to do the colorization
            ImageAttributes Remapper = new ImageAttributes();
            Remapper.SetRemapTable(Colors);
            // Draw our mask onto our memory bitmap work surface using the new color mapping scheme
            Surface.DrawImage(Mask, new Rectangle(0, 0, Mask.Width, Mask.Height), 0, 0, Mask.Width, Mask.Height, GraphicsUnit.Pixel, Remapper);
            // Send back newly colorized memory bitmap
            return Output;
        }
        private ColorMap[] CreatePaletteIndex(byte Alpha)
        {
            ColorMap[] OutputMap = new ColorMap[256];
            // Change this path to wherever you saved the palette image.
            //Bitmap Palette = (Bitmap)Bitmap.FromFile(@"C:\Documents and Settings\Administrator\My Documents\Visual Studio 2008\Projects\CenStatsHeatMap\CenStatsHeatMap.UI\images\HeatmapPalette.bmp");
            // Loop through each pixel and create a new color mapping
            for (int X = 0; X <= 255; X++)
            {
                OutputMap[X] = new ColorMap
                {
                    OldColor = Color.FromArgb(X, X, X),
                    NewColor = Color.FromArgb(Alpha, this.Palette.GetPixel(X, 0))
                };
            }

            for (var i = 255; i >= (byte)(Settings.Sensitivity * 255 / 100); i--)
            {
                //Makes any point with no heat coloring 100% transparent
                OutputMap[i].OldColor = Color.FromArgb(i, i, i);
                OutputMap[i].NewColor = Color.Transparent;
                //                
            }

            return OutputMap;
        }
        public Bitmap AddBorder(Bitmap image)
        {
            int x = 0;
            int y = 0;

            //bottom border
            for (; x < image.Width; x++) image.SetPixel(x, y, Color.Black);
            x--;
            //right border
            for (; y < image.Height; y++) image.SetPixel(x, y, Color.Black);
            y--;
            //top border
            for (; x > 0; x--) image.SetPixel(x, y, Color.Black);
            //lefft border
            for (; y > 0; y--) image.SetPixel(x, y, Color.Black);

            return image;
        }

        private double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

    }
}
