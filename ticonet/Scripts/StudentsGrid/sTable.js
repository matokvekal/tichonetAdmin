var stbl = {
    grid: null,
    formReady: false,
    families: [],
    FStudentGrid: null,
    init: function () {
        stbl.families = $.parseJSON($("input[name=Families]").val());
        stbl.createGrid();
        $("#tabsFamily").tabs();

        $("#btToggleFilter").click(function () {
            var cls = $("#btToggleFilter").attr("class");
            if (cls == "glyphicon glyphicon-chevron-up toggle") {
                $("#btToggleFilter").attr("class", "glyphicon glyphicon-chevron-down toggle");
            } else {
                $("#btToggleFilter").attr("class", "glyphicon glyphicon-chevron-up toggle");

            }
            if (!stbl.formReady) {
                console.log(stbl.formReady);
                $("#lstClasses").chosen({ placeholder_text_multiple: "Classes", width: "175px", display_selected_options: false });
                $("#lstShicvas").chosen({ placeholder_text_multiple: "Shicva", width: "175px", display_selected_options: false });
                $("#lstLines").chosen({ placeholder_text_multiple: "Lines", width: "175px", display_selected_options: false });
                $("#lstStations").chosen({ placeholder_text_multiple: "Stations", width: "250px", display_selected_options: false });
                stbl.formReady = true;
            }
            $("#dFilter").toggle();
        });

    },
    createGrid: function () {
        stbl.grid = $("#tbStudents").jqGrid({
            datatype: 'json',
            height: '100%',
            regional: 'il',
            hidegrid: false,
            multiselect: false,
            pager: '#pgStudents',
            mtype: 'post',
            rowNum: 25,
            rowList: [10, 25, 50, 100],
            viewrecords: true,
            width: '100%',
            loadui: 'enable',
            altRows: false,
            sortable: true,
            sortname: "StudentId",
            sortorder: "asc",
            altclass: "ui-state-default",
            url: "/tblStudent/StudentsForTable",
            colNames: ["Id", "First Name", "Last Name", "Class", "Shicva", "Address", "Pay", "Active", "Sibiling", "Request", "Distance To School", "Line & Station", "#"],
            colModel: [{
                name: 'StudentId',
                index: 'StudentId',
                sorttype: "text",
                align: 'center',
                width: 30
            }, {
                name: 'FirstName',
                index: 'FirstName',
                sorttype: "text",
                width: 75
            }, {
                name: 'LastName',
                index: 'LastName',
                sorttype: "text",
                width: 75
            }, {
                name: 'Class',
                index: 'Class',
                sorttype: "text",
                align: 'center',

                width: 50
            }, {
                name: 'Shicva',
                index: 'Shicva',
                sorttype: "text",
                align: 'center',
                width: 50
            }, {
                name: 'addr',
                index: 'addr',
                sorttype: "text",
                width: 150
            }, {
                name: 'Payment',
                index: 'Payment',
                sorttype: "text",
                width: 50,
                align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    return '<input disabled="disabled" type="checkbox"' + (cellvalue == "True" ? ' checked="checked"' : '') + '/>';
                }
            },
            {
                name: 'Active',
                index: 'Active',
                sorttype: "text",
                width: 50,
                align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    return '<input disabled="disabled" type="checkbox"' + (cellvalue == "True" ? ' checked="checked"' : '') + '/>';
                }
            }, {
                name: 'SibilingAtSchool',
                index: 'SibilingAtSchool',
                sorttype: "text",
                width: 60,
                align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    return '<input disabled="disabled" type="checkbox"' + (cellvalue == "True" ? ' checked="checked"' : '') + '/>';
                }
            }, {
                name: 'SpecialRequest',
                index: 'SpecialRequest',
                sorttype: "text",
                width: 60,
                align: 'center',
                formatter: function (cellvalue, options, rowObject) {
                    return '<input disabled="disabled" type="checkbox"' + (cellvalue == "True" ? ' checked="checked"' : '') + '/>';
                }
            }, {
                name: 'Distance',
                index: 'Distance',
                sorttype: "text",
                align: 'center',
                width: 150,
                formatter: function (cellvalue, options, rowObject) {
                    var c = parseInt(cellvalue);
                    var u = " m";
                    if (c >= 1000) {
                        c = (c / 1000).toFixed(2);
                        u = " km";
                    }
                    return c.toString() + u;
                }
            }, {
                name: 'Line',
                index: 'Line',
                sorttype: "text",
                width: 150
            }, {
                name: 'Action',
                index: 'Action',
                sortable: false,
                align: 'center',
                width: 175,
                formatter: function (cellvalue, options, rowObject) {
                    var id = options.rowId;
                    var res = "<a href='javascript:stbl.editFamily(" + id + ")'>Family</a>";
                    res += "&nbsp;";
                    res += "<a href='javascript:stbl.edit(" + id + ")'>Edit</a>";
                    res += "&nbsp;";
                    res += "<a href='javascript:stbl.deleteStudent(" + id + ")'>Delete</a>";
                    return res;
                }
            }
            ],
            subGrid: true,
            subGridRowExpanded: stbl.openSubRow,
            postData: {
                Id: function () { return $("#tbStudentId").val(); },
                Name: function () { return $("#tbName").val() },
                Classes: function () {
                    var res = "";
                    $("#lstClasses").find("option:selected").each(function (i, e) {
                        res += $(e).val() + "|";
                    });
                    return res;
                },
                Shicvas: function () {
                    var res = "";
                    $("#lstShicvas").find("option:selected").each(function (i, e) {
                        res += $(e).val() + "|";
                    });
                    return res;
                },
                Lines: function () {
                    var res = "";
                    $("#lstLines").find("option:selected").each(function (i, e) {
                        res += $(e).val() + "|";
                    });
                    return res;
                },
                Stations: function () {
                    var res = "";
                    $("#lstStations").find("option:selected").each(function (i, e) {
                        res += $(e).val() + "|";
                    });
                    return res;
                },
                City: function () { return $("#tbCity").val() },
                Street: function () { return $("#tbStreet").val() },
                House: function () { return $("#tbHouse").val() },
                Active: function () { return $("#ddlActive").val() },
                PayStatus: function () { return $("#ddlPayStatus").val() },
                Subcidy: function () { return $("#ddlSubcidy").val() },
                Sibiling: function () { return $("#ddlSibiling").val() },
                Request: function () { return $("#ddlRequest").val() },
                DFSFrom: function () { return $("#tbDTSFrom").val() },
                DFSTo: function () { return $("#tbDTSTo").val() },
                DFStFrom: function () { return $("#tbDTStFrom").val() },
                DFStTo: function () { return $("#tbDTStTo").val() },
                Direction: function () { return $("#ddlDirection").val() }
            }
        });
    },
    openSubRow: function (subgridDivId, rowId) {
        $("<div id='tabsSRow" + rowId + "'><ul><li><a href='#srtabs-1" + rowId + "'>Lines</a></li><li><a href='#srtabs-2" + rowId + "'>Siblings</a></li></ul><div id='srtabs-1" + rowId + "'></div><div id='srtabs-2" + rowId + "'></div></div>").appendTo("#" + subgridDivId);
        $("#tabsSRow" + rowId ).tabs();
        $("<table id='tblLines" + rowId + "'></table><div id='pgLines" + rowId + "'></div>").appendTo("#srtabs-1" + rowId + "");
        $("<table id='tblSibiling" + rowId + "'></table><div id='pgSibiling" + rowId + "'></div>").appendTo("#srtabs-2" + rowId + "");
        $("#tblLines" + rowId + "").jqGrid({
            datatype: 'json',
            height: '100%',
            regional: 'il',
            hidegrid: false,
            multiselect: false,
            pager: '#pgLines' +rowId,
            mtype: 'post',
            rowNum: 10,
            rowList: [10, 25, 50],
            viewrecords: true,
            width: '100%',
            loadui: 'enable',
            altRows: false,
            sortable: true,
            sortname: "Number",
            sortorder: "asc",
            altclass: "ui-state-default",
            url: "/tblStudent/GetLinesForStudent?studentId=" + rowId,
            colNames: ["Color", "Number", "Name", "Direction", "Date"],
            colModel: [{
                name: 'Color',
                index: 'Color',
                sorttype: "text",
                align: 'center',
                width: 75,
                formatter: function (cellvalue, options, rowObject) {
                    
                    var color = stbl.fixCssColor(cellvalue);
                    console.log(color);
                    return '<div style="width:46px; height:10px;background-color:' + color + '" title="' + color + '"></div>';
                }
            }, {
                name: 'Number',
                index: 'Number',
                sorttype: "text",
                align: 'center',
                width: 100
            }, {
                name: 'Name',
                index: 'Name',
                sorttype: "text",
                align: 'center',
                width: 100
            }, {
                name: 'Direction',
                index: 'Direction',
                sorttype: "text",
                align: 'center',
                width: 100
            }, {
                name: 'Date',
                index: 'Date',
                sorttype: "text",
                align: 'center',
                width: 150
            }
            ]
        });
        $("#tblSibiling" + rowId).jqGrid({
            datatype: 'json',
            height: '100%',
            regional: 'il',
            hidegrid: false,
            multiselect: false,
            pager: '#pgSibiling' + rowId,
            mtype: 'post',
            rowNum: 25,
            rowList: [10, 25, 50, 100],
            viewrecords: true,
            width: '100%',
            loadui: 'enable',
            altRows: false,
            sortable: true,
            sortname: "StudentId",
            sortorder: "asc",
            altclass: "ui-state-default",
            url: "/tblStudent/StudentsForFTable?studentId=" + rowId,
            colNames: ["Id", "Name", "Class", "Shicva", "Address"],
            colModel: [{
                name: 'StudentId',
                index: 'StudentId',
                sorttype: "text",
                align: 'center',
                width: 50
            }, {
                name: 'Name',
                index: 'Name',
                sorttype: "text",
                align: 'center',
                width: 130
            }, {
                name: 'Class',
                index: 'Class',
                sorttype: "text",
                align: 'center',
                width: 50
            }, {
                name: 'Shicva',
                index: 'Shicva',
                sorttype: "text",
                align: 'center',
                width: 50
            }, {
                name: 'Address',
                index: 'Address',
                sorttype: "text",
                align: 'center',
                width: 450
            }]
        });
    },
    fixCssColor: function (color) { //fix color for use in css properies
        if (color.substring(0, 1) != "#") color = "#" + color;
        return color;
    },
    edit: function (id) {
        $("#frmStudent").find("input[name=pk]").val(id);

        if (id > 0) {
            $.get("/tblStudent/GetStudent/" + id).done(function (loader) {
                var st = $.parseJSON(loader);
                for (var key in st) {
                    var ctrl = $("#frmStudent").find("input[name=" + key + "]");

                    if ($(ctrl).attr("type") == "text") $(ctrl).val(st[key]);
                    if ($(ctrl).attr("type") == "checkbox") $(ctrl).prop("checked", st[key]);
                }

                if (st.EmailConfirm == true) {
                    $("#indSE").removeClass("glyphicon-remove");
                    $("#indSE").addClass("glyphicon-ok");
                    $("#indSE").css("color", "green");
                    $("#indSE").attr("title", "E-mail confirmed");
                } else {
                    $("#indSE").removeClass("glyphicon-ok");
                    $("#indSE").addClass("glyphicon-remove");
                    $("#indSE").css("color", "red");
                    $("#indSE").attr("title", "E-mail has not been confirmed");
                }

                if (st.CellConfirm == true) {
                    $("#indSP").removeClass("glyphicon-remove");
                    $("#indSP").addClass("glyphicon-ok");
                    $("#indSP").css("color", "green");
                    $("#indSP").attr("title", "Phone confirmed");
                } else {
                    $("#indSP").removeClass("glyphicon-ok");
                    $("#indSP").addClass("glyphicon-remove");
                    $("#indSP").css("color", "red");
                    $("#indSP").attr("title", "Phone has not been confirmed");
                }

                stbl.openStudentDialog(st.familyId);

            });
        } else {
            document.getElementById("frmStudent").reset();
            stbl.openStudentDialog();
        }

    },
    openStudentDialog: function (familyId) {
        $("#ddlFamily").empty();
        $("#ddlFamily").append("<option value='0'>-- Select family--</option>");
        for (var i in stbl.families) {
            var f = stbl.families[i];
            var opt = "<option value='" + f.Id + "' ";
            if (f.Id == familyId) opt += "selected='selected'";
            opt += ">" + f.Name + "</option>";
            $("#ddlFamily").append(opt);
        }
        var dlg = $("#dlgStudent").dialog({
            autoOpen: true,
            width: 840,
            modal: true,
            buttons: {
                "Save": function () {
                    stbl.saveStudent();
                    dlg.dialog("close");
                    stbl.reload();
                },
                Cancel: function () {
                    document.getElementById("frmStudent").reset();
                    dlg.dialog("close");
                }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    editFamily: function (studentId) {
        if (studentId == 0) {
            document.getElementById("frmFamily").reset();
            stbl.showSecondParent();
        } else {
            $.get("/tblStudent/GetFamily/" + studentId).done(function (loader) {
                var st = $.parseJSON(loader);
                if (st != null) {
                    $("#frmFamily").find("input[name=familyId]").val(st.familyId);
                    for (var key in st) {
                        var ctrl = $("#frmFamily").find("input[name=" + key + "]");

                        if ($(ctrl).attr("type") == "text") $(ctrl).val(st[key]);
                        if ($(ctrl).attr("type") == "checkbox") $(ctrl).prop("checked", st[key]);
                    }
                    console.log(st);
                    if (st.parent1EmailConfirm == true) {
                        $("#indSE1").removeClass("glyphicon-remove");
                        $("#indSE1").addClass("glyphicon-ok");
                        $("#indSE1").css("color", "green");
                        $("#indSE1").attr("title", "E-mail confirmed");
                    } else {
                        $("#indSE1").removeClass("glyphicon-ok");
                        $("#indSE1").addClass("glyphicon-remove");
                        $("#indSE1").css("color", "red");
                        $("#indSE1").attr("title", "E-mail has not been confirmed");
                    }

                    if (st.parent1CellConfirm == true) {
                        $("#indSP1").removeClass("glyphicon-remove");
                        $("#indSP1").addClass("glyphicon-ok");
                        $("#indSP1").css("color", "green");
                        $("#indSP1").attr("title", "Phone confirmed");
                    } else {
                        $("#indSP1").removeClass("glyphicon-ok");
                        $("#indSP1").addClass("glyphicon-remove");
                        $("#indSP1").css("color", "red");
                        $("#indSP1").attr("title", "Phone has not been confirmed");
                    }

                    if (st.parent2EmailConfirm == true) {
                        $("#indSE2").removeClass("glyphicon-remove");
                        $("#indSE2").addClass("glyphicon-ok");
                        $("#indSE2").css("color", "green");
                        $("#indSE2").attr("title", "E-mail confirmed");
                    } else {
                        $("#indSE2").removeClass("glyphicon-ok");
                        $("#indSE2").addClass("glyphicon-remove");
                        $("#indSE2").css("color", "red");
                        $("#indSE2").attr("title", "E-mail has not been confirmed");
                    }

                    if (st.parent2CellConfirm == true) {
                        $("#indSP2").removeClass("glyphicon-remove");
                        $("#indSP2").addClass("glyphicon-ok");
                        $("#indSP2").css("color", "green");
                        $("#indSP2").attr("title", "Phone confirmed");
                    } else {
                        $("#indSP2").removeClass("glyphicon-ok");
                        $("#indSP2").addClass("glyphicon-remove");
                        $("#indSP2").css("color", "red");
                        $("#indSP2").attr("title", "Phone has not been confirmed");
                    }
                    stbl.showFStudentsGrid(st.familyId);
                } else {
                    document.getElementById("frmFamily").reset();

                    $("#indSE1").removeClass("glyphicon-ok");
                    $("#indSE1").addClass("glyphicon-remove");
                    $("#indSE1").css("color", "red");
                    $("#indSE1").attr("title", "E-mail has not been confirmed");

                    $("#indSP1").removeClass("glyphicon-ok");
                    $("#indSP1").addClass("glyphicon-remove");
                    $("#indSP1").css("color", "red");
                    $("#indSP1").attr("title", "Phone has not been confirmed");

                    $("#indSE2").removeClass("glyphicon-ok");
                    $("#indSE2").addClass("glyphicon-remove");
                    $("#indSE2").css("color", "red");
                    $("#indSE2").attr("title", "E-mail has not been confirmed");

                    $("#indSP2").removeClass("glyphicon-ok");
                    $("#indSP2").addClass("glyphicon-remove");
                    $("#indSP2").css("color", "red");
                    $("#indSP2").attr("title", "Phone has not been confirmed");
                }
                stbl.showSecondParent();
            });

        }

        var dlg = $("#dlgFamily").dialog({
            autoOpen: true,
            width: 840,
            height: 400,
            modal: true,
            buttons: {
                "Save": function () {
                    stbl.saveFamily();
                    dlg.dialog("close");
                },
                Cancel: function () { dlg.dialog("close"); }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    showFStudentsGrid: function (familyId) {
        if (stbl.FStudentGrid == null) {
            stbl.FStudentGrid = $("#tblFStudents").jqGrid({
                datatype: 'json',
                height: '100%',
                regional: 'il',
                hidegrid: false,
                multiselect: false,
                pager: '#pgFStudents',
                mtype: 'post',
                rowNum: 25,
                rowList: [10, 25, 50, 100],
                viewrecords: true,
                width: '100%',
                loadui: 'enable',
                altRows: false,
                sortable: true,
                sortname: "StudentId",
                sortorder: "asc",
                altclass: "ui-state-default",
                url: "/tblStudent/StudentsForFTable?familyId=" + familyId,
                colNames: ["Id", "Name", "Class", "Shicva", "Address"],
                colModel: [{
                    name: 'StudentId',
                    index: 'StudentId',
                    sorttype: "text",
                    align: 'center',
                    width: 50
                }, {
                    name: 'Name',
                    index: 'Name',
                    sorttype: "text",
                    align: 'center',
                    width: 130
                }, {
                    name: 'Class',
                    index: 'Class',
                    sorttype: "text",
                    align: 'center',
                    width: 50
                }, {
                    name: 'Shicva',
                    index: 'Shicva',
                    sorttype: "text",
                    align: 'center',
                    width: 50
                }, {
                    name: 'Address',
                    index: 'Address',
                    sorttype: "text",
                    align: 'center',
                    width: 450
                }]
            });
        } else {
            stbl.FStudentGrid.setGridParam({ url: "/tblStudent/StudentsForFTable?familyId=" + familyId }).trigger("reloadGrid");
        }
    },
    deleteStudent: function (id) {
        $("#hfConfirmId").val(id);
        var dlg = $("#dlgConfirm").dialog({
            autoOpen: true,
            width: 300,
            modal: true,
            buttons: {
                "Delete": function () {
                    $.get("/tblStudent/DeleteStudent/" + $("#hfConfirmId").val()).done(function (loader) {
                        dlg.dialog("close");
                        stbl.reload();
                    });
                    dlg.dialog("close");
                },
                Cancel: function () { dlg.dialog("close"); }
            }
        });
        $(".ui-dialog-buttonset").children("button").addClass("btn btn-default");
    },
    reload: function () {
        stbl.grid.trigger('reloadGrid');
        return false;
    },
    showSecondParent: function () {
        var c = $("#cbOneParent").prop("checked");
        if (c) {
            $("div[rel=secondParent]").hide();
        } else {
            $("div[rel=secondParent]").show();
        }
    },
    saveFamily: function () {
        var data = $("#frmFamily").serialize();
        $.post("/tblStudent/SaveFamily", data).done(function (loader) {

        });
    },
    saveStudent: function () {
        var data = $("#frmStudent").serialize();
        $.post("/tblStudent/SaveStudent", data).done(function (loader) {

        });
    }
}