smap.UI = {
    searchBarOpen: false,
    autoCompleteService: null,
    autoCompleteResultFunction: null,
    init: function () {
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
                        break;
                    case "stud":
                        var st = smap.getStudent(ui.item.Id);
                        smap.mainMap.setCenter(new google.maps.LatLng(st.Lat, st.Lng));
                        smap.mainMap.setZoom(16);
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
            $("#dSearchForm").animate({ width: 0, opacity: 0 }, d, function() {
                $("#tbSearch").val("");
            });
            $("#dSearchBar").animate({ width: "44px" }, d, function () { });
        } else {
            $("#dSearchForm").animate({ width: "319px", opacity: 1 }, d, function () { });
            $("#dSearchBar").animate({ width: "370px" }, d, function () { });
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
    }
}