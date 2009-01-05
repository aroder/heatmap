using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AdamRoderick.HeatMap
{
    public class HeatMap
    {
        #region fields
        private Bitmap _palette;
        #endregion

        #region properties
        public IEnumerable<HeatPoint> Points { get; set; }
        public Size Size { get; set; }
        public Bitmap Palette
        {
            get
            {
                if (null == _palette)
                {
                    _palette = PaletteFactory.Instance.GetPaletteBitmap(PALLETES.DEFAULT);
                }
                return _palette;
            }
            set { _palette = value; }
        }
        #endregion

        #region constructors
        public HeatMap(IEnumerable<HeatPoint> points, Size size)
        {
            Points = points;
            this.Size = size;
        }
        #endregion

        #region methods
        public Bitmap GetBitmap()
        {
            Bitmap img = new Bitmap(this.Size.Width, this.Size.Height);
            img = CreateIntensityMask(img, this.Points);
            img = Colorize(img, 255);

            return img;
        }
        private Bitmap CreateIntensityMask(Bitmap surface, IEnumerable<HeatPoint> heatPoints)
        {
            // Create new graphics surface from memory bitmap
            Graphics DrawSurface = Graphics.FromImage(surface);
            // Set background color to white so that pixels can be correctly colorized
            DrawSurface.Clear(Color.White);
            // Traverse heat point data and draw masks for each heat point
            foreach (HeatPoint dataPoint in heatPoints)
            {
                // Render current heat point on draw surface
                DrawHeatPoint(DrawSurface, dataPoint, 25);
            }
            return surface;
        }
        private void DrawHeatPoint(Graphics canvas, HeatPoint heatPoint, int radius)
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
            int iIntensity = (byte)(heatPoint.Intensity - ((heatPoint.Intensity - bHalf) * 2));
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
                CircumferencePoint.X = Convert.ToInt32(heatPoint.X + radius * Math.Cos(ConvertDegreesToRadians(i)));
                CircumferencePoint.Y = Convert.ToInt32(heatPoint.Y + radius * Math.Sin(ConvertDegreesToRadians(i)));
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
            GradientSpecifications.Positions = new float[3] { 0, fIntensity, 1 };
            // Define gradient colors and their alpha values, adjust alpha of gradient colors to match intensity
            GradientSpecifications.Colors = new Color[3] { Color.FromArgb(0, Color.White), Color.FromArgb(heatPoint.Intensity, Color.Black), Color.FromArgb(heatPoint.Intensity, Color.Black) };
            // Pass off color blend to PathGradientBrush to instruct it how to generate the gradient
            GradientShaper.InterpolationColors = GradientSpecifications;
            // Draw polygon (circle) using our point array and gradient brush
            canvas.FillPolygon(GradientShaper, CircumferencePointsArray);
        }
        private double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
        private Bitmap Colorize(Bitmap mask, byte alpha)
        {
            // Create new bitmap to act as a work surface for the colorization process
            Bitmap Output = new Bitmap(mask.Width, mask.Height, PixelFormat.Format32bppArgb);
            // Create a graphics object from our memory bitmap so we can draw on it and clear it's drawing surface
            Graphics Surface = Graphics.FromImage(Output);
            Surface.Clear(Color.Transparent);
            // Build an array of color mappings to remap our greyscale mask to full color
            // Accept an alpha byte to specify the transparancy of the output image
            ColorMap[] Colors = CreatePaletteIndex(alpha);
            // Create new image attributes class to handle the color remappings
            // Inject our color map array to instruct the image attributes class how to do the colorization
            ImageAttributes Remapper = new ImageAttributes();
            Remapper.SetRemapTable(Colors);
            // Draw our mask onto our memory bitmap work surface using the new color mapping scheme
            Surface.DrawImage(mask, new Rectangle(0, 0, mask.Width, mask.Height), 0, 0, mask.Width, mask.Height, GraphicsUnit.Pixel, Remapper);
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

            //for (var i = 255; i >= (byte)(Settings.Sensitivity * 255 / 100); i--)
            for (var i = 255; i >= 255; i--)
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
        public Bitmap GetWindow(Bitmap img, Size size, Point windowTopLeftCornerLocation)
        {
            Bitmap retval = new Bitmap(size.Width, size.Height);
            for (int x = windowTopLeftCornerLocation.X; x < windowTopLeftCornerLocation.X + size.Width; x++)
            {
                for (int y = windowTopLeftCornerLocation.Y; y < windowTopLeftCornerLocation.Y + size.Height; y++)
                {
                    retval.SetPixel(x - windowTopLeftCornerLocation.X, y - windowTopLeftCornerLocation.Y, img.GetPixel(x, y));
                }
            }
            return retval;
        }

        #endregion
    }
}
