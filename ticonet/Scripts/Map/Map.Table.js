smap.table = {
    studentsGrid: null,
    init: function () {
        $("#btToggleStudents").click(function () {
            var cls = $("#btToggleStudents").attr("class");
            if (cls == "glyphicon glyphicon-chevron-up toggle") {
                $("#btToggleStudents").attr("class", "glyphicon glyphicon-chevron-down toggle");
            } else {
                $("#btToggleStudents").attr("class", "glyphicon glyphicon-chevron-up toggle");
            }
            $("#dStudentsTable").toggle();
        });


        smap.table.studentsGrid = $("#grStudents").jqGrid({
            datatype: "clientSide",
            height: '100%',
            regional: 'ru',
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
            ondblClickRow: smap.table.clickRow,
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
            colNames: ["", "Id", "Name", "Shicva", "Class", "Address", "Color", "Line"],
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
                { name: 'Address', index: 'Address', clearSearch: false, width: 200 },
                { name: 'Color', index: 'Color', clearSearch: false, width: 50, search: false, formatter: smap.table.colorFormatter },
                { name: 'LineColor', index: 'LineColor', clearSearch: false, width: 50, search: false }
            ]
        }).filterToolbar({ searchOnEnter: true, defaultSearch:'cn' });

        for (var i = 0; i < smap.students.length; i++)
            $("#grStudents").jqGrid('addRowData', smap.students[i].Id, smap.students[i]);

        $("#grStudents").jqGrid("sortGrid", "Name");
        $("#grStudents").jqGrid('setGridParam', { sortorder: 'asc' });


        $(".ui-search-clear").remove();

        $("#grStudents_show").empty();
        $("#grStudents_show").css("padding-top","10px");
        $('<input />', { type: 'checkbox', id: 'cbAll', checked: "checked" }).appendTo($("#grStudents_show"));
       
        $("#cbAll").click(function (event) {
           var s = $(this).prop("checked");
            for (var j = 0; j < smap.students.length; j++) {
                smap.table.swithMarker(smap.students[j].Id, s);
            }
        });


    },
    clickRow: function (id) {
        smap.table.resetBounce();
        var st = smap.getStudent(id);
        if (st) {
            if (st.Marker != null) {
                st.Marker.setAnimation(google.maps.Animation.BOUNCE);
                window.setTimeout("smap.table.resetBounce();", 5000);
                smap.mainMap.setCenter(st.Marker.getPosition());
            }
        }

    },
    resetBounce: function () {
        for (var i = 0; i < smap.students.length; i++) {
            var st = smap.students[i];
            if (st.Marker != null)
                st.Marker.setAnimation(null);

        }
    },
    cboxFormatter: function (cellvalue, options, rowObject) {
        var id = options.rowId;
        var student = smap.getStudent(id);
        return '<input ref="cbSt" rel="' + id + '" type="checkbox"' + (student.show ? ' checked="checked"' : '') +
            'onchange="smap.table.preSwithMarker(' + id + ')"/>';

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
        var color = cellvalue;
        if (color.substring(0, 1) != "#") color = "#" + color;
        return '<div style="width:50px; height:10px;background-color:' + color + '" title="' + color + '"></div>';
    }

}