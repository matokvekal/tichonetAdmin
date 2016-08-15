var AngularApp;
(function (AngularApp) {
    var col = TSNetLike.Collections;
    var fnc = TSNetLike.Functors;
    var FetchParams = (function () {
        function FetchParams() {
            var _this = this;
            this.Skip = null;
            this.Count = null;
            this.filters = [];
            this.addSkip = function (val) {
                _this.Skip = val;
                return _this;
            };
            this.addCount = function (val) {
                _this.Count = val;
                return _this;
            };
            this.addFilt = function (key, value, operator) {
                //let val = typeof value === 'undefined' ? null : value.toString()
                _this.filters.push({ key: key, val: value, op: operator });
                return _this;
            };
        }
        return FetchParams;
    }());
    AngularApp.FetchParams = FetchParams;
    function CloneRequestArgs(data) {
        return {
            urlalias: data.urlalias,
            params: data.params,
            before: data.before,
            onSucces: data.onSucces,
            onFailed: data.onFailed
        };
    }
    function CloneShallow(original) {
        var clone = {};
        for (var key in original) {
            if (original.hasOwnProperty(key)) {
                clone[key] = original[key];
            }
        }
        return clone;
    }
    AngularApp.CloneShallow = CloneShallow;
    /**this doesnt handles with recoursive references!*/
    function CloneDeep(original) {
        var clone = {};
        var _loop_1 = function(key) {
            if (original.hasOwnProperty(key)) {
                if (original[key] instanceof Array) {
                    clone[key] = [];
                    var arr_1 = clone[key];
                    original[key].forEach(function (ele) { return arr_1.push(CloneDeep(ele)); });
                }
                else if (typeof original[key] === "object")
                    clone[key] = CloneDeep(original[key]);
                else
                    clone[key] = original[key];
            }
        };
        for (var key in original) {
            _loop_1(key);
        }
        return clone;
    }
    AngularApp.CloneDeep = CloneDeep;
    function ParseHtmlID(fullID, separator) {
        if (separator === void 0) { separator = "___::::___"; }
        return fullID.split(separator)[1];
    }
    AngularApp.ParseHtmlID = ParseHtmlID;
    function MakeHtmlID(prefix, id, separator) {
        if (separator === void 0) { separator = "___::::___"; }
        return prefix + separator + id;
    }
    AngularApp.MakeHtmlID = MakeHtmlID;
    function IsNullOrUndefined(obj) {
        return typeof obj === 'undefined' || obj === null;
    }
    AngularApp.IsNullOrUndefined = IsNullOrUndefined;
    var Controller = (function () {
        function Controller(rootScope, scope, http) {
            var _this = this;
            this.rootScope = rootScope;
            this.scope = scope;
            this.http = http;
            this.initUrlModule = function (urls) {
                _this.urls = urls;
            };
            this.initUrlModuleFromRowObj = function (urls) {
                _this.urls = new col.Dictionary(urls);
            };
            this.url = function (alias) {
                return _this.urls.take(alias);
            };
            this.TurnHoldViewCount = 0;
            this.turnHoldView = function (state) {
                if (state)
                    _this.TurnHoldViewCount++;
                else
                    _this.TurnHoldViewCount--;
                if (_this.TurnHoldViewCount <= 0) {
                    _this.TurnHoldViewCount = 0;
                    _this.scope.holdview = false;
                }
                else {
                    _this.scope.holdview = true;
                }
                _this.rootScope.$broadcast('ControllerHoldedView', { controller: _this, state: state });
            };
            this.TurnHoldViewOnOthersControllers_on = false;
            this.TurnHoldViewOnOthersControllers = function () {
                if (_this.TurnHoldViewOnOthersControllers_on === true)
                    return;
                _this.TurnHoldViewOnOthersControllers_on = true;
                _this.ControllerHoldedView_unsubcribe = _this.rootScope.$on('ControllerHoldedView', function (event, args) {
                    if (args.controller === _this)
                        return;
                    _this.turnHoldView(args.state);
                });
            };
            /**This action works if page has Notification controller*/
            this.ShowNotification = function (header, body, type, showdelay) {
                var msg = new AngularApp.Controllers.NotifMessage(header, body, type, showdelay);
                _this.rootScope.$broadcast(AngularApp.Controllers.NotificationController.MSGEVENT, { message: msg });
            };
            this.request = function (holdTillResponse, data) {
                if (holdTillResponse)
                    _this.turnHoldView(true);
                fnc.F(data.before);
                _this.http({ method: 'POST', url: _this.url(data.urlalias), data: data.params }).
                    then(function (response) {
                    fnc.F(data.onSucces, response);
                    if (holdTillResponse)
                        _this.turnHoldView(false);
                }, function (response) {
                    fnc.F(data.onFailed, response);
                    if (holdTillResponse)
                        _this.turnHoldView(false);
                });
            };
            /**looks on response.data.items*/
            this.fetchtodict = function (holdTillResponse, data, container, keyValueSelector, clearContainer) {
                var successCB = data.onSucces;
                data = CloneRequestArgs(data);
                data.onSucces = function (response) {
                    if (clearContainer)
                        container.clear();
                    container.addrange(response.data.items, keyValueSelector);
                    fnc.F(successCB, response);
                };
                _this.request(holdTillResponse, data);
            };
            /**looks on response.data.items*/
            this.fetchtoarr = function (holdTillResponse, data, container, clearContainer) {
                var successCB = data.onSucces;
                data = CloneRequestArgs(data);
                data.onSucces = function (response) {
                    if (clearContainer)
                        container.splice(0, container.length);
                    response.data.items.forEach(function (e) { return container.push(e); });
                    fnc.F(successCB, response);
                };
                _this.request(holdTillResponse, data);
            };
            /**looks on response.data.items*/
            this.updatetoarr = function (holdTillResponse, data, container, equalityPredicate, pushnew) {
                if (pushnew === void 0) { pushnew = true; }
                var successCB = data.onSucces;
                data = CloneRequestArgs(data);
                data.onSucces = function (response) {
                    response.data.items.forEach(function (e1) {
                        var index = -1;
                        for (var i = 0; i < container.length; i++) {
                            if (equalityPredicate(e1, container[i])) {
                                index = i;
                                break;
                            }
                        }
                        if (index != -1)
                            container[index] = e1;
                        else if (pushnew)
                            container.push(e1);
                    });
                    fnc.F(successCB, response);
                };
                _this.request(holdTillResponse, data);
            };
            this.scope.holdview = false;
            this.va = this.buildVa();
            this.scope.va = this.va;
            this.scope.init = function (data) { return _this.init(data); };
            this.scope.notif = function (header, body, type, showdelay) {
                return _this.ShowNotification(header, body, type, showdelay);
            };
            scope.$on('$destroy', function () {
                //here we Dispose All Resources used by controller
                if (_this.TurnHoldViewOnOthersControllers_on)
                    _this.ControllerHoldedView_unsubcribe();
            });
        }
        Controller.$inject = ['$rootScope', '$scope', '$http'];
        return Controller;
    }());
    AngularApp.Controller = Controller;
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=Controller.js.map