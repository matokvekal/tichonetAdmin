namespace AngularApp.Controllers {

    class NotificationVA {
        Messages = []
    }

    export class NotifType {
        nclass: string
        glicon: string
    }

    export class NotifMessage {
        constructor(public header: string,
            public body: string,
            public type: NotifType,
            public showdelay?: number) {
            this.created = Date.now().toLocaleString()
        }
        created: string
    }

    export class NotificationController extends Controller<NotificationVA>{
        static MSGEVENT = 'Notificator.MSG'

        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }

        buildVa(): NotificationVA { return new NotificationVA }

        init(data): void {
            this.scope.CloseMsg = (Msg: NotifMessage) => this.killMsgobj(Msg)

            let controller = this
            this.rootScope.$on(NotificationController.MSGEVENT, function (event, args) {
                let msgobj: NotifMessage = CloneShallow(args.message)
                controller.va.Messages.push(msgobj)
                let sd = msgobj.showdelay
                if ('undefined' !== typeof sd && sd !== null || sd > 0)
                    setTimeout(function () { controller.killMsgobj(msgobj) }, sd)
            });
        }

        killMsgobj = (msgobj: NotifMessage) => {
            let index = this.va.Messages.indexOf(msgobj)
            if (index > -1) { this.va.Messages.splice(index, 1); this.scope.$apply() }
        }
    }

}