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
                    items.forEach(function (x) {
                        var s = x.Value;
                        if (s.lastIndexOf && s.lastIndexOf("/Date", 0) === 0)
                            x.Value = new Date(parseInt(x.Value.substr(6)));
                    });
                    break;
                case "time":
                    break;
                case "bit":
                    break;
            }
        }
        Controllers.formatValsOps = formatValsOps;
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
                    return "datetime-local";
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