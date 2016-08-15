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
                this.templatesHeader_ElemId = 'templates_header';
                this.templatesBody_ElemId = 'templates_body';
                //
                this.demomsgs = [];
            }
            return TemplatesVA;
        }());
        var TemplateVM = (function () {
            function TemplateVM() {
                this.RecepientFilterId = -1;
            }
            return TemplateVM;
        }());
        Controllers.TemplateVM = TemplateVM;
        var TemplatesController = (function (_super) {
            __extends(TemplatesController, _super);
            function TemplatesController($rootScope, $scope, $http) {
                var _this = this;
                _super.call(this, $rootScope, $scope, $http);
                this.refetchTemplates = function () {
                    _this.fetchtoarr(true, { urlalias: "gettemplates" }, _this.va.templates, true);
                };
                this.refetchMfilters = function (onSucces) {
                    _this.fetchtoarr(true, { urlalias: "getmfilters" }, _this.va.metafilters, true);
                };
                this.refetchWildcards = function (mfilt) {
                    _this.fetchtoarr(true, {
                        urlalias: "getwildcards",
                        params: new AngularApp.FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                    }, _this.va.wildcards, true);
                };
                this.turnTemplateCreate = function () {
                    _this.va.curtemplate = new TemplateVM();
                    _this.va.curtemplate.Id = -1;
                };
                this.setMFilter = function (mfilt) {
                    _this.va.curtemplate.RecepientFilterId = mfilt.Id;
                    _this.refetchWildcards(mfilt);
                };
                this.turnTemplateEdit = function (templ) {
                    _this.va.curtemplate = AngularApp.CloneShallow(templ);
                    var filt = _this.va.metafilters.first(function (x) { return x.Id === templ.RecepientFilterId; });
                    _this.refetchWildcards(filt);
                };
                this.turnOffTemplateEdition = function () {
                    _this.va.curtemplate = null;
                };
                this.pushCurtemplate = function (asNew) {
                    var params = { models: [_this.va.curtemplate], mode: "" };
                    params.mode = asNew ? "cr" : "up";
                    _this.request(true, {
                        urlalias: "mngtemplates",
                        params: params,
                        onSucces: function (response) { return _this.refetchTemplates(); }
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
            }
            TemplatesController.prototype.buildVa = function () { return new TemplatesVA; };
            TemplatesController.prototype.init = function (data) {
                var _this = this;
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
                this.scope.DEMO = function () {
                    _this.fetchtoarr(true, {
                        urlalias: "mockmsgs", params: { templateId: _this.va.curtemplate.Id }
                    }, _this.va.demomsgs, true);
                };
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