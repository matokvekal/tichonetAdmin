namespace AngularApp.Controllers {
    import fnc = TSNetLike.Functors

    class SendMessagesVA {
        mschedules: MessageScheduleVM[] = []
        templates: TemplateVM[] = []
        repeatmodes: string[] = []
        cursched: MessageScheduleVM = null

        wildcards: WildcardVM[] = []
        filters: FilterVM[] = []
        reccards: RecepientCardVM[] = []

        schedsHeader_ElemId = 'schedss_header'
        schedsBody_ElemId = 'schedss_body'

        gridSettings: GridSettingsManager;
    }

    class GridSettingsManager {
        constructor(RepeatModes: string[], DateOperators: string[]) {
            this.repeatmodes = RepeatModes.concat("");
            this.dateoperators = DateOperators.concat("");
            this.ClearSettings()
        }


        //Settings
        IsActive: boolean
        IsUnActive: boolean

        IsSms: boolean
        IsEmail: boolean

        Name: string
        TemplateName: string
        DateOperator: string
        Date: Date
        RepeatMode: string

        //Utility Values
        repeatmodes: string[]
        dateoperators: string[]
        //-------

        ClearSettings = () => {
            this.IsActive = false
            this.IsUnActive = false

            this.IsSms = false
            this.IsEmail = false

            this.Name = ""
            this.TemplateName = ""
            this.DateOperator = ""
            this.Date = null
            this.RepeatMode = ""
        }

        GetFetchParams = () => {
            let fp = new FetchParams();
            if (this.IsActive !== this.IsUnActive)
                fp.addFilt("IsActive", this.IsActive, "=")
            if (this.IsSms !== this.IsEmail)
                fp.addFilt("IsSms", this.IsSms, "=")
            if (!isEmptyOrSpaces(this.Name))
                fp.addFilt("Name", this.Name, "like")
            if (!isEmptyOrSpaces(this.TemplateName))
                //TODO this is a temporary implementation, need some dictionary-based solution,
                //to avoid hardcode in querring...
                fp.addFilt("TemplateName", this.TemplateName, "like", true)
            if (!isEmptyOrSpaces(this.DateOperator) && !IsNullOrUndefined(this.Date))
                fp.addFilt("ScheduleDate", this.Date, this.DateOperator)
            if (!isEmptyOrSpaces(this.RepeatMode))
                fp.addFilt("RepeatMode", this.RepeatMode, "=")
            return fp
        }
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

            //---textArea Highlighting
            //this is temporary cos it should be in directive
            let TextArea

            this.scope.InitCodeArea = (divID: string) => {
                TextArea = new CodeArea(divID)
                //setTimeout(() => TextArea.HandleInput(),100)
            }
            this.scope.HandleCodeArea = () => {
                TextArea.HandleInput()
            }

            //---

            this.scope.CreateSched = this.newMSchdedule
            this.scope.EditSched = this.editMschedule
            this.scope.DeleteSched = this.deleteSched

            this.scope.hideEditor = this.turnOffSchedEdition
            this.scope.SaveSched = (sched: MessageScheduleVM, boolDontRefetch = true) => {
                sched = sched || this.va.cursched
                //new from scratch starts with id == -1
                this.pushSched(sched, sched.Id === -1, this.va.cursched !== sched, boolDontRefetch)
                if (this.va.cursched !== null && this.va.cursched.Id === sched.Id)
                    this.turnOffSchedEdition()
            }

            this.scope.GetTemplName = (id: number) => FindById(this.va.templates, id).Name

            this.scope.GetRepeatMode = (sched: MessageScheduleVM) => {
                let mode = this.va.repeatmodes.first(x => x === sched.RepeatMode)
                if (mode === this.va.repeatmodes[0])
                    return "never repeat"
                return "repeat every " + mode;
            }

            this.scope.setTempl = this.setTemplateToCurrent

            this.scope.HasReccard = this.hasRecepient
            this.scope.SwitchReccard = this.switchRecepient

            this.scope.shedulesTextDropped = (x, y, z) => {
                this.shedulesTextDropped(x, y, z)
                TextArea.HandleInput()
            }

            this.scope.InputType = (SQLtype: string) => inputTypeForSQLType(SQLtype)

            this.scope.GetFilterValueCont = (filt: FilterVM) =>
                GetFilterValueCont(this.va.cursched, filt)

            this.scope.SwitchFilterValueContVal = (filt: FilterVM, index: number) =>
                SwitchFilterValueContVal(this.va.cursched, filt, index)

            this.scope.HasFilterValueContVal = (filt: FilterVM, value: any) =>
                HasFilterValueContVal(this.va.cursched, filt, value)

            this.scope.RefetchSchedules = this.refetchSchedules

            //------------------- Inner Init

            this.initUrlModuleFromRowObj(data.urls)

            // Grid Manager Init
            let CreateGridManagerAndFetchSchedules =
                new ConcurentRequestHandler(
                    () => {
                        this.va.gridSettings = new GridSettingsManager(this.va.repeatmodes, dateOperators.select(x => x.SQLString))
                        this.refetchSchedules()
                    },
                    true)

            let dateOperators = []
            
            this.fetchtoarr(true, {
                urlalias: "getoperators",
                params: { typename: "datetime" },
                onSucces: CreateGridManagerAndFetchSchedules
            }, dateOperators)

            this.fetchtoarr(true, {
                urlalias: "getrepeatmodes",
                onSucces: CreateGridManagerAndFetchSchedules
            }, this.va.repeatmodes, true)

            this.fetchtoarr(true, { urlalias: "gettemplates" }, this.va.templates, true)
           
        }

        refetchSchedules = () => {
            this.fetchtoarr(true, {
                urlalias: "getmschedules",
                params: this.va.gridSettings.GetFetchParams(),
                onSucces: () => {
                    this.va.mschedules.forEach(x => x.ScheduleDate = formatVal(x.ScheduleDate,"datetime"))
                }
            }, this.va.mschedules, true)
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
                RepeatMode: this.va.repeatmodes[0],
                ScheduleDate: new Date(Date.now()),
                BatchesCount: 0,

                ng_JustCreated: true,
                ng_ToDelete: false,
            }
            this.va.cursched = ms
        }

        editMschedule = (sched: MessageScheduleVM) => {
            this.va.cursched = sched
            let templ = FindById(this.va.templates,sched.TemplateId)
            this.refetchFilters(templ)
            this.refetchReccards(templ)
            this.refetchWildcards(templ, () => this.scope.HandleCodeArea())

        }

        //
        turnOffSchedEdition = () => {
            this.va.cursched = null
        }

        pushSched = (sched: MessageScheduleVM,asNew: boolean, background = true, dontRefetch = false) => {
            let params = { models: [sched], mode: "" }
            params.mode = asNew ? "cr" : "up"
            let cb = dontRefetch ? null : (response) => this.refetchSchedules()
            this.request(!background, {
                urlalias: "mngmschedules",
                params: params,
                onSucces: cb
            })
        }

        deleteSched = (sched: MessageScheduleVM) => {
            if (this.va.cursched !== null && this.va.cursched.Id === sched.Id)
                this.turnOffSchedEdition()
            let params = { models: [sched], mode: "dl" }
            this.request(true, {
                urlalias: "mngmschedules",
                params: params,
                onSucces: (response) => this.refetchSchedules()
            })
        }
        //

        setTemplateToCurrent = (templ: TemplateVM) => {
            let sched = this.va.cursched
            sched.TemplateId = templ.Id
            sched.MsgHeader = templ.MsgHeader
            sched.MsgBody = templ.MsgBody
            sched.ChoosenReccards = templ.ChoosenReccards
            sched.FilterValueContainers = templ.FilterValueContainers
            sched.IsSms = templ.IsSms
            this.refetchFilters(templ)
            this.refetchReccards(templ)
            this.refetchWildcards(templ, () => this.scope.HandleCodeArea())
        }

        refetchFilters = (templ: TemplateVM, onSucces?: (response?) => void) => {
            this.fetchtoarr(true, {
                urlalias: "getfilters",
                params: new FetchParams()
                    .addFilt("tblRecepientFilterId", templ.RecepientFilterId)
                    .addFilt("allowUserInput", true),
                onSucces: (r) => {
                    this.va.filters.forEach(ele => {
                        formatValsOps(ele.ValsOps, ele.Type)
                    })
                    fnc.F(onSucces, r)
                }
            }, this.va.filters, true)
        }

        refetchReccards = (templ: TemplateVM) => {
            this.fetchtoarr(true,
                {
                    urlalias: "getreccards",
                    params: new FetchParams().addFilt("tblRecepientFilterId", templ.RecepientFilterId),
                },
                this.va.reccards, true);
        }

        refetchWildcards = (templ: TemplateVM, onSucces?: (response?) => void) => {
            this.va.wildcards = []
            let callback = IsNullOrUndefined(onSucces) ? null : new ConcurentRequestHandler(onSucces,true)
            this.fetchtoarr(true,
                {
                    urlalias: "getwildcards",
                    params: new FetchParams().addFilt("tblRecepientFilterId", templ.RecepientFilterId),
                    onSucces: callback
                },
                this.va.wildcards, false);
            //this is a reserved wildcards, used for placing recepient credentials
            //as convention them has negative ids <= -10
            this.fetchtoarr(true, { urlalias: "getreservedcards", onSucces: callback }, this.va.wildcards, false);
        }

        hasRecepient = (rc: RecepientCardVM) => {
            return this.va.cursched.ChoosenReccards.any(x => x === rc.Id)
        }

        switchRecepient = (rc: RecepientCardVM) => {
            let has: boolean = this.hasRecepient(rc);
            if (has)
                this.va.cursched.ChoosenReccards.remove(rc.Id)
            else
                this.va.cursched.ChoosenReccards.push(rc.Id)
        }

        shedulesTextDropped = (dragID: string, dropID: string, dragClass: string) => {
            if (dragClass !== 'wildcard') return;
            let clearID = parseInt(ParseHtmlID(dragID, "_"))
            let wc = this.va.wildcards.first(x => x.Id === clearID)
            if (typeof wc === 'undefined') return
            let textarea = document.getElementById(dropID)
            InputBoxInsertTextAtCursor(textarea, wc.Code)
            let val = (<HTMLInputElement>textarea).value
            //dunno why $apply doesnt work (cos 'ondrop' is already wrapped in $apply)
            if (dropID === this.va.schedsHeader_ElemId)
                this.va.cursched.MsgHeader = val
            else if (dropID === this.va.schedsBody_ElemId)
                this.va.cursched.MsgBody = val
            textarea.focus()
        }
    }
    
}