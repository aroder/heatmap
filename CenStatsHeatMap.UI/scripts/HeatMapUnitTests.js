// Modules:
// HeatMapModule
// GetCenterModule
// HeatMapPointsModule
// HeatMapTileModule
// HeatMapPointModule

function HeatMapModule() {
    module("HeatMapModule");
    //    test("GetQueryStringParm", function() {
    //        var expectedTestValue = "working";
    //        var expectedTest2Value = "stillWorking";
    //        var queryString = "http://localhost/index.html?test=working&test2=stillWorking";
    //        equals(HeatMap.GetQueryStringParm("test", queryString), expectedTestValue);
    //        equals(HeatMap.GetQueryStringParm("test2", queryString), expectedTest2Value);
    //        equals(HeatMap.GetQueryStringParm("test", window.location.href), "");
    //    });
    test("ClearHeatMapPoints", function() {
        var p = new HeatMapPoint(30, 30);
        HeatMap.AddHeatMapPoint(p);
        ok(0 < hashSize(HeatMap.HeatMapPoints), String.format("Size ({0}) is greater than 0", hashSize(HeatMap.HeatMapPoints)));

        HeatMap.ClearHeatMapPoints();

        ok(0 === hashSize(HeatMap.HeatMapPoints), String.format("Size ({0}) equals 0", hashSize(HeatMap.HeatMapPoints)));
    });
}
function HeatMapPointModule() {
    module("HeatMapPointModule");
    test("HeatMapPoint properties function properly", function() {
        var p = new HeatMapPoint(30, 30);
        var expectedLat = 30;
        var expectedLng = 30;
        var expectedTileCount1 = 0;
        var expectedTileCount2 = 1;

        same(p.GLatLng().lat(), expectedLat, "GLatLng lat evaluation");
        same(p.GLatLng().lng(), expectedLng, "GLatLng lng evaulation");
    });

    test("HeatMapPoint.GetAssociatedTiles", function() {
        var p1 = new HeatMapPoint(30, 30);
        var z1 = 8;
        var expectedTileCount1 = 1;

        equals(p1.GetAssociatedTiles(z1).length, expectedTileCount1, "GetAssociatedTiles evaluation");

        var p2 = new HeatMapPoint(45, 90);
        var z2 = 4;
        var expectedTileX = 12;
        var expectedTileY = 5;

        equals(p2.GetAssociatedTiles(z2)[0].X(), expectedTileX, "Correct tile found (X evaluation)");
        equals(p2.GetAssociatedTiles(z2)[0].Y(), expectedTileY, "Correct tile found (Y evaluation)");
    });
}

