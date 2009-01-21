
/// the application object, an anonymous class
var HeatMap = {
    HeatMapPoints: [], // associative array where the key is "<lat>,<lng>", value is HeatMapPoint
    AssociatedHeatMapPoints: [], // key is "<lat>,<lng>,<zoomLevel>", value is HeatMapPoint
    HeatMapTiles: [], // associative array where the key is "<tileX>,<tileY>,<zoom>", value is HeatMapTile
    Settings: {
        MinZoomLevel: 1,
        MaxZoomLevel: 12,
        PointRadius: 25,
        DefaultZoom: 8,
        DebugMode: false,
        Center: new GLatLng(39.5, -105),
        ShowMarkers: false,
        ShowTileBorder: true,
        Keys: {
            ShowTileBorder: "tb"
            //, ShowMarkers: "m"
        }
    },
    GoogleMap: function(map) {
        if (map) {
            this._map = map;
        }
        return this._map;
    },
    Overlay: function(overlay, zoomLevel) {
        // if zoomLevel not defined, use the current zoom level
        if (!zoomLevel) {
            zoomLevel = this.GoogleMap().getZoom();
        }

        // if the private array is not defined, define it
        if (!this._overlays) {
            this._overlays = [];
            }

        // if the caller passed in a value, treat this as a setter
        if (overlay) {
            this._overlays[zoomLevel] = overlay;
        }

        // return the overlay for the zoom level regardless of whether this call was a getter or setter
        return this._overlays[zoomLevel];
    },
    /// A Google map projection, used to convert pixels to coordinates and back
    Projection: function() {
        if (!this._projection) { this._projection = G_NORMAL_MAP.getProjection(); }
        return this._projection;
    },
    Geocoder: function() {
        if (!this._geocoder) { this._geocoder = new GClientGeocoder(); }
        return this._geocoder;
    },
    /// adds the HeatMapPoint to the HeatMap, and associates the point and its tiles
    AddHeatMapPoint: function(heatMapPoint) {

        // add the heatMapPoint to the full list, or return if it already exists
        var pointKey = heatMapPoint.GLatLng().lat() + ',' + heatMapPoint.GLatLng().lng(); // String.format("{0},{1}", heatMapPoint.GLatLng().lat(), heatMapPoint.GLatLng().lng());
        if (this.HeatMapPoints[pointKey]) { return; }
        this.HeatMapPoints[pointKey] = heatMapPoint;

        // SPEED OPTIMIZATION: INSTEAD OF GOING OVER EACH ZOOM LEVEL NOW, THE INDIVIDUAL ZOOM LEVELS ARE GONE OVER DURING HEATMAP.ADDTILEOVERLAY
        // for each zoom level, associate the point and its tiles
        //        for (var zoomLevel = HeatMap.Settings.MinZoomLevel; zoomLevel < HeatMap.Settings.MaxZoomLevel; zoomLevel++) {
        //            var tileArray = heatMapPoint.GetAssociatedTiles(zoomLevel);
        //            // create the association from the tiles to the point
        //            for (var i = 0; i < tileArray.length; i++) {
        //                tileArray[i].AssociatedHeatMapPoints().push(heatMapPoint);
        //            }
        //        }

        // show a marker (pushpin) on the map if the settings direct it
        if (HeatMap.Settings.ShowMarkers) {
            HeatMap.GoogleMap().addOverlay(new GMarker(heatMapPoint.GLatLng()));
        }
    },
    /// adds the array of HeatMapPoints to the HeatMap, and associates each point with its tiles
    AddHeatMapPointArray: function(heatMapPointArray) {
        for (var i = 0; i < heatMapPointArray.length; i++) {

            this.AddHeatMapPoint(heatMapPointArray[i]);
        }
        HeatMap.Debug("Heatmap now contains " + hashSize(HeatMap.HeatMapPoints) + " HeatMapPoints.");
    },

    AddressesProcessing: 0,
    /// address - string representing an address
    /// addressNotFoundCallbackFunction - a function reference to call if the address is not found, sends back the address as a parameter
    AddAddress: function(address, addressNotFoundCallbackFunction) {
        HeatMap.AddressesProcessing++;
        this.Geocoder().getLatLng(
            address                     // the address to geocode
            , function(gLatLng) {       // the callback function
                HeatMap.AddressesProcessing--;
                if (gLatLng) {
                    HeatMap.AddHeatMapPoint(new HeatMapPoint(gLatLng.lat(), gLatLng.lng()));
                    HeatMap.Debug(String.format('Geocoded address "{0}" to [{1},{2}]', address, gLatLng.lat(), gLatLng.lng()));
                } else if (addressNotFoundCallbackFunction) {
                    HeatMap.Debug('Could not find address"' + address + '", calling callback function');
                    addressNotFoundCallbackFunction(address);
                } else {
                    HeatMap.Debug('Could not find address "' + address + '", and no callback defined');
                }
            }
        );
    },
    AddAddressArray: function(addressArray, addressNotFoundCallbackFunction) {
        for (var i = 0; i < addressArray.length; i++) {
            HeatMap.AddAddress(addressArray[i], addressNotFoundCallbackFunction);
        }
    },
    
    ClearHeatMapPoints: function() {
        this.HeatMapPoints = [];
    },
    /// returns a GLatLng representing the center of all HeatMapPoints
    GetHeatMapCenter: function() {
        if (0 === hashSize(this.HeatMapPoints)) {
            throw "Center cannot be created because no HeatMapPoints have been added to the HeatMap";
        }
        var lngSum = 0;
        var latSum = 0;
        var avgLng, avgLat;
        for (var key in this.HeatMapPoints) {
            lngSum += this.HeatMapPoints[key].GLatLng().lng();
            latSum += this.HeatMapPoints[key].GLatLng().lat();
        }
        avgLng = lngSum / hashSize(this.HeatMapPoints);
        avgLat = latSum / hashSize(this.HeatMapPoints);
        var centerPoint = new GLatLng(avgLat, avgLng);
        return centerPoint;
    },
    AddTileOverlay: function() {
        var zoomLevel = this.GoogleMap().getZoom();
    
        for (var key in this.HeatMapPoints) {
            // the point will tell us what tiles it pertains to (the tile that contains it plus any tiles it overlaps onto)
            var tileArray = this.HeatMapPoints[key].GetAssociatedTiles(zoomLevel);
            for (var i = 0; i < tileArray.length; i++) {
                // create the association from the tile to the point
                tileArray[i].AssociatedHeatMapPoints().push(this.HeatMapPoints[key]);
            }
        }
        var TileLayer = new GTileLayer(null, null, null);
        TileLayer.getTileUrl = function(tile, zoom) {
            var key = String.format("{0},{1},{2}", tile.x, tile.y, zoom);
            if (!HeatMap.HeatMapTiles[key]) {
                var t = new HeatMapTile(tile.x, tile.y, zoom);
                HeatMap.HeatMapTiles[key] = t;
            }
            return HeatMap.HeatMapTiles[key].GetTileUrl();
        };
        
        TileLayer.isPng = function() { return true; };
        TileLayer.getOpacity = function() { return 1.0; };
        var o = new GTileLayerOverlay(TileLayer);
        this.GoogleMap().addOverlay(o);
        this.Overlay(o);
    }
    /// removes the overlay at the current zoom level
    , RemoveTileOverlay: function() {
        if (this.Overlay()) {
            this.GoogleMap().removeOverlay(this.Overlay());
        }
    }
    /// completedCallbackFunction - called after the heatmap is applied.
    , ApplyHeatMap: function(completedCallbackFunction) {
        if (0 < HeatMap.AddressesProcessing) { // retry in a small amount of time if addresses are still being geocoded
            setTimeout("HeatMap.ApplyHeatMap(" + completedCallbackFunction + ")", 250);
            HeatMap.Debug('Delaying ApplyHeatMap - ' + HeatMap.AddressesProcessing + ' addresses still processing');
        } else {
            if (!HeatMap.Overlay()) { 
                HeatMap.AddTileOverlay();
            }
            if (completedCallbackFunction) {
                HeatMap.Debug('HeatMap applied, calling callback function');
                completedCallbackFunction();
            } else {
                HeatMap.Debug('HeatMap applied, no callback defined');
            }
            
        }
    }
    // removes and re adds the tile overlay at the current level
    , RefreshTileOverlay: function() {
        this.RemoveTileOverlay();
        this.AddTileOverlay();
    }
    /// clears overlays at all levels and then applies the heat map for the current level.  Other levels are applied when they are zoomed to
    , ReapplyHeatMap: function() {
        this._overlays = null;
        this.ApplyHeatMap();
    }
    , Initialize: function(divId) {
            var script = document.createElement
            this.LoadGoogle(divId);
            if (this.Settings.DebugMode) {
                $("body").append("<div id='heatMapDebugDiv'><h3>Debug Log</h3></div>");
                this._debugDiv = $("#heatMapDebugDiv");
                //this._debugDiv.draggable();
            }
            this.Debug('HeatMap initialized');
    }
    , LoadGoogle: function(mapDivId) {
        if (GBrowserIsCompatible()) {
            HeatMap.GoogleMap(new GMap2(document.getElementById(mapDivId)));
            HeatMap.GoogleMap().addControl(new GLargeMapControl());
            HeatMap.GoogleMap().addControl(new GMapTypeControl());
            HeatMap.GoogleMap().setCenter(HeatMap.Settings.Center, HeatMap.Settings.DefaultZoom);

            // REMOVED THIS LINE WHEN CREATED APPLYHEATMAPMETHOD
            //HeatMap.AddTileOverlay(HeatMap.GoogleMap());

            GEvent.addListener(HeatMap.GoogleMap(), "zoomend", function(oldLevel, newLevel) {
                HeatMap.ApplyHeatMap();
            });
            $(window).unload(GUnload);
            
        }
        else {
            alert('Your Internet browser is not compatible with this website.');
        }
    }
    , Debug: function(msg, link) {
        if (this.Settings.DebugMode && this._debugDiv) {
            var linkText = '';
            if (link) { linkText = String.format("<a href='{0}' target='_blank' class='heatMapDebug-link'>Link ({1} characters)</a>", link, link.length); }
            var stamp = new Date();
            var stampText = String.format(
                "<span class='heatMapDebug-timestamp'>{0}:{1}:{2}.{3}</span>",
                stamp.getHours(),
                stamp.getMinutes(),
                stamp.getSeconds(),
                stamp.getMilliseconds()
            );
            var msgText = String.format("<span class='heatMapDebug-message'>{0}</span>", msg);

            var output = String.format("{0} {1} {2}<br />", stampText, msgText, linkText);
            this._debugDiv.append(output);
        }
    }
    , GenerateHeatMapTile: function(tileX, tileY, zoomLevel) {
        // create the tile if it does not exist
        var key = String.format("{0},{1},{2}", tileX, tileY, zoomLevel);
        if (!HeatMap.HeatMapTiles[key]) {
            var t = new HeatMapTile(tileX, tileY, zoomLevel);
            HeatMap.HeatMapTiles[key] = t;
        }

        // push the tile to the array to be returned
        return HeatMap.HeatMapTiles[key];
    }
};







