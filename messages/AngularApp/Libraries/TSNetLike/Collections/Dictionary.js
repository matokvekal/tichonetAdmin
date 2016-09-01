var TSNetLike;
(function (TSNetLike) {
    var Collections;
    (function (Collections) {
        var fnc = TSNetLike.Functors;
        var Dictionary = (function () {
            function Dictionary(rawObject) {
                var _this = this;
                if (rawObject === void 0) { rawObject = null; }
                this.count = 0;
                if (rawObject !== null) {
                    this.raw = rawObject;
                    this.iter(function () { return _this.count++; });
                }
                else
                    this.raw = {};
            }
            Object.defineProperty(Dictionary.prototype, "Count", {
                get: function () { return this.count; },
                enumerable: true,
                configurable: true
            });
            Dictionary.prototype.add = function (key, value) {
                if (!this.cont(key))
                    this.count++;
                this.raw[key] = value;
            };
            Dictionary.prototype.addpair = function (pair) {
                this.add(pair.key, pair.val);
            };
            Dictionary.prototype.cont = function (key) {
                return 'undefined' !== typeof this.raw[key];
            };
            Dictionary.prototype.take = function (key) {
                return this.raw[key];
            };
            Dictionary.prototype.addrange = function (range, kv_creator) {
                var it = this;
                range.forEach(function (ele, ind, arr) {
                    var pair = kv_creator(ele, ind, arr);
                    it.add(pair.key, pair.val);
                });
            };
            Dictionary.prototype.clear = function () {
                this.raw = {};
            };
            Dictionary.prototype.rem = function (key) {
                if (this.cont(key)) {
                    delete this.raw[key];
                    this.count--;
                }
            };
            Dictionary.prototype.iter = function (iterator) {
                for (var key in this.raw)
                    if (this.raw.hasOwnProperty(key)) {
                        iterator(key, this.raw[key]);
                    }
            };
            Dictionary.prototype.toarr = function (pusher) {
                var arr = [];
                this.iter(function (k, v) { return arr.push(pusher(k, v)); });
                return arr;
            };
            Dictionary.prototype.valstoarr = function () { return this.toarr(fnc.snd); };
            Dictionary.prototype.keystoarr = function () { return this.toarr(fnc.fst); };
            return Dictionary;
        }());
        Collections.Dictionary = Dictionary;
    })(Collections = TSNetLike.Collections || (TSNetLike.Collections = {}));
})(TSNetLike || (TSNetLike = {}));
