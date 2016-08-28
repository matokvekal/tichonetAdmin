namespace AngularApp.Controllers {
    import fnc = TSNetLike.Functors

    class TemplatesVA {
        templates: TemplateVM[] = []
        metafilters: MetaFilterVM[] = []
        curtemplate: TemplateVM = null
        wildcards: WildcardVM[] = []
        filters: FilterVM[] = []

        reccards: RecepientCardVM[] = []

        templatesHeader_ElemId = 'templates_header'
        templatesBody_ElemId = 'templates_body'

        //
        demomsgs = []
    }

    export class TemplatesController extends Controller<TemplatesVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): TemplatesVA { return new TemplatesVA }
        init(data): void {

            //------------------- RequestMsgs

            this.request_msgHandlerSucces = (msg) => {
                this.ShowNotification("Info", msg, { glicon: "info-sign", nclass: "info" }, 3000)
            }
            this.request_msgHandlerFail = (msg) => {
                this.ShowNotification("Error", msg, { glicon: "ban-circle", nclass: "error" })
            }

            //------------------- Scope Init

            this.scope.templCreate = () => this.turnTemplateCreate()
            this.scope.templEdit = (templ) => this.turnTemplateEdit(templ)
            this.scope.setMFilt = (mfilt) => this.setMFilter(mfilt)
            this.scope.hideEditor = () => this.turnOffTemplateEdition()
            this.scope.templSave = () => {
                //new from scratch template starts with id == -1
                this.pushCurtemplate(this.va.curtemplate.Id === -1)
                this.turnOffTemplateEdition()
            }
            this.scope.templDelete = (templ) => this.deleteTemplate(templ)
            this.scope.templatesTextDropped = (x, y, z) => this.templatesTextDropped(x, y, z)

            this.scope.InputType = (SQLtype: string) => inputTypeForSQLType(SQLtype)

            this.scope.GetFilterValueCont = (filt: FilterVM) => this.getFilterValueCont(filt)

            this.scope.SwitchFilterValueContVal = (filt: FilterVM, index: number) => {
                let val = this.getFilterValueCont(filt)
                if (!filt.allowMultipleSelection)
                    val.Values = []
                val.Values[index] = IsNullOrUndefined(val.Values[index]) ?
                    filt.ValsOps[index].Value
                    : null
            }

            this.scope.HasFilterValueContVal = (filt: FilterVM, value: any) => {
                return this.getFilterValueCont(filt).Values.any(x => x === value)
            }

            this.scope.HasReccard = this.hasRecepient
            this.scope.SwitchReccard = this.switchRecepient

            this.scope.DEMO = () => {
                let func = () => this.fetchtoarr(true, {
                        urlalias: "mockmsgs", params: { templateId: this.va.curtemplate.Id }
                    }, this.va.demomsgs, true)
                this.pushCurtemplate(this.va.curtemplate.Id === -1, func)
            }

            //------------------- Inner Init

            this.initUrlModuleFromRowObj(data.urls)
            this.refetchTemplates()
            this.refetchMfilters()
        }

        refetchTemplates = (onSucces?: (response) => void) => {
            this.fetchtoarr(true, {
                urlalias: "gettemplates",
                onSucces: onSucces,
            }, this.va.templates, true)
        }

        refetchMfilters = (onSucces?) => {
            this.fetchtoarr(true, { urlalias: "getmfilters" }, this.va.metafilters, true)
        }

        refetchFilters = (mfilt: MetaFilterVM, onSucces?: (response?) => void) => {
            this.fetchtoarr(true, {
                urlalias: "getfilters",
                params: new FetchParams()
                    .addFilt("tblRecepientFilterId", mfilt.Id)
                    .addFilt("allowUserInput", true),
                onSucces: (r) => {
                    this.va.filters.forEach(ele => {
                        formatValsOps(ele.ValsOps, ele.Type)
                    })
                    fnc.F(onSucces,r)
                }
            }, this.va.filters, true)
        }

        refetchReccards = (mfilt: MetaFilterVM) => {
            this.fetchtoarr(true,
                {
                    urlalias: "getreccards",
                    params: new FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                },
                this.va.reccards, true);
        }

        refetchWildcards = (mfilt: MetaFilterVM) => {
            this.va.wildcards = []
            this.fetchtoarr(true,
                {
                    urlalias: "getwildcards",
                    params: new FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                },
                this.va.wildcards, false);
            //this is a reserved wildcards, used for placing recepient credentials
            //as convention them has negative ids <= -10
            this.fetchtoarr(true,{urlalias: "getreservedcards"},this.va.wildcards, false);
        }

        turnTemplateCreate = () => {
            this.va.curtemplate = new TemplateVM()
            this.va.curtemplate.Id = -1
            this.va.filters = []
        }

        setMFilter = (mfilt: MetaFilterVM) => {
            this.va.curtemplate.RecepientFilterId = mfilt.Id
            this.refetchReccards(mfilt)
            this.refetchWildcards(mfilt)
            this.refetchFilters(mfilt)
            this.va.curtemplate.FilterValueContainers = []
        }

        turnTemplateEdit = (templ: TemplateVM) => {
            this.va.curtemplate = CloneShallow(templ)
            let mfilt = this.va.metafilters.first(x => x.Id === templ.RecepientFilterId)
            this.refetchReccards(mfilt)
            this.refetchWildcards(mfilt)
            this.refetchFilters(mfilt, r => {
                this.va.filters.forEach(x => {
                    let filtValCont = this.va.curtemplate.FilterValueContainers.first(y => y.FilterId === x.Id)
                    if (filtValCont !== undefined && !IsNullOrUndefined(filtValCont.Values))
                        filtValCont.Values.forEach((ele, ind) => filtValCont.Values[ind] = formatVal(ele,x.Type) )
                })
            })
        }

        turnOffTemplateEdition = () => {
            this.va.curtemplate = null
        }

        pushCurtemplate = (asNew: boolean, onSucces?: (response) => void) => {
            let params = { models: [this.va.curtemplate], mode: "" }
            params.mode = asNew ? "cr" : "up"
            this.request(true, {
                urlalias: "mngtemplates",
                params: params,
                onSucces: (response) => this.refetchTemplates(onSucces)
            })
        }

        deleteTemplate = (templ: TemplateVM) => {
            if (this.va.curtemplate !== null && this.va.curtemplate.Id === templ.Id)
                this.turnOffTemplateEdition()
            let params = { models: [templ], mode: "dl" }
            this.request(true, {
                urlalias: "mngtemplates",
                params: params,
                onSucces: (response) => this.refetchTemplates()
            })
        }

        templatesTextDropped = (dragID: string, dropID: string, dragClass: string) => {
            if (dragClass !== 'wildcard') return;
            let clearID = parseInt( ParseHtmlID(dragID, "_") )
            let wc = this.va.wildcards.first(x => x.Id === clearID)
            if (typeof wc === 'undefined') return
            let textarea = document.getElementById(dropID)
            this.insertTextAtCursor(textarea, wc.Code)
            let val = (<HTMLInputElement>textarea).value
            //dunno why $apply doesnt work (cos 'ondrop' is already wrapped in $apply)
            if (dropID === this.va.templatesHeader_ElemId)
                this.va.curtemplate.MsgHeader = val
            else
                this.va.curtemplate.MsgBody = val
            textarea.focus()
        }

        insertTextAtCursor = (textArea, text: string) => {
            //TODO check in IE
            //IE support
            let doc = document as any
            if (doc.selection) {
                textArea.focus()
                let sel = doc.selection.createRange()
                sel.text = text
            }
            //MOZILLA and others
            else if (textArea.selectionStart || textArea.selectionStart == '0') {
                let startPos = textArea.selectionStart
                let endPos = textArea.selectionEnd
                textArea.value = textArea.value.substring(0, startPos)
                    + text
                    + textArea.value.substring(endPos, textArea.value.length);
            }
            else {
                textArea.value += text;
            }
        }

        getFilterValueCont = (filt: FilterVM) => {
            let output = this.va.curtemplate.FilterValueContainers.first(x => x.FilterId === filt.Id)
            if (output !== undefined) {
                if (IsNullOrUndefined(output.Values))
                    output.Values = []
            }
            else {
                output = { FilterId: filt.Id, Values: [] }
                this.va.curtemplate.FilterValueContainers.push(output)
            }
            return output
        }

        hasRecepient = (rc: RecepientCardVM) => {
            return this.va.curtemplate.ChoosenReccards.any(x => x === rc.Id)
        }

        switchRecepient = (rc: RecepientCardVM) => {
            let has: boolean = this.hasRecepient(rc);
            if (has)
                this.va.curtemplate.ChoosenReccards.remove(rc.Id)
            else
                this.va.curtemplate.ChoosenReccards.push(rc.Id)
        }

    }


}