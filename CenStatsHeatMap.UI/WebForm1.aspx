<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="CenStatsHeatMap.UI.WebForm1"
    MasterPageFile="~/One.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            HeatMap.Settings.ShowTileBorder = true;
            HeatMap.Settings.ShowMarkers = false;
            HeatMap.Settings.DebugMode = true;
            HeatMap.Initialize("map");

//            var points = [];
//
//            for (var i = 0; i < 10; i++) {
//                var lat = 30 + Math.random();
//                var lng = -109 - Math.random();
//                points.push(new HeatMapPoint(lat, lng));
//            }
//            HeatMap.AddHeatMapPointArray(points);

            //HeatMap.AddAddress("asdfEnglewood, CO", function errorCallback(address) { alert('service couldn\'t even find ' + address); } );
            HeatMap.AddAddress("Franktown, CO");
            var addressArray = [];
            addressArray.push("Aurora, CO");
            addressArray.push("Denver, CO");
            addressArray.push("80112");
            addressArray.push("Castle Rock, CO");
            HeatMap.AddAddress("Englewood, CO");
            HeatMap.AddAddressArray(addressArray);

//            HeatMap.GoogleMap().setCenter(HeatMap.GetHeatMapCenter(), HeatMap.Settings.DefaultZoom);
            HeatMap.ApplyHeatMap(function() {alert('apply completed');});

        });
    </script>
</asp:Content>
<asp:Content ID="contentArea" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="map" style="width: 600px; height: 500px">
    </div>
</asp:Content>
