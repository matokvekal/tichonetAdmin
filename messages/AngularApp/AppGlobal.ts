namespace AngularApp {
    import col = TSNetLike.Collections
    import fnc = TSNetLike.Functors

    /**returns true if string is undefined, null, empty, or contains only spaces*/
    export function isEmptyOrSpaces(str: string) {
        return typeof str === 'undefined' || str === null || str.match(/^ *$/) !== null;
    }

    export function CloneShallow<T>(original: T) {
        let clone: any = {}
        for (let key in original) {
            if (original.hasOwnProperty(key)) {
                clone[key] = original[key]
            }
        }
        return clone as T
    }

    /**this doesnt handles recoursive references!*/
    export function CloneDeep<T>(original: T) {
        let clone: any = {}
        for (let key in original) {
            if (original.hasOwnProperty(key)) {
                if (original[key] instanceof Array) {
                    clone[key] = [];
                    let arr: any[] = clone[key];
                    original[key].forEach(ele => arr.push(CloneDeep(ele)))
                }
                else if (typeof original[key] === "object")
                    clone[key] = CloneDeep(original[key])
                else
                    clone[key] = original[key]
            }
        }
        return clone as T
    }

    export function ParseHtmlID(fullID: string, separator: string = "___::::___") {
        return fullID.split(separator)[1]
    }

    /**
    Returns Array:
    array[0] - prefix
    array[1] - id
     */
    export function ParseHtmlIDFull(fullID: string, separator: string = "___::::___") {
        return fullID.split(separator)
    }

    export function MakeHtmlID(prefix: string, id: string, separator: string = "___::::___") {
        return prefix + separator + id
    }

    export function IsNullOrUndefined(obj) {
        return typeof obj === 'undefined' || obj === null
    }

}