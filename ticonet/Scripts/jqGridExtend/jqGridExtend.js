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
                f.trigger("reloadGrid");
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
            reloadAfterSubmit: true
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

