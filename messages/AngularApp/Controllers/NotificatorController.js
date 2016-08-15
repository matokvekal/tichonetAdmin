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
        var Message = (function () {
            function Message() {
            }
            return Message;
        }());
        var NotificationController = (function (_super) {
            __extends(NotificationController, _super);
            function NotificationController($rootScope, $scope, $http) {
                _super.call(this, $rootScope, $scope, $http);
            }
            NotificationController.prototype.buildVa = function () { return new NotificationVA; };
            NotificationController.prototype.init = function (data) {
            };
            NotificationController.defaultTimeOut = 3500;
            return NotificationController;
        }(AngularApp.Controller));
        Controllers.NotificationController = NotificationController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
var NotificatorControllerOLD = function ($scope, $rootScope, $http) {
    var defaultTimeOut = 3500;
    $scope.Messages = [];
    $scope.init = function (data) {
        $rootScope.$on('Notificator.Msg', function (event, args) {
            var msgobj = {
                Text: args.msg,
                Type: args.type,
                Created: Date.now().toLocaleString()
            };
            $scope.Messages.push(msgobj);
            var sd = args.showdelay;
            if ('undefined' === typeof sd || sd === null || sd <= 0)
                sd = defaultTimeOut;
            setTimeout(function () { killMsgobj(msgobj); }, sd);
        });
    };
    function killMsgobj(msgobj) {
        var index = $scope.Messages.indexOf(msgobj);
        if (index > -1) {
            $scope.Messages.splice(index, 1);
            $scope.$apply();
        }
    }
};
NotificatorControllerOLD.$inject = ['$scope', '$rootScope', '$http'];
//var Notificator = {};
//Notificator.Msg = function ($Scope, msg, type, showdelay) {
//    $Scope.$broadcast('Notificator.Msg', { msg: msg, type: type, showdelay: showdelay });
//} 
