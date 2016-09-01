var TSNetLike;
(function (TSNetLike) {
    var Functors;
    (function (Functors) {
        function fst(f, s) { return f; }
        Functors.fst = fst;
        function snd(f, s) { return s; }
        Functors.snd = snd;
        function F(f, x) {
            if ('undefined' !== typeof f && f !== null) {
                if ('undefined' !== typeof x)
                    f(x);
                else
                    f();
            }
        }
        Functors.F = F;
        function PF(defaultVal, f, x) {
            if ('undefined' !== typeof f && f !== null) {
                if ('undefined' !== typeof x)
                    return f(x);
                else
                    return f();
            }
            return defaultVal;
        }
        Functors.PF = PF;
    })(Functors = TSNetLike.Functors || (TSNetLike.Functors = {}));
})(TSNetLike || (TSNetLike = {}));
