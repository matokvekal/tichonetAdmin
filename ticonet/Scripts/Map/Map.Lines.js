smap.lines = {
    list: [],
    ttGrid: null,
    durations: null,
    notSaveGeometry: false,
    preSwitch: function (id) {
        var sel = $("input[ref=cbLn][rel=" + id + "]").prop("checked");
        if (sel) {
            smap.lines.showLine(id);
        } else {
            smap.lines.hideLine(id);
        }
    },
    updateLine: function (line, show) {

        line.show = (show == true);
        if (show == true) smap.lines.hideLine(line.Id);

        var oldline = smap.getLine(line.Id);
        if (oldline) {
            var index = smap.lines.list.indexOf(oldline);
            smap.lines.list[index] = line;
            smap.table.linesGrid.setRowData(line.Id, line);
            //smap.ways = null; //TODO: Redid for save old ways
        } else {
            smap.lines.list.push(line);
            smap.table.linesGrid.jqGrid('addRowData', line.Id, line);
        }
        if (show == true) smap.lines.showLine(line.Id);

    },
    showLine: function (id) {

        var line = smap.getLine(id);

        if (!(line.Stations)) return;
        if (line.Stations.length < 2) return;
        line.show = true;

        smap.lines.calcLine(id, function (line) {
            for (var i in line.ways) {
                line.ways[i].display.setMap(smap.mainMap);
            }
            smap.lines.overlay.showLineOverlay();
        });
       
    },
    showSegment: function (line) {
        var st1 = smap.stations.getStation(line.currentStationsList[0]);
        var st2 = smap.stations.getStation(line.currentStationsList[1]);


        if (smap.directionsService == null) smap.directionsService = new google.maps.DirectionsService();


        var rendererOptions = smap.lines.getRenderOptions(line);
        if (line.gDirectionsDisplay == null) line.gDirectionsDisplay = new google.maps.DirectionsRenderer(rendererOptions);
        line.gDirectionsDisplay.setMap(smap.mainMap);

        google.maps.event.addListener(line.gDirectionsDisplay, "directions_changed", function (result) {
            console.log(line.gDirectionsDisplay.getDirections());
        });


        var request = {
            origin: new google.maps.LatLng(st1.StrLat, st1.StrLng),
            destination: new google.maps.LatLng(st2.StrLat, st2.StrLng),
            travelMode: google.maps.DirectionsTravelMode.DRIVING,
            transitOptions: { modes: ["BUS"] }
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
                if (smap.lines.durations == null) { //draw if not durations calculation
                    line.gDirectionsDisplay.setDirections(line.route);
                } else {
                    var d = 0;
                    for (var i = 0; i < response.routes[0].legs.length; i++) {
                        d += response.routes[0].legs[i].duration.value;
                    }

                    smap.lines.durations.Durations.push({
                        StationId: line.currentStationsList[0],
                        Duration: d
                    });
                }

            } else {
                //var d = google.maps.geometry.spherical.computeDistanceBetween(addr1, addr2);
                //$("#dAttachDist").html("Distance " + d + "m (directly)");
            }
            line.currentStationsList.splice(0, 1);
            if (line.currentStationsList.length >= 2) {
                smap.lines.showSegment(line);
            } else {
                if (smap.lines.durations != null) {
                    smap.lines.saveDurations();
                }
            }
        });
    },
    getRenderOptions: function (line) {
        var rendererOptions = {
            draggable: true,
            hideRouteList: true,
            preserveViewport: true,
            markerOptions: {
                visible: false
            },
            polylineOptions: {
                strokeColor: smap.lines.getColor(line.Id)
            }
        };
        return rendererOptions;
    },
    addDirectionChangedListener: function (way, line) {
        google.maps.event.addListener(way.display, "directions_changed", function () {


            for (var j in line.ways) {
                if (line.ways[j].path) {
                    var d = line.ways[j].display.getDirections()
                    var dur = 0;
                    var dist = 0;
                    for (var z in d.routes[0].legs) {
                        dist += d.routes[0].legs[z].distance.value;
                        dur += d.routes[0].legs[z].duration.value;
                    }

                    line.ways[j].path = { routes: [d.routes[0]], request: d.request };                  
                    line.ways[j].distance = dist;
                    line.ways[j].duration = dur;
                }
            }
            if (!smap.lines.notSaveGeometry) smap.lines.saveGeometry(line);
        });
    },
    calcValues: null,
    calcLine: function (id, callback) {
        var line = smap.getLine(id);
        if (line.ways == null) line.ways = [];
        if (!(line.Stations)) return;
        if (line.Stations.length < 2) return;



        var rendererOptions = smap.lines.getRenderOptions(line);
        for (var i = 0; i < line.Stations.length - 1; i++) {
            var oldWay = null;
            for (var j in line.ways) {
                if (line.ways[j].startStationId == line.Stations[i].StationId &&
                    line.ways[j].endStationId == line.Stations[i + 1].StationId) {
                    oldWay = line.ways[j];
                    break;
                }
            }
            if (oldWay == null) {
                var way = {
                    startStationId: line.Stations[i].StationId,
                    endStationId: line.Stations[i + 1].StationId,
                    position: i,
                    display: new google.maps.DirectionsRenderer(rendererOptions)
                };
                line.ways.push(way);
                smap.lines.addDirectionChangedListener(way, line);
            } else {
                oldWay.position= i;
            }
        }
        for (var i = 0; i < line.ways.length;) { //remove ways with incorrect stations
            if (smap.lines.checkStationsInLine(line.Id, [line.ways[i].startStationId, line.ways[i].endStationId])) {
                i++;
            }
            else {
                console.log("Remove way from station " + line.ways[i].startStationId + " to station " + line.ways[i].endStationId);
                line.ways[i].display.setMap(null);
                line.ways.splice(i, 1);
            }
        }

        line.ways.sort(function (a, b) {
            var compA = a.position;
            var compB = b.position;
            return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
        });

        smap.lines.calcValues = {
            line: line,
            index: 0,
            callback: callback
        };
        smap.lines.calcSegment();
    },
    calcSegment() {
        if (smap.lines.calcValues == null) return;
        var i = smap.lines.calcValues.index;
        var line = smap.lines.calcValues.line;
        var callback = smap.lines.calcValues.callback;


        if (line.ways[i].path) {// do not calc
            if ((i + 1) < line.ways.length) {
                smap.lines.calcValues = {
                    line: line,
                    index: i + 1,
                    callback: callback
                };
                smap.lines.calcSegment();
            }
            else {
                callback(line);
            }
            return;
        }
        var st1 = smap.stations.getStation(line.ways[i].startStationId);
        var st2 = smap.stations.getStation(line.ways[i].endStationId);

        if (smap.directionsService == null) smap.directionsService = new google.maps.DirectionsService();

        var request = {
            origin: new google.maps.LatLng(st1.StrLat, st1.StrLng),
            destination: new google.maps.LatLng(st2.StrLat, st2.StrLng),
            travelMode: google.maps.DirectionsTravelMode.DRIVING,
            transitOptions: { modes: ["BUS"] }
        };

        smap.directionsService.route(request, function (response, status) {

            if (status == "OK") {

                var route = response.routes[0];

                var dur = 0;
                var dist = 0;
                for (var z in route.legs) {
                    dist += route.legs[z].distance.value;
                    dur += route.legs[z].duration.value;
                }

                line.ways[i].path = { routes: [route], request: request };

                line.ways[i].distance = dist;
                line.ways[i].duration = dur;

                line.ways[i].display.setDirections(line.ways[i].path);
                if (line.show) line.ways[i].display.setMap(smap.mainMap);

                if ((i + 1) < line.ways.length) {

                    smap.lines.calcValues = {
                        line: line,
                        index: i + 1,
                        callback: callback
                    };
                    window.setTimeout("smap.lines.calcSegment();", 500);

                }
                else {
                    callback(line);
                }
            }
            else {
                if (status == "OVER_QUERY_LIMIT") {
                    window.setTimeout("smap.lines.calcSegment();", 5000);
                }
                else {
                    console.log("Navigation error: " + status);
                }
            }
        });
    },
    saveGeometry: function (line) {
        var data = [];
        for (var i in line.ways) {
            var way = line.ways[i];
            data.push({
                path: way.path,
                startStationId: way.startStationId,
                endStationId: way.endStationId,
                distance: way.distance,
                duration: way.duration,
                position: way.position
            });
        }
        // console.log(data);
        $.post("/api/map/SaveGeometry", { Id: line.Id, Data: escape(JSON.stringify(data)) });

    },
    hideLine: function (id) {

        var line = smap.getLine(id);
        if (line != null) {
            line.show = false;
            for (var i in line.ways) {
                line.ways[i].display.setMap(null);
            }
            smap.lines.overlay.showLineOverlay();
        }
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
    },
    editLine: function (id) {
        smap.closeConextMenu();
        $("#frmEditLine")[0].reset();
        $("#hfEditLineId").val(id);
        var color = '#00FF00';
        if (id != 0) {
            $("#dlgAddLine").attr("title", "Edit line");
            var line = smap.getLine(id);
            $("#tbEditLineNumber").val(line.LineNumber);
            $("#tbEditLineName").val(line.Name);
            $("#ddlEditLineDirection").val(line.Direction);
            color = line.Color;
        } else {
            $("#dlgAddLine").attr("title", "Add new line");
        }
        $("#hfEditLineColor").val(color);
        $("#tbEditLineColor").spectrum({
            color: color,
            change: function (color) {
                $("#hfEditLineColor").val(color.toHexString());
            }
        });


        var dialog = $("#dlgAddLine").dialog({
            autoOpen: true,
            width: 300,
            modal: true,
            buttons: {
                "Save": function () {
                    var data = $("#frmEditLine").serialize();
                    $.post("/api/map/SaveLine", data).done(function (loader) {
                        smap.lines.updateLine(loader.Line, true);
                        for (var i = 0; i < loader.Stations.length; i++) {
                            smap.stations.updateStation(loader.Stations[i]);
                        }
                        for (var j in loader.Students) {
                            smap.updateStudent(loader.Students[j]);
                        }
                    });
                    dialog.dialog("close");
                },
                cancel: function () {
                    dialog.dialog("close");
                }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    deleteLine: function (id) {
        var line = smap.getLine(id);
        $("#dConfirmMessage").html("Do you want delete line '" + line.Name + "' ?");
        $("#hfCurrentId").val(line.Id);
        var dialog = $("#dlgConfirm").dialog({
            autoOpen: true,
            width: 350,
            modal: true,
            buttons: {
                "Yes": function () {
                    $.post("/api/map/deleteLine/" + line.Id, null).done(function (loader) {
                        if (loader.Done == true) {
                            var line = smap.getLine(loader.Line.Id);
                            if (line != null) {
                                smap.lines.hideLine(line.Id);
                                var index = smap.lines.list.indexOf(line);
                                smap.lines.list.splice(index, 1);
                            }
                            smap.table.linesGrid.jqGrid('delRowData', loader.Line.Id);
                        }
                    });
                    dialog.dialog("close");
                },
                Cancel: function () {

                    dialog.dialog("close");
                }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    saveLineAcive: function (id) {
        var ctrl = $("input[ref=lnActive][rel=" + id + "]").prop("checked");
        $.post("/api/map/LineActiveSwitch", { LineId: id, Active: ctrl }).done(function (loader) {
            if (loader.Done == true) {
                var ln = smap.getLine(loader.LineId);
                ln.Active = loader.Active;
            }
        });
    },
    showTimeTable: function (id) {
        var ln = smap.getLine(id);
        if (ln.Stations.length == 0) {
            $("#dConfirmMessage").html("Line " + ln.LineNumber + "has no stations");
            var dlgInfo = $("#dlgConfirm").dialog({
                autoOpen: true,
                width: 350,
                modal: true,
                buttons: {
                    Cancel: function () {
                        dlgInfo.dialog("close");
                    }
                }
            });
            return;
        }
        $("#hfTTLineId").val(ln.Id);
        var mess = "";
        var st;
        if (ln.Direction == 0) {
            st = ln.Stations[ln.Stations.length - 1];

            mess = "Arrival to " + smap.stations.getStation(st.StationId).Name + " at";

        } else {
            st = ln.Stations[0];
            mess = "Departure from " + smap.stations.getStation(st.StationId).Name + " at";
        }

        var tparts = st.ArrivalDateString.split(":");
        $("#tbTTHours").val(tparts[0]);
        $("#tbTTMinutes").val(tparts[1]);

        $("#dTTMessage").html(mess);
        $("#dlgTimeTable").dialog({
            autoOpen: true,
            width: 450,
            modal: true
        });
        if (smap.lines.ttGrid == null) {
            smap.lines.ttGrid = $("#tblTTGrid").jqGrid({
                datatype: 'local',
                colNames: ['Position', 'Station', 'Time'],
                colModel: [
                    { name: 'Position', width: 100, align: 'center' },
                    { name: 'StationId', width: 200, formatter: smap.table.stationNameFormatter },
                    { name: 'ArrivalDateString', width: 100, align: 'center' }
                ]
            });
        }
        smap.lines.ttGrid.jqGrid("clearGridData");
        for (var i in ln.Stations) {
            smap.lines.ttGrid.jqGrid("addRowData", ln.Stations[i].Id, ln.Stations[i]);
        }
    },
    startReCalcimeTable: function (lineId) {
        $("#hfTTLineId").val(lineId);
        smap.lines.reCalcTimeTable();
    },
    reCalcTimeTable: function () {
        var lnId = $("#hfTTLineId").val();
        var ln = smap.getLine(lnId);
        smap.lines.durations = {
            FirstStation: $("#tbTTHours").val() + ":" + $("#tbTTMinutes").val(),
            LineId: lnId,
            Durations: []
        };
        for (var i in ln.ways) {
            smap.lines.durations.Durations.push({
                StationId: ln.ways[i].startStationId,
                Duration: ln.ways[i].duration
            });
        }
        smap.lines.saveDurations();
        //$("#spStatus").html("Update time table for line " + ln.LineNumber);
        //smap.lines.showLine(lnId, false);

    },
    saveDurations: function () {
        $.post("/api/map/SaveDurations", smap.lines.durations).done(function (loader) {
            smap.restoryWays(loader);
            console.log(loader);
            smap.lines.updateLine(loader, true);
            if (smap.lines.ttGrid != null)
                for (var i in loader.Stations) {
                    var st = loader.Stations[i];
                    smap.lines.ttGrid.setRowData(st.Id, st);
                }
            $("#spStatus").html("");
        });
        smap.lines.durations = null;

        //console.log(smap.lines.durations);
    },
    lineStationsVisibleSwitch: function (id) {
        var c = "glyphicon-eye-close";
        var o = "glyphicon-eye-open";
        var btn = $("span[rel=lsswitch][ref=" + id + "]");
        var ln = smap.getLine(id);
        var h = false; // hide = true, show =false
        if ($(btn).hasClass(o)) {
            //hide
            $(btn).removeClass(o);
            $(btn).addClass(c);
            h = true;
        } else {
            //show
            h = false;
            $(btn).removeClass(c);
            $(btn).addClass(o);
        }
        for (var i1 in ln.Stations) {
            var st1 = smap.stations.getStation(ln.Stations[i1].StationId);
            if (h) {
                if (st1.Type == 0 && st1.Marker != null) {
                    st1.Marker.setMap(null);
                    st1.Marker = null;
                }
            } else {
                if (st1.Marker == null) {
                    smap.stations.setMarker(st1);
                }
            }

        }
    },
    checkStationsInLine: function (lineId, stationIds) {// check line have all stations in list
        var line = smap.getLine(lineId);
        var res = 0;
        for (var i in stationIds) {
            for (var j in line.Stations) {
                if (line.Stations[j].StationId == stationIds[i]) res++;
            }
        }
        return res == stationIds.length;
    }
}