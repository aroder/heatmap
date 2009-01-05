using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CenStatsHeatMap.UI;
using NUnit.Framework;
using AdamRoderick.HeatMap;
using System.Collections.Specialized;

namespace HeatMapTests
{
    [TestFixture]
    public class QueryStringParserTests
    {
        private NameValueCollection _queryStringMissingParameter;
        private NameValueCollection _qs1;
        private HeatMapHandlerQueryStringParser _parser;

        [TestFixtureSetUp]
        public void Setup()
        {
            _queryStringMissingParameter = new NameValueCollection();
            _qs1 = new NameValueCollection();
            _qs1.Add("data", "10x10,20x20,30x30-2");
            _parser = new HeatMapHandlerQueryStringParser(_qs1);
        }
            [Test]
        [ExpectedException(ExceptionType = typeof(ArgumentException))]
        public void ThrowsArgumentExceptionIfNullParameter()
        {
                _parser.GetHeatPointList();
        }

        [Test]
        public void ReturnsEmptyIfQueryStringValueMissing()
        {
            _parser.GetHeatPointList();
        }

        [Test]
        public void ReturnsCorrectNumberOfHeatPoints()
        {
            List<HeatPoint> list = _parser.GetHeatPointList();
            Assert.AreEqual(list.Count, 4);

        }
        [Test]
        public void GetHeatPointRadiusReturnsValueFromQueryString()
        {
            int expected = 100;
            NameValueCollection queryString = new NameValueCollection();
            queryString.Add(Constants.QueryString.HeatMapSettings.PointRadiusKey, expected.ToString());
            HeatMapHandlerQueryStringParser parser = new HeatMapHandlerQueryStringParser(queryString);
            int actual = parser.GetHeatPointRadius();
            Assert.AreEqual(expected, actual);
            Assert.AreNotEqual(Constants.HeatMapSettingsDefaults.PointRadius, actual, "The test value should not be equal to the default value of " + Constants.HeatMapSettingsDefaults.PointRadius.ToString());
        }
        [Test]
        public void GetHeatPointRadiusReturnsDefaultIfNotInQueryString()
        {
            int expected = Constants.HeatMapSettingsDefaults.PointRadius;
            NameValueCollection emptyQueryString = new NameValueCollection();
            HeatMapHandlerQueryStringParser parser = new HeatMapHandlerQueryStringParser(emptyQueryString);
            int actual = parser.GetHeatPointRadius();
            Assert.AreEqual(expected, actual);
        }
    }
}
