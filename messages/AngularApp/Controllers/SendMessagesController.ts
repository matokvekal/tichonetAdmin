namespace AngularApp.Controllers {

    class SendMessagesVA {
        mschedules: MessageScheduleVM[] = []
        templates: TemplateVM[] = []
        repeatmodes: string[] = []
    }

    export class MessageScheduleVM implements IIndeficated, INgViewModel {
        Id: number
        TemplateId: number = -1
        Name: string = "New Template"

        ScheduleDate: Date
        RepeatMode: string

        IsActive: boolean
        InArchive: boolean

        IsSms: boolean
        MsgHeader: string
        MsgBody: string

        BatchesCount: number

        FilterValueContainers: FilterValueContainer[] = []
        ChoosenReccards: number[] = []

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        ng_Unsaved: boolean
    }

    export class SendMessagesController extends Controller<SendMessagesVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): SendMessagesVA { return new SendMessagesVA }
        init(data): void {
            //------------------- RequestMsgs

            this.request_msgHandlerSucces = (msg) => {
                this.ShowNotification("Info", msg, { glicon: "info-sign", nclass: "info" }, 3000)
            }
            this.request_msgHandlerFail = (msg) => {
                this.ShowNotification("Error", msg, { glicon: "ban-circle", nclass: "error" })
            }

            //------------------- Scope Init

            this.scope.SchedCreate = this.newMSchdedule

            //------------------- Inner Init

            this.initUrlModuleFromRowObj(data.urls)
            this.fetchtoarr(true, { urlalias: "gettemplates" }, this.va.templates, true)
            this.fetchtoarr(true, { urlalias: "getrepeatmodes" }, this.va.repeatmodes, true)
            this.refetchSchedules()
        }

        refetchSchedules = () => {
            this.fetchtoarr(true, { urlalias: "getmschedules" }, this.va.mschedules, true)
        }

        newMSchdedule = () => {
            let ms: MessageScheduleVM = {
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
            }
            this.va.mschedules.unshift(ms);
        }

    }
}