function HeatMapTileModule() {
    module("HeatMapTile");
    test("HeatMapTile.GetKey() works", function() {
        var x = 1;
        var y = 2;
        var zoom = 3;
        var expectedKey = String.format("{0},{1},{2}", x, y, zoom);
        var t = new HeatMapTile(x, y, zoom);
        var actualKey = t.GetKey();
        equals(actualKey, expectedKey, String.format("Keys match: {0}, {1}", actualKey, expectedKey));
    });
    test("HeatMapTile properties function properly", function() {
        HeatMap.ClearHeatMapPoints();

        var t = new HeatMapTile(0, 0, 8);
        var expectedX = 0;
        var expectedY = 0;
        var expectedZoom = 8;
        var expectedPointsCount1 = 0;
        var expectedPointsCount2 = 1;
        var expectedBoundsTopLeftLat = 85.0511287798066;
        var expectedBoundsTopLeftLng = -180;
        var expectedBoundsBottomRightLat = 84.92832092949963;
        var expectedBoundsBottomRightLng = -178.59375;
        var expectedExtendedBoundsTopLeftLat = 85.055865290837;
        var expectedExtendedBoundsTopLeftLng = 179.945068359375;
        var expectedExtendedBoundsBottomRightLat = 84.92346254591743;
        var expectedExtendedBoundsBottomRightLng = -178.538818359375;


        equals(t.X(), expectedX, "Property X evaluation");
        equals(t.Y(), expectedY, "Property Y evaluation");
        equals(t.Zoom(), expectedZoom, "Property Zoom evaluation");
        equals(t.Bounds().getNorthEast().lat(), expectedBoundsTopLeftLat, "Bounds TopLeftLat evaluation");
        equals(t.Bounds().getSouthWest().lng(), expectedBoundsTopLeftLng, "Bounds TopLeftLng evaluation");
        equals(t.Bounds().getSouthWest().lat(), expectedBoundsBottomRightLat, "Bounds BottomRightLat evaluation");
        equals(t.Bounds().getNorthEast().lng(), expectedBoundsBottomRightLng, "Bounds BottomRightLng evaluation");
        equals(t.GetExtendedBounds().getNorthEast().lat(), expectedExtendedBoundsTopLeftLat, "ExtendedBounds TopLeftLat evaluation");
        equals(t.GetExtendedBounds().getSouthWest().lng(), expectedExtendedBoundsTopLeftLng, "ExtendedBounds TopLeftLng evaluation");
        equals(t.GetExtendedBounds().getSouthWest().lat(), expectedExtendedBoundsBottomRightLat, "ExtendedBounds BottomRightLat evaluation");
        equals(t.GetExtendedBounds().getNorthEast().lng(), expectedExtendedBoundsBottomRightLng, "ExtendedBounds BottomRightLng evaulation");
        equals(t.AssociatedHeatMapPoints().length, expectedPointsCount1, "Property AssociatedHeatMapPoints evaluation (tile " + t.X() + ',' + t.Y() + ")");
        HeatMap.AddHeatMapPoint(new HeatMapPoint(84.93, -179));
        equals(HeatMap.HeatMapTiles["0,0,8"].AssociatedHeatMapPoints().length, expectedPointsCount2, "Property AssociatedHeatMapPoints evaluation");
    });

    test("HeatMapTile.GetExtendedBounds returns bounds larger than original Bounds", function() {
        var t = new HeatMapTile(1, 1, 8);
        ok(true, String.format("Using Bounds SW:[{0},{1}],NE:[{2},{3}]",
            t.Bounds().getSouthWest().lat(),
            t.Bounds().getSouthWest().lng(),
            t.Bounds().getNorthEast().lat(),
            t.Bounds().getNorthEast().lng()
        ));

        ok(true, String.format("Using ExtendedBounds SW:[{0},{1}],NE:[{2},{3}]",
            t.GetExtendedBounds().getSouthWest().lat(),
            t.GetExtendedBounds().getSouthWest().lng(),
            t.GetExtendedBounds().getNorthEast().lat(),
            t.GetExtendedBounds().getNorthEast().lng()
        ));
        ok(t.GetExtendedBounds().containsBounds(t.Bounds()), "ExtendedBounds contains Bounds");
        ok(t.GetExtendedBounds().getSouthWest().lat() < t.Bounds().getSouthWest().lat(), "Extended Southern lat is more South than original Southern lat");
        ok(t.GetExtendedBounds().getSouthWest().lng() < t.Bounds().getSouthWest().lng(), "Extended Western lng is more West than original Western lng");
        ok(t.GetExtendedBounds().getNorthEast().lat() > t.Bounds().getNorthEast().lat(), "Extended Northern lat is more North than original Northern lat");
        ok(t.GetExtendedBounds().getNorthEast().lng() > t.Bounds().getNorthEast().lng(), "Extended Eastern lng is more East than original Eastern lng");
    });
    test("HeatMapTile.ContainsHeatMapPointInExtendedBounds returns true for point in Bounds", function() {
        var t = new HeatMapTile(1, 1, 8);
        var p = new HeatMapPoint(84.9, -178);
        ok(
            true,
            String.format(
                "Using Bounds SW:[{0},{1}],NE:[{2},{3}] and Point [{4},{5}]",
                t.Bounds().getSouthWest().lat(),
                t.Bounds().getSouthWest().lng(),
                t.Bounds().getNorthEast().lat(),
                t.Bounds().getNorthEast().lng(),
                p.GLatLng().lat(),
                p.GLatLng().lng()
            )
        );

        ok(t.ContainsHeatMapPointInBounds(p), "Bounds contains Point");
    });
    //TODO: finish this test
    test("HeatMapTile.ContainsHeatMapPointInExtendedBounds returns true for point in ExtendedBounds", function() {
        var t = new HeatMapTile(1, 1, 8);
        var p = new HeatMapPoint(84.8, -178);
        ok(
            true,
            String.format(
                "Using Bounds SW:[{0},{1}],NE:[{2},{3}]",
                t.Bounds().getSouthWest().lat(),
                t.Bounds().getSouthWest().lng(),
                t.Bounds().getNorthEast().lat(),
                t.Bounds().getNorthEast().lng()
            )
        );
        ok(
            true,
            String.format(
                "Using ExtendedBounds SW:[{0},{1}],NE:[{2},{3}]",
                t.GetExtendedBounds().getSouthWest().lat(),
                t.GetExtendedBounds().getSouthWest().lng(),
                t.GetExtendedBounds().getNorthEast().lat(),
                t.GetExtendedBounds().getNorthEast().lng()
            )
        );
        ok(
            true,
            String.format("Using Point [{0},{1}]", p.GLatLng().lat(), p.GLatLng().lng())
        );

        ok(!t.ContainsHeatMapPointInBounds(p), "This test should use a Point outside the regular Bounds");
        ok(t.ContainsHeatMapPointInExtendedBounds(p), "ExtendedBounds contains Point");

    });
    test("HeatMapTile.ContainsHeatMapPointInExtendedBounds returns false for point outside ExtendedBounds", function() {
        var t = new HeatMapTile(1, 1, 8);
        var p = new HeatMapPoint(34.8, -100);
        ok(
            true,
            String.format(
                "Using Bounds SW:[{0},{1}],NE:[{2},{3}]",
                t.Bounds().getSouthWest().lat(),
                t.Bounds().getSouthWest().lng(),
                t.Bounds().getNorthEast().lat(),
                t.Bounds().getNorthEast().lng()
            )
        );
        ok(
            true,
            String.format(
                "Using ExtendedBounds SW:[{0},{1}],NE:[{2},{3}]",
                t.GetExtendedBounds().getSouthWest().lat(),
                t.GetExtendedBounds().getSouthWest().lng(),
                t.GetExtendedBounds().getNorthEast().lat(),
                t.GetExtendedBounds().getNorthEast().lng()
            )
        );
        ok(
            true,
            String.format("Using Point [{0},{1}]", p.GLatLng().lat(), p.GLatLng().lng())
        );

        ok(!t.ContainsHeatMapPointInExtendedBounds(p), "Bounds does not contain Point");
    });
    test("HeatMapTile.GetTileUrl returns correct URL", function() {
        HeatMap.ClearHeatMapPoints();
        var t = new HeatMapTile(1, 1, 8);
        var p = new HeatMapPoint(84.9, -178);
        var actualUrl = t.GetTileUrl();
        ok(!actualUrl, "Empty URL when Tile has no associated Points");
        HeatMap.AddHeatMapPoint(p);
        actualUrl = HeatMap.HeatMapTiles["1,1,8"].GetTileUrl();
        ok(actualUrl, "URL exists when Tile has associated Point(s): " + actualUrl);
        HeatMap.ClearHeatMapPoints();
    });

}

