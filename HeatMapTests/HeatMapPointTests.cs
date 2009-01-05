using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AdamRoderick.HeatMap;

namespace HeatMapTests
{
    [TestFixture]
    public class HeatMapPointTests
    {
        [Test]
        public void IntensityAssignmentIsConsistent()
        {
            HeatPoint hp = new HeatPoint(39, 104, 50);

            for (var i = 0; i <=100; i++)
            {
                hp.Intensity = i;
                Assert.AreEqual(i, hp.Intensity);
            }
        }

        [Test]
        public void PixelWidthTest()
        {
            HeatPoint h1 = new HeatPoint(39, -104);
            HeatPoint h2 = new HeatPoint(38, -105);
            double width = HeatPoint.GetPixelWidth(h1, h2, 1);
            Assert.AreEqual(53, width);

            h1 = new HeatPoint(39, -104);
            h2 = new HeatPoint(39, -104);
            Assert.AreEqual(0, HeatPoint.GetPixelWidth(h1, h2, 1));

            h1 = new HeatPoint(39, -104);
            h2 = new HeatPoint(39, -106);
            Assert.AreEqual(53.5*2, HeatPoint.GetPixelWidth(h1, h2, 1));
        }

        [Test]
        public void PixelHeightTest()
        {
            HeatPoint h1 = new HeatPoint(39, -104);
            HeatPoint h2 = new HeatPoint(38, -105);
            double height = HeatPoint.GetPixelHeight(h1, h2, 1);
            Assert.AreEqual(69, height);

            h1 = new HeatPoint(39, -104);
            h2 = new HeatPoint(39, -104);
            Assert.AreEqual(0, HeatPoint.GetPixelHeight(h1, h2, 1));

            h1 = new HeatPoint(39, -104);
            h2 = new HeatPoint(37, -104);
            Assert.AreEqual(69*2, HeatPoint.GetPixelHeight(h1, h2, 1));
        }

        [Test]
        public void ScaleTests()
        {
            HeatPoint h1 = new HeatPoint(39, -104);
            HeatPoint h2 = new HeatPoint(38, -105);

            Assert.AreEqual(69*2, HeatPoint.GetPixelHeight(h1, h2, 2));

            Assert.AreEqual(107, HeatPoint.GetPixelWidth(h1, h2, 2));
        }

    }
}
