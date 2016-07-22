function ArrayToObject(arr, prefix, propSelector) {
    if (typeof propSelector === 'undefined')
        propSelector = function (item) { return item }
    var obj = {}
    arr.forEach(function (ele, ind, arr) {
        obj[prefix + ind] = propSelector(ele)
    })
    return obj
}

function AppendArrAsFields(obj, arr, prefix) {
    arr.forEach(function (ele, ind, arr) {
        obj[prefix + ind] = ele
    })
}

function AppendArrAsNamedFields(obj, arr, namesArr) {
    arr.forEach(function (ele, ind, arr) {
        obj[namesArr[ind]] = ele
    })
}

function PushPropertyLikeObj(arr, data, property, customName, propertyFunc) {
    if (typeof propertyFunc === 'undefined')
        propertyFunc = function (p) { return p }
    if (typeof customName !== 'undefined')
        arr.push({ Name: customName, Val: propertyFunc(data[property]) })
    else
        arr.push({ Name: property, Val: propertyFunc(data[property]) })
}



hackedfmatter_checkbox = function (b, c) {
    if (typeof b === 'number')
        return b

    var d, e = $.extend({}, c.checkbox);
    void 0 !== c.colModel && void
    0 !== c.colModel.formatoptions && (e = $.extend({}, e, c.colModel.formatoptions)), d = e.disabled === !0 ? 'disabled="disabled"' : "", ($.fmatter.isEmpty(b) || void
    0 === b) && (b = $.fn.fmatter.defaultFormat(b, e)), b = String(b), b = (b + "").toLowerCase();
    var f = b.search(/(false|f|0|no|n|off|undefined)/i) < 0 ? " checked='checked' " : "";
    return '<input type="checkbox" ' + f + ' value="' + b + '" offval="no" ' + d + "/>"
}