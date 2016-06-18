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
    updateLine: function (line, show) {
        line.show = (show == true);
        if (show == true) smap.lines.hideLine(line.Id);
        var oldline = smap.getLine(line.Id);
        if (oldline) {
            var index = smap.lines.list.indexOf(oldline);
            smap.lines.list[index] = line;
            smap.table.linesGrid.setRowData(line.Id, line);
        } else {
            //console.log("Add");
            //console.log(line);
            smap.lines.list.push(line);
            smap.table.linesGrid.jqGrid('addRowData', line.Id, line);
        }
        if (show == true) smap.lines.showLine(line.Id);

    },
    showLine: function (id) {
        //console.log(id);
        var line = smap.getLine(id);
        //console.log(line);
        if (!(line.Stations)) return;
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
        if (line != null)
            if (line.gDirectionsDisplay != null)
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
                                smap.lines.list.splice(index,1);
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
        $.post("/api/map/LineActiveSwitch", { LineId: id, Active: ctrl }).done(function(loader) {
            if (loader.Done == true) {
                var ln = smap.getLine(loader.LineId);
                ln.Active = loader.Active;
            }
        });
    }
}