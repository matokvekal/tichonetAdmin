namespace AngularApp.Controllers {
    import col = TSNetLike.Collections

    class PageBlocksVA {
        filters_folded = true
        templates_folded = true
        mails_folded = true
        reports_folded = true
    }

    export class MessageModulePageController extends Controller<PageBlocksVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
            this.TurnHoldViewOnOthersControllers()
        }
        buildVa(): PageBlocksVA { return new PageBlocksVA }
        init(data): void {}
    }


}