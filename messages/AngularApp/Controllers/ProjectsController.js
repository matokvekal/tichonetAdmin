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
        var ProjectsController = (function (_super) {
            __extends(ProjectsController, _super);
            function ProjectsController($rootScope, $scope, $http) {
                _super.call(this, $rootScope, $scope, $http);
            }
            ProjectsController.prototype.buildVa = function () { return new ProjectsVA; };
            ProjectsController.prototype.init = function (data) {
                //alert("Init Works")
                //this.initUrlModuleFromRowObj(data.urls)
                //this.fetchtoarr(true, {
                //    urlalias: "getall",
                //    params: { yolo: true },
                //    onSucces: (r) => { alert("fetched")}
                //}, this.va.projects)
            };
            return ProjectsController;
        }(AngularApp.Controller));
        Controllers.ProjectsController = ProjectsController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
