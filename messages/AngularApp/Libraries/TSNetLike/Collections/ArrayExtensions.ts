    interface Array<T> {
        firstIndex(predicate: (item: T) => boolean): number;
        /**returns undefined if no matching item found*/
        first(predicate: (item: T) => boolean): T;
        any(predicate: (item: T) => boolean): boolean;
        remove(item: T): boolean;
        max(selector: (item: T) => number): number;
        count(selector: (item: T) => number): number;
    }

    /** returns -1 if nothing was found*/
    Array.prototype.firstIndex = function (predicate) {
        if (this == null)
            throw new TypeError('Array.prototype.firstIndex called on null or undefined')
        if (typeof predicate !== 'function') 
            throw new TypeError('predicate must be a function')
        let list = Object(this)
        var length = list.length >>> 0
        for (var i = 0; i < length; i++) {
            if (predicate(list[i])) 
                return i
        }
        return -1;
    }
    
    Array.prototype.first = function (predicate) {
        let i = this.firstIndex(predicate)
        if (i === -1)
            return undefined
        return this[i]
    }

    Array.prototype.any = function (predicate) {
        return this.firstIndex(predicate) !== -1
    }

    Array.prototype.remove = function (item) {
        let a = this
        let i = a.indexOf(item)
        if (i === -1) return false
        a.splice(i, 1)
        return true
    }

    Array.prototype.max = function (selector: (item) => number) {
        let max = null;
        this.forEach(ele => {
            let a = selector(ele)
            if (max === null || a > max)
                max = a
        })
        return max === null ? 0 : max
    }

    Array.prototype.count = function (selector: (item) => number) {
        let count = 0;
        this.forEach(ele => {
            count += selector(ele)
        })
        return count
    }
    

