var smap = {
    mainMap: null,
    students: [],
    Geocoder: null,
    directionsService: null,
    showStationsWithoutLines: true,
    checkDistanceStation: null,
    checkDistanceStudents: [],
    graphicElements: [],
    attachGrid: null,
    init: function () {
        // getting map's center
        var latCenter = 32.086368;
        var lngCenter = 34.889135;
        var intZoom = 12;

        if ($("#hfCenterLat").length > 0) latCenter = $("#hfCenterLat").val();
        if ($("#hfCenterLng").length > 0) lngCenter = $("#hfCenterLng").val();
        if ($("#hfZoom").length > 0) intZoom = parseInt($("#hfZoom").val());

        //creating map
        var mapOptions = {
            center: new google.maps.LatLng(latCenter, lngCenter),
            zoom: intZoom,
            draggable: true,
            scrollwheel: true,

            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        smap.mainMap = new google.maps.Map(document.getElementById("map-canvas"), mapOptions);

        google.maps.event.addDomListener(window, "resize", function () {
            var center = smap.mainMap.getCenter();
            google.maps.event.trigger(smap.mainMap, "resize");
            smap.mainMap.setCenter(center);
        });

        google.maps.event.addListener(smap.mainMap, "rightclick", function (event) { smap.showContextMenu(event.latLng); });
        google.maps.event.addListener(smap.mainMap, "click", function (event) { smap.closeConextMenu(); });


        smap.Geocoder = new google.maps.Geocoder();

        smap.loadData();
        smap.UI.init();
        //smap.loadStudents();
        //smap.stations.load();
        $("#tbAttachDate").datepicker();
    },
    loadData: function () {
        $.get("/api/Map/State").done(function (loader) {
            smap.lines.list = loader.Lines;
            for (var j = 0; j < smap.lines.list.length; j++) {
                smap.lines.list[j].show = false;
            }
            for (var i = 0; i < loader.Stations.length; i++) {
                var stt = loader.Stations[i];
                if (smap.stations.getStation(stt.Id) == null) {
                    smap.stations.list.push(stt);
                }
                var lines = smap.stations.getLines(stt.Id);
                if (smap.showStationsWithoutLines || lines.length > 0)
                    smap.stations.setMarker(stt);
            }
            smap.students = loader.Students;
            for (var k = 0; k < smap.students.length; k++) {
                var student = smap.students[k];
                student.show = student.Active;
                if (student.Lat != null && student.Lng != null) smap.setMarker(student);
            }
            smap.table.init();
            smap.findLatLngForStudent();
        });
    },
    loadStudents: function () {
        $.get("/api/Students/StudentsForMap").done(function (loader) {
            smap.students = loader;
            for (var i = 0; i < loader.length; i++) {
                var student = loader[i];
                student.show = student.Active;
                if (student.Lat != null && student.Lng != null) smap.setMarker(student);
            }
            smap.table.init();
            smap.findLatLngForStudent();
        });
    },
    switchStationsVisible: function () {
        smap.showStationsWithoutLines = !smap.showStationsWithoutLines;
        smap.showStationsVisibleButton();
    },
    showStationsVisibleButton: function () {
        if (smap.showStationsWithoutLines) {
            $("#btToggleStationsVisible").attr("class", "glyphicon glyphicon-eye-open");
            for (var i = 0; i < smap.stations.list.length; i++) {
                smap.stations.setMarker(smap.stations.list[i]);
            }
        } else {
            $("#btToggleStationsVisible").attr("class", "glyphicon glyphicon-eye-close");
            for (var j = 0; j < smap.stations.list.length; j++) {
                var station = smap.stations.list[j];
                var lines = smap.stations.getLines(station.Id);
                if (lines.length == 0 && station.Marker != null) {
                    station.Marker.setMap(null);
                    station.Marker = null;
                }

            }
        }
    },
    getStudent: function (id) {
        var res = null;
        for (var i = 0; i < smap.students.length; i++) {
            if (smap.students[i].Id == id) {
                res = smap.students[i];
                break;
            }
        }
        return res;
    },
    updateStudent: function (student) {
        var old = smap.getStudent(student.Id);
        if (old == null) {
            // add new student
        } else {
            // update exists
            student.Marker = old.Marker;
            student.Marker.student = student;
            student.show = old.show;
            var index = smap.students.indexOf(old);
            smap.students[index] = student;
            smap.setMarker(student);
            smap.table.studentsGrid.setRowData(student.Id, student);
        }
    },
    getLine: function (id) {
        var res = null;
        for (var i = 0; i < smap.lines.list.length; i++) {
            if (smap.lines.list[i].Id == id) {
                res = smap.lines.list[i];
                break;
            }
        }
        return res;
    },
    getMarkerIcon: function (student) {
        var color = student.Color;
        if (color == null || color == undefined) color = "FF0000";
        if (color.length < 3) color = "FF0000";
        if (color.substring(0, 1) == "#") {
            color = color.substring(1, color.length);

        }
        return "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=S|" + color;
    },
    setMarker: function (student) {//Add or move student marker
        var myLatlng = new google.maps.LatLng(student.Lat, student.Lng);
        if (student.Marker) {
            //Move marker
            student.Marker.setPosition(myLatlng);
            student.Marker.setIcon(smap.getMarkerIcon(student));
            student.Marker.setTitle(student.Name);
        } else {
            //Add marker
            var icon = smap.getMarkerIcon(student);
            // Place a draggable marker on the map
            student.Marker = new google.maps.Marker({
                position: myLatlng,
                map: smap.mainMap,
                draggable: true,
                icon: icon,
                title: student.Name,
                student: student
            });

            student.Marker.addListener('dblclick', function (e) {

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
                    }
                    st.IW = null;
                }
                smap.clearGraphic();
                if (student.IW != null) {
                    student.IW.open(smap.mainMap, student.Marker);
                } else {
                    var content = "<div class='student-info-window' id='dIW" + student.Id + "'>";
                    content += "<h4>" + student.Name + "</h4>";
                    content += "<div>" + student.CellPhone + "...." + student.Email + "</div>";
                    content += "<div>" + student.Address + "</div>";
                    content += "<div rel='family'><img src='/Content/img/ajax-loader.gif' /></div>";
                    content += "</div>";
                    var infowindow = new google.maps.InfoWindow({
                        content: content
                    });
                    student.IW = infowindow;
                    infowindow.open(smap.mainMap, student.Marker);
                    smap.loadFamily(student.Id);
                }

                //show stations connect lines
                var stations = smap.getAttachInfo(student.Id);
                console.log(stations);
                for (var j in stations) {
                    var stt = smap.stations.getStation(stations[j].StationId);
                    var nm = stations[j].Date != null;
                    smap.drawLine(student.Lat, student.Lng, stt.StrLat, stt.StrLng, nm);
                }

                //handle closing info window for hide lines
                student.IW.addListener('closeclick', function () { smap.clearGraphic(); });
            });
            google.maps.event.addListener(student.Marker, "rightclick", function (event) { smap.showStudentContextMenu(event.latLng, student); });
            google.maps.event.addListener(student.Marker, "click", function (event) { smap.closeConextMenu(); });
            google.maps.event.addListener(student.Marker, "dragend", function (event) {
                var st = smap.getStudent(student.Id);
                smap.stations.studentDargEnd(event.latLng, st);

                smap.setMarker(st);
            });
            google.maps.event.addListener(student.Marker, "dragstart", function (event) { smap.stations.showBorders() });
        }
    },
    loadFamily: function (id) {//load info about family for show in InfoWindow
        $.get("/api/Students/Family", { id: id }).done(function (loader) {

            var cont = $("#dIW" + loader.Id).children("div[rel=family]");
            $(cont).empty();
            if (loader.Family != null) {
                var p1 = loader.Family.parent1Type + "</br>";
                p1 += loader.Family.parent1FirstName + " " + loader.Family.parent1LastName + "</br>";
                p1 += loader.Family.parent1CellPhone + "</br>";
                p1 += loader.Family.parent1Email + "</br>";

                var p2 = loader.Family.parent2Type + "</br>";
                p2 += loader.Family.parent2FirstName + " " + loader.Family.parent2LastName + "</br>";
                p2 += loader.Family.parent2CellPhone + "</br>";
                p2 += loader.Family.parent2Email + "</br>";
                $(cont).append("<hr/><table class='tbl-family' id='tblFamily" + loader.Id + "'><tr><td rel='p1'>" + p1 + "</td><td rel='p2'>" + p2 + "</td></tr></table");

            }
        });
    },
    findLatLngForStudent: function () { // Looking student coordinates by address

        var st = null;
        for (var i = 0; i < smap.students.length; i++) {
            if (smap.students[i].Lat == null || smap.students[i].Lng == null) {
                st = smap.students[i];
                break;
            }
        }

        if (st != null) {
            $("#spStatus").html("Looking " + st.Name + " by address...");
            //geocoding address
            smap.Geocoder.geocode({ 'address': st.Address }, function (results1, status1) {

                if (results1.length > 0) {
                    st.Lat = results1[0].geometry.location.lat();
                    st.Lng = results1[0].geometry.location.lng();

                    //save coordinates
                    $.get("/api/Students/SaveCoords", { id: st.Id, lat: st.Lat, lng: st.Lng }).done(function (loader) {

                    });


                    smap.setMarker(st);
                    setTimeout("smap.findLatLngForStudent();", 1000);
                } else {
                    console.log("not found");
                    console.log(st);
                }
            });
        } else {
            $("#spStatus").html("");
        }
    },
    setMenuXY: function (caurrentLatLng) { //Move context menu to clicked point
        var mapWidth = $('#map-canvas').width();
        var mapHeight = $('#map-canvas').height();
        var menuWidth = $('.contextmenu').width();
        var menuHeight = $('.contextmenu').height();
        var clickedPosition = smap.getCanvasXY(caurrentLatLng);
        var x = clickedPosition.x;
        var y = clickedPosition.y;

        if ((mapWidth - x) < menuWidth)//if to close to the map border, decrease x position
            x = x - menuWidth;
        if ((mapHeight - y) < menuHeight)//if to close to the map border, decrease y position
            y = y - menuHeight;

        $('.contextmenu').css('left', x);
        $('.contextmenu').css('top', y);
    },
    getCanvasXY: function (caurrentLatLng) {//convert coordinates to canvas X and Y
        var scale = Math.pow(2, smap.mainMap.getZoom());
        var nw = new google.maps.LatLng(
            smap.mainMap.getBounds().getNorthEast().lat(),
            smap.mainMap.getBounds().getSouthWest().lng()
        );
        var worldCoordinateNw = smap.mainMap.getProjection().fromLatLngToPoint(nw);
        var worldCoordinate = smap.mainMap.getProjection().fromLatLngToPoint(caurrentLatLng);
        var caurrentLatLngOffset = new google.maps.Point(
            Math.floor((worldCoordinate.x - worldCoordinateNw.x) * scale),
            Math.floor((worldCoordinate.y - worldCoordinateNw.y) * scale)
        );
        return caurrentLatLngOffset;
    },
    showContextMenu: function (currentLatLng) {
        var lat = currentLatLng.lat();
        var lng = currentLatLng.lng();
        smap.closeConextMenu();
        var contextmenuDir = document.createElement("div");
        contextmenuDir.className = 'contextmenu';
        contextmenuDir.innerHTML = '<a id="menu1" href="javascript:smap.stations.openPopup(null,' + lat + ',' + lng + ');"><div class="context">Add station<\/div><\/a>';
        contextmenuDir.innerHTML += '<a id="menu1" href="javascript:smap.lines.editLine(0);"><div class="context">Add new line</div></a>';
        $(smap.mainMap.getDiv()).append(contextmenuDir);

        smap.setMenuXY(currentLatLng);

        contextmenuDir.style.visibility = "visible";
    },
    showStudentContextMenu: function (currentLatLng, student) {
        smap.closeConextMenu();
        var contextmenuDir = document.createElement("div");
        contextmenuDir.className = 'contextmenu';
        contextmenuDir.innerHTML = '<a id="menuS1" href="javascript:smap.showSchedule(' + student.Id + ');"><div class="context">Stations<\/div><\/a>';

        $(smap.mainMap.getDiv()).append(contextmenuDir);

        smap.setMenuXY(currentLatLng);

        contextmenuDir.style.visibility = "visible";
    },
    closeConextMenu: function () {
        $('.contextmenu').remove();
    },
    fixCssColor: function (color) { //fix color for use in css properies
        if (color.substring(0, 1) != "#") color = "#" + color;
        return color;
    },
    getAttachInfo: function (studentId) {
        var res = [];
        for (var i in smap.stations.list) {
            var st = smap.stations.list[i];
            for (var j in st.Students) {
                if (st.Students[j].StudentId == studentId)
                    res.push(st.Students[j]);
            }
        }
        return res;
    },
    updateDistance: function () {
        if (smap.checkDistanceStation == null || smap.checkDistanceStudents.length == 0) {
            $("#spStatus").html("");
            smap.checkDistanceStation = null;
            smap.checkDistanceStudents = [];
            return;
        }
        var st = smap.getStudent(smap.checkDistanceStudents[0]);
        $("#spStatus").html("Calculatedistance for " + st.Name);

        if (smap.directionsService == null) smap.directionsService = new google.maps.DirectionsService();

        var addr1 = new google.maps.LatLng(st.Lat, st.Lng);
        var addr2 = new google.maps.LatLng(smap.checkDistanceStation.StrLat, smap.checkDistanceStation.StrLng);

        var request = {
            origin: addr1,
            destination: addr2,
            travelMode: google.maps.DirectionsTravelMode.WALKING
        };
        smap.directionsService.route(request, function (response, status) {
            var d = 0;
            if (status == google.maps.DirectionsStatus.OK) {
                var legs = response.routes[0].legs;
                d = legs[0].distance.value;
            } else {
                d = google.maps.geometry.spherical.computeDistanceBetween(addr1, addr2);

            }

            // save
            var data = new Object();
            data.StudentId = st.Id;
            data.StationId = smap.checkDistanceStation.Id;
            data.Distance = d;
            $.post("/api/stations/UpdateAttachStudent", data).done(function (loader) {
                var stt = smap.stations.getStation(loader.StationId);
                for (var i in stt.Students) {
                    if (stt.Students[i].StudentId == loader.StudentId) {
                        stt.Students[i].Distance = loader.Distance;
                        var st = smap.getStudent(stt.Students[i].StudentId);
                        smap.updateStudent(st);
                    }
                }

                smap.checkDistanceStudents.splice(0, 1);
                smap.updateDistance();
            });
        });
    },
    drawLine: function (lat1, lng1, lat2, lng2, notMain) {
        var point1 = new google.maps.LatLng(lat1, lng1);
        var point2 = new google.maps.LatLng(lat2, lng2);
        var color = '#222222';
        var opacity = 0.7;
        var weight = 2;
        if (notMain == true) {
            opacity = 0.7;
            weight = 1;
        }
        var polyline = new google.maps.Polyline({
            path: [point1, point2],
            geodesic: true,
            strokeColor: color,
            strokeOpacity: opacity,
            strokeWeight: weight
        });
        smap.graphicElements.push(polyline);
        polyline.setMap(smap.mainMap);
    },
    clearGraphic: function () {
        for (var i in smap.graphicElements) {
            var el = smap.graphicElements[i];
            el.setMap(null);
        }
        smap.graphicElements = [];
    },
    showSchedule: function (studentId) {
        smap.closeConextMenu();

        $("#hfAttachListStationId").val(studentId);
        var st = smap.getStudent(studentId);
        if (st != null) $("#dAttachName").html(st.Name);

        if (smap.attachGrid == null) {
            smap.attachGrid = $("#dScheduleGrid").jqGrid({
                datatype: "clientSide",
                height: '100%',
                regional: 'il',
                hidegrid: false,
                multiselect: false,
                rowNum: 10,
                rowList: [10, 20],
                viewrecords: true,
                width: '100%',
                loadui: 'disable',
                altRows: false,
                emptyrecords: 'No lines',
                sortable: true,
                altclass: "ui-state-default",
                colNames: ["Line", "Station", "Distance", "Date", "Time", ""],
                colModel: [
                    { name: 'LineId', width: 50, align: 'center', formatter: smap.table.lineNameFormatter },
                    { name: 'StationId', width: 100, align: 'center', formatter: smap.table.stationNameFormatter },
                    { name: 'Distance', width: 100, align: 'center', formatter: smap.table.simpleDistanceFormatter },
                    { name: 'StrDate', width: 100, align: 'center' },
                    { name: 'StrTime', width: 50, align: 'center' },
                    { name: 'Id', width: 50, align: 'center', formatter: smap.table.attachActionFormatter }
                ]
            });
        }
        smap.attachGrid.jqGrid("clearGridData");
        var data = smap.getAttachInfo(studentId);
        for (var i in data) {
            smap.attachGrid.jqGrid('addRowData', data[i].Id, data[i]);
        }

        smap.attachGrid.jqGrid('setGridParam', { sortorder: 'asc' });
        smap.attachGrid.jqGrid("sortGrid", "StrDate");


        $("#dlgSchedule").dialog({
            autoOpen: true,
            width: 500,
            modal: true
        });
    },
    deleteAttach: function (id) {
        $("#dConfirmMessage").html("Are you sure?");
        $("#hfCurrentId").val(id);
        var dialog = $("#dlgConfirm").dialog({
            autoOpen: true,
            width: 500,
            modal: true,
            buttons: {
                "Delete": function () {
                    $.post("/api/stations/DeleteAttachStudent/" + id, null).done(function (loader) {
                        console.log(loader);
                        for (var k in loader.Lines) {
                            smap.lines.updateLine(loader.Lines[k]);
                        }
                        for (var i in loader.Stations) {
                            smap.stations.updateStation(loader.Stations[i]);
                        }
                        smap.attachGrid.jqGrid("clearGridData");
                        var data = smap.getAttachInfo($("#hfAttachListStationId").val());
                        for (var j in data) {
                            smap.attachGrid.jqGrid('addRowData', data[j].Id, data[j]);
                        }
                        dialog.dialog("close");
                    });
                },
                Cancel: function () {
                    dialog.dialog("close");
                }
            }
        });
    },
    getRandomColor: function () {
        var letters = '0123456789ABCDEF'.split('');
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }
}