insets.table = {
    busesGrid: null,
    jumpTimer: null,
    lastSel: null,
    init: function () {
        //Toggle buttons
        $("#btToggleBuses").click(function () {
            var cls = $("#btToggleBuses").attr("class");
            if (cls == "glyphicon glyphicon-chevron-up toggle") {
                $("#btToggleBuses").attr("class", "glyphicon glyphicon-chevron-down toggle");
            } else {
                $("#btToggleBuses").attr("class", "glyphicon glyphicon-chevron-up toggle");
            }
            $("#dBusesTable").toggle();
        });

        //buses table
        insets.table.busesGrid = $("#grBuses").jqGrid({
            datatype: "clientSide",
            height: "100%",
            autowidth: true,
            regional: "il",
            hidegrid: false,
            multiselect: false,
            pager: "#pgBuses",
            mtype: "post",
            rowNum: 10,
            rowList: [10, 20],
            viewrecords: true,
            //width: '100%',
            loadui: "disable",
            altRows: false,
            sortable: true,
            altclass: "ui-state-default",
            editurl: "/api/InsetsApi/SaveBus",
            colNames: ["BusId", "PlateNumber", "Owner", "seats", "price", "munifacturedate", "LicensingDueDate", "insuranceDueDate", "winterLicenseDueDate", "brakeTesDueDate", ""],
            colModel: [
                {
                    name: 'BusId',
                    index: 'BusId',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'PlateNumber',
                    index: 'PlateNumber',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'Owner',
                    index: 'Owner',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'seats',
                    index: 'seats',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'price',
                    index: 'price',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'munifacturedate',
                    index: 'munifacturedate',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'LicensingDueDate',
                    index: 'LicensingDueDate',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'insuranceDueDate',
                    index: 'insuranceDueDate',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'winterLicenseDueDate',
                    index: 'winterLicenseDueDate',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: 'brakeTesDueDate',
                    index: 'brakeTesDueDate',
                    sorttype: "text",
                    width: 110,
                    editable: true
                },
                {
                    name: "Id",
                    index: 'Id',
                    width: 75,
                    formatter: insets.table.lineActionsFormatter,
                    align: "center"
                }
                //{
                //    name: "show",
                //    index: "show",
                //    align: "center",
                //    edittype: "checkbox",
                //    formatter: smap.table.cboxFormatterLine,
                //    formatoptions: { disabled: false },
                //    editable: true,
                //    editoptions: { value: "true:false", defaultValue: "true" },
                //    search: false,
                //    sortable: false,
                //    width: 25
                //},
                //{
                //    name: "LineNumber",
                //    index: "LineNumber",
                //    width: 60,
                //    sorttype: "integer",
                //    template: "integer",
                //    align: "center"
                //},
                //{
                //    name: 'Name',
                //    index: 'Name',
                //    sorttype: "text",
                //    width: 110
                //},
                //{
                //    name: 'Color',
                //    index: 'Color',
                //    width: 50,
                //    search: false,
                //    formatter: smap.table.colorFormatter
                //},
                //{
                //    name: "Id",
                //    index: 'Id',
                //    width: 50,
                //    formatter: smap.table.lineActiveFormatter,
                //    align: "center"
                //},
                //{ name: 'Direction', index: 'Direction', width: 50, align: "center", formatter: smap.table.directionFormatter },
                //{ name: 'StudentsCount', index: 'StudentsCount', width: 75, align: "center" },
                //{ name: 'Duration', index: 'Duration', width: 75, align: "center" },
                //{
                //    name: "Id",
                //    index: 'Id',
                //    width: 75,
                //    formatter: smap.table.lineActionsFormatter,
                //    align: "center"
                //}
            ],
            //subGrid: true,

            //subGridRowExpanded: function (subgridDivId, rowId) {
            //    var subgridTableId = subgridDivId + "_t";
            //    $("#" + subgridDivId).html("<table id='" + subgridTableId + "'></table><div id='" + subgridDivId + "_d'></div>");
            //    var sbGrd = $("#" + subgridTableId).jqGrid({
            //        datatype: 'local',
            //        pager: '#' + subgridDivId + "_d",
            //        rowList: [10, 25, 50],
            //        onSelectRow: function (id) {
            //            smap.table.resetBounce();
            //            var st = smap.stations.getStation(id);
            //            if (st) {
            //                if (st.Marker != null) {
            //                    smap.table.showMarker(st.Marker);
            //                }
            //            }
            //        },
            //        colNames: ['Position', 'Station', 'Address', 'Time'],
            //        colModel: [
            //            { name: 'Position', width: 100, align: 'center' },
            //            { name: 'StationId', width: 100, formatter: smap.table.stationNameFormatter },
            //            { name: 'Address', width: 100, align: 'center' },
            //            { name: 'ArrivalDateString', width: 100, align: 'center' }
            //        ]
            //    });
            //    var lst = smap.getLine(rowId).Stations;
            //    for (var x in lst) {
            //        sbGrd.jqGrid('addRowData', lst[x].StationId, lst[x]);
            //    }
            //    sbGrd.jqGrid('setGridParam', { sortorder: 'asc' });
            //    sbGrd.jqGrid("sortGrid", "Position");
            //}

        });

        for (var k = 0; k < insets.buses.list.length; k++) {
            $("#grBuses").jqGrid('addRowData', insets.buses.list[k].Id, insets.buses.list[k]);
        }

        //Default sorting
        $("#grBuses").jqGrid("sortGrid", "BusId");
        $("#grBuses").jqGrid('setGridParam', { sortorder: 'asc' });

        $("#grBuses").jqGrid('editRow', insets.table.lastSel,
        {
            keys: true,
            oneditfunc: function () {
                alert("edited");
            }
        });

        $("#grBuses").jqGrid('navGrid', '#pgBuses',
        {
            //edit: false,
            //editicon: "ui-icon-pencil",
            //add: true,
            //addicon: "ui-icon-plus",
            //save: false,
            //saveicon: "ui-icon-disk",
            //cancel: false,
            //del: false,
            //search: false,
            //refresh: false,
            //cancelicon: "ui-icon-cancel",
            //addParams: { useFormatter: false },
            //editParams: {}
        });

        ////Hide buttons "Clear search"
        //$(".ui-search-clear").remove();

    },
    //clickRow: function (id) {//Click on row in students table
    //    smap.table.resetBounce();
    //    var st = smap.getStudent(id);
    //    if (st) {
    //        if (st.Marker != null) {
    //            smap.table.showMarker(st.Marker);
    //        }
    //    }

    //},
    //resetBounce: function () {//Stop student marker animation after 5 sec
    //    if (smap.table.jumpTimer != null) {
    //        clearTimeout(smap.table.jumpTimer);
    //        smap.table.jumpTimer = null;
    //    }
    //    for (var i = 0; i < smap.students.length; i++) {
    //        var st = smap.students[i];
    //        if (st.Marker != null)
    //            st.Marker.setAnimation(null);

    //    }
    //    for (var j = 0; j < smap.stations.list.length; j++) {
    //        var stt = smap.stations.list[j];
    //        if (stt.Marker != null)
    //            stt.Marker.setAnimation(null);

    //    }
    //},
    //cboxFormatter: function (cellvalue, options, rowObject) {
    //    var id = options.rowId;
    //    var student = smap.getStudent(id);
    //    return '<input ref="cbSt" rel="' + id + '" type="checkbox"' + (student.show ? ' checked="checked"' : '') +
    //        'onchange="smap.table.preSwithMarker(' + id + ')"/>';

    //},
    //cboxFormatterLine: function (cellvalue, options, rowObject) {
    //    var id = options.rowId;
    //    var line = smap.getLine(id);
    //    return '<input ref="cbLn" rel="' + id + '" type="checkbox"' + (line.show ? ' checked="checked"' : '') +
    //        'onchange="smap.lines.preSwitch(' + id + ')"/>';

    //},
    //stationNameFormatter: function (cellvalue, options, rowObject) {
    //    var station = smap.stations.getStation(cellvalue);
    //    if (station == null) return "--";
    //    return station.Name;
    //},
    //preSwithMarker: function (id) {
    //    var sel = $("input[ref=cbSt][rel=" + id + "]").prop("checked");
    //    smap.table.swithMarker(id, sel);
    //},
    //swithMarker: function (id, val) {

    //    var student = smap.getStudent(id);
    //    student.show = val;
    //    if (val) {
    //        //set marker
    //        smap.setMarker(student);
    //    } else {
    //        //ide marker
    //        if (student.Marker) {
    //            student.Marker.setMap(null);
    //        }
    //        student.Marker = null;
    //    }
    //    smap.table.studentsGrid.setRowData(id, student);

    //},
    //colorFormatter: function (cellvalue, options, rowObject) {
    //    var color = smap.fixCssColor(cellvalue);
    //    return '<div style="width:46px; height:10px;background-color:' + color + '" title="' + color + '"></div>';
    //},
    lineActionsFormatter: function (cellvalue, options, rowObject) {
        var res = "";

        //res += "<a href='javascript:insets.buses.lineStationsVisibleSwitch(" + cellvalue + ")' title='Show/hide stations'><span rel='lsswitch' ref='" + cellvalue + "' class='glyphicon glyphicon-eye-open'></span></a>";
        //res += "&nbsp;&nbsp;";
        //res += "<a href='javascript:insets.buses.showTimeTable(" + cellvalue + ")' title='Time table'><span class='glyphicon glyphicon-time'></span></a>";
        //res += "&nbsp;&nbsp;";
        res += "<span class='js-bus-edit-button-container-" + cellvalue + "'>";
        res += "<a href='javascript:insets.buses.editBus(" + cellvalue + ")' title='Edit bus'><span class='glyphicon glyphicon-pencil'></span></a>";
        res += "&nbsp;&nbsp;";
        res += "<a href='javascript:insets.buses.deleteBus(" + cellvalue + ")' title='Delete bus'><span class='glyphicon glyphicon-trash'></span></a>";
        res += "</span>";
        res += "<span class='js-bus-save-button-container-" + cellvalue + "' hidden>";
        res += "<a href='javascript:insets.buses.saveBus(" + cellvalue + ")' title='Save bus'><span class='glyphicon glyphicon-ok'></span></a>";
        res += "&nbsp;&nbsp;";
        res += "<a href='javascript:insets.buses.cancelEditBus(" + cellvalue + ")' title='Cancel'><span class='glyphicon glyphicon-remove'></span></a>";
        res += "</span>";
        return res;
    },
    //lineNameFormatter: function (cellvalue, options, rowObject) {
    //    var ln = smap.getLine(cellvalue);
    //    if (ln == null) return "--";
    //    return ln.Name;
    //},
    //lineNumberFormatter: function (cellvalue, options, rowObject) {
    //    var ln = smap.getLine(cellvalue);
    //    if (ln == null) return "--";
    //    return ln.LineNumber;
    //},
    //lineActiveFormatter: function (cellvalue, options, rowObject) {
    //    var res = "<input type='checkbox'";
    //    var ln = smap.getLine(cellvalue);
    //    if (ln.Active == true) res += " checked='checked' ";
    //    res += "onchange='smap.lines.saveLineAcive(" + cellvalue + ")'";
    //    res += " ref='lnActive' rel='" + cellvalue + "' />";
    //    return res;
    //},
    //lineColorFormatter: function (cellvalue, options, rowObject) {

    //    var id = cellvalue;

    //    var stl = null;
    //    var station = null;
    //    var line = null;
    //    var res = "";
    //    for (var i in smap.stations.list) {
    //        var st = smap.stations.list[i];

    //        for (var j in st.Students) {
    //            if (st.Students[j].StudentId == id && st.Students[j].Date == null) {
    //                stl = st.Students[j];

    //                station = st;
    //                break;
    //            }
    //        }
    //    }
    //    var color = "";
    //    var title = "";
    //    if (stl != null) {
    //        if (stl.LineId == null) {
    //            if (station != null) {
    //                color = station.Color;
    //                title = station.Name;
    //            }
    //        } else {
    //            line = smap.getLine(stl.LineId);
    //            if (line != null && station != null) {
    //                color = line.Color;
    //                title = line.LineNumber;
    //            }
    //        }
    //    }
    //    if (color != "") {
    //        color = smap.fixCssColor(color);
    //        res = '<div style="width:46px; height:10px;background-color:' + color + '" title="' + title + '"></div>';
    //    }
    //    return title;
    //},
    //directionFormatter: function (cellvalue, options, rowObject) {
    //    var res = cellvalue;
    //    if (cellvalue == 0) res = "TO";
    //    if (cellvalue == 1) res = "FROM";
    //    return res;
    //},
    //distanceFormatter: function (cellvalue, options, rowObject) {
    //    var res = "";
    //    var atts = smap.getAttachInfo(cellvalue);
    //    for (var i in atts) {
    //        if (atts[i].Date == null) {//default route
    //            res = atts[i].Distance.toString() + " m";
    //        }
    //    }
    //    return res;
    //},
    //simpleDistanceFormatter: function (cellvalue, options, rowObject) {
    //    return cellvalue.toString() + " m";
    //},
    //attachActionFormatter: function (cellvalue, options, rowObject) {
    //    var res = "<a href='javascript:smap.deleteAttach(" + cellvalue + ")' title='Delete'><span class='glyphicon glyphicon-trash'></span></a>";
    //    return res;
    //},
    //showMarker: function (marker) {
    //    smap.table.resetBounce();
    //    marker.setAnimation(google.maps.Animation.BOUNCE);
    //    smap.table.jumpTimer = window.setTimeout("smap.table.resetBounce();", 5000);
    //    if (!smap.mainMap.getBounds().contains(marker.getPosition())) {
    //        smap.mainMap.setCenter(Marker.getPosition());
    //    }
    //}
}