/// class HeatMapTile
function HeatMapTile(x, y, zoom) {
    this._tileX = x;
    this._tileY = y;
    this._zoom = zoom;

    this._associatedHeatMapPoints = [];
    this._bounds = new GLatLngBounds();

    var topleftLatLng = HeatMap.Projection().fromPixelToLatLng(new GPoint(x * 256, y * 256), zoom);
    var bottomrightLatLng = HeatMap.Projection().fromPixelToLatLng(new GPoint((x + 1) * 256, (y + 1) * 256), zoom);

    this._bounds.extend(topleftLatLng);
    this._bounds.extend(bottomrightLatLng);
}

HeatMapTile.prototype.X = function(val) { if (val) { this._tileX = val; } return this._tileX; };
HeatMapTile.prototype.Y = function(val) { if (val) { this._tileY = val; } return this._tileY; };
HeatMapTile.prototype.Zoom = function(val) { if (val) { this._zoom = val; } return this._zoom; };
HeatMapTile.prototype.Bounds = function(val) { if (val) { this._bounds = val; } return this._bounds; };
HeatMapTile.prototype.AssociatedHeatMapPoints = function(val) { if (val) { this._associatedHeatMapPoints = val; } return this._associatedHeatMapPoints; };
HeatMapTile.prototype.GetKey = function() {
    if (!this._key) { this._key = String.format("{0},{1},{2}", this.X(), this.Y(), this.Zoom()); }
    return this._key;
};
HeatMapTile.prototype.GetExtendedBounds = function() {
    if (!this._extendedBounds) {
        // get the extreme coordinates of the tile
        var bottomLeftLatLng = this.Bounds().getSouthWest();
        var topRightLatLng = this.Bounds().getNorthEast();

        // convert the coordinates of the tile to points
        var originalBottomLeftPoint = HeatMap.Projection().fromLatLngToPixel(bottomLeftLatLng, this.Zoom());
        var originalTopRightPoint = HeatMap.Projection().fromLatLngToPixel(topRightLatLng, this.Zoom());

        // extend the points by the size of the radius of the HeatPoints
        var extendedBottomLeftPoint = new GPoint(originalBottomLeftPoint.x - HeatMap.Settings.PointRadius, originalBottomLeftPoint.y + HeatMap.Settings.PointRadius);
        var extendedTopRightPoint = new GPoint(originalTopRightPoint.x + HeatMap.Settings.PointRadius, originalTopRightPoint.y - HeatMap.Settings.PointRadius);

        // create the extended bounds
        this._extendedBounds = new GLatLngBounds();
        this._extendedBounds.extend(HeatMap.Projection().fromPixelToLatLng(extendedBottomLeftPoint, this.Zoom()));
        this._extendedBounds.extend(HeatMap.Projection().fromPixelToLatLng(extendedTopRightPoint, this.Zoom()));
    }
    return this._extendedBounds;
};
// returns true if the extended bounds of the tile contain the point
HeatMapTile.prototype.ContainsHeatMapPointInExtendedBounds = function(heatMapPoint) {
    if (this.GetExtendedBounds().containsLatLng(heatMapPoint.GLatLng())) {
        return true;
    }
    return false;
};
HeatMapTile.prototype.ContainsHeatMapPointInBounds = function(heatMapPoint) {
    if (this.Bounds().containsLatLng(heatMapPoint.GLatLng())) {
        return true;
    }
    return false;
};

