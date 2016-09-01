var AngularApp;
(function (AngularApp) {
    var col = TSNetLike.Collections;
    var fnc = TSNetLike.Functors;
    function CloneRequestArgs(data) {
        return {
            urlalias: data.urlalias,
            params: data.params,
            before: data.before,
            onSucces: data.onSucces,
            onFailed: data.onFailed
        };
    }
    /**
     This Class used for simultaneously running requests to server.
     It runs it's callback only when all binded requests will be ended.
     */
    var ConcurentRequestHandler = (function () {
        /**
        callOnlyOnce = true, means if callback WAS CALLED already, it won't be called again.
        callOnlyOnce = false, if all requests ended already, callback will be called, even if it was called already.
        */
        function ConcurentRequestHandler(callback, callOnlyOnce) {
            var _this = this;
            if (callOnlyOnce === void 0) { callOnlyOnce = true; }
            this.callback = callback;
            this.callOnlyOnce = callOnlyOnce;
            this._registered = 0;
            this.RegisterRequestStart = function () {
                _this._registered++;
            };
            this.TryCall = function (response) {
                _this._registered--;
                if (_this._registered < 1) {
                    if (_this.callOnlyOnce && _this._registered == 0)
                        fnc.F(_this.callback, response);
                    else if (!_this.callOnlyOnce)
                        fnc.F(_this.callback, response);
                }
            };
        }
        return ConcurentRequestHandler;
    }());
    AngularApp.ConcurentRequestHandler = ConcurentRequestHandler;
    function IsConcurentRequestHandler(cb) {
        return !(AngularApp.IsNullOrUndefined(cb) || AngularApp.IsNullOrUndefined(cb.TryCall));
    }
    function RunCallbackOrHandler(callback, response) {
        //argument type check
        if (AngularApp.IsNullOrUndefined(callback))
            return;
        var cb = callback;
        if (IsConcurentRequestHandler(cb))
            cb.TryCall(response);
        else
            fnc.F(cb, response);
    }
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
            this.request_msgHandlerSucces = null;
            this.request_msgHandlerFail = null;
            this.request = function (holdTillResponse, data) {
                if (IsConcurentRequestHandler(data.onSucces))
                    data.onSucces.RegisterRequestStart();
                if (IsConcurentRequestHandler(data.onFailed))
                    data.onFailed.RegisterRequestStart();
                if (holdTillResponse)
                    _this.turnHoldView(true);
                fnc.F(data.before);
                _this.http({ method: 'POST', url: _this.url(data.urlalias), data: data.params }).
                    then(function (response) {
                    if (_this.request_msgHandlerSucces !== null
                        && !AngularApp.IsNullOrUndefined(response.data.message))
                        _this.request_msgHandlerSucces(response.data.message);
                    RunCallbackOrHandler(data.onSucces, response);
                    if (holdTillResponse)
                        _this.turnHoldView(false);
                }, function (response) {
                    if (!AngularApp.IsNullOrUndefined(response.data)) {
                        if (_this.request_msgHandlerFail !== null
                            && !AngularApp.IsNullOrUndefined(response.data.message))
                            _this.request_msgHandlerFail(response.data.message);
                        RunCallbackOrHandler(data.onSucces, response);
                    }
                    else if (_this.request_msgHandlerFail !== null)
                        _this.request_msgHandlerFail("no connection to server");
                    if (holdTillResponse)
                        _this.turnHoldView(false);
                });
            };
            /**looks on response.data.items*/
            this.fetchtodict = function (holdTillResponse, data, container, keyValueSelector, clearContainer) {
                if (IsConcurentRequestHandler(data.onSucces))
                    data.onSucces.RegisterRequestStart();
                if (IsConcurentRequestHandler(data.onFailed))
                    data.onFailed.RegisterRequestStart();
                var successCB = data.onSucces;
                data = CloneRequestArgs(data);
                data.onSucces = function (response) {
                    if (clearContainer)
                        container.clear();
                    container.addrange(response.data.items, keyValueSelector);
                    RunCallbackOrHandler(successCB, response);
                };
                _this.request(holdTillResponse, data);
            };
            /**looks on response.data.items*/
            this.fetchtoarr = function (holdTillResponse, data, container, clearContainer) {
                if (IsConcurentRequestHandler(data.onSucces))
                    data.onSucces.RegisterRequestStart();
                if (IsConcurentRequestHandler(data.onFailed))
                    data.onFailed.RegisterRequestStart();
                var successCB = data.onSucces;
                data = CloneRequestArgs(data);
                data.onSucces = function (response) {
                    if (clearContainer)
                        container.splice(0, container.length);
                    response.data.items.forEach(function (e) { return container.push(e); });
                    RunCallbackOrHandler(successCB, response);
                };
                _this.request(holdTillResponse, data);
            };
            /**looks on response.data.items*/
            this.updatetoarr = function (holdTillResponse, data, container, equalityPredicate, pushnew) {
                if (pushnew === void 0) { pushnew = true; }
                if (IsConcurentRequestHandler(data.onSucces))
                    data.onSucces.RegisterRequestStart();
                if (IsConcurentRequestHandler(data.onFailed))
                    data.onFailed.RegisterRequestStart();
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
                    RunCallbackOrHandler(successCB, response);
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
