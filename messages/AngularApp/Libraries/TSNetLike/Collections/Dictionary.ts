namespace TSNetLike.Collections {
    import fnc = TSNetLike.Functors

    export interface IDictionary<TVal> {
        Count:number
        add(key: string, value: TVal): void
        addpair(pair: IKeyValuePair<string, TVal>) 
        addrange(range: any[],
            kv_creator: (e: any, i: number, a: any[]) => IKeyValuePair<string, TVal>) : void
        take(key: string) : TVal
        cont(key: string) : boolean
        rem(key: string) : void
        clear() : void
        iter(iterator: (k: string, v: TVal) => void) : void
        toarr(pusher: (k: string, v: TVal) => any) : TVal[]
        valstoarr(): TVal[]
        keystoarr(): string[]
    }

    export interface IKeyValuePair<TKey, TVal> {
        key: TKey
        val: TVal
    }

    export class Dictionary<TVal> implements IDictionary<TVal> {
        private raw : Object
        private count = 0
        get Count() { return this.count }

        constructor(rawObject = null) {
            if (rawObject !== null) {
                this.raw = rawObject
                this.iter(()=>this.count++)
            }
            else
                this.raw = {}
        }


        add (key: string, value: TVal) {
            if (!this.cont(key))
                this.count++
            this.raw[key] = value
        }

        addpair(pair: IKeyValuePair<string, TVal>) {
            this.add(pair.key,pair.val)
        }

        cont(key: string) {
            return 'undefined' !== typeof this.raw[key]
        }

        take(key: string): TVal {
            return this.raw[key]
        }

        addrange (range: any[], kv_creator: (e: any, i: number, a: any[]) => IKeyValuePair<string, TVal>){
            var it = this
            range.forEach( (ele, ind, arr) => {
                var pair = kv_creator(ele, ind, arr)
                it.add(pair.key, pair.val)
            })
        }

        clear() {
            this.raw = {}
        }

        rem(key: string) {
            if (this.cont(key)) {
                delete this.raw[key]
                this.count--
            }
        }
        iter (iterator: (k?: string, v?:TVal) => void) {
            for (var key in this.raw)
                if (this.raw.hasOwnProperty(key)) {
                    iterator(key, this.raw[key])
                }
        }
        toarr(pusher: (k?: string, v?: TVal) => any) {
            var arr = []
            this.iter((k, v) => arr.push(pusher(k, v)) )
            return arr
        }

        valstoarr(): TVal[] { return this.toarr(fnc.snd) }
        keystoarr(): string[] { return this.toarr(fnc.fst) }
    }
}