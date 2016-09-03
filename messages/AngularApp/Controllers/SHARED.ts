namespace AngularApp.Controllers {

    export function formatValsOps (items: ValOp[], typeName: string) {
        switch (typeName) {
            case "int":
                break
            case "nvarchar":
                break
            case "datetime":
            case "date":
                for (let i = 0; i < items.length; i++) {
                    items[i].Value = formatVal(items[i].Value,typeName)
                }
                break
            case "time":
                break
            case "bit":
                break
        }
    }

    export function formatVal(value, typeName: string) {
        switch (typeName) {
            case "int":
                return value
            case "nvarchar":
                return value
            case "datetime":
            case "date":
                if (IsNullOrUndefined(value))
                    return null
                let s = value as string
                if (s.lastIndexOf && s.lastIndexOf("/Date", 0) === 0)
                    return new Date(parseInt(s.substr(6)))
                break
            case "time":
                return value
            case "bit":
                return value
        }
        return value
    }

    export function glyphiconforSQLTYPE (typeName: string) {
        switch (typeName) {
            case "int":
                return "glyphicon glyphicon-certificate"
            case "nvarchar":
                return "glyphicon glyphicon-font"
            case "datetime":
            case "date":
                return "glyphicon glyphicon-calendar"
            case "time":
                return "glyphicon glyphicon-time"
            case "bit":
                return "glyphicon glyphicon-check"
        }
        return "glyphicon glyphicon-briefcase"
    }

    export function inputTypeForSQLType (typeName: string){
        switch (typeName) {
            case "int":
                return "number"
            case "nvarchar":
                return "text"
            case "datetime":
            case "date":
                return "datetime-local"
            case "time":
                return "time"
            case "bit":
                return "checkbox"
        }
        return "text"
    }

    export function InputBoxInsertTextAtCursor (textArea, text: string) {
        //TODO check in IE
        //IE support
        let doc = document as any
        if (doc.selection) {
            textArea.focus()
            let sel = doc.selection.createRange()
            sel.text = text
        }
        //MOZILLA and others
        else if (textArea.selectionStart || textArea.selectionStart == '0') {
            let startPos = textArea.selectionStart
            let endPos = textArea.selectionEnd
            textArea.value = textArea.value.substring(0, startPos)
                + text
                + textArea.value.substring(endPos, textArea.value.length);
        }
        else {
            textArea.value += text;
        }
    }

    export function GetFilterValueCont (containerOnwer: IHasFilterValueContainer, filt: FilterVM) {
        let output = containerOnwer.FilterValueContainers.first(x => x.FilterId === filt.Id)
        if (output !== undefined) {
            if (IsNullOrUndefined(output.Values))
                output.Values = []
        }
        else {
            output = { FilterId: filt.Id, Values: [] }
            containerOnwer.FilterValueContainers.push(output)
        }
        return output
    }

    export function SwitchFilterValueContVal(containerOnwer: IHasFilterValueContainer, filt: FilterVM, index: number) {
        let val = GetFilterValueCont(containerOnwer,filt)
        if (!filt.allowMultipleSelection)
            val.Values = []
        val.Values[index] = IsNullOrUndefined(val.Values[index]) ?
            filt.ValsOps[index].Value
            : null
    }

    export function HasFilterValueContVal(containerOnwer: IHasFilterValueContainer, filt: FilterVM, value: any) {
        return GetFilterValueCont(containerOnwer,filt).Values.any(x => x === value)
    }

}