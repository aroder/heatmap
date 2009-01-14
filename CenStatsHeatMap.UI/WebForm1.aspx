<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="CenStatsHeatMap.UI.WebForm1"
    MasterPageFile="~/One.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">

    <script src="scripts/Asynchronizer.js" type="text/javascript"></script>
    <script type="text/javascript" src="http://maps.google.com/maps?file=api&amp;v=2&amp;key=http://maps.google.com/maps?file=api&amp;v=2&amp;key=ABQIAAAAwROTpWdVzJB01-DGrse6FRSqQmF3E0Lbf7V2PAQkIYw9373E9xT6v6TCoDjdK2JPF70MX0KZ5Fmlng"></script>

    <script src="/scripts/HeatMap.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function() {
            HeatMap.Settings.ShowTileBorder = false;
            HeatMap.Settings.ShowMarkers = false;
            HeatMap.Initialize("map");

            var points = [];

            for (var i = 0; i < 1000; i++) {
                var x = 30 + Math.random();
                var y = -109 - Math.random();
                points.push(new HeatMapPoint(x, y));
                //                HeatMap.AddHeatMapPoint(new HeatMapPoint(x, y));
            }
            HeatMap.AddHeatMapPointArray(points);

            //HeatMap.AddHeatMapPoint(new HeatMapPoint(30.8, -109.7));
            //HeatMap.AddHeatMapPoint(new HeatMapPoint(30.75, -109.7));
            HeatMap.GoogleMap().setCenter(HeatMap.GetHeatMapCenter(), HeatMap.Settings.Zoom);

            //            HeatMap.AddHeatMapPoint(new HeatMapPoint(HeatMap.GetHeatMapCenter().lat(), HeatMap.GetHeatMapCenter().lng()));
            //            HeatMap.RefreshTileOverlay();

            HeatMap.ApplyHeatMap();

        });
    </script>

</asp:Content>
<asp:Content id="contentArea" contentplaceholderid="ContentPlaceHolder1" runat="server">
    <div id="map" style="width: 600px; height: 500px">
    </div>
</asp:Content>
