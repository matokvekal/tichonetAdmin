var smap = {
    mainMap: null,
    students: [],
    Geocoder: null,
    directionsService: null,
    showStationsWithoutLines: true,
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
        //smap.loadStudents();
        //smap.stations.load();
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
                    //st.IW = null;
                }
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

            });
            google.maps.event.addListener(student.Marker, "rightclick", function (event) { smap.showStudentContextMenu(event.latLng, student); });
            google.maps.event.addListener(student.Marker, "click", function (event) { smap.closeConextMenu(); });
            google.maps.event.addListener(student.Marker, "dragend", function (event) {
                smap.stations.studentDargEnd(event.latLng, student);
                smap.setMarker(student);
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
            //geocoding address
            smap.Geocoder.geocode({ 'address': st.city + ", " + st.street + ", " + st.houseNumber }, function (results1, status1) {

                if (results1.length > 0) {
                    st.Lat = results1[0].geometry.location.lat();
                    st.Lng = results1[0].geometry.location.lng();

                    //save coordinates
                    $.get("api/Students/SaveCoords", { id: st.Id, lat: st.Lat, lng: st.Lng }).done(function (loader) {

                    });


                    smap.setMarker(st);
                    smap.findLatLngForStudent();
                }
            });
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
        contextmenuDir.innerHTML = '<a id="menuS1" href="javscript:smap.stations.openPopup(null);"><div class="context">' + student.Name + '<\/div><\/a>';

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
    }
  
}