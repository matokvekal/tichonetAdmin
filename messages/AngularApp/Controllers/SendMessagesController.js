var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var fnc = TSNetLike.Functors;
        var SendMessagesVA = (function () {
            function SendMessagesVA() {
                this.mschedules = [];
                this.templates = [];
                this.repeatmodes = [];
                this.cursched = null;
                this.wildcards = [];
                this.filters = [];
                this.reccards = [];
                this.schedsHeader_ElemId = 'schedss_header';
                this.schedsBody_ElemId = 'schedss_body';
            }
            return SendMessagesVA;
        }());
        var SendMessagesController = (function (_super) {
            __extends(SendMessagesController, _super);
            function SendMessagesController($rootScope, $scope, $http) {
                var _this = this;
                _super.call(this, $rootScope, $scope, $http);
                this.refetchSchedules = function () {
                    _this.fetchtoarr(true, {
                        urlalias: "getmschedules",
                        onSucces: function () {
                            _this.va.mschedules.forEach(function (x) { return x.ScheduleDate = Controllers.formatVal(x.ScheduleDate, "datetime"); });
                        }
                    }, _this.va.mschedules, true);
                };
                this.newMSchdedule = function () {
                    var ms = {
                        Id: -1,
                        Name: "New Schedule",
                        FilterValueContainers: [],
                        InArchive: false,
                        IsActive: false,
                        IsSms: false,
                        MsgHeader: "",
                        MsgBody: "",
                        TemplateId: -1,
                        ChoosenReccards: [],
                        RepeatMode: _this.va.repeatmodes[0],
                        ScheduleDate: new Date(Date.now()),
                        BatchesCount: 0,
                        ng_JustCreated: true,
                        ng_ToDelete: false,
                    };
                    _this.va.cursched = ms;
                };
                this.editMschedule = function (sched) {
                    _this.va.cursched = sched;
                    var templ = Controllers.FindById(_this.va.templates, sched.TemplateId);
                    _this.refetchFilters(templ);
                    _this.refetchReccards(templ);
                    _this.refetchWildcards(templ, function () { return _this.scope.HandleCodeArea(); });
                };
                //
                this.turnOffSchedEdition = function () {
                    _this.va.cursched = null;
                };
                this.pushSched = function (sched, asNew, background, dontRefetch) {
                    if (background === void 0) { background = true; }
                    if (dontRefetch === void 0) { dontRefetch = false; }
                    var params = { models: [sched], mode: "" };
                    params.mode = asNew ? "cr" : "up";
                    var cb = dontRefetch ? null : function (response) { return _this.refetchSchedules(); };
                    _this.request(!background, {
                        urlalias: "mngmschedules",
                        params: params,
                        onSucces: cb
                    });
                };
                this.deleteSched = function (sched) {
                    if (_this.va.cursched !== null && _this.va.cursched.Id === sched.Id)
                        _this.turnOffSchedEdition();
                    var params = { models: [sched], mode: "dl" };
                    _this.request(true, {
                        urlalias: "mngmschedules",
                        params: params,
                        onSucces: function (response) { return _this.refetchSchedules(); }
                    });
                };
                //
                this.setTemplateToCurrent = function (templ) {
                    var sched = _this.va.cursched;
                    sched.TemplateId = templ.Id;
                    sched.MsgHeader = templ.MsgHeader;
                    sched.MsgBody = templ.MsgBody;
                    sched.ChoosenReccards = templ.ChoosenReccards;
                    sched.FilterValueContainers = templ.FilterValueContainers;
                    sched.IsSms = templ.IsSms;
                    _this.refetchFilters(templ);
                    _this.refetchReccards(templ);
                    _this.refetchWildcards(templ, function () { return _this.scope.HandleCodeArea(); });
                };
                this.refetchFilters = function (templ, onSucces) {
                    _this.fetchtoarr(true, {
                        urlalias: "getfilters",
                        params: new AngularApp.FetchParams()
                            .addFilt("tblRecepientFilterId", templ.RecepientFilterId)
                            .addFilt("allowUserInput", true),
                        onSucces: function (r) {
                            _this.va.filters.forEach(function (ele) {
                                Controllers.formatValsOps(ele.ValsOps, ele.Type);
                            });
                            fnc.F(onSucces, r);
                        }
                    }, _this.va.filters, true);
                };
                this.refetchReccards = function (templ) {
                    _this.fetchtoarr(true, {
                        urlalias: "getreccards",
                        params: new AngularApp.FetchParams().addFilt("tblRecepientFilterId", templ.RecepientFilterId),
                    }, _this.va.reccards, true);
                };
                this.refetchWildcards = function (templ, onSucces) {
                    _this.va.wildcards = [];
                    var callback = AngularApp.IsNullOrUndefined(onSucces) ? null : new AngularApp.ConcurentRequestHandler(onSucces, true);
                    _this.fetchtoarr(true, {
                        urlalias: "getwildcards",
                        params: new AngularApp.FetchParams().addFilt("tblRecepientFilterId", templ.RecepientFilterId),
                        onSucces: callback
                    }, _this.va.wildcards, false);
                    //this is a reserved wildcards, used for placing recepient credentials
                    //as convention them has negative ids <= -10
                    _this.fetchtoarr(true, { urlalias: "getreservedcards", onSucces: callback }, _this.va.wildcards, false);
                };
                this.hasRecepient = function (rc) {
                    return _this.va.cursched.ChoosenReccards.any(function (x) { return x === rc.Id; });
                };
                this.switchRecepient = function (rc) {
                    var has = _this.hasRecepient(rc);
                    if (has)
                        _this.va.cursched.ChoosenReccards.remove(rc.Id);
                    else
                        _this.va.cursched.ChoosenReccards.push(rc.Id);
                };
                this.shedulesTextDropped = function (dragID, dropID, dragClass) {
                    if (dragClass !== 'wildcard')
                        return;
                    var clearID = parseInt(AngularApp.ParseHtmlID(dragID, "_"));
                    var wc = _this.va.wildcards.first(function (x) { return x.Id === clearID; });
                    if (typeof wc === 'undefined')
                        return;
                    var textarea = document.getElementById(dropID);
                    Controllers.InputBoxInsertTextAtCursor(textarea, wc.Code);
                    var val = textarea.value;
                    //dunno why $apply doesnt work (cos 'ondrop' is already wrapped in $apply)
                    if (dropID === _this.va.schedsHeader_ElemId)
                        _this.va.cursched.MsgHeader = val;
                    else if (dropID === _this.va.schedsBody_ElemId)
                        _this.va.cursched.MsgBody = val;
                    textarea.focus();
                };
            }
            SendMessagesController.prototype.buildVa = function () { return new SendMessagesVA; };
            SendMessagesController.prototype.init = function (data) {
                //------------------- RequestMsgs
                var _this = this;
                this.request_msgHandlerSucces = function (msg) {
                    _this.ShowNotification("Info", msg, { glicon: "info-sign", nclass: "info" }, 3000);
                };
                this.request_msgHandlerFail = function (msg) {
                    _this.ShowNotification("Error", msg, { glicon: "ban-circle", nclass: "error" });
                };
                //------------------- Scope Init
                //---textArea Highlighting
                //this is temporary cos it should be in directive
                var TextArea;
                this.scope.InitCodeArea = function (divID) {
                    TextArea = new CodeArea(divID);
                    //setTimeout(() => TextArea.HandleInput(),100)
                };
                this.scope.HandleCodeArea = function () {
                    TextArea.HandleInput();
                };
                //---
                this.scope.CreateSched = this.newMSchdedule;
                this.scope.EditSched = this.editMschedule;
                this.scope.DeleteSched = this.deleteSched;
                this.scope.hideEditor = this.turnOffSchedEdition;
                this.scope.SaveSched = function (sched, boolDontRefetch) {
                    if (boolDontRefetch === void 0) { boolDontRefetch = true; }
                    sched = sched || _this.va.cursched;
                    //new from scratch template starts with id == -1
                    _this.pushSched(sched, sched.Id === -1, _this.va.cursched !== sched, boolDontRefetch);
                    if (_this.va.cursched !== null && _this.va.cursched.Id === sched.Id)
                        _this.turnOffSchedEdition();
                };
                this.scope.GetTemplName = function (id) { return Controllers.FindById(_this.va.templates, id).Name; };
                this.scope.GetRepeatMode = function (sched) { return _this.va.repeatmodes.first(function (x) { return x === sched.RepeatMode; }); };
                this.scope.setTempl = this.setTemplateToCurrent;
                this.scope.HasReccard = this.hasRecepient;
                this.scope.SwitchReccard = this.switchRecepient;
                this.scope.shedulesTextDropped = function (x, y, z) {
                    _this.shedulesTextDropped(x, y, z);
                    TextArea.HandleInput();
                };
                this.scope.InputType = function (SQLtype) { return Controllers.inputTypeForSQLType(SQLtype); };
                this.scope.GetFilterValueCont = function (filt) {
                    return Controllers.GetFilterValueCont(_this.va.cursched, filt);
                };
                this.scope.SwitchFilterValueContVal = function (filt, index) {
                    return Controllers.SwitchFilterValueContVal(_this.va.cursched, filt, index);
                };
                this.scope.HasFilterValueContVal = function (filt, value) {
                    return Controllers.HasFilterValueContVal(_this.va.cursched, filt, value);
                };
                //------------------- Inner Init
                this.initUrlModuleFromRowObj(data.urls);
                this.fetchtoarr(true, { urlalias: "gettemplates" }, this.va.templates, true);
                this.fetchtoarr(true, { urlalias: "getrepeatmodes" }, this.va.repeatmodes, true);
                this.refetchSchedules();
            };
            return SendMessagesController;
        }(AngularApp.Controller));
        Controllers.SendMessagesController = SendMessagesController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=SendMessagesController.js.map