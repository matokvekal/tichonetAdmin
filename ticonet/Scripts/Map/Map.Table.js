smap.table = {
    studentsGrid: null,
    linesGrid: null,
    jumpTimer: null,
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

        //Students table
        smap.table.studentsGrid = $("#grStudents").jqGrid({
            datatype: "clientSide",
            height: '100%',
            regional: 'il',
            hidegrid: false,
            multiselect: false,
            pager: '#pgStudents',
            mtype: 'post',
            rowNum: 10,
            rowList: [10, 20],
            viewrecords: true,
            width: '100%',
            loadui: 'disable',
            altRows: false,
            sortable: true,
            altclass: "ui-state-default",
            onSelectRow: smap.table.clickRow,
            search: {
                caption: "Search...",
                Find: "Find",
                Reset: "Reset",
                odata: ['contains'],
                groupOps: [{ op: "AND", text: "all" }],
                matchText: " match",
                rulesText: " rules",
                clearSearch: false
            },
            colNames: ["", "Id", "Name", "Shicva", "Class", "Address", "Color", "Line", "Dist"],
            colModel: [
                {
                    name: "show",
                    index: "show",
                    align: "center",
                    edittype: "checkbox",
                    formatter: smap.table.cboxFormatter,
                    formatoptions: { disabled: false },
                    editable: true,
                    editoptions: { value: "true:false", defaultValue: "true" },
                    search: false,
                    sortable: false,
                    width: 25
                },
                {
                    name: "StudentId",
                    index: "StudentId",
                    search: false,
                    width: 25,
                    sorttype: "integer",
                    template: "integer"
                },
                {
                    name: 'Name',
                    index: 'Name',
                    clearSearch: false,
                    sorttype: "text",
                    width: 100
                },
                { name: 'Shicva', index: 'Shicva', clearSearch: false, width: 50 },
                { name: 'Class', index: 'Class', clearSearch: false, width: 50, align: "center" },
                { name: 'Address', index: 'Address', clearSearch: false, width: 198 },
                { name: 'Color', index: 'Color', clearSearch: false, width: 50, search: false, formatter: smap.table.colorFormatter },
                { name: 'Id', index: 'Id', clearSearch: false, width: 50, search: false, formatter: smap.table.lineColorFormatter },
                { name: 'Id', index: 'Id', clearSearch: false, width: 50, align: "center", search: false, formatter: smap.table.distanceFormatter }
            ]
        }).filterToolbar({ searchOnEnter: true, defaultSearch: 'cn' });
        //Add students to table
        for (var i = 0; i < smap.students.length; i++)
            $("#grStudents").jqGrid('addRowData', smap.students[i].Id, smap.students[i]);
        //Default sorting
        $("#grStudents").jqGrid("sortGrid", "Name");
        $("#grStudents").jqGrid('setGridParam', { sortorder: 'asc' });

        //Check box "Show / hide all"
        $("#grStudents_show").empty();
        $("#grStudents_show").css("padding-top", "10px");
        $('<input />', { type: 'checkbox', id: 'cbAll', checked: "checked" }).appendTo($("#grStudents_show"));
        $("#cbAll").click(function (event) {
            var s = $(this).prop("checked");
            for (var j = 0; j < smap.students.length; j++) {
                smap.table.swithMarker(smap.students[j].Id, s);
            }
        });

        //lines table
        smap.table.linesGrid = $("#grLines").jqGrid({
            datatype: "clientSide",
            height: '100%',
            regional: 'il',
            hidegrid: false,
            multiselect: false,
            pager: '#pgLines',
            mtype: 'post',
            rowNum: 10,
            rowList: [10, 20],
            viewrecords: true,
            width: '100%',
            loadui: 'disable',
            altRows: false,
            sortable: true,
            altclass: "ui-state-default",
            colNames: ["", "Number", "Name", "Color", "Active", "Dir", "Students", "Duration", ""],
            colModel: [
                {
                    name: "show",
                    index: "show",
                    align: "center",
                    edittype: "checkbox",
                    formatter: smap.table.cboxFormatterLine,
                    formatoptions: { disabled: false },
                    editable: true,
                    editoptions: { value: "true:false", defaultValue: "true" },
                    search: false,
                    sortable: false,
                    width: 25
                },
                {
                    name: "LineNumber",
                    index: "LineNumber",
                    width: 60,
                    sorttype: "integer",
                    template: "integer",
                    align: "center"
                },
                {
                    name: 'Name',
                    index: 'Name',
                    sorttype: "text",
                    width: 110
                },
                {
                    name: 'Color',
                    index: 'Color',
                    width: 50,
                    search: false,
                    formatter: smap.table.colorFormatter
                },
                {
                    name: "Id",
                    index: 'Id',
                    width: 50,
                    formatter: smap.table.lineActiveFormatter,
                    align: "center"
                },
                { name: 'Direction', index: 'Direction', width: 50, align: "center", formatter: smap.table.directionFormatter },
                { name: 'StudentsCount', index: 'StudentsCount', width: 75, align: "center" },
                { name: 'Duration', index: 'Duration', width: 75, align: "center" },
                {
                    name: "Id",
                    index: 'Id',
                    width: 75,
                    formatter: smap.table.lineActionsFormatter,
                    align: "center"
                }
            ],
            subGrid: true,
            subGridRowExpanded: function (subgridDivId, rowId) {
                var subgridTableId = subgridDivId + "_t";
                $("#" + subgridDivId).html("<table id='" + subgridTableId + "'></table><div id='" + subgridDivId + "_d'></div>");
                var sbGrd = $("#" + subgridTableId).jqGrid({
                    datatype: 'local',
                    pager: '#' + subgridDivId + "_d",
                    rowList: [10, 25, 50],
                    onSelectRow: function (id) {
                        smap.table.resetBounce();
                        var st = smap.stations.getStation(id);
                        if (st) {
                            if (st.Marker != null) {
                                smap.table.showMarker(st.Marker);
                            }
                        }
                    },
                    colNames: ['Position', 'Station', 'Address', 'Time'],
                    colModel: [
                        { name: 'Position', width: 100, align: 'center' },
                        { name: 'StationId', width: 100, formatter: smap.table.stationNameFormatter },
                        { name: 'Address', width: 100, align: 'center' },
                        { name: 'ArrivalDateString', width: 100, align: 'center' }
                    ]
                });
                var lst = smap.getLine(rowId).Stations;
                for (var x in lst) {
                    sbGrd.jqGrid('addRowData', lst[x].StationId, lst[x]);
                }
                sbGrd.jqGrid('setGridParam', { sortorder: 'asc' });
                sbGrd.jqGrid("sortGrid", "Position");
            }

        });

        for (var k = 0; k < smap.lines.list.length; k++) {

            $("#grLines").jqGrid('addRowData', smap.lines.list[k].Id, smap.lines.list[k]);
        }

        //Default sorting
        $("#grLines").jqGrid("sortGrid", "LineNumber");
        $("#grLines").jqGrid('setGridParam', { sortorder: 'asc' });

        //Hide buttons "Clear search"
        $(".ui-search-clear").remove();

    },
    clickRow: function (id) {//Click on row in students table
        smap.table.resetBounce();
        var st = smap.getStudent(id);
        if (st) {
            if (st.Marker != null) {
                smap.table.showMarker(st.Marker);
            }
        }

    },
    resetBounce: function () {//Stop student marker animation after 5 sec
        if (smap.table.jumpTimer != null) {
            clearTimeout(smap.table.jumpTimer);
            smap.table.jumpTimer = null;
        }
        for (var i = 0; i < smap.students.length; i++) {
            var st = smap.students[i];
            if (st.Marker != null)
                st.Marker.setAnimation(null);

        }
        for (var j = 0; j < smap.stations.list.length; j++) {
            var stt = smap.stations.list[j];
            if (stt.Marker != null)
                stt.Marker.setAnimation(null);

        }
    },
    cboxFormatter: function (cellvalue, options, rowObject) {
        var id = options.rowId;
        var student = smap.getStudent(id);
        return '<input ref="cbSt" rel="' + id + '" type="checkbox"' + (student.show ? ' checked="checked"' : '') +
            'onchange="smap.table.preSwithMarker(' + id + ')"/>';

    },
    cboxFormatterLine: function (cellvalue, options, rowObject) {
        var id = options.rowId;
        var line = smap.getLine(id);
        return '<input ref="cbLn" rel="' + id + '" type="checkbox"' + (line.show ? ' checked="checked"' : '') +
            'onchange="smap.lines.preSwitch(' + id + ')"/>';

    },
    stationNameFormatter: function (cellvalue, options, rowObject) {
        var station = smap.stations.getStation(cellvalue);
        if (station == null) return "--";
        return station.Name;
    },
    preSwithMarker: function (id) {
        var sel = $("input[ref=cbSt][rel=" + id + "]").prop("checked");
        smap.table.swithMarker(id, sel);
    },
    swithMarker: function (id, val) {

        var student = smap.getStudent(id);
        student.show = val;
        if (val) {
            //set marker
            smap.setMarker(student);
        } else {
            //ide marker
            if (student.Marker) {
                student.Marker.setMap(null);
            }
            student.Marker = null;
        }
        smap.table.studentsGrid.setRowData(id, student);

    },
    colorFormatter: function (cellvalue, options, rowObject) {
        var color = smap.fixCssColor(cellvalue);
        return '<div style="width:46px; height:10px;background-color:' + color + '" title="' + color + '"></div>';
    },
    lineActionsFormatter: function (cellvalue, options, rowObject) {
        var res = "<a href='javascript:smap.lines.showTimeTable(" + cellvalue + ")' title='Time table'><span class='glyphicon glyphicon-time'></span></a>"
        res += "&nbsp;&nbsp;";
        res += "<a href='javascript:smap.lines.editLine(" + cellvalue + ")' title='Edit line'><span class='glyphicon glyphicon-pencil'></span></a>";
        res += "&nbsp;&nbsp;";
        res += "<a href='javascript:smap.lines.deleteLine(" + cellvalue + ")' title='Delete line'><span class='glyphicon glyphicon-trash'></span></a>";
        return res;
    },
    lineNameFormatter: function (cellvalue, options, rowObject) {
        var ln = smap.getLine(cellvalue);
        if (ln == null) return "--";
        return ln.Name;
    },
    lineNumberFormatter: function (cellvalue, options, rowObject) {
        var ln = smap.getLine(cellvalue);
        if (ln == null) return "--";
        return ln.LineNumber;
    },
    lineActiveFormatter: function (cellvalue, options, rowObject) {
        var res = "<input type='checkbox'";
        var ln = smap.getLine(cellvalue);
        if (ln.Active == true) res += " checked='checked' ";
        res += "onchange='smap.lines.saveLineAcive(" + cellvalue + ")'";
        res += " ref='lnActive' rel='" + cellvalue + "' />";
        return res;
    },
    lineColorFormatter: function (cellvalue, options, rowObject) {

        var id = cellvalue;

        var stl = null;
        var station = null;
        var line = null;
        var res = "";
        for (var i in smap.stations.list) {
            var st = smap.stations.list[i];

            for (var j in st.Students) {
                if (st.Students[j].StudentId == id && st.Students[j].Date == null) {
                    stl = st.Students[j];

                    station = st;
                    break;
                }
            }
        }
        var color = "";
        var title = "";
        if (stl != null) {
            if (stl.LineId == null) {
                if (station != null) {
                    color = station.Color;
                    title = station.Name;
                }
            } else {
                line = smap.getLine(stl.LineId);
                if (line != null && station != null) {
                    color = line.Color;
                    title = line.LineNumber;
                }
            }
        }
        if (color != "") {
            color = smap.fixCssColor(color);
            res = '<div style="width:46px; height:10px;background-color:' + color + '" title="' + title + '"></div>';
        }
        return title;
    },
    directionFormatter: function (cellvalue, options, rowObject) {
        var res = cellvalue;
        if (cellvalue == 0) res = "TO";
        if (cellvalue == 1) res = "FROM";
        return res;
    },
    distanceFormatter: function (cellvalue, options, rowObject) {
        var res = "";
        var atts = smap.getAttachInfo(cellvalue);
        for (var i in atts) {
            if (atts[i].Date == null) {//default route
                res = atts[i].Distance.toString() + " m";
            }
        }
        return res;
    },
    simpleDistanceFormatter: function (cellvalue, options, rowObject) {
        return cellvalue.toString() + " m";
    },
    attachActionFormatter: function (cellvalue, options, rowObject) {
        var res = "<a href='javascript:smap.deleteAttach(" + cellvalue + ")' title='Delete'><span class='glyphicon glyphicon-trash'></span></a>";
        return res;
    },
    showMarker: function (marker) {
        smap.table.resetBounce();
        marker.setAnimation(google.maps.Animation.BOUNCE);
        smap.table.jumpTimer = window.setTimeout("smap.table.resetBounce();", 5000);
        if (!smap.mainMap.getBounds().contains(marker.getPosition())) {
            smap.mainMap.setCenter(Marker.getPosition());
        }
    }

}