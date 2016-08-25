var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        function formatValsOps(items, typeName) {
            switch (typeName) {
                case "int":
                    break;
                case "nvarchar":
                    break;
                case "datetime":
                case "date":
                    for (var i = 0; i < items.length; i++) {
                        items[i].Value = formatVal(items[i].Value, typeName);
                    }
                    break;
                case "time":
                    break;
                case "bit":
                    break;
            }
        }
        Controllers.formatValsOps = formatValsOps;
        function formatVal(value, typeName) {
            switch (typeName) {
                case "int":
                    return value;
                case "nvarchar":
                    return value;
                case "datetime":
                case "date":
                    var s = value;
                    if (s.lastIndexOf && s.lastIndexOf("/Date", 0) === 0)
                        return new Date(parseInt(s.substr(6)));
                    break;
                case "time":
                    return value;
                case "bit":
                    return value;
            }
            return value;
        }
        Controllers.formatVal = formatVal;
        function glyphiconforSQLTYPE(typeName) {
            switch (typeName) {
                case "int":
                    return "glyphicon glyphicon-certificate";
                case "nvarchar":
                    return "glyphicon glyphicon-font";
                case "datetime":
                case "date":
                    return "glyphicon glyphicon-calendar";
                case "time":
                    return "glyphicon glyphicon-time";
                case "bit":
                    return "glyphicon glyphicon-check";
            }
            return "glyphicon glyphicon-briefcase";
        }
        Controllers.glyphiconforSQLTYPE = glyphiconforSQLTYPE;
        function inputTypeForSQLType(typeName) {
            switch (typeName) {
                case "int":
                    return "number";
                case "nvarchar":
                    return "text";
                case "datetime":
                case "date":
                    return "datetime";
                case "time":
                    return "time";
                case "bit":
                    return "checkbox";
            }
            return "text";
        }
        Controllers.inputTypeForSQLType = inputTypeForSQLType;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=SHARED.js.map