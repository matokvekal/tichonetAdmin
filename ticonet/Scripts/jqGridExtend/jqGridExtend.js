$.fn.extend({
    rowActionsExtended: function (b) {
        var a = $;
        var c = a(this).closest("tr.jqgrow"),
            d = c.attr("id"),
            e = a(this).closest("table.ui-jqgrid-btable").attr("id").replace(/_frozen([^_]*)$/, "$1"),
            f = a("#" + e),
            g = f[0],
            h = g.p,
            i = h.colModel[a.jgrid.getCellIndex(this)],
            j = i.frozen ? a("tr#" + d + " td:eq(" + a.jgrid.getCellIndex(this) + ") > div", f) : a(this).parent(),
            k = {
                extraparam: {}
            },
            l = function (b, c) {
                a.isFunction(k.afterSave) && k.afterSave.call(g, b, c);
                j.find("div.ui-inline-edit,div.ui-inline-del").show();
                j.find("div.ui-inline-save,div.ui-inline-cancel").hide();
                if (f.jqGrid("getGridParam", "datatype") !== "local") {
                    f.trigger("reloadGrid");
                }
            },
            m = function (b) {
                a.isFunction(k.afterRestore) && k.afterRestore.call(g, b);
                j.find("div.ui-inline-edit,div.ui-inline-del").show();
                j.find("div.ui-inline-save,div.ui-inline-cancel").hide();
            };
        void 0 !== i.formatoptions && (k = a.extend(k, i.formatoptions));
        void 0 !== h.editOptions && (k.editOptions = h.editOptions);
        void 0 !== h.delOptions && (k.delOptions = h.delOptions);
        c.hasClass("jqgrid-new-row") && (k.extraparam[h.prmNames.oper] = h.prmNames.addoper);
        var n = {
            //keys: k.keys,
            oneditfunc: k.onEdit,
            successfunc: k.onSuccess,
            url: k.url,
            extraparam: k.extraparam,
            aftersavefunc: l,
            errorfunc: k.onError,
            afterrestorefunc: m,
            restoreAfterError: k.restoreAfterError,
            mtype: k.mtype,
            keys: true,
            reloadAfterSubmit: true,
        };
        switch (b) {
            case "edit":
                f.jqGrid("editRow", d, n);
                j.find("div.ui-inline-edit,div.ui-inline-del").hide();
                j.find("div.ui-inline-save,div.ui-inline-cancel").show();
                f.triggerHandler("jqGridAfterGridComplete");
                break;
            case "save":
                f.jqGrid("saveRow", d, n);
                j.find("div.ui-inline-edit,div.ui-inline-del").show();
                j.find("div.ui-inline-save,div.ui-inline-cancel").hide();
                f.triggerHandler("jqGridAfterGridComplete");
                break;
            case "cancel":
                f.jqGrid("restoreRow", d, m);
                j.find("div.ui-inline-edit,div.ui-inline-del").show();
                j.find("div.ui-inline-save,div.ui-inline-cancel").hide();
                f.triggerHandler("jqGridAfterGridComplete");
                break;
            case "del":
                f.jqGrid("delGridRow", d, k.delOptions);
                break;
            case "formedit":
                f.jqGrid("setSelection", d);
                f.jqGrid("editGridRow", d, k.editOptions);
        }
    }
});

//var gridExtend = new jqGridExtend(gridSelector);
var jqGridExtend = (function () {
    function jqGridExtend(gridSelector) {
        this.lastSelection = -1;
        this.grid = gridSelector;
    }
    jqGridExtend.prototype.init = function () {
        //Hide buttons "Clear search"
        $(".ui-search-clear").remove();
        this.lastSelection = -1;
    };
    jqGridExtend.prototype.restore = function (id) {
        if (id && id !== this.lastSelection) {
            $("div.ui-inline-edit,div.ui-inline-del", this.grid).not("#jEditButton_" + id).not("#jDeleteButton_" + id).show();
            $("div.ui-inline-save,div.ui-inline-cancel", this.grid).not("#jSaveButton_" + id).not("#jCancelButton_" + id).hide();
            this.grid.triggerHandler("jqGridAfterGridComplete");
            this.grid.jqGrid("restoreRow", this.lastSelection);
            this.lastSelection = id;
        }
    };
    jqGridExtend.prototype.editRow = function (id) {
        $.fn.rowActionsExtended.call($(this.grid.find("tbody").find("#" + id + " div.ui-inline-edit")), "edit");
    };
    jqGridExtend.prototype.populateDescription = function (data) {
        var colModel = this.grid.getGridParam("colModel");
        var grid = this.grid;
        $.each(colModel, function (index, col) {
            if (col.edittype === "select") {
                $.each(grid.getDataIDs(), function (index, id) {
                    var row = grid.getRowData(id);
                    var value = row[col.name];
                    var text = row[col.name + "Description"];
                    row[col.name] = text;
                    grid.setRowData(id, row);
                });
            }
        });
    };
    return jqGridExtend;
}());

