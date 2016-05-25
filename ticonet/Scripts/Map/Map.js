var smap = {
    mainMap: null,
    students: [],
    Geocoder: null,
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



        smap.Geocoder = new google.maps.Geocoder();

        smap.loadStudents();
        
    },
    loadStudents: function () {
        $.get("api/Students/StudentsForMap").done(function (loader) {
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
    getMarkerIcon: function (student) {
        var color = student.Color;
        if (color == null || color == undefined) color = "FF0000";
        if (color.length < 3) color = "FF0000";
        if (color.substring(0, 1) == "#") {
            color = color.substring(1, color.length);
          
        }
        return "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=S|" + color;
    },
    setMarker: function (student) {
        var myLatlng = new google.maps.LatLng(student.Lat, student.Lng);
        if (student.Marker) {
            //Move marker
            student.Marker.setPosition(myLatlng);
        } else {
            //Add marker
            var fullName = student.firstName + " " + student.lastName;
            var icon = smap.getMarkerIcon(student);
            // Place a draggable marker on the map
            student.Marker = new google.maps.Marker({
                position: myLatlng,
                map: smap.mainMap,
                draggable: true,
                icon: icon,
                title: fullName
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
                    content += "<h4>" + student.Name  + "</h4>";
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
        }
    },
    loadFamily:function(id) {
        $.get("/api/Students/Family", { id: id }).done(function(loader) {
            
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
                $(cont).append("<hr/><table class='tbl-family' id='tblFamily" + loader.Id + "'><tr><td rel='p1'>" + p1 +"</td><td rel='p2'>"+ p2 +"</td></tr></table");
               
            }
        });
    },
    findLatLngForStudent: function () {
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
    }
}