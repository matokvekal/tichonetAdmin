var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var RFilterVA = (function () {
            function RFilterVA() {
            }
            return RFilterVA;
        }());
        var RFilterController = (function (_super) {
            __extends(RFilterController, _super);
            function RFilterController() {
                _super.apply(this, arguments);
            }
            RFilterController.prototype.buildVa = function () { return new RFilterVA; };
            RFilterController.prototype.init = function (data) {
            };
            return RFilterController;
        }(AngularApp.Controller));
        Controllers.RFilterController = RFilterController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