HeatMapTile.prototype.GetTileUrl = function() {
    ////var startTime = new Date();

    if (!this._tileUrl) {
        if (0 < this.AssociatedHeatMapPoints().length) {
            var data = new StringBuffer();

            ////var startTime2 = new Date();

            for (var i = 0; i < this.AssociatedHeatMapPoints().length; i++) {
                var tileRelativePoint = HeatMap.Projection().fromLatLngToPixel(this.AssociatedHeatMapPoints()[i].GLatLng(), this.Zoom());
                var x = tileRelativePoint.x - (this.X() * 256);
                var y = tileRelativePoint.y - (this.Y() * 256);
                data.Append(x);
                data.Append('x');
                data.Append(y);
                data.Append(',');
            }
            if (0 < data._buffer.length) { data._buffer.pop(); }
            ////var endTime2 = new Date();
            ////var duration2 = endTime2.getTime() - startTime2.getTime();
            ////HeatMap.Debug("    Created data query string parameter.  Duration: " + duration2);
            
            this._tileUrl = String.format(
                "http://localhost/heatmap/images/HeatMapGenerator.ashx?data={0}", 
                data.ToString()
            );
            if (HeatMap.Settings.ShowTileBorder) {
                this._tileUrl += String.format("&{0}={1}", HeatMap.Settings.Keys.ShowTileBorder, HeatMap.Settings.ShowTileBorder);
            }
        } else {
            this._tileUrl = "";
        }
    }
    return this._tileUrl;
};







