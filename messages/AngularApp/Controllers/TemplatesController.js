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
        var TemplatesVA = (function () {
            function TemplatesVA() {
                this.templates = [];
                this.metafilters = [];
                this.curtemplate = null;
                this.wildcards = [];
                this.filters = [];
                this.reccards = [];
                this.templatesHeader_ElemId = 'templates_header';
                this.templatesBody_ElemId = 'templates_body';
                //
                this.demomsgs = [];
            }
            return TemplatesVA;
        }());
        var TemplatesController = (function (_super) {
            __extends(TemplatesController, _super);
            function TemplatesController($rootScope, $scope, $http) {
                var _this = this;
                _super.call(this, $rootScope, $scope, $http);
                this.refetchTemplates = function (onSucces) {
                    _this.fetchtoarr(true, {
                        urlalias: "gettemplates",
                        onSucces: onSucces,
                    }, _this.va.templates, true);
                };
                this.refetchMfilters = function (onSucces) {
                    _this.fetchtoarr(true, { urlalias: "getmfilters" }, _this.va.metafilters, true);
                };
                this.refetchFilters = function (mfilt, onSucces) {
                    _this.fetchtoarr(true, {
                        urlalias: "getfilters",
                        params: new AngularApp.FetchParams()
                            .addFilt("tblRecepientFilterId", mfilt.Id)
                            .addFilt("allowUserInput", true),
                        onSucces: function (r) {
                            _this.va.filters.forEach(function (ele) {
                                Controllers.formatValsOps(ele.ValsOps, ele.Type);
                            });
                            fnc.F(onSucces, r);
                        }
                    }, _this.va.filters, true);
                };
                this.refetchReccards = function (mfilt) {
                    _this.fetchtoarr(true, {
                        urlalias: "getreccards",
                        params: new AngularApp.FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                    }, _this.va.reccards, true);
                };
                this.refetchWildcards = function (mfilt) {
                    _this.va.wildcards = [];
                    _this.fetchtoarr(true, {
                        urlalias: "getwildcards",
                        params: new AngularApp.FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                    }, _this.va.wildcards, false);
                    //this is a reserved wildcards, used for placing recepient credentials
                    //as convention them has negative ids <= -10
                    _this.fetchtoarr(true, { urlalias: "getreservedcards" }, _this.va.wildcards, false);
                };
                this.turnTemplateCreate = function () {
                    _this.va.curtemplate = new Controllers.TemplateVM();
                    _this.va.curtemplate.Id = -1;
                    _this.va.filters = [];
                };
                this.setMFilter = function (mfilt) {
                    _this.va.curtemplate.RecepientFilterId = mfilt.Id;
                    _this.refetchReccards(mfilt);
                    _this.refetchWildcards(mfilt);
                    _this.refetchFilters(mfilt);
                    _this.va.curtemplate.FilterValueContainers = [];
                };
                this.turnTemplateEdit = function (templ, callback) {
                    _this.va.curtemplate = AngularApp.CloneShallow(templ);
                    var mfilt = _this.va.metafilters.first(function (x) { return x.Id === templ.RecepientFilterId; });
                    _this.refetchReccards(mfilt);
                    _this.refetchWildcards(mfilt);
                    _this.refetchFilters(mfilt, function (r) {
                        _this.va.filters.forEach(function (x) {
                            var filtValCont = _this.va.curtemplate.FilterValueContainers.first(function (y) { return y.FilterId === x.Id; });
                            if (filtValCont !== undefined && !AngularApp.IsNullOrUndefined(filtValCont.Values))
                                filtValCont.Values.forEach(function (ele, ind) { return filtValCont.Values[ind] = Controllers.formatVal(ele, x.Type); });
                        });
                    });
                    fnc.F(callback);
                };
                this.turnOffTemplateEdition = function () {
                    _this.va.curtemplate = null;
                };
                this.pushCurtemplate = function (asNew, onSucces) {
                    var params = { models: [_this.va.curtemplate], mode: "" };
                    params.mode = asNew ? "cr" : "up";
                    _this.request(true, {
                        urlalias: "mngtemplates",
                        params: params,
                        onSucces: function (response) { return _this.refetchTemplates(onSucces); }
                    });
                };
                this.deleteTemplate = function (templ) {
                    if (_this.va.curtemplate !== null && _this.va.curtemplate.Id === templ.Id)
                        _this.turnOffTemplateEdition();
                    var params = { models: [templ], mode: "dl" };
                    _this.request(true, {
                        urlalias: "mngtemplates",
                        params: params,
                        onSucces: function (response) { return _this.refetchTemplates(); }
                    });
                };
                this.templatesTextDropped = function (dragID, dropID, dragClass) {
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
                    if (dropID === _this.va.templatesHeader_ElemId)
                        _this.va.curtemplate.MsgHeader = val;
                    else if (dropID === _this.va.templatesBody_ElemId)
                        _this.va.curtemplate.MsgBody = val;
                    textarea.focus();
                };
                this.hasRecepient = function (rc) {
                    return _this.va.curtemplate.ChoosenReccards.any(function (x) { return x === rc.Id; });
                };
                this.switchRecepient = function (rc) {
                    var has = _this.hasRecepient(rc);
                    if (has)
                        _this.va.curtemplate.ChoosenReccards.remove(rc.Id);
                    else
                        _this.va.curtemplate.ChoosenReccards.push(rc.Id);
                };
            }
            TemplatesController.prototype.buildVa = function () { return new TemplatesVA; };
            TemplatesController.prototype.init = function (data) {
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
                this.scope.templCreate = function () {
                    _this.turnTemplateCreate();
                    TextArea.Clear();
                };
                this.scope.templEdit = function (templ) {
                    //this is temporary cos it should be in directive
                    return _this.turnTemplateEdit(templ, function () { return setTimeout(function () { return TextArea.HandleInput(); }, 100); });
                };
                this.scope.setMFilt = function (mfilt) { return _this.setMFilter(mfilt); };
                this.scope.hideEditor = function () { return _this.turnOffTemplateEdition(); };
                this.scope.templSave = function () {
                    //new from scratch template starts with id == -1
                    _this.pushCurtemplate(_this.va.curtemplate.Id === -1);
                    _this.turnOffTemplateEdition();
                };
                this.scope.templDelete = function (templ) { return _this.deleteTemplate(templ); };
                this.scope.templatesTextDropped = function (x, y, z) {
                    _this.templatesTextDropped(x, y, z);
                    TextArea.HandleInput();
                };
                this.scope.InputType = function (SQLtype) { return Controllers.inputTypeForSQLType(SQLtype); };
                this.scope.GetFilterValueCont = function (filt) {
                    return Controllers.GetFilterValueCont(_this.va.curtemplate, filt);
                };
                this.scope.SwitchFilterValueContVal = function (filt, index) {
                    return Controllers.SwitchFilterValueContVal(_this.va.curtemplate, filt, index);
                };
                this.scope.HasFilterValueContVal = function (filt, value) {
                    return Controllers.HasFilterValueContVal(_this.va.curtemplate, filt, value);
                };
                this.scope.HasReccard = this.hasRecepient;
                this.scope.SwitchReccard = this.switchRecepient;
                this.scope.DEMO = function () {
                    var CB = function () { return _this.fetchtoarr(true, {
                        urlalias: "mockmsgs", params: { templateId: _this.va.curtemplate.Id, MaxCount: 10 }
                    }, _this.va.demomsgs, true); };
                    _this.pushCurtemplate(_this.va.curtemplate.Id === -1, CB);
                };
                //------------------- Inner Init
                this.initUrlModuleFromRowObj(data.urls);
                this.refetchTemplates();
                this.refetchMfilters();
            };
            return TemplatesController;
        }(AngularApp.Controller));
        Controllers.TemplatesController = TemplatesController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=TemplatesController.js.map