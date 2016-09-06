smap.lines.overlay = {
    showLineOverlay: function () {
        if (smap.overlay) {
            smap.overlay.setMap(null);
            smap.overlay = null;
        }
        
        var bounds = new google.maps.LatLngBounds();
        for (var i in smap.stations.list) {
            var st = smap.stations.list[i];
            if (st.Marker) {
                bounds.extend(st.Marker.getPosition());
            }
        }
        smap.overlay = new LineOverlay(bounds, smap.mainMap);
    }
}

//Type definition
LineOverlay.prototype = new google.maps.OverlayView();

/** @constructor */
function LineOverlay(bounds, map) {

    // Initialize all properties.
    this.bounds_ = bounds;
    this.map_ = map;
    this.labels_ = [];
    // Define a property to hold the image's div. We'll
    // actually create this div upon receipt of the onAdd()
    // method so we'll leave it null for now.
    this.div_ = null;

    // Explicitly call setMap on this overlay.
    this.setMap(map);
}

LineOverlay.prototype.onAdd = function () {

    // Note: an overlay's receipt of onAdd() indicates that
    // the map's panes are now available for attaching
    // the overlay to the map via the DOM.


    // We add an overlay to a map via one of the map's panes.
    // We'll add this overlay to the overlayLayer pane.
    var panes = this.getPanes();


    for (var i in smap.stations.list) {
        var st = smap.stations.list[i];
        if (st.Marker) {
            var lines = smap.stations.getLines(st.Id);
            var divs = [];
            if (lines.length > 0) {
                for (var l in lines) {
                    var line = lines[l];
                    if (line.show) {
                        var sti = null;
                        for (var s in line.Stations) {
                            if (line.Stations[s].StationId == st.Id) sti = line.Stations[s];
                        }
                        if (sti != null) {
                            var ldiv = document.createElement('div');
                            var snum = document.createElement('span');
                            snum.innerHTML = line.LineNumber;

                            var cdiv = document.createElement('div');
                            cdiv.style.backgroundColor = smap.fixCssColor(line.Color);
                            cdiv.style.width = "10px";
                            cdiv.style.height = "10px";
                            cdiv.style.display = "inline-block";

                            var stime = document.createElement('span');
                            stime.innerHTML = "(" + sti.Position + ")&nbsp; - &nbsp;" + sti.ArrivalDateString;
                            //ldiv.innerHTML =  + " > " + sti.ArrivalDateString
                            ldiv.appendChild(snum);
                            ldiv.appendChild(cdiv);
                            ldiv.appendChild(stime);

                            divs.push(ldiv);
                        }

                    }
                }
            }
            if (divs.length > 0) {
                var div = document.createElement('div');
                $(div).addClass('st-label');             
                
                for (var d in divs) {
                    div.appendChild(divs[d]);
                }

                this.labels_.push({
                    station: st,
                    div: div
                });
                panes.overlayLayer.appendChild(div);
            }


        }
    }
}

LineOverlay.prototype.draw = function () {

    
    var overlayProjection = this.getProjection();

    for (var i in this.labels_) {
        var lb = this.labels_[i];
        var p = overlayProjection.fromLatLngToDivPixel(lb.station.Marker.getPosition());
        lb.div.style.left = p.x + 'px';
        lb.div.style.top = p.y + 'px';
    }
    if (smap.mainMap.getZoom() <= 10) {
        this.hide();
    } else {
        this.show();
    }

}

// The onRemove() method will be called automatically from the API if
// we ever set the overlay's map property to 'null'.
LineOverlay.prototype.onRemove = function () {
    for (var i in this.labels_) {
        var lb = this.labels_[i];
        lb.div.parentNode.removeChild(lb.div);
        lb.div = null;
    }
};

// Set the visibility to 'hidden' or 'visible'.
LineOverlay.prototype.hide = function () {
    for (var i in this.labels_) {
        var lb = this.labels_[i];
        if (lb.div) {
            // The visibility property must be a string enclosed in quotes.
            lb.div.style.visibility = 'hidden';
        }
    }

};

LineOverlay.prototype.show = function () {
    for (var i in this.labels_) {
        var lb = this.labels_[i];
        if (lb.div) {
            lb.div.style.visibility = 'visible';
        }
    }
};

LineOverlay.prototype.toggle = function () {
    if (this.labels_.length == 0) return;
    if (this.labels_[0].div.style.visibility === 'hidden') {
        this.show();
    } else {
        this.hide();
    }
};

// Detach the map from the DOM via toggleDOM().
// Note that if we later reattach the map, it will be visible again,
// because the containing <div> is recreated in the overlay's onAdd() method.
LineOverlay.prototype.toggleDOM = function () {
    if (this.getMap()) {
        // Note: setMap(null) calls OverlayView.onRemove()
        this.setMap(null);
    } else {
        this.setMap(this.map_);
    }
};