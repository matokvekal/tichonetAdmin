var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var SendMessagesVA = (function () {
            function SendMessagesVA() {
                this.mschedules = [];
                this.templates = [];
                this.repeatmodes = [];
            }
            return SendMessagesVA;
        }());
        var MessageScheduleVM = (function () {
            function MessageScheduleVM() {
                this.TemplateId = -1;
                this.Name = "New Template";
                this.FilterValueContainers = [];
                this.ChoosenReccards = [];
            }
            return MessageScheduleVM;
        }());
        Controllers.MessageScheduleVM = MessageScheduleVM;
        var SendMessagesController = (function (_super) {
            __extends(SendMessagesController, _super);
            function SendMessagesController($rootScope, $scope, $http) {
                var _this = this;
                _super.call(this, $rootScope, $scope, $http);
                this.refetchSchedules = function () {
                    _this.fetchtoarr(true, { urlalias: "getmschedules" }, _this.va.mschedules, true);
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
                        RepeatMode: "",
                        ScheduleDate: new Date(Date.now()),
                        BatchesCount: 0,
                        ng_JustCreated: true,
                        ng_ToDelete: false,
                        ng_Unsaved: true
                    };
                    _this.va.mschedules.unshift(ms);
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
                this.scope.SchedCreate = this.newMSchdedule;
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
