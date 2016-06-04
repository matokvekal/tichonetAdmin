smap.lines = {
    list: [],
    preSwitch: function (id) {
        var sel = $("input[ref=cbLn][rel=" + id + "]").prop("checked");
        if (sel) {
            smap.lines.showLine(id);
        } else {
            smap.lines.hideLine(id);
        }
    },
    showLine: function (id) {
        var line = smap.getLine(id);
        if (line.Stations.length < 2) return;
        line.route = null;
        line.gDirectionsDisplay = null;
        line.currentStationsList = [];
        for (var i = 0; i < line.Stations.length; i++) {
            line.currentStationsList.push(line.Stations[i].StationId);
        }
        
        smap.lines.showSegment(line);
    },
    showSegment: function (line) {
        var st1 = smap.stations.getStation(line.currentStationsList[0]);
        var st2 = smap.stations.getStation(line.currentStationsList[1]);


        if (smap.directionsService == null) smap.directionsService = new google.maps.DirectionsService();


        var rendererOptions = {
            draggable: false,
            hideRouteList: true,
            preserveViewport: true,
            markerOptions: {
                visible: false
            },
            polylineOptions: {
                strokeColor: smap.lines.getColor(line.Id)
            }
        };
        if (line.gDirectionsDisplay == null) line.gDirectionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);
        line.gDirectionsDisplay.setMap(smap.mainMap);

        var request = {
            origin: new google.maps.LatLng(st1.StrLat, st1.StrLng),
            destination: new google.maps.LatLng(st2.StrLat, st2.StrLng),
            travelMode: google.maps.DirectionsTravelMode.DRIVING
        };
        smap.directionsService.route(request, function (response, status) {

            if (status == google.maps.DirectionsStatus.OK) {
               
                if (line.route) {
                    for (var i = 0; i < response.routes[0].legs.length; i++) {
                        line.route.routes[0].legs.push(response.routes[0].legs[i]);
                    }
                } else {
                    line.route = response;
                }
                line.gDirectionsDisplay.setDirections(line.route);

            } else {
                //var d = google.maps.geometry.spherical.computeDistanceBetween(addr1, addr2);
                //$("#dAttachDist").html("Distance " + d + "m (directly)");
            }
            line.currentStationsList.splice(0, 1);
            if (line.currentStationsList.length >= 2) smap.lines.showSegment(line);
        });
    },
    hideLine: function (id) {
        var line = smap.getLine(id);
        line.gDirectionsDisplay.setMap(null);
    },
    getColor: function (id) {
        var line = smap.getLine(id);
        if (line) {
            var color = line.Color;
            if (color.length < 3) return "#0000FF";
            if (color.substring(0, 1) != "#") color = "#" + color;
            return color;
        } else {
            return "#0000FF";
        }
    }

}