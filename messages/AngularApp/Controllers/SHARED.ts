namespace AngularApp.Controllers {

    export function formatValsOps (items: ValOp[], typeName: string) {
        switch (typeName) {
            case "int":
                break
            case "nvarchar":
                break
            case "datetime":
            case "date":
                items.forEach(x => {
                    let s = x.Value as string
                    if (s.lastIndexOf && s.lastIndexOf("/Date", 0) === 0)
                        x.Value = new Date(parseInt(x.Value.substr(6)))
                })
                break
            case "time":
                break
            case "bit":
                break
        }
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

}