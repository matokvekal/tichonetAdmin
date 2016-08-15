    interface Array<T> {
        firstIndex(predicate: (item: T) => boolean): number;
        first(predicate: (item: T) => boolean): T;
        remove(item: T): boolean;
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

    Array.prototype.remove = function (item) {
        let a = this;
        let i = a.indexOf(item);
        if (i === -1) return false
        a.splice(i, 1)
        return true
    }
    