// class HeatMapPoint
// 
function HeatMapPoint(lat, lng) {
    this._gLatLng = new GLatLng(lat, lng);
    this._associatedTiles = [];
}

/// GLatLng
HeatMapPoint.prototype.GLatLng = function(val) { if (val) { this._gLatLng = val; } return this._gLatLng; };

/// Calculates which tiles this heat point is associated to.
/// Returns the associated tiles.  Does not put them in the associated tiles list
HeatMapPoint.prototype.CalculateTileAssociations = function(zoomLevel) {
    var associatedTiles = [];

    // get the tile coordinates for this point at this zoom level
    var point = HeatMap.Projection().fromLatLngToPixel(this.GLatLng(), zoomLevel);
    var x = Math.floor(point.x / 256);
    var y = Math.floor(point.y / 256);
    var tileRelativeX = point.x % 256;
    var tileRelativeY = point.y % 256;

    // add this tile
    associatedTiles.push(HeatMap.GenerateHeatMapTile(x, y, zoomLevel));

    // check left 3 tiles
    if (tileRelativeX <= HeatMap.Settings.PointRadius) {
        // middle left tile
        associatedTiles.push(HeatMap.GenerateHeatMapTile(x - 1, y, zoomLevel));

        // top left tile
        if (tileRelativeY <= HeatMap.Settings.PointRadius) {
            associatedTiles.push(HeatMap.GenerateHeatMapTile(x - 1, y - 1, zoomLevel));
        }
        // bottom left tile
        if (tileRelativeY >= 256 - HeatMap.Settings.PointRadius) {
            associatedTiles.push(HeatMap.GenerateHeatMapTile(x - 1, y + 1, zoomLevel));
        }
    }
    // check the right 3 tiles
    if (tileRelativeX >= 256 - HeatMap.Settings.PointRadius) {
        // middle right tile
        associatedTiles.push(HeatMap.GenerateHeatMapTile(x + 1, y, zoomLevel));

        // top right tile
        if (tileRelativeY <= HeatMap.Settings.PointRadius) {
            associatedTiles.push(HeatMap.GenerateHeatMapTile(x + 1, y - 1, zoomLevel));
        }
        // bottom right tile
        if (tileRelativeY >= 256 - HeatMap.Settings.PointRadius) {
            associatedTiles.push(HeatMap.GenerateHeatMapTile(x + 1, y + 1, zoomLevel));
        }
    }

    //top middle tile
    if (tileRelativeY <= HeatMap.Settings.PointRadius) {
        associatedTiles.push(HeatMap.GenerateHeatMapTile(x, y - 1, zoomLevel));
    }
    // bottom middle tile
    if (tileRelativeY >= 256 - HeatMap.Settings.PointRadius) {
        associatedTiles.push(HeatMap.GenerateHeatMapTile(x, y + 1, zoomLevel));
    }


    return associatedTiles;
};

