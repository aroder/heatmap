using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AdamRoderick.HeatMap;

namespace HeatMapTests
{
    [TestFixture]
    public class HeatPointListTests
    {
        private HeatPointList _points;
        [TestFixtureSetUp]
        public void Setup()
        {
            _points = new HeatPointList();
            _points.Add(new HeatPoint(39, -104));
            _points.Add(new HeatPoint(38, -105));
            _points.Add(new HeatPoint(-10, -100));
            _points.Add(new HeatPoint(-10, 100));
            _points.Add(new HeatPoint(10, -100));
        }

        [Test]
        public void GetMinLatTest()
        {
            Assert.AreEqual(-10, _points.GetMinLatitude());
        }

        [Test]
        public void GetMinLonTest()
        {
            Assert.AreEqual(-105, _points.GetMinLongitude());
        }

        [Test]
        public void GetMaxLatTest()
        {
            Assert.AreEqual(39, _points.GetMaxLatitude());
        }

        [Test]
        public void GetMaxLonTest()
        {
            Assert.AreEqual(100, _points.GetMaxLongitude());
        }
    }
}
