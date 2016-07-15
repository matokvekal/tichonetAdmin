smap.UI = {
    searchBarOpen: false,
    autoCompleteService: null,
    autoCompleteResultFunction: null,
    init: function () {
        //Toggle buttons
        $("#btToggleStudents").click(function () {
            var cls = $("#btToggleStudents").attr("class");
            if (cls == "glyphicon glyphicon-chevron-up toggle") {
                $("#btToggleStudents").attr("class", "glyphicon glyphicon-chevron-down toggle");
            } else {
                $("#btToggleStudents").attr("class", "glyphicon glyphicon-chevron-up toggle");
            }
            $("#dStudentsTable").toggle();
        });
        $("#btToggleLines").click(function () {
            var cls = $("#btToggleLines").attr("class");
            if (cls == "glyphicon glyphicon-chevron-up toggle") {
                $("#btToggleLines").attr("class", "glyphicon glyphicon-chevron-down toggle");
            } else {
                $("#btToggleLines").attr("class", "glyphicon glyphicon-chevron-up toggle");
            }
            $("#dLinesTable").toggle();
        });

        $("#btSearch").click(smap.UI.toggleSearchBar);
        $("#btGoSearch").click(function () {
            smap.UI.showAddress($('#tbSearch').val());
        });
        // $("#tbSearch").keyup(function (e) { if (e.keyCode == 13) smap.UI.showAddress($('#tbSearch').val()); });
        $('#tbSearch').autocomplete({
            source: function (request, response) {
                var q = request.term;
                smap.UI.autoCompleteResultFunction = response;
                if (smap.UI.autoCompleteService == null)
                    smap.UI.autoCompleteService = new google.maps.places.AutocompleteService();

                smap.UI.autoCompleteService.getPlacePredictions({ input: q, bounds: smap.mainMap.getBounds() }, function (predictions, status) {
                    var sug = [];

                    for (var i1 in smap.students) {
                        var st = smap.students[i1].Name.toLowerCase();
                        if (st.indexOf(q.toLowerCase()) > -1) {
                            sug.push({ Id: smap.students[i1].Id, Type: "stud", Text: smap.students[i1].Name });
                        }
                    }
                    for (var i2 in smap.stations.list) {
                        var stt = smap.stations.list[i2].Name.toLowerCase();
                        if (stt.indexOf(q.toLowerCase()) > -1) {
                            sug.push({ Id: smap.stations.list[i2].Id, Type: "stat", Text: smap.stations.list[i2].Name });
                        }
                    }
                    for (var i3 in smap.lines.list) {
                        var ln = smap.lines.list[i3].Name.toLowerCase();
                        ln += " " + smap.lines.list[i3].LineNumber.toLowerCase();
                        if (ln.indexOf(q.toLowerCase()) > -1) {
                            sug.push({ Id: smap.lines.list[i3].Id, Type: "line", Text: "Line " + smap.lines.list[i3].LineNumber + " (" + smap.lines.list[i3].Name + ")" });
                        }
                    }
                    for (var i0 in predictions) {
                        sug.push({ Id: 0, Type: "addr", Text: predictions[i0].description });
                    }
                    smap.UI.autoCompleteResultFunction(sug);
                });
            },
            position: {
                my: "left bottom",
                at: "left top",
                collision: "flip flip"
            },
            select: function (event, ui) {

                $("#tbSearch").val(ui.item.Text);
                switch (ui.item.Type) {
                    case "addr":
                        smap.UI.showAddress(ui.item.Text);
                        break;
                    case "line":
                        smap.lines.showLine(ui.item.Id, true);
                        var b = new google.maps.LatLngBounds();
                        var ln = smap.getLine(ui.item.Id);
                        for (var i in ln.Stations) {
                            var st0 = smap.stations.getStation(ln.Stations[i].StationId);
                            b.extend(new google.maps.LatLng(st0.StrLat, st0.StrLng));
                        }
                        smap.mainMap.fitBounds(b);
                        break;
                    case "stat":
                        var stt = smap.stations.getStation(ui.item.Id);
                        smap.mainMap.setCenter(new google.maps.LatLng(stt.StrLat, stt.StrLng));
                        smap.mainMap.setZoom(16);
                        if (stt.Marker != null) smap.table.showMarker(stt.Marker);
                        break;
                    case "stud":
                        var st = smap.getStudent(ui.item.Id);
                        smap.mainMap.setCenter(new google.maps.LatLng(st.Lat, st.Lng));
                        smap.mainMap.setZoom(16);
                        if (st.Marker != null) smap.table.showMarker(st.Marker);
                        break;
                }
                return false;
            }
        }).autocomplete("instance")._renderItem = function (ul, item) {
            var html = "";
            switch (item.Type) {
                case "addr":
                    html = "<span class='glyphicon glyphicon-map-marker' aria-hidden='true'></span>&nbsp;&nbsp;" + item.Text;
                    break;
                case "line":
                    var ln = smap.getLine(item.Id);
                    html = "<span style='color:" + smap.fixCssColor(ln.Color) + "' class='glyphicon glyphicon-road' aria-hidden='true'></span>&nbsp;&nbsp;" + item.Text;
                    break;
                case "stat":
                    var stt = smap.stations.getStation(item.Id);
                    var url = smap.stations.getMarkerIcon(stt);
                    html = "<img src='" + url + "' style='height:16pt;'/>&nbsp;&nbsp;" + item.Text;
                    break;
                case "stud":
                    var st = smap.getStudent(item.Id);
                    var url1 = smap.getMarkerIcon(st);
                    html = "<img src='" + url1 + "' style='height:16pt;'/>&nbsp;&nbsp;" + item.Text;
                    break;
            }


            return $("<li>").append(html).appendTo(ul);
        };

    },
    toggleSearchBar: function () {
        var d = 500;
        if (smap.UI.searchBarOpen) {
            $("#dSearchForm").animate({ width: 0, opacity: 0 }, d, function () {
                $("#tbSearch").val("");
            });
            $("#dSearchBar").animate({ width: "44px" }, d, function () { });
        } else {
            $("#dSearchForm").animate({ width: "319px", opacity: 1 }, d, function () { });
            $("#dSearchBar").animate({ width: "370px" }, d, function () { });
            $("#tbSearch").focus();
        }
        smap.UI.searchBarOpen = !smap.UI.searchBarOpen;
    },
    showAddress: function (addr) {
        smap.Geocoder.geocode({ 'address': addr }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                var b = results[0].geometry.bounds;
                if (b == undefined) {
                    smap.mainMap.setCenter(results[0].geometry.location);
                    smap.mainMap.setZoom(16);
                } else {
                    smap.mainMap.fitBounds(b);
                }
            }
        });
    },
    openStudentInfoWindow: function (student) {
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

        var content = "<div class='student-info-window' id='dIW" + student.Id + "'>";
        content += "<ul class='nav nav-tabs'>";
        content += "<li role='presentation' class='active'><a href='javascript:smap.UI.switchInfoWindowTab(" + student.Id + ",0);'>Student</a></li>";
        content += "<li role='presentation'><a href='javascript:smap.UI.switchInfoWindowTab(" + student.Id + ",1);'>Buses</a></li>";
        //content += "<li role='presentation'><a href='#'>Messages</a></li>";
        content += "</ul>";
        content += "<div class='iw-tab-content' >";
        content += "<h4>" + student.Name + "</h4>";
        content += "<div>" + student.CellPhone + "...." + student.Email + "</div>";
        content += "<div>" + student.Address + "</div>";
        content += "<div rel='family'><img src='/Content/img/ajax-loader.gif' /></div>";
        content += "</div>";
        content += "<div class='iw-tab-content' rel='buses' style='display:none;'>";
        content += "</div>";
        content += "</div>";
        var infowindow = new google.maps.InfoWindow({
            content: content
        });
        infowindow.open(smap.mainMap, student.Marker);
        if (student.Family) {
            smap.UI.showFamilyInfo(student.Id, student.Family);
        } else {
            smap.UI.loadFamily(student.Id);
        }
        

        //show stations connect lines
        var stations = smap.getAttachInfo(student.Id);

        for (var j in stations) {
            var stt = smap.stations.getStation(stations[j].StationId);
            var nm = stations[j].Date != null;
            smap.drawLine(student.Lat, student.Lng, stt.StrLat, stt.StrLng, nm);
        }

        //handle closing info window for hide lines
        infowindow.addListener('closeclick', function () { smap.clearGraphic(); });
        return false;
    },
    switchInfoWindowTab: function (studentId, tabIndex) {
        var cont = $("#dIW" + studentId);
        $(cont).children("ul").children("li").each(function (i, e) {
            if (i == tabIndex) {
                $(e).addClass("active");
            } else {
                $(e).removeClass("active");
            }
        });
        $(cont).children(".iw-tab-content").each(function (i, e) {
            if (i == tabIndex) {
                $(e).css("display", "block");
            } else {
                $(e).css("display", "none");
            }
        });
        if (tabIndex == 1) smap.UI.showBuses(studentId);
    },
    loadFamily: function (id) {//load info about family for show in InfoWindow
        $.get("/api/Students/Family", { id: id }).done(function (loader) {

            var cont = $("#dIW" + loader.Id).find("div[rel=family]");
            $(cont).empty();
            var st = smap.getStudent(loader.Id);
            st.showFamilyInfo = loader.Family;
            smap.UI.showFamilyInfo(loader.Id, loader.Family);
        }).fail(function () {
            console.log("Error");
        });
    },
    showFamilyInfo: function (id, family) {
        var cont = $("#dIW" + id).find("div[rel=family]");
        if (family != null) {
            var p1 = family.parent1Type + "</br>";
            p1 += family.parent1FirstName + " " + family.parent1LastName + "</br>";
            p1 += family.parent1CellPhone + "</br>";
            p1 += family.parent1Email + "</br>";

            var p2 = family.parent2Type + "</br>";
            p2 += family.parent2FirstName + " " + family.parent2LastName + "</br>";
            p2 += family.parent2CellPhone + "</br>";
            p2 += family.parent2Email + "</br>";
            $(cont).append("<hr/><table class='tbl-family' id='tblFamily" + id + "'><tr><td rel='p1'>" + p1 + "</td><td rel='p2'>" + p2 + "</td></tr></table");

        }
    },
    showBuses: function (studenId) {
        var cont = $("#dIW" + studenId).find("div[rel=buses]");
        $(cont).empty();
        var lst = smap.getAttachInfo(studenId);
        for (var i in lst) {
            var st = smap.stations.getStation(lst[i].StationId);
            $("<div class='iw-bus-info'><b>" + lst[i].Distance + "m</b> to " + st.Name + "</div>").appendTo(cont);
            var t = "";
            if (lst[i].LineId != null) {
                var ln = smap.getLine(lst[i].LineId);
                var tm = "";
                for (var j in ln.Stations) {
                    if (ln.Stations[j].StationId == lst[i].StationId) tm = ln.Stations[j].ArrivalDateString;
                }
                t += "<span style='margin-right:10px;'>Line " + ln.LineNumber + " arrive at " + tm + ".</span>";
            }
            if (lst[i].StrDate != "--") {
                t += "<span>" + lst[i].StrDate + " " + lst[i].StrTime + "&nbsp;only.</span>";
            }
            $("<div class='iw-bus-info-small'>" + t + "</div>").appendTo(cont);
        }
    }
}