/// Calculates and adds associated tiles to this point's associated tiles list.
/// If called a second time, does not reperform the calculations
HeatMapPoint.prototype.ApplyTileAssociations = function(zoomLevel) {
    if (!this._associatedTiles[zoomLevel]) {
        //for (var zoomLevel = HeatMap.Settings.MinZoomLevel; zoomLevel < HeatMap.Settings.MaxZoomLevel; zoomLevel++) {
            this._associatedTiles[zoomLevel] = this.CalculateTileAssociations(zoomLevel);
        //}
    }
};
/// Array of HeatMapTile.
HeatMapPoint.prototype.GetAssociatedTiles = function(zoomLevel) {
    if (!zoomLevel) { throw "zoomLevel was not set in GetAssociatedTiles"; }
    if (!this._associatedTiles[zoomLevel]) {
        this.ApplyTileAssociations(zoomLevel);
    }
    return this._associatedTiles[zoomLevel];
};








// util functions

String.format = function(text) {
    //check if there are two arguments in the arguments list
    if (arguments.length <= 1) {
        //if there are not 2 or more arguments there’s nothing to replace
        //just return the original text
        return text;
    }
    //decrement to move to the second argument in the array
    var tokenCount = arguments.length - 2;
    for (var token = 0; token <= tokenCount; token++) {
        //iterate through the tokens and replace their placeholders from the original text in order
        text = text.replace(new RegExp('\\{' + token + '\\}', 'gi'),
                                                arguments[token + 1]);
    }
    return text;
};


