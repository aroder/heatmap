<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scaling.aspx.cs" Inherits="CenStatsHeatMap.UI.Scaling" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
        <script src="http://www.google.com/jsapi"></script>
            <script src="http://maps.google.com/maps?file=api&amp;v=2&amp;key=http://maps.google.com/maps?file=api&amp;v=2&amp;key=ABQIAAAAwROTpWdVzJB01-DGrse6FRSqQmF3E0Lbf7V2PAQkIYw9373E9xT6v6TCoDjdK2JPF70MX0KZ5Fmlng"
        type="text/javascript"></script>
<script>
    // Load jQuery
    google.load("jquery", "1");
</script>
<script>
    
    var map = null;
    var heatMapOverlay;
    var bounds;
    var overlayUrl = function() { return "images/HeatMapTileOverlay.aspx?zoom=10&data=39.655432,-104.788842,39.658207,-104.791245&rando=" + Math.random(); };
    function loadGoogle() {
        if (GBrowserIsCompatible()) {
            map = new GMap2(document.getElementById("map"));
            map.addControl(new GLargeMapControl());
            map.addControl(new GMapTypeControl());
            map.setCenter(new GLatLng(39.67, -104.75), 12);
            //map.addOverlay(overlay);

            //ground overlay
            bounds = new GLatLngBounds(new GLatLng(39.649112, -104.789571), new GLatLng(39.7, -104.7));
            heatMapOverlay = new GGroundOverlay(overlayUrl(), bounds);
            map.addOverlay(heatMapOverlay);

        }
        else
            alert('Your Internet browser is not compatible with this website.');
    }


    $(document).ready(loadGoogle);
    $(window).unload(GUnload);
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <div id="map" style="width: 400px; height: 350px">
    
    </div>
    </form>
</body>
</html>
