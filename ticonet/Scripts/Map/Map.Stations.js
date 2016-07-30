smap.stations = {
    defaultColor: "#FF0000", //default station color
    list: [],//List of stations
    SearchBox: null,
    latestLineId: null,
    load: function () { //Loading exists stations from DB
        $.get("/api/stations/List").done(function (loader) {
            for (var i = 0; i < loader.length; i++) {
                var stt = loader[i];
                if (smap.stations.getStation(stt.Id) == null) {
                    smap.stations.list.push(stt);
                }
                smap.stations.setMarker(stt);
            }
        });
    },
    openPopup: function (id, lat, lng) { //Open dialog for create / edit station
        smap.closeConextMenu();
        var sColor = smap.getRandomColor();
        var bSaveName = "";
        if (id == null) { //Add new station
            $("#hfCreateLat").val(lat);
            $("#hfCreateLng").val(lng);
            $("#hfStationId").val(0);
            $("#tbName").val("New station");
            bSaveName = "Add";
            $("#dCSAddrBlock").css("display", "block");
            $("#rCSThis").prop("checked", true);
            $("#tbCSAddress").val("");
            $("#cbCSSchool").prop("checked", false);

            //address search box
            smap.stations.resetSearchBox();
            smap.stations.SearchBox = new google.maps.places.SearchBox(document.getElementById("tbCSAddress"), {
                bounds: smap.mainMap.getBounds()
            });
            smap.stations.SearchBox.addListener('places_changed', function () {
                smap.Geocoder.geocode({ 'address': $("#tbCSAddress").val() }, function (results1, status1) {
                    smap.stations.resetSearchBox();

                    if (status1 == google.maps.GeocoderStatus.OK) {
                        $("#spCSSucIcon").css("display", "inline");
                        $("#dCSControl").addClass("has-success");
                        var loc = results1[0].geometry.location;
                        $("#hfCSSelectedLat").val(loc.lat());
                        $("#hfCSSelectedLng").val(loc.lng());
                    } else {
                        $("#spCSWarnIcon").css("display", "inline");
                        $("#dCSControl").addClass("has-warning");
                    }
                });
            });
        } else {
            var st = smap.stations.getStation(id);
            $("#hfStationId").val(id);
            $("#tbName").val(st.Name);
            $("#hfCreateLat").val(st.StrLat);
            $("#hfCreateLng").val(st.StrLng);
            sColor = st.Color;
            $("#dCSAddrBlock").css("display", "none");
            $("#rCSThis").prop("checked", true);
            $("#tbCSAddress").val(st.Address);
            $("#cbCSSchool").prop("checked", (st.Type == 1));
            bSaveName = "Save";
        }




        var dialog = $("#dialog-form").dialog({
            autoOpen: true,
            width: 400,
            modal: true,
            buttons: {
                "Save": function () {
                    var t = 0;
                    if ($("#cbCSSchool").prop("checked") == true) t = 1;
                    var lat = $("#hfCreateLat").val();
                    var lng = $("#hfCreateLng").val();
                    if ($("#rCSAddr").prop("checked") == true) {
                        console.log("Pos from ad");
                        lat = $("#hfCSSelectedLat").val();
                        lng = $("#hfCSSelectedLng").val();
                    }
                    var data = {
                        Id: $("#hfStationId").val(),
                        Name: $("#tbName").val(),
                        Color: $("#hfCreateColor").val(),
                        StrLat: lat,
                        StrLng: lng,
                        Address: $("#tbCSAddress").val(),
                        Type: t
                    };
                    $.post("/api/stations/Save", data)
                        .done(function (loader) {
                            var stt = loader.Data.Station;
                            smap.stations.updateStation(stt);

                            for (var i = 0; i < stt.Students.length; i++) {
                                var student = smap.getStudent(stt.Students[i].StudentId);
                                student.Color = stt.Color;
                                smap.setMarker(student);
                            }
                            dialog.dialog("close");
                        });
                },
                Cancel: function () {
                    dialog.dialog("close");
                }
            }
        });
        $("#hfCreateColor").val(sColor);
        $("#tbColor").spectrum({
            color: sColor,
            change: function (color) {
                $("#hfCreateColor").val(color.toHexString());
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");

    },
    getStation: function (id) { //Get station object from list by Id
        var res = null;
        for (var i = 0; i < smap.stations.list.length; i++) {
            if (smap.stations.list[i].Id == id) {
                res = smap.stations.list[i];
                break;
            }
        }
        return res;
    },
    getMarkerIcon: function (station) {
        var color = station.Color;
        if (color == null || color == undefined) color = "FF0000";
        if (color.length < 3) color = "FF0000";
        if (color.substring(0, 1) == "#") {
            color = color.substring(1, color.length);

        }
        var t = 0;
        if (station.Type) t = station.Type;
        return "/icons/StationIcon?color=" + color + "&type=" + t;
    },
    getLines: function (stationId) {
        var res = [];
        for (var i = 0; i < smap.lines.list.length; i++) {
            var line = smap.lines.list[i];
            for (var j = 0; j < line.Stations.length; j++) {
                if (line.Stations[j].StationId == stationId) res.push(line);
            }
        }
        return res;
    },
    setMarker: function (station) { //Add or move station marker
        var myLatlng = new google.maps.LatLng(station.StrLat, station.StrLng);

        if (station.Marker) {
            //Move marker
            station.Marker.setPosition(myLatlng);
            station.Marker.setIcon(smap.stations.getMarkerIcon(station));
            station.Marker.setTitle(station.Name);
        } else {
            //Add maker
            station.Marker = new google.maps.Marker({
                position: myLatlng,
                map: smap.mainMap,
                draggable: true,
                icon: smap.stations.getMarkerIcon(station),
                title: station.Name,
                station: station
            });
            // Handle events
            google.maps.event.addListener(station.Marker, "rightclick", function (event) { smap.stations.showStationContextMenu(event.latLng, station); });
            google.maps.event.addListener(station.Marker, "click", function (event) { smap.closeConextMenu(); });
            google.maps.event.addListener(station.Marker, "dragend", function (event) {

                smap.stations.moveStation(station);
            });
            google.maps.event.addListener(station.Marker, "dblclick", function (event) {
                var stt = smap.stations.getStation(station.Id);
                for (var i = 0; i < smap.students.length; i++) {
                    var st = smap.students[i];
                    if (st.IW != null) {
                        st.IW.close();
                    }
                }
                for (var i in smap.stations.list) {
                    var st = smap.stations.list[i];
                    if (st.IW != null) {
                        st.IW.close();
                        st.IW = null;
                    }
                }
                smap.clearGraphic();

                if (stt.IW != null) {
                    stt.IW.close();
                }
                var content = "<h4 style='margin-bottom:2px;'>" + stt.Name + "</h4>";
                content += "<div style='margin-bottom:5px;'>Total students: <b>" + stt.Students.length.toString() + "</b></div>";
                var lines = [];
                for (var l in smap.lines.list) {
                    var ln = smap.lines.list[l];
                    for (var m in ln.Stations) {
                        if (ln.Stations[m].StationId == stt.Id) lines.push(ln.Stations[m]);
                    }
                }

                if (lines.length > 0) {
                    content += "<table class='table table-striped lines-table'>";
                    content += "<tr>";
                    content += "<td>Line</td>";
                    content += "<td>Arrival</td>";
                    content += "<td>Order</td>";
                    content += "</tr>";
                    for (var k in lines) {
                        var line = smap.getLine(lines[k].LineId);
                        console.log(line);
                        content += "<tr>";
                        content += "<td>" + line.LineNumber + "</td>";
                        content += "<td>" + lines[k].ArrivalDateString + "</td>";
                        content += "<td>" + lines[k].Position + "</td>";
                        content += "</tr>";
                    }
                    content += "</table>";
                } else {
                    content += "<div style='text-align:center;'>No lines</div>";
                }
                var infowindow = new google.maps.InfoWindow({
                    content: content
                });
                stt.IW = infowindow;
                infowindow.open(smap.mainMap, stt.Marker);

                for (var j in stt.Students) {
                    var stud = smap.getStudent(stt.Students[j].StudentId);
                    smap.drawLine(stud.Lat, stud.Lng, stt.StrLat, stt.StrLng, false);
                }

                //handle closing info window for hide lines
                stt.IW.addListener('closeclick', function () {
                    smap.clearGraphic();
                    stt.IW = null;
                });

            });
        }
    },
    showStationContextMenu: function (currentLatLng, station) {// Open station context menu
        smap.closeConextMenu();
        var contextmenuDir = document.createElement("div");
        contextmenuDir.className = 'contextmenu';
        contextmenuDir.innerHTML = '<a id="menuST1" href="javascript:smap.stations.openPopup(' + station.Id + ');"><div class="context">Edit<\/div><\/a>';
        contextmenuDir.innerHTML += '<a id="menuST2" href="javascript:smap.stations.deleteStation(' + station.Id + ');"><div class="context">Delete<\/div><\/a>';
        contextmenuDir.innerHTML += '<hr  class="context" style="margin-top:0px;margin-bottom:0px;"/>';
        contextmenuDir.innerHTML += '<a id="menuST3" href="javascript:smap.stations.addToLine(' + station.Id + ');"><div class="context">Add to Line<\/div><\/a>';
        if (smap.stations.getLines(station.Id).length > 0) {
            contextmenuDir.innerHTML += '<a id="menuST3" href="javascript:smap.stations.editToLine(' + station.Id + ');"><div class="context">Edit Line<\/div><\/a>';
            contextmenuDir.innerHTML += '<a id="menuST4" href="javascript:smap.stations.deleteFromLines(' + station.Id + ');"><div class="context">Remove from line<\/div><\/a>';
        }


        $(smap.mainMap.getDiv()).append(contextmenuDir);

        smap.setMenuXY(currentLatLng);

        contextmenuDir.style.visibility = "visible";
    },
    moveStation: function (station) {
        $("#dConfirmMessage").html("Do you want move station '" + station.Name + "' to new place?");
        $("#hfCurrentId").val(station.Id);
        var dialog = $("#dlgConfirm").dialog({
            autoOpen: true,
            height: 200,
            width: 350,
            modal: true,
            buttons: {
                "Yes": function () {
                    //Save new station position
                    var st = smap.stations.getStation($("#hfCurrentId").val());
                    st.Lat = st.Marker.getPosition().lat();
                    st.Lng = st.Marker.getPosition().lng();
                    var model = {
                        Id: st.Id,
                        Name: st.Name,
                        Color: st.Color,
                        StrLat: st.Lat,
                        StrLng: st.Lng
                    }
                    $.post("/api/stations/Save", model)
                        .done(function (loader) {
                            dialog.dialog("close");

                            smap.stations.updateStation(loader.Data.Station);
                            for (var i = 0; i < loader.Data.Lines.length; i++) {
                                smap.lines.updateLine(loader.Data.Lines[i], true);
                            }

                            // update students distances
                            for (var i in loader.Data.Station.Students) {
                                smap.checkDistanceStudents.push(loader.Data.Station.Students[i].StudentId);
                            }
                            smap.checkDistanceStation = loader.Data.Station;
                            smap.updateDistance();

                        });
                },
                Cancel: function () {
                    //Move marker back
                    var st = smap.stations.getStation($("#hfCurrentId").val());
                    smap.stations.setMarker(st);
                    dialog.dialog("close");
                }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    updateStation: function (station) {
        var oldStation = smap.stations.getStation(station.Id);
        if (oldStation != null) {
            station.Marker = oldStation.Marker;
            var index = smap.stations.list.indexOf(oldStation);
            smap.stations.list[index] = station;
        } else {
            smap.stations.list.push(station);
        }
        smap.stations.setMarker(station);
    },
    deleteStation: function (id) {
        smap.closeConextMenu();
        var station = smap.stations.getStation(id);
        $("#dConfirmMessage").html("Do you want to delete station '" + station.Name + "'?");
        $("#hfCurrentId").val(station.Id);
        var dialog = $("#dlgConfirm").dialog({
            autoOpen: true,
            height: 200,
            width: 350,
            modal: true,
            buttons: {
                "Delete": function () {
                    $.post("/api/stations/Delete/" + $("#hfCurrentId").val(), null)
                       .done(function (loader) {
                           if (loader.Data == true) {
                               var st = smap.stations.getStation($("#hfCurrentId").val());
                               st.Marker.setMap(null);
                               var ind = smap.stations.list.indexOf(st);
                               smap.stations.list.splice(ind, 1);
                               dialog.dialog("close");
                           } else {
                               $("#dConfirmMessage").html("Station was not deleted");
                           }
                       });
                },
                Cancel: function () {
                    dialog.dialog("close");
                }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    showBorders: function () {//show areas around all stations
        var z = (23 - smap.mainMap.getZoom()) / 4;
        for (var i = 0; i < smap.stations.list.length; i++) {
            //smap.stations.list[i].Marker.setAnimation(google.maps.Animation.BOUNCE);
            if (smap.stations.list[i].Marker != null) {
                smap.stations.list[i].Marker.Circle = new google.maps.Circle({
                    strokeColor: smap.fixCssColor(smap.stations.list[i].Color),
                    strokeOpacity: 0.8,
                    strokeWeight: 1,
                    fillColor: smap.fixCssColor(smap.stations.list[i].Color),
                    fillOpacity: 0.35,
                    map: smap.mainMap,
                    center: smap.stations.list[i].Marker.getPosition(),
                    radius: z * 30
                });
            }
        }
    },
    studentDargEnd: function (position, student) {// check where was moved studen
        for (var i = 0; i < smap.stations.list.length; i++) {
            var m = smap.stations.list[i].Marker;
            if (m == null) return;
            var d = google.maps.geometry.spherical.computeDistanceBetween(m.getPosition(), position);
            var r = m.Circle.getRadius();
            if (d <= r) {//marker in circle
                smap.stations.attachStudentToStation(student, smap.stations.list[i]);
            }
            m.setAnimation(null);
            m.Circle.setMap(null);
            m.Circle = null;
        }
    },
    attachStudentToStation: function (student, station) {
        $("#dConfirmAttach").html("Do you want to attach " + student.Name + " to station '" + station.Name + "'?");
        $("#dAttachDist").html('<img src="/Content/img/ajax-loader.gif"/>');
        $("#hfAttachStudentId").val(student.Id);
        $("#hfAttachStationId").val(station.Id);

        var lines = smap.stations.getLines(station.Id);
        $("#ddlAttachLines").empty();
        if (lines.length == 0) {
            $("div[rel=AttachLines]").css("display", "none");
            $("#rAttachStation").prop("checked", true);
        } else {
            $("div[rel=AttachLines]").css("display", "block");
            for (var i in lines) {
                $("<option value='" + lines[i].Id + "'>" + lines[i].Name + " (" + lines[i].StudentsCount + " students)</option>").appendTo($("#ddlAttachLines"));
            }
            smap.stations.attachStudentLineSelected();
            $("#rAttachStation").prop("checked", true);
        }

        $("#ciAttachLeave").css("background-color", smap.fixCssColor(student.Color));
        $("#ciAttachLeave").attr("title", smap.fixCssColor(student.Color));
        $("#ciAttachStation").css("background-color", smap.fixCssColor(station.Color));
        $("#ciAttachStation").attr("title", smap.fixCssColor(student.Color));

        var attaches = smap.getAttachInfo(student.Id);
        var mainAttach = null;
        for (var i in attaches) {
            if (attaches[i].Date == null) mainAttach = attaches[i];
        }

        $("#rAttachReplace").prop("checked", true);
        $("#tbAttachHours").val(0);
        $("#tbAttachMinutes").val(0);
        $("#tbAttachDate").datepicker("option", "gotoCurrent", true);
        $("#tbAttachDate").datepicker("setDate", new Date());
        if (mainAttach == null) {
            $("#dAttachMultiline").css("display", "none");
        }
        else {
            $("#dAttachMultiline").css("display", "block");
            $("#sAttachMName").html(student.Name);
            $("#sAttachMStation").html(station.Name);
        }

        $("#rAttachSchedule").change(smap.stations.dateVisibleSwitch);
        $("#rAttachReplace").change(smap.stations.dateVisibleSwitch);
        smap.stations.dateVisibleSwitch();

        var addr1 = new google.maps.LatLng(student.Lat, student.Lng);
        var addr2 = new google.maps.LatLng(station.StrLat, station.StrLng);
        var dialog = $("#dlgAttach").dialog({
            autoOpen: true,
            width: 350,
            modal: true,
            buttons: {
                "Attach": function () {
                    var data = $("#frmAttach").serialize();
                    $.post("/api/stations/AttachStudent", data)
                        .done(function (loader) {

                            if (loader.Done == true) {
                                for (var i in loader.Lines) {
                                    smap.lines.updateLine(loader.Lines[i]);
                                }
                                for (var i in loader.Stations) {
                                    smap.stations.updateStation(loader.Stations[i]);
                                }
                                smap.updateStudent(loader.Student);
                            }
                            $("#dlgAttach").dialog("close");
                        });
                },
                Cancel: function () {
                    dialog.dialog("close");
                }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
        if (smap.directionsService == null) smap.directionsService = new google.maps.DirectionsService();

        var request = {
            origin: addr1,
            destination: addr2,
            travelMode: google.maps.DirectionsTravelMode.WALKING
        };
        smap.directionsService.route(request, function (response, status) {

            if (status == google.maps.DirectionsStatus.OK) {

                var legs = response.routes[0].legs;
                //gDirectionsDisplay.setDirections(response);
                //wlk.panorama.setPosition(addr1);

                $("#dAttachDist").html("Distance " + legs[0].distance.text + " (" + legs[0].duration.text + ")");
                $("#hfAttachDistance").val(legs[0].distance.value);
            } else {
                var d = google.maps.geometry.spherical.computeDistanceBetween(addr1, addr2);
                $("#dAttachDist").html("Distance " + d + "m (directly)");
            }
        });
    },
    attachStudentLineSelected: function () {
        var lineId = $("#ddlAttachLines").val();
        var line = smap.getLine(lineId);
        $("#ciAttachLine").css("background-color", smap.fixCssColor(line.Color));
        $("#ciAttachLine").attr("title", smap.fixCssColor(line.Color));
    },
    dateVisibleSwitch: function () {
        var check = $("#rAttachSchedule").prop("checked");
        $("#tbAttachDate").prop("disabled", !check);
        $("#tbAttachHours").prop("disabled", !check);
        $("#tbAttachMinutes").prop("disabled", !check);
    },
    addToLine: function (id) {
        smap.closeConextMenu();

        $("#hfAddStationId").val(id);
        //Fill lines drop down list (All lines exclude already attached)
        $("#ddlAddLine").empty();
        var lines = smap.stations.getLines(id);
        for (var i = 0; i < smap.lines.list.length; i++) {
            var l = smap.lines.list[i];
            if (lines.indexOf(l) == -1) {
                var t = "<option value='" + l.Id + "' ";
                if (l.Id == smap.stations.latestLineId) t += "selected='selected'";
                t += ">" + l.Name + "  " + l.LineNumber +"</option>";
                $(t).appendTo("#ddlAddLine");
            }
        }
        $("#ddlAddLine").change(function () {
            smap.stations.fillPositionDropDown($("#ddlAddLine").val());
        });
        smap.stations.fillPositionDropDown($("#ddlAddLine").val());

        $("#tbAddLineHours").val(0);
        $("#tbAddLineMinutes").val(0);

        var station = smap.stations.getStation(id);
        $("#dAddStation").css("background-color", smap.fixCssColor(station.Color));

        $("#rAddStation").prop("checked", true);

        var dialog = $("#dlgAddToLine").dialog({
            autoOpen: true,
            width: 420,
            modal: true,
            buttons: {
                "Add": function () {
                    smap.stations.latestLineId = $("#ddlAddLine").val();
                    var data = $("#frmAddStationTolIne").serialize();
                    $.post("/api/stations/AddToLine", data).done(function (loader) {

                        dialog.dialog("close");
                        smap.lines.updateLine(loader.Line, false);


                        smap.stations.updateStation(loader.Station);

                        for (var i in loader.Students) {
                            smap.updateStudent(loader.Students[i]);
                        }

                        smap.lines.startReCalcimeTable(loader.Line.Id);
                    });
                },
                Cancel: function () {
                    dialog.dialog("close");
                }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    fillPositionDropDown: function (lineId) {
        var line = smap.getLine(lineId);
        $("#ddlAddPosition").empty();
        for (var j = 1; j <= line.Stations.length + 1; j++) {
            $("<option value='" + j + "'>" + j + "</option>").appendTo("#ddlAddPosition");
        }
        $("#ddlAddPosition").val(line.Stations.length + 1);
        $("#dAddLine").css("background-color", smap.fixCssColor(line.Color));
    },
    editToLine: function (id) {
        smap.closeConextMenu();
        $("#hfEditToLineStationId").val(id);
        var lines = smap.stations.getLines(id);
        $("#tabLines").empty();
        $("<div id='tabLinesCont'></div>").appendTo("#tabLines");
        $("<ul id='lstTabLines'></ul>").appendTo($("#tabLinesCont"));
        for (var i = 0; i < lines.length; i++) {
            var line = lines[i];
            $("<li rel='" + line.Id + "'><a href='#line" + line.Id + "'>"
                + line.Name + "<span class='color-indicator-small' style='background-color:"
                + smap.fixCssColor(line.Color) + "'></span></a></li>").appendTo($("#lstTabLines"));
            $("<div id='line" + line.Id + "'></div>").appendTo($("#tabLinesCont"));
        }

        smap.stations.fillEditOnLineDialog(lines[0], id);
        $("#hfEditToLineLineId").val(lines[0].Id);
        var tabs = $("#tabLinesCont").tabs({
            activate: function (event, ui) {
                var lineId = $(ui.newTab).attr("rel");
                $("#hfEditToLineLineId").val(lineId);
                smap.stations.fillEditOnLineDialog(smap.getLine(lineId), $("#hfEditToLineStationId").val());
            }
        });
        $("#cbEditLineColor").prop("checked", false);
        var dialog = $("#dlgEditToLine").dialog({
            autoOpen: true,
            width: 500,
            modal: true,
            buttons: {
                "Save": function () {
                    var data = $("#frmEditToLine").serialize();
                    $.post("/api/stations/SaveOnLine", data).done(function (loader) {
                        dialog.dialog("close");
                        smap.lines.updateLine(loader.Line, true);
                        var station = smap.stations.getStation(loader.Station.Id);
                        loader.Station.Marker = station.Marker;
                        var index = smap.stations.list.indexOf(station);
                        smap.stations.list[index] = loader.Station;
                        smap.stations.setMarker(loader.Station);
                    });
                },
                Cancel: function () {
                    dialog.dialog("close");
                }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    fillEditOnLineDialog: function (line, stationId) {
        if (line == null) return;
        var station = null;
        for (var i = 0; i < line.Stations.length; i++) {
            if (line.Stations[i].StationId == stationId) {
                station = line.Stations[i];
                break;
            }
        }
        if (station == null) return;
        var t = station.ArrivalDateString.split(":");
        $("#tbEditLineHours").val(t[0]);
        $("#tbEditLineMinutes").val(t[1]);
        $("#ddlEditPosition").empty();
        for (var j = 1; j <= line.Stations.length; j++) {
            $("<option value='" + j + "'>" + j + "</option>").appendTo($("#ddlEditPosition"));
        }
        $("#ddlEditPosition").val(station.Position);
    },
    deleteFromLines: function (id) {
        smap.closeConextMenu();
        smap.stations.fillDeleteFromLinesDialog(id);
        var dialog = $("#dlgDeleteFromLine").dialog({
            autoOpen: true,
            width: 350,
            modal: true
        });
    },
    fillDeleteFromLinesDialog: function (id) {
        $("#hfDeleteFromLineStationId").val(id);
        $("#dDeleteLines").empty();
        var lines = smap.stations.getLines(id);
        for (var i = 0; i < lines.length; i++) {
            $("<div>" + lines[i].Name + "<span class='color-indicator-small' style='margin-right:20px;background-color:"
                + smap.fixCssColor(lines[i].Color) + "'></span><a style='color:red;' href='javascript:smap.stations.doDeleteFromLine(" +
                 lines[i].Id + ")'><span class='glyphicon glyphicon-remove'></span></a></div>").appendTo($("#dDeleteLines"));
        }
    },
    doDeleteFromLine: function (id) {
        $("#dConfirmMessage").html("Are you sure?");
        var dialog = $("#dlgConfirm").dialog({
            autoOpen: true,
            width: 350,
            modal: true,
            buttons: {
                "Delete": function () {
                    dialog.dialog("close");
                    var data = {
                        StationId: $("#hfDeleteFromLineStationId").val(),
                        LineId: id
                    }
                    $.post("/api/stations/DeleteFomLine", data).done(function (loader) {

                        smap.lines.updateLine(loader.Line, false);
                        smap.lines.startReCalcimeTable(loader.Line.Id);
                        var lines = smap.stations.getLines(loader.Station.Id);
                        if (lines.length == 0)
                            $("#dlgDeleteFromLine").dialog("close");
                        else
                            smap.stations.fillDeleteFromLinesDialog(loader.Station.Id);
                    });
                },
                Cancel: function () {
                    dialog.dialog("close");
                }
            }
        });

    },
    resetSearchBox: function () {
        //reset address search box
        $("#spCSSucIcon").css("display", "none");
        $("#spCSWarnIcon").css("display", "none");
        $("#dCSControl").removeClass("has-success");
        $("#dCSControl").removeClass("has-warning");
    }
}