/// StringBuffer
/// usage: 
/// var buf = new StringBuffer();
/// buf.append("Hello");
/// buf.append("World");
/// alert(buf.ToString());
///
/// OUTPUT:
/// HelloWorld
function StringBuffer() {
    this._buffer = [];
};
StringBuffer.prototype.Append = function(string) {
    this._buffer.push(string);
    return this;
};
StringBuffer.prototype.ToString = function() {
    return this._buffer.join('');
};


function hashSize(hash) {
    var length = 0; //hash.length ? --hash.length : -1;
    for (var k in hash) {
        length++;
    }
    return length;
}

// *** Service Calling Proxy Class
if (!this.serviceProxy) {
    this.serviceProxy = function(serviceUrl) {
        var _I = this;
        this.serviceUrl = serviceUrl;
        // *** Call a wrapped object
        this.invoke = function(method, data, callback, error, bare) {
            // *** Convert input data into JSON - REQUIRES Json2.js        
            var json = this.JSON2.stringify(data);
            // *** The service endpoint URL
            var url = _I.serviceUrl + method;
            $.ajax({
                url: url,
                data: json,
                type: "POST",
                processData: false,
                contentType: "application/json",
                timeout: 10000,
                dataType: "text",
                // not "json" we'll parse                    
                success:
            function(res) {
                if (!callback) { return; }
                // *** Use json library so we can fix up MS AJAX dates                        
                var result = this.JSON2.parse(res);
                // *** Bare message IS result                        
                if (bare) {
                    callback(result);
                    return;
                }
                // *** Wrapped message contains top level object node
                // *** strip it off
                for (var property in result) {
                    if (true) {
                        callback(result[property]);
                        break;
                    }
                }
            },
                error: function(xhr) {
                    if (!error) { return; }
                    if (xhr.responseText) {
                        try {
                            var err = this.JSON2.parse(xhr.responseText);
                        } catch (ex) { } //alert(xhr.responseText); }
                        if (err) { error(err); }
                        else { error({ Message: "Unknown server error." }); }
                    }
                    return;
                }
            });
        };
    };
}



