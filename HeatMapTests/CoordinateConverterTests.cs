using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AdamRoderick.HeatMap;

namespace HeatMapTests
{
    [TestFixture]
    public class CoordinateConverterTests
    {
        [Test]
        public void LongitudeToXPixelBackToLongitude()
        {
            double testVal = 58.38133351447725;
            double expected = testVal;
            double actual = CoordinateConverter.XToLon(CoordinateConverter.LonToX(testVal));

            Assert.AreEqual(Math.Round(expected, 5), Math.Round(actual, 5));
        }
        [Test]
        public void LatitudeToYPixelBackToLatitude()
        {
            double testVal = 24.516592025756836;
            double expected = testVal;
            double actual = CoordinateConverter.YToLat(CoordinateConverter.LatToY(testVal));

            Assert.AreEqual(Math.Round(expected, 5), Math.Round(actual, 5));

        }
        [Test]
        public void AdjustLonByPixels()
        {
            double testVal = 58.38133351447725;
            double expected = 58.51866;
            double actual = Math.Round(CoordinateConverter.AdjustLonByPixels(testVal, 100, 10), 5);

            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void AdjustLatByPixels()
        {
            double testVal = 24.516592025756836;
            double expected = 24.39158;
            double actual = Math.Round(CoordinateConverter.AdjustLatByPixels(testVal, 100, 10), 5);

            Assert.AreEqual(expected, actual);
        }
    }
}
