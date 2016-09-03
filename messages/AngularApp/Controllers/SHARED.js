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
                    if (AngularApp.IsNullOrUndefined(value))
                        return null;
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
                    return "datetime-local";
                case "time":
                    return "time";
                case "bit":
                    return "checkbox";
            }
            return "text";
        }
        Controllers.inputTypeForSQLType = inputTypeForSQLType;
        function InputBoxInsertTextAtCursor(textArea, text) {
            //TODO check in IE
            //IE support
            var doc = document;
            if (doc.selection) {
                textArea.focus();
                var sel = doc.selection.createRange();
                sel.text = text;
            }
            else if (textArea.selectionStart || textArea.selectionStart == '0') {
                var startPos = textArea.selectionStart;
                var endPos = textArea.selectionEnd;
                textArea.value = textArea.value.substring(0, startPos)
                    + text
                    + textArea.value.substring(endPos, textArea.value.length);
            }
            else {
                textArea.value += text;
            }
        }
        Controllers.InputBoxInsertTextAtCursor = InputBoxInsertTextAtCursor;
        function GetFilterValueCont(containerOnwer, filt) {
            var output = containerOnwer.FilterValueContainers.first(function (x) { return x.FilterId === filt.Id; });
            if (output !== undefined) {
                if (AngularApp.IsNullOrUndefined(output.Values))
                    output.Values = [];
            }
            else {
                output = { FilterId: filt.Id, Values: [] };
                containerOnwer.FilterValueContainers.push(output);
            }
            return output;
        }
        Controllers.GetFilterValueCont = GetFilterValueCont;
        function SwitchFilterValueContVal(containerOnwer, filt, index) {
            var val = GetFilterValueCont(containerOnwer, filt);
            if (!filt.allowMultipleSelection)
                val.Values = [];
            val.Values[index] = AngularApp.IsNullOrUndefined(val.Values[index]) ?
                filt.ValsOps[index].Value
                : null;
        }
        Controllers.SwitchFilterValueContVal = SwitchFilterValueContVal;
        function HasFilterValueContVal(containerOnwer, filt, value) {
            return GetFilterValueCont(containerOnwer, filt).Values.any(function (x) { return x === value; });
        }
        Controllers.HasFilterValueContVal = HasFilterValueContVal;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=SHARED.js.map