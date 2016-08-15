namespace AngularApp.Controllers {

    class ProjectsVA {
        projects = []
    }

    export class SendMessagesController extends Controller<ProjectsVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): ProjectsVA { return new ProjectsVA }
        init(data): void {

        }

    }
}