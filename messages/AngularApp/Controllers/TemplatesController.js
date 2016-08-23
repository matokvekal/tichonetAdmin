var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
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
                    _this.fetchtoarr(true, { urlalias: "gettemplates", onSucces: onSucces }, _this.va.templates, true);
                };
                this.refetchMfilters = function (onSucces) {
                    _this.fetchtoarr(true, { urlalias: "getmfilters" }, _this.va.metafilters, true);
                };
                this.refetchFilters = function (mfilt) {
                    _this.fetchtoarr(true, {
                        urlalias: "getfilters",
                        params: new AngularApp.FetchParams()
                            .addFilt("tblRecepientFilterId", mfilt.Id)
                            .addFilt("allowUserInput", true),
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
                this.turnTemplateEdit = function (templ) {
                    _this.va.curtemplate = AngularApp.CloneShallow(templ);
                    var mfilt = _this.va.metafilters.first(function (x) { return x.Id === templ.RecepientFilterId; });
                    _this.refetchReccards(mfilt);
                    _this.refetchWildcards(mfilt);
                    _this.refetchFilters(mfilt);
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
                    _this.insertTextAtCursor(textarea, wc.Code);
                    var val = textarea.value;
                    //dunno why $apply doesnt work (cos 'ondrop' is already wrapped in $apply)
                    if (dropID === _this.va.templatesHeader_ElemId)
                        _this.va.curtemplate.MsgHeader = val;
                    else
                        _this.va.curtemplate.MsgBody = val;
                    textarea.focus();
                };
                this.insertTextAtCursor = function (textArea, text) {
                    //TODO check in IE
                    //IE support
                    var doc = document;
                    if (doc.selection) {
                        textArea.focus();
                        var sel = doc.selection.createRange();
                        sel.text = text;
                    }
                    else if (textArea.selectionStart || textArea.selectionStart == '0') {
                        var startPos = textArea.selectionStart;
                        var endPos = textArea.selectionEnd;
                        textArea.value = textArea.value.substring(0, startPos)
                            + text
                            + textArea.value.substring(endPos, textArea.value.length);
                    }
                    else {
                        textArea.value += text;
                    }
                };
                this.getFilterValueCont = function (filt) {
                    var output = _this.va.curtemplate.FilterValueContainers.first(function (x) { return x.FilterId === filt.Id; });
                    if (output !== undefined) {
                        if (AngularApp.IsNullOrUndefined(output.Value))
                            output.Value = [];
                    }
                    else {
                        output = { FilterId: filt.Id, Value: [] };
                        _this.va.curtemplate.FilterValueContainers.push(output);
                    }
                    return output;
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
                this.scope.templCreate = function () { return _this.turnTemplateCreate(); };
                this.scope.templEdit = function (templ) { return _this.turnTemplateEdit(templ); };
                this.scope.setMFilt = function (mfilt) { return _this.setMFilter(mfilt); };
                this.scope.hideEditor = function () { return _this.turnOffTemplateEdition(); };
                this.scope.templSave = function () {
                    //new from scratch template starts with id == -1
                    _this.pushCurtemplate(_this.va.curtemplate.Id === -1);
                    _this.turnOffTemplateEdition();
                };
                this.scope.templDelete = function (templ) { return _this.deleteTemplate(templ); };
                this.scope.templatesTextDropped = function (x, y, z) { return _this.templatesTextDropped(x, y, z); };
                this.scope.InputType = function (SQLtype) { return Controllers.inputTypeForSQLType(SQLtype); };
                this.scope.GetFilterValueCont = function (filt) { return _this.getFilterValueCont(filt); };
                this.scope.SwitchFilterValueContVal = function (filt, index) {
                    var val = _this.getFilterValueCont(filt);
                    if (!filt.allowMultipleSelection)
                        val.Value = [];
                    val.Value[index] = AngularApp.IsNullOrUndefined(val.Value[index]) ?
                        filt.ValsOps[index].Value
                        : null;
                };
                this.scope.HasFilterValueContVal = function (filt, value) {
                    return _this.getFilterValueCont(filt).Value.any(function (x) { return x === value; });
                };
                this.scope.HasReccard = this.hasRecepient;
                this.scope.SwitchReccard = this.switchRecepient;
                this.scope.DEMO = function () {
                    var func = function () { return _this.fetchtoarr(true, {
                        urlalias: "mockmsgs", params: { templateId: _this.va.curtemplate.Id }
                    }, _this.va.demomsgs, true); };
                    _this.pushCurtemplate(_this.va.curtemplate.Id === -1, func);
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