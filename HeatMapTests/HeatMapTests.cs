using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AdamRoderick.HeatMap;
using CenStatsHeatMap.UI;
using System.Collections.Specialized;

namespace HeatMapTests
{
    [TestFixture(Description = "Tests the HeatMap dll")]
    public class HeatMapTests
    {

        [Test]
        public void PaletteFactoryFindsPalette()
        {
            PaletteFactory.Instance.GetPaletteBitmap(PALLETES.DEFAULT);
        }
        //[Test]
        //public void HeatMapDefaultValuesAreSet()
        //{
        //    HeatMap hm = new HeatMap(new HeatmapSettings(new HeatPointList()));
        //    Assert.AreEqual(HeatmapSettings.DEFAULT_INTENSITY, hm.Intensity);
        //    Assert.AreEqual(HeatmapSettings.DEFAULT_SENSITIVITY, hm.Sensitivity);
        //    Assert.AreEqual(HeatmapSettings.DEFAULT_SIZE, hm.Size);
        //    Assert.AreEqual(HeatmapSettings.DEFAULT_TRANSPARENCY, hm.Transparency);
        //    Assert.AreEqual(HeatmapSettings.DEFAULT_ZOOM, hm.Zoom);
        //}

        //[Test]
        //public void SensitivityAssignmentIsConsistent()
        //{
        //    HeatMap hm = new HeatMap(new HeatmapSettings(new HeatPointList()));

        //    for (int i = 0; i <= 100; i++)
        //    {
        //        hm.Sensitivity = i;
        //        Assert.AreEqual(i, hm.Sensitivity);
        //    }
        //}
        //[Test]
        //public void IntensityAssignmentIsConsistent()
        //{
        //    HeatMap hm = new HeatMap(new HeatmapSettings(new HeatPointList()));

        //    for (int i = 0; i <= 100; i++)
        //    {
        //        hm.Intensity = i;
        //        Assert.AreEqual(i, hm.Intensity);
        //    }
        //}

        //[Test]
        //public void TransparencyAssignmentIsConsistent()
        //{
        //    HeatMap hm = new HeatMap(new HeatmapSettings(new HeatPointList()));

        //    for (int i = 0; i <= 100; i++)
        //    {
        //        hm.Transparency = i;
        //        Assert.AreEqual(i, hm.Transparency);
        //    }
        //}
    }
}
