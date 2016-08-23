var AngularApp;
(function (AngularApp) {
    function isEmptyOrSpaces(str) {
        return typeof str === 'undefined' || str === null || str.match(/^ *$/) !== null;
    }
    AngularApp.isEmptyOrSpaces = isEmptyOrSpaces;
    function CloneShallow(original) {
        var clone = {};
        for (var key in original) {
            if (original.hasOwnProperty(key)) {
                clone[key] = original[key];
            }
        }
        return clone;
    }
    AngularApp.CloneShallow = CloneShallow;
    /**this doesnt handles recoursive references!*/
    function CloneDeep(original) {
        var clone = {};
        var _loop_1 = function(key) {
            if (original.hasOwnProperty(key)) {
                if (original[key] instanceof Array) {
                    clone[key] = [];
                    var arr_1 = clone[key];
                    original[key].forEach(function (ele) { return arr_1.push(CloneDeep(ele)); });
                }
                else if (typeof original[key] === "object")
                    clone[key] = CloneDeep(original[key]);
                else
                    clone[key] = original[key];
            }
        };
        for (var key in original) {
            _loop_1(key);
        }
        return clone;
    }
    AngularApp.CloneDeep = CloneDeep;
    function ParseHtmlID(fullID, separator) {
        if (separator === void 0) { separator = "___::::___"; }
        return fullID.split(separator)[1];
    }
    AngularApp.ParseHtmlID = ParseHtmlID;
    /**
    Returns Array:
    array[0] - prefix
    array[1] - id
     */
    function ParseHtmlIDFull(fullID, separator) {
        if (separator === void 0) { separator = "___::::___"; }
        return fullID.split(separator);
    }
    AngularApp.ParseHtmlIDFull = ParseHtmlIDFull;
    function MakeHtmlID(prefix, id, separator) {
        if (separator === void 0) { separator = "___::::___"; }
        return prefix + separator + id;
    }
    AngularApp.MakeHtmlID = MakeHtmlID;
    function IsNullOrUndefined(obj) {
        return typeof obj === 'undefined' || obj === null;
    }
    AngularApp.IsNullOrUndefined = IsNullOrUndefined;
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=AppGlobal.js.map