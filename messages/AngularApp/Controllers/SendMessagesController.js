var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var ProjectsVA = (function () {
            function ProjectsVA() {
                this.projects = [];
            }
            return ProjectsVA;
        }());
        var SendMessagesController = (function (_super) {
            __extends(SendMessagesController, _super);
            function SendMessagesController($rootScope, $scope, $http) {
                _super.call(this, $rootScope, $scope, $http);
            }
            SendMessagesController.prototype.buildVa = function () { return new ProjectsVA; };
            SendMessagesController.prototype.init = function (data) {
            };
            return SendMessagesController;
        }(AngularApp.Controller));
        Controllers.SendMessagesController = SendMessagesController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=SendMessagesController.js.map