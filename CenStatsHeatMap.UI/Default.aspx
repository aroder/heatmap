<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CenStatsHeatMap.UI._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <title>CenStats HeatMap</title>

    <script src="http://www.google.com/jsapi"></script>
    <script src="/scripts/HeatMap.js"></script>
    <script src="/scripts/json2.js"></script>
    <script src="/scripts/serviceProxy.js"></script>
    <script src="/scripts/MapFunctions.js"></script>



    <script src="http://maps.google.com/maps?file=api&amp;v=2&amp;key=http://maps.google.com/maps?file=api&amp;v=2&amp;key=ABQIAAAAwROTpWdVzJB01-DGrse6FRSqQmF3E0Lbf7V2PAQkIYw9373E9xT6v6TCoDjdK2JPF70MX0KZ5Fmlng"
        type="text/javascript"></script>

    <script type="text/javascript">
        var map = null;  // the google map
        var heatMapOverlay;  // the google map overlay
        var bounds;  // the bounds of the overlay
        var _cornerIcon;
        var CORNER_ICON = new function() {
            if (!_cornerIcon) {
                _cornerIcon = new GIcon(G_DEFAULT_ICON, "http://maps.google.com/mapfiles/ms/micons/yellow.png");

                //_cornerIcon.image = "http://maps.google.com/mapfiles/ms/micons/yellow.png";
            }
            return _cornerIcon;
        };
        var _boundsIcon;
        var BOUNDS_ICON = new function() {
            if (!_boundsIcon) {
                _boundsIcon = new GIcon(G_DEFAULT_ICON, "http://maps.google.com/mapfiles/ms/micons/blue.png");
            }
            return _boundsIcon;
        };
        
        var greyTileOverlayImageUrl = "http://chart.apis.google.com/chart?chst=d_text_outline&chs=256x256&chf=bg,s,00000055&chld=FFFFFF|16|h|000000|b|";

        // assign the data points to an array
        var HeatPoints = new Array();
        var LatLngArray = new Array();
        for (var i = 0; i < 35; i++) {
            var lat = 39 + (Math.random());
            var lng = -104 - (Math.random());
            HeatPoints[i] = new HeatPoint(lat, lng);
            LatLngArray[i] = new GLatLng(lat, lng);
        }
        HeatPoints[35] = new HeatPoint(39.649468, -104.789228);
        HeatPoints[36] = new HeatPoint(39.649468, -104.789228);
        HeatPoints[37] = new HeatPoint(39.649468, -104.789228);
        HeatPoints[38] = new HeatPoint(39.649468, -104.789228);
        //        HeatPoints[39] = new HeatPoint(39.649468, -104.789228);
        //        HeatPoints[40] = new HeatPoint(39.649468, -104.789228);
        //        HeatPoints[41] = new HeatPoint(39.649468, -104.789228);
        //        HeatPoints[42] = new HeatPoint(39.649468, -104.789228);
        //        HeatPoints[43] = new HeatPoint(39.649468, -104.789228);

        var centerPoint = GetCenter(HeatPoints);

        //var overlayUrl = function() { return "images/HeatMapTileOverlay.aspx?zoom=10&data=" + GetCsvForQueryString(HeatPoints) + "&rando=" + Math.random(); };
        //var overlayUrl = function() { return "images/HeatMapTileOverlay.aspx?token=" + $("#map").data("tokenString[0]") };

        //prompt(null, "http://localhost:4845/" + overlayUrl());
        function loadGoogle() {
            if (GBrowserIsCompatible()) {
                map = new GMap2(document.getElementById("map"));
                map.addControl(new GLargeMapControl());
                map.addControl(new GMapTypeControl());
                map.setCenter(new GLatLng(centerPoint.Latitude, centerPoint.Longitude), 8);

                //add the settings and get the overlay token
                var data = { heatPoints: HeatPoints };
                HeatmapService().invoke(
                    "GenerateHeatMapOverlayToken"
                    , data
                    , function(result) { //onsuccess
                        $("#map").data("dataCacheToken", result);
                        RefreshOverlay(map);
                    }
                    , function(request, errorType, ex) {
                        alert(request[0] + "\r\n" + errorType + "\r\n" + ex[0]);
                    }
                );

                // add a listener that will add a heatpoint to the overlay when the map is clicked
                //                GEvent.addListener(map, "click", function(overlay, latLng, overlayLatLng) {
                //                    // add a new data point, representing the coord of the click, to the end of the array
                //                    HeatPoints[HeatPoints.length] = new HeatPoint(latLng.lat(), latLng.lng());
                //                    RefreshOverlay(map);
                //                });
                GEvent.addListener(map, "zoomend", function(oldLevel, newLevel) {
                    //eventually, the option should be available to get a fresh overlay after a zoom event occurs
                    RefreshOverlay(map);
                });
//                Globals.PROJECTION = G_NORMAL_MAP.getProjection();
            }
            else
                alert('Your Internet browser is not compatible with this website.');
        }

        function RefreshOverlay(map) {
            //remove the old overlay, then add the new one
            try {
                map.removeOverlay(heatMapOverlay);
            } catch (ex) {
                //alert(ex.description);
            }

            var GreyTileIfHeatMapImageShouldBeRequestedTileLayer = new GTileLayer(null, null, null);
            GreyTileIfHeatMapImageShouldBeRequestedTileLayer.isPng = function() { return true; };
            GreyTileIfHeatMapImageShouldBeRequestedTileLayer.getOpacity = function() { return 1.0 };
            GreyTileIfHeatMapImageShouldBeRequestedTileLayer.getTileUrl = function(tile, zoom) {
                var requestHeatMapTile = false;
                var topleftLatLng = Globals.PROJECTION.fromPixelToLatLng(new GPoint(tile.x * 256, tile.y * 256), zoom);
                var bottomrightLatLng = Globals.PROJECTION.fromPixelToLatLng(new GPoint((tile.x + 1) * 256, (tile.y + 1) * 256), zoom);
                var tileBounds = new GLatLngBounds();
                tileBounds.extend(topleftLatLng);
                tileBounds.extend(bottomrightLatLng);
                PlaceMarker(tileBounds.getCenter(), BOUNDS_ICON);
                PlaceMarker(tileBounds.getSouthWest(), CORNER_ICON);
                PlaceMarker(tileBounds.getNorthEast(), CORNER_ICON);

                for (var i = 0; i < LatLngArray.length; i++) {
                    if (tileBounds.containsLatLng(LatLngArray[i])) {
                        //if ((tile.x % 2 && !(tile.y % 2)) || (!(tile.x % 2) && tile.y % 2)) {
                        return greyTileOverlayImageUrl;
                    } else {
                        return "";
                    }
                }
            };
            map.addOverlay(new GTileLayerOverlay(GreyTileIfHeatMapImageShouldBeRequestedTileLayer));

            var TileLayer = new GTileLayer(null, null, null);
            TileLayer.getTileUrl = function(tile, zoom) {
                //return "";
                var topleftGLatLng = Globals.PROJECTION.fromPixelToLatLng(new GPoint(tile.x * 256, tile.y * 256), zoom);
                var bottomrightGLatLng = Globals.PROJECTION.fromPixelToLatLng(new GPoint((tile.x + 1) * 256, (tile.y + 1) * 256), zoom);

                //PlaceMarker(topleftGLatLng, CORNER_ICON);
                //PlaceMarker(bottomrightGLatLng, CORNER_ICON);

                var tileUrl = String.format(
                    "http://localhost:1723/images/HeatMapTileGenerator.ashx?topLeftLat={0}&topLeftLng={1}&bottomRightLat={2}&bottomRightLng={3}&dataCacheToken={4}&zoom={5}"
                    , topleftGLatLng.lat()                      //topLeftLat
                    , topleftGLatLng.lng()                      //topLeftLng
                    , bottomrightGLatLng.lat()                  //bottomRightLat
                    , bottomrightGLatLng.lng()                  //bottomRightLng
                    , $("#map").data("dataCacheToken")          //dataCacheToken
                    , zoom                                      //zoom
                );

                $("#sanityImages").prepend('<img src="' + tileUrl + '" border="1" />');
                return tileUrl;
            };
            TileLayer.isPng = function() { return true; };
            TileLayer.getOpacity = function() { return 1.0 };
            map.addOverlay(new GTileLayerOverlay(TileLayer));


            //heatMapOverlay = new GGroundOverlay(overlayUrl(), GetBounds(HeatPoints));
            //map.addOverlay(heatMapOverlay);
            //$("#sanityImage").get(0).src = overlayUrl();

        }

        var _heatMapService;
        function HeatmapService() {
            if (null == _heatMapService) {
                _heatMapService = new serviceProxy("HeatMapService.svc/", "application/text");
            }
            return _heatMapService;
        }

        $(document).ready(function() {
            loadGoogle();
            PlaceTileMarkers(null, null)
        });

        var mySettings;

        $(window).unload(GUnload);

        //returns GLatLng whose Lat represents the Lat offset value, Lng for Lng offset value
        function GetOffsetLatLng(originalGLatLng, heatPointSizeInPixels) {
            var proj = map.getCurrentMapType().getProjection();
            var originalPoint = proj.fromLatLngToPixel(originalGLatLng, map.getZoom());
            var pixelsToOffset = Math.ceil(heatPointSizeInPixels / 2);
            var offsetPoint = new GPoint(originalPoint.x + pixelsToOffset, originalPoint.y + pixelsToOffset);
            var offsetGLatLng = proj.fromPixelToLatLng(offsetPoint, map.getZoom());
            var offsetLat = Math.abs(offsetGLatLng.lat() - originalGLatLng.lat());
            var offsetLng = Math.abs(offsetGLatLng.lng() - originalGLatLng.lng());
            var retval = new GLatLng(offsetLat, offsetLng);
            return retval;
        }

        //returns GLatLng whose Lat represents degrees Lat per pixel, Lng represents degrees Lng per pixel
        function GetDegreesPerPixel() {
            var point1 = map.getCurrentMapType().getProjection().fromLatLngToPixel(map.getCenter(), map.getZoom());
            var degreesPerPixelX = Math.abs(map.getCenter().lat() / point1.y);
            var degreesPerPixelY = Math.abs(map.getCenter().lng() / point1.x);
            var retval = new GLatLng(degreesPerPixelY, degreesPerPixelX);
            return retval;
        }

        function AjaxError(xmlHttpRequest, errorType, exceptionObject) {
            alert('ajax error: ' + errorType);
        }

        /// tile - a GTile object
        /// zoom - an int representing the map zoom level
        function PlaceTileMarkers(tile, zoom) {
            for (var i = 0; i < LatLngArray.length; i++) {
                PlaceMarker(LatLngArray[i]); //latlng);
            }
        }

        /// latlng - a GLatLng object
        /// iconToUse - a GIcon object (optional)
        function PlaceMarker(latlng, iconToUse) {
            var marker;
            if (!iconToUse) {
                marker = new GMarker(latlng);
            } else {
                var options = { icon: iconToUse };
                marker = new GMarker(latlng, options);
            }
            map.addOverlay(marker);
        }



        $(document).ready(function() {
            $("#WcfPostButton").click(function() {
                //var points = new Array();
                var data = { heatPoints: HeatPoints };
                HeatmapService().invoke(
                    "GenerateHeatMapOverlayToken"
                    , data
                    , function(result) { //onsuccess
                        $("#map").data("tokenString[0]", result)
                    }
                    , function(request, errorType, ex) {
                        alert(request[0] + "\r\n" + errorType + "\r\n" + ex[0]);
                    }
                );
            });
            $("#WcfUpdateSettings").click(function() {
                HeatmapService().invoke(
                    "UpdateHeatmapOverlaySettings"
                    , { token: $("#map").data("tokenString[0]"), settings: mySettings }
                    , function(result) {
                        $("#map").data("tokenString[0]", result);
                    }
                    , function(request, errorType, ex) { alert('error'); }
                );
            });
            $("#WcfRetrieveSettings").click(function() {
                HeatmapService().invoke(
                        "RetrieveHeatMapOverlaySettings"
                        , { token: $("#map").data("tokenString[0]") }
                        , function(result) { //onsuccess
                            mySettings = result;
                            $("#output").html(JSON2.stringify(result));
                        }
                        , function(request, errorType, ex) {
                            alert(request[0] + "\r\n" + errorType + "\r\n" + ex[0]);
                        }
                    );
            });

            $("#GetPixelButton").click(function() {
                var degreesPerPixel = GetDegreesPerPixel();
                alert(String.format(
                    "\{{0}, {1}\}"
                    , degreesPerPixel.lat()
                    , degreesPerPixel.lng()
                ));
            });
        });
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div id="map" style="width: 600px; height: 500px">
    </div>
    <input type="button" value="Add Settings" id="WcfPostButton" />
    <input type="button" value="Update Settings" id="WcfUpdateSettings" />
    <input type="button" value="Retrieve Settings" id="WcfRetrieveSettings" />
    <br />
    <input type="button" value="Get Pixel for Center" id="GetPixelButton" />
    <br />
    <div id="sanityImages">
    </div>
    <div id="output" style="border: 1px solid white;">
    </div>
    </form>
</body>
</html>
