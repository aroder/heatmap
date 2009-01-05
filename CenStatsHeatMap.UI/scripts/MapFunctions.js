/* The DataPoint Class */
function HeatPoint(lat, lng) {
    this.Latitude = lat;
    this.Longitude = lng;
}

/// DataPointArray - an array of HeatPoint objects
/// returns a HeatPoint object representing the center point of the points in HeatPointArray
function GetCenter(HeatPointArray) {
    var lngSum = 0;
    var latSum = 0;
    var avgLng, avgLat;
    for (var i = 0; i < HeatPointArray.length; i++) {
        lngSum += HeatPointArray[i].Longitude
        latSum += HeatPointArray[i].Latitude;
    }
    avgLng = lngSum / HeatPointArray.length;
    avgLat = latSum / HeatPointArray.length;
    var centerPoint = new HeatPoint(avgLat, avgLng);
    return centerPoint;
}

// Creates and returns a HeatPoint representing the southwesternmost point of the HeatPointArray
function GetSouthwestCorner(HeatPointArray) {
    var westMost = HeatPointArray[0].Longitude;
    var southMost = HeatPointArray[0].Latitude;
    for (var i = 0; i < HeatPointArray.length; i++) {
        var lng = HeatPointArray[i].Longitude;
        var lat = HeatPointArray[i].Latitude;
        if (lng < westMost) westMost = lng;
        if (lat < southMost) southMost = lat;
    }
    return new HeatPoint(southMost, westMost);
}

// Creates and returns a HeatPoint representing the northeasternmost point of the HeatPointArray
function GetNortheastCorner(HeatPointArray) {
    var northMost = HeatPointArray[0].Latitude;
    var eastMost = HeatPointArray[0].Longitude;
    for (var i = 0; i < HeatPointArray.length; i++) {
        var lat = HeatPointArray[i].Latitude;
        var lng = HeatPointArray[i].Longitude;
        if (lng > eastMost) eastMost = lng;
        if (lat > northMost) northMost = lat;
    }
    return new HeatPoint(northMost, eastMost);
}

//returns string in CSV format: lat1,lon1,lat2,lon2...latN,lonN
function GetCsvForQueryString(HeatPointArray) {
    var retval = '';
    for (var i = 0; i < HeatPointArray.length; i++) {
        retval += HeatPointArray[i].Latitude + ',' + HeatPointArray[i].Longitude + ',';
    }
    // remove trailing comma
    retval = retval.slice(0, -1);

    return retval;
}

function GetBounds(HeatPointArray) {
    var NortheastMostPoint = GetNortheastCorner(HeatPointArray);
    var SouthwestMostPoint = GetSouthwestCorner(HeatPointArray);
    return new GLatLngBounds(new GLatLng(SouthwestMostPoint.Latitude, SouthwestMostPoint.Longitude), new GLatLng(NortheastMostPoint.Latitude, NortheastMostPoint.Longitude));
}

