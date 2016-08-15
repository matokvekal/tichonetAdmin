namespace AngularApp.Controllers {

    class TemplatesVA {
        templates: TemplateVM[] = []
        metafilters: MetaFilterVM[] = []
        curtemplate: TemplateVM = null
        wildcards: WildcardVM[] = []

        templatesHeader_ElemId = 'templates_header'
        templatesBody_ElemId = 'templates_body'

        //
        demomsgs = []
    }

    export class TemplateVM {
        Id: number
        RecepientFilterId: number = -1
        Name: string
        IsSms: boolean
        MsgHeader: string
        MsgBody: string
    }

    export class TemplatesController extends Controller<TemplatesVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): TemplatesVA { return new TemplatesVA }
        init(data): void {

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
            this.scope.templatesTextDropped = (x, y, z) => this.templatesTextDropped(x,y,z)

            this.scope.DEMO = () => {
                this.fetchtoarr(true, {
                    urlalias: "mockmsgs", params: { templateId: this.va.curtemplate.Id }
                }, this.va.demomsgs, true )
            }

            this.initUrlModuleFromRowObj(data.urls)
            this.refetchTemplates()
            this.refetchMfilters()
        }

        refetchTemplates = () => {
            this.fetchtoarr(true, { urlalias: "gettemplates" }, this.va.templates, true)
        }

        refetchMfilters = (onSucces?) => {
            this.fetchtoarr(true, { urlalias: "getmfilters" }, this.va.metafilters, true)
        }

        refetchWildcards = (mfilt : MetaFilterVM) => {
            this.fetchtoarr(true,
                {
                    urlalias: "getwildcards",
                    params: new FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                    //onSucces: () => this.refetchPossibleKeys(table)
                },
                this.va.wildcards, true);
        }

        turnTemplateCreate = () => {
            this.va.curtemplate = new TemplateVM()
            this.va.curtemplate.Id = -1
        }

        setMFilter = (mfilt: MetaFilterVM) => {
            this.va.curtemplate.RecepientFilterId = mfilt.Id
            this.refetchWildcards(mfilt)
        }

        turnTemplateEdit = (templ: TemplateVM) => {
            this.va.curtemplate = CloneShallow(templ)
            let filt = this.va.metafilters.first(x => x.Id === templ.RecepientFilterId)
            this.refetchWildcards(filt)
        }

        turnOffTemplateEdition = () => {
            this.va.curtemplate = null
        }

        pushCurtemplate = (asNew: boolean) => {
            let params = { models: [this.va.curtemplate], mode: "" }
            params.mode = asNew ? "cr" : "up"
            this.request(true, {
                urlalias: "mngtemplates",
                params: params,
                onSucces: (response) => this.refetchTemplates()
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

    }


}