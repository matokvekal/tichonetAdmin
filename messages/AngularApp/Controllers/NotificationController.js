var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var NotificationVA = (function () {
            function NotificationVA() {
                this.Messages = [];
            }
            return NotificationVA;
        }());
        var NotifType = (function () {
            function NotifType() {
            }
            return NotifType;
        }());
        Controllers.NotifType = NotifType;
        var NotifMessage = (function () {
            function NotifMessage(header, body, type, showdelay) {
                this.header = header;
                this.body = body;
                this.type = type;
                this.showdelay = showdelay;
                this.created = Date.now().toLocaleString();
            }
            return NotifMessage;
        }());
        Controllers.NotifMessage = NotifMessage;
        var NotificationController = (function (_super) {
            __extends(NotificationController, _super);
            function NotificationController($rootScope, $scope, $http) {
                var _this = this;
                _super.call(this, $rootScope, $scope, $http);
                this.killMsgobj = function (msgobj) {
                    var index = _this.va.Messages.indexOf(msgobj);
                    if (index > -1) {
                        _this.va.Messages.splice(index, 1);
                        _this.scope.$apply();
                    }
                };
            }
            NotificationController.prototype.buildVa = function () { return new NotificationVA; };
            NotificationController.prototype.init = function (data) {
                var _this = this;
                this.scope.CloseMsg = function (Msg) { return _this.killMsgobj(Msg); };
                var controller = this;
                this.rootScope.$on(NotificationController.MSGEVENT, function (event, args) {
                    var msgobj = AngularApp.CloneShallow(args.message);
                    controller.va.Messages.push(msgobj);
                    var sd = msgobj.showdelay;
                    if ('undefined' !== typeof sd && sd !== null || sd > 0)
                        setTimeout(function () { controller.killMsgobj(msgobj); }, sd);
                });
            };
            NotificationController.MSGEVENT = 'Notificator.MSG';
            return NotificationController;
        }(AngularApp.Controller));
        Controllers.NotificationController = NotificationController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
