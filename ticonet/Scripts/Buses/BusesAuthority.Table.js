busesAuthority.table = {
    grid: null,
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
    //    busesAuthority.table.grid = $("#grBuses").jqGrid({
    //        datatype: "clientSide",
    //        height: "100%",
    //        autowidth: true,
    //        regional: "il",
    //        hidegrid: false,
    //        multiselect: false,
    //        pager: "#pgBuses",
    //        mtype: "post",
    //        rowNum: 10,
    //        rowList: [10, 20],
    //        viewrecords: true,
    //        //width: '100%',
    //        loadui: "disable",
    //        altRows: false,
    //        sortable: true,
    //        altclass: "ui-state-default",
    //        editurl: "/api/Buses/EditBus",
    //        afterSubmit: function () {
    //            $("#grBuses").jqGrid("setGridParam", { datatype: 'json' });
    //            return [true];
    //        },
    //        closeAfterEdit: true,
    //        colNames: ["BusId", "PlateNumber", "Owner", "seats", "price", "munifacturedate", "LicensingDueDate", "insuranceDueDate", "winterLicenseDueDate", "brakeTesDueDate"/*, ""*/],
    //        colModel: [
    //            {
    //                name: 'BusId',
    //                index: 'BusId',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'PlateNumber',
    //                index: 'PlateNumber',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'Owner',
    //                index: 'Owner',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'seats',
    //                index: 'seats',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'price',
    //                index: 'price',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'munifacturedate',
    //                index: 'munifacturedate',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'LicensingDueDate',
    //                index: 'LicensingDueDate',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'insuranceDueDate',
    //                index: 'insuranceDueDate',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'winterLicenseDueDate',
    //                index: 'winterLicenseDueDate',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            {
    //                name: 'brakeTesDueDate',
    //                index: 'brakeTesDueDate',
    //                sorttype: "text",
    //                width: 110,
    //                editable: true
    //            },
    //            //{
    //            //    name: "Id",
    //            //    index: 'Id',
    //            //    width: 75,
    //            //    formatter: busesAuthority.table.lineActionsFormatter,
    //            //    align: "center"
    //            //}
    //        ]
    //    });

    //    for (var k = 0; k < busesAuthority.items.list.length; k++) {
    //        $("#grBuses").jqGrid('addRowData', busesAuthority.items.list[k].Id, busesAuthority.items.list[k]);
    //    }

    //    //Default sorting
    //    $("#grBuses").jqGrid("sortGrid", "BusId");
    //    $("#grBuses").jqGrid('setGridParam', { sortorder: 'asc' });

    //    $("#grBuses").jqGrid('editRow', busesAuthority.table.lastSel,
    //    {
    //        keys: true,
    //        oneditfunc: function () {
    //            alert("edited");
    //        }
    //    });

    //    $("#grBuses").jqGrid('navGrid', '#pgBuses',
    //    {
    //    });

    //    ////Hide buttons "Clear search"
    //    //$(".ui-search-clear").remove();

    },
    //lineActionsFormatter: function (cellvalue, options, rowObject) {
    //    var res = "";
    //    res += "<span class='js-bus-edit-button-container-" + cellvalue + "'>";
    //    res += "<a href='javascript:busesAuthority.items.editBus(" + cellvalue + ")' title='Edit bus'><span class='glyphicon glyphicon-pencil'></span></a>";
    //    res += "&nbsp;&nbsp;";
    //    res += "<a href='javascript:busesAuthority.items.deleteBus(" + cellvalue + ")' title='Delete bus'><span class='glyphicon glyphicon-trash'></span></a>";
    //    res += "</span>";
    //    res += "<span class='js-bus-save-button-container-" + cellvalue + "' hidden>";
    //    res += "<a href='javascript:busesAuthority.items.saveBus(" + cellvalue + ")' title='Save bus'><span class='glyphicon glyphicon-ok'></span></a>";
    //    res += "&nbsp;&nbsp;";
    //    res += "<a href='javascript:busesAuthority.items.cancelEditBus(" + cellvalue + ")' title='Cancel'><span class='glyphicon glyphicon-remove'></span></a>";
    //    res += "</span>";
    //    return res;
    //}
}