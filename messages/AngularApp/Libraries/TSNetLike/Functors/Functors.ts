namespace TSNetLike.Functors {

    export function fst(f, s) { return f }
    export function snd(f, s) { return s }

    export function F(f: (x?)=>any, x?) {
        if ('undefined' !== typeof f && f !== null) {
            if ('undefined' !== typeof x)
                f(x)
            else
                f()
        }
    }

    export function PF(defaultVal, f: (x?) => any, x?) {
        if ('undefined' !== typeof f && f !== null) {
            if ('undefined' !== typeof x)
                return f(x)
            else
                return f()
        }
        return defaultVal
    }

}