/*
http://www.JSON.org/json2.js
2008-03-24

Public Domain.

NO WARRANTY EXPRESSED OR IMPLIED. USE AT YOUR OWN RISK.

See http://www.JSON.org/js.html
    
modified by Rick Strahl to support MS AJAX style
date formats
*/
/*
if (!this.JSON2) {

// Create a JSON object only if one does not already exist. We create the
// object in a closure to avoid global variables.

this.JSON2 = function() {

function f(n) {    // Format integers to have at least two digits.
return n < 10 ? '0' + n : n;
}

//*** RAS - removed date .toJSON for MS Ajax - string double encodes otherwise

//        Date.prototype.toJSON = function () {

//// Eventually, this method will be based on the date.toISOString method.
//              
//              // RAS MODIFIED: Return MS AJAX Style dates
//              var xx = '"\/Date(' + this.getTime() + ')\/"';                                    
//              return xx;
////            return this.getUTCFullYear()   + '-' +
////                 f(this.getUTCMonth() + 1) + '-' +
////                 f(this.getUTCDate())      + 'T' +
////                 f(this.getUTCHours())     + ':' +
////                 f(this.getUTCMinutes())   + ':' +
////                 f(this.getUTCSeconds())   + 'Z';
//        };


var escapeable = /["\\\x00-\x1f\x7f-\x9f]/g,
gap,
indent,
meta = {    // table of character substitutions
'\b': '\\b',
'\t': '\\t',
'\n': '\\n',
'\f': '\\f',
'\r': '\\r',
'"': '\\"',
'\\': '\\\\'
},
rep;


function quote(string) {

// If the string contains no control characters, no quote characters, and no
// backslash characters, then we can safely slap some quotes around it.
// Otherwise we must also replace the offending characters with safe escape
// sequences.

return escapeable.test(string) ?
'"' + string.replace(escapeable, function(a) {
var c = meta[a];
if (typeof c === 'string') {
return c;
}
c = a.charCodeAt();
return '\\u00' + Math.floor(c / 16).toString(16) +
(c % 16).toString(16);
}) + '"' :
'"' + string + '"';
}


function str(key, holder) {

// Produce a string from holder[key].

var i,          // The loop counter.
k,          // The member key.
v,          // The member value.
length,
mind = gap,
partial,
value = holder[key];

// If the value has a toJSON method, call it to obtain a replacement value.

if (value && typeof value === 'object' &&
typeof value.toJSON === 'function') {
value = value.toJSON(key);
}

// If we were called with a replacer function, then call the replacer to
// obtain a replacement value.

if (typeof rep === 'function') {
value = rep.call(holder, key, value);
}

// What happens next depends on the value's type.

switch (typeof value) {

case 'string':
return quote(value);

case 'number':

// JSON numbers must be finite. Encode non-finite numbers as null.

return isFinite(value) ? String(value) : 'null';

case 'boolean':
case 'null':

// If the value is a boolean or null, convert it to a string. Note:
// typeof null does not produce 'null'. The case is included here in
// the remote chance that this gets fixed someday.

return String(value);

// If the type is 'object', we might be dealing with an object or an array or
// null.

case 'object':

// Due to a specification blunder in ECMAScript, typeof null is 'object',
// so watch out for that case.

if (!value) {
return 'null';
}

// *** RAS - MS AJAX style date encoding
if (value.toUTCString) {
var xx = '"\\/Date(' + value.getTime() + ')\\/"';
return xx;
}

// Make an array to hold the partial results of stringifying this object value.

gap += indent;
partial = [];

// If the object has a dontEnum length property, we'll treat it as an array.

if (typeof value.length === 'number' &&
!(value.propertyIsEnumerable('length'))) {

// The object is an array. Stringify every element. Use null as a placeholder
// for non-JSON values.

length = value.length;
for (i = 0; i < length; i += 1) {
partial[i] = str(i, value) || 'null';
}

// Join all of the elements together, separated with commas, and wrap them in
// brackets.

v = partial.length === 0 ? '[]' :
gap ? '[\n' + gap + partial.join(',\n' + gap) +
'\n' + mind + ']' :
'[' + partial.join(',') + ']';
gap = mind;
return v;
}

// If the replacer is an array, use it to select the members to be stringified.

if (typeof rep === 'object') {
length = rep.length;
for (i = 0; i < length; i += 1) {
k = rep[i];
if (typeof k === 'string') {
v = str(k, value, rep);
if (v) {
partial.push(quote(k) + (gap ? ': ' : ':') + v);
}
}
}
} else {

// Otherwise, iterate through all of the keys in the object.

for (k in value) {
if (k) {
v = str(k, value, rep);
if (v) {
partial.push(quote(k) + (gap ? ': ' : ':') + v);
}
}
}
}

// Join all of the member texts together, separated with commas,
// and wrap them in braces.

v = partial.length === 0 ? '{}' :
gap ? '{\n' + gap + partial.join(',\n' + gap) +
'\n' + mind + '}' :
'{' + partial.join(',') + '}';
gap = mind;
return v;
}
}


// Return the JSON object containing the stringify, parse, and quote methods.

return {
stringify: function(value, replacer, space) {

// The stringify method takes a value and an optional replacer, and an optional
// space parameter, and returns a JSON text. The replacer can be a function
// that can replace values, or an array of strings that will select the keys.
// A default replacer method can be provided. Use of the space parameter can
// produce text that is more easily readable.

var i;
gap = '';
indent = '';
if (space) {

// If the space parameter is a number, make an indent string containing that
// many spaces.

if (typeof space === 'number') {
for (i = 0; i < space; i += 1) {
indent += ' ';
}

// If the space parameter is a string, it will be used as the indent string.

} else if (typeof space === 'string') {
indent = space;
}
}

// If there is no replacer parameter, use the default replacer.

if (!replacer) {
rep = function(key, value) {
if (!Object.hasOwnProperty.call(this, key)) {
return undefined;
}
return value;
};

// The replacer can be a function or an array. Otherwise, throw an error.

} else if (typeof replacer === 'function' ||
(typeof replacer === 'object' &&
typeof replacer.length === 'number')) {
rep = replacer;
} else {
throw new Error('JSON.stringify');
}

// Make a fake root object containing our value under the key of ''.
// Return the result of stringifying the value.

return str('', { '': value });
},


parse: function(text, reviver) {

// The parse method takes a text and an optional reviver function, and returns
// a JavaScript value if the text is a valid JSON text.
var j;

function walk(holder, key) {

// The walk method is used to recursively walk the resulting structure so
// that modifications can be made.

var k, v, value = holder[key];
if (value && typeof value === 'object') {
for (k in value) {
if (Object.hasOwnProperty.call(value, k)) {
v = walk(value, k);
if (v !== undefined) {
value[k] = v;
} else {
delete value[k];
}
}
}
}
return reviver.call(holder, key, value);
}


// Parsing happens in three stages. In the first stage, we run the text against
// regular expressions that look for non-JSON patterns. We are especially
// concerned with '()' and 'new' because they can cause invocation, and '='
// because it can cause mutation. But just to be safe, we want to reject all
// unexpected forms.

// We split the first stage into 4 regexp operations in order to work around
// crippling inefficiencies in IE's and Safari's regexp engines. First we
// replace all backslash pairs with '@' (a non-JSON character). Second, we
// replace all simple value tokens with ']' characters. Third, we delete all
// open brackets that follow a colon or comma or that begin the text. Finally,
// we look to see that the remaining characters are only whitespace or ']' or
// ',' or ':' or '{' or '}'. If that is so, then the text is safe for eval.

if (/^[\],:{}\s]*$/.test(text.replace(/\\["\\\/bfnrtu]/g, '@').
replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

// In the second stage we use the eval function to compile the text into a
// JavaScript structure. The '{' operator is subject to a syntactic ambiguity
// in JavaScript: it can begin a block or an object literal. We wrap the text
// in parens to eliminate the ambiguity.


// *** RAS Update:  Fix up Dates: ISO and MS AJAX format support
//var regEx = /(\"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}.*?\")|(\"\\*\/Date\(.*?\)\\*\/")/g;
var regEx = /(\"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}.*?\")|(\"\\*\/Date\(.*?\)\\*\/")/g;
text = text.replace(regEx, this.regExDate);
// *** End RAS Update

j = eval('(' + text + ')');

// In the optional third stage, we recursively walk the new structure, passing
// each name/value pair to a reviver function for possible transformation.

return typeof reviver === 'function' ?
walk({ '': j }, '') : j;
}

// If the text is not JSON parseable, then a  is thrown.
throw new SyntaxError('JSON.parse');
},
// *** RAS Update: RegEx handler for dates ISO and MS AJAX style
regExDate: function(str, p1, p2, offset, s) {
str = str.substring(1).replace('"', '');
var date = str;

// MS Ajax date: /Date(19834141)/
if (/\/Date(.*)\//.test(str)) {
str = str.match(/Date\((.*?)\)/)[1];
date = "new Date(" + parseInt(str, 10) + ")";
}
else { // ISO Date 2007-12-31T23:59:59Z                                     
var matches = str.split(/[\-,:,T,Z]/);
matches[1] = (parseInt(matches[1], 0) - 1).toString();
date = "new Date(Date.UTC(" + matches.join(",") + "))";
}
return date;
},

quote: quote
};
} ();
}

*/