function GetCenterModule() {
    module("Get Center");
    test("HeatMap.GetCenter latitude test", function() {
        HeatMap.ClearHeatMapPoints();
        HeatMap.AddHeatMapPoint(new HeatMapPoint(30, 110));
        HeatMap.AddHeatMapPoint(new HeatMapPoint(30, 111));
        var expected = 110.5;
        var actual = HeatMap.GetHeatMapCenter().lng();
        same(actual, expected);
        HeatMap.ClearHeatMapPoints();
    });
    test("HeatMap.GetCenter longitude test", function() {
        HeatMap.ClearHeatMapPoints();
        //Arrange
        HeatMap.AddHeatMapPoint(new HeatMapPoint(30, 110));
        HeatMap.AddHeatMapPoint(new HeatMapPoint(31, 110));
        var expected = 30.5;

        //Act
        var actual = HeatMap.GetHeatMapCenter().lat();

        //Assert
        same(actual, expected);

        //cleanup
        HeatMap.ClearHeatMapPoints();
    });
    test("HeatMap.GetCenter simple combined lat/lng test", function() {
        HeatMap.ClearHeatMapPoints();
        //Arrange
        HeatMap.AddHeatMapPoint(new HeatMapPoint(30, 110));
        HeatMap.AddHeatMapPoint(new HeatMapPoint(31, 111));
        var expectedLat = 30.5;
        var expectedLng = 110.5;

        //Act
        var actualLat = HeatMap.GetHeatMapCenter().lat();
        var actualLng = HeatMap.GetHeatMapCenter().lng();

        //Assert
        same(actualLat, expectedLat, "Simple latitude assertion");
        same(actualLng, expectedLng, "Simple longitude assertion");

        //cleanup
        HeatMap.ClearHeatMapPoints();
    });

    test("HeatMap.GetCenter complex combined lat/lng test", function() {
        HeatMap.ClearHeatMapPoints();
        //Arrange
        HeatMap.AddHeatMapPoint(new HeatMapPoint(30, 110));
        HeatMap.AddHeatMapPoint(new HeatMapPoint(-90, 127));
        HeatMap.AddHeatMapPoint(new HeatMapPoint(5, 0));
        HeatMap.AddHeatMapPoint(new HeatMapPoint(5, -1));
        var expectedLat = -12.5;
        var expectedLng = 59;

        //Act
        var actualLat = HeatMap.GetHeatMapCenter().lat();
        var actualLng = HeatMap.GetHeatMapCenter().lng();

        //Assert
        equals(actualLat, expectedLat, "Complex latitude assertion");
        equals(actualLng, expectedLng, "Complex longitude assertion");

        //cleanup
        HeatMap.ClearHeatMapPoints();
    });
}
function HeatMapPointsModule() {
    module("HeatMap Points");

    test("HeatMap.AddHeatMapPoint adds the point to HeatMap.HeatMapPoints", function() {
        HeatMap.ClearHeatMapPoints();
        var originalCount = HeatMap.HeatMapPoints.length;
        var expectedCount = originalCount + 1;
        HeatMap.AddHeatMapPoint(new HeatMapPoint(30, -108));
        var actualCount = hashSize(HeatMap.HeatMapPoints);

        equals(actualCount, expectedCount, "HeatMap.HeatMapPoints count assertion");
        HeatMap.ClearHeatMapPoints();
    });
}









function SampleTestsNoModule() {
    module("");
    test("a basic test example", function() {
        ok(true, "this test is fine");
        var value = "hello";
        equals("hello", value, "We expect value to be hello");
    });
}
function SampleTestsModuleA() {
    module("A");

    test("first test within module", function() {
        ok(true, "all pass");
    });
    test("second test within module", function() {
        ok(true, "all pass");
    });
}
function SampleTestsModuleB() {
    module("B");

    test("some other test", function() {
        expect(1);
        ok(true, "well");
    });

    test("a failed test", function() {
        expect(1);
        same(true, false, "Expected to fail");
    });
}
function SampleTests() {
    SampleTestsModuleA();
    SampleTestsModuleB();
    SampleTestsNoModule();
}





function RunTests() {
    HeatMapModule();
    HeatMapPointModule();
    HeatMapTileModule();
    HeatMapPointsModule();
    GetCenterModule();
}

//        $(document).ready(SampleTests);
$(document).ready(RunTests);