namespace AngularApp.Controllers {
    import col = TSNetLike.Collections

    class MFiltersVA {
        basetables: BaseTableVM[] = []
        curbasetable: BaseTableVM
        showTableChoose: boolean = false

        possiblekeys: KeyVM[] = []
        reservedwildcards: WildcardVM[] = []

        metafilters: MetaFilterVM[] = []
        curmetafilter: MetaFilterVM = null

        curmetafilterBaseTableName = () => {
            let t = FindById(this.basetables, this.curmetafilter.BaseTableId)
            return t === undefined ? "" : t.Name
        }

        wildcardCreator_ID = '__wildcardCreator'
        filterCreator_ID = '__filterCreator'
        reccardCreator_ID = '__recepientcardCreator'

        reccardDropName = "___reccardDropName"
        reccardDropEmail = "___reccardDropEmail"
        reccardDropPhone = "___reccardDropPhone"


        keyIdPrefix = '_tablekey'
        keyDragClass = 'tablekey'

        //just for folding
        SHOW_FILTS = false
        SHOW_RCARDS = false
        SHOW_WCARDS = false
    }

    export class MFiltersController extends Controller<MFiltersVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): MFiltersVA { return new MFiltersVA }
        init(data): void {

            //------------------- RequestMsgs
            this.request_msgHandlerSucces = (msg) => {
                this.ShowNotification("Info", msg, { glicon: "info-sign", nclass: "info" }, 3000)
            }
            this.request_msgHandlerFail = (msg) => {
                this.ShowNotification("Error", msg, { glicon:"ban-circle",nclass:"error"})
            }

            //------------------- Scope Init

            this.scope.NewMFilt = () => this.turnMFilterCreate()

            this.scope.EditMFilt = (mfilt: MetaFilterVM) => this.turnMFilterEdit(mfilt)

            this.scope.RemoveMfilt = (mfilt: MetaFilterVM) => alert("NOT IMPLEMENTED =/")

            this.scope.CloseEditor = () => this.closeMFiltEditor()

            this.scope.TypeToIcon = (type: string) => glyphiconforSQLTYPE(type)

            this.scope.InputType = (type: string) => inputTypeForSQLType(type)

            this.scope.SetCurBaseTable = (t: BaseTableVM) => this.setCurBaseTable(t)

            this.scope.ShowTableChoose = () => this.va.showTableChoose = true

            this.scope.SetCurTableToCurMFilt = () => {
                this.va.curmetafilter.BaseTableId = this.va.curbasetable.Id
                let table = FindById(this.va.basetables, this.va.curmetafilter.BaseTableId)
                this.refetchPossibleKeysAndValidate(table)
                this.va.showTableChoose = false
            }

            this.scope.SetCurTableFromCurMFilt = () => {
                let table = FindById(this.va.basetables, this.va.curmetafilter.BaseTableId)
                this.va.curbasetable = table === undefined ? null : table
                this.refetchPossibleKeysAndValidate(table)
                this.va.showTableChoose = false
            }

            this.scope.IsCurMFiltHasTable = (table : BaseTableVM) => {
                return this.va.curmetafilter.BaseTableId === table.Id
            }

            this.scope.IsCurMFiltHasAnyTable = () => {
                return this.va.curmetafilter.BaseTableId != -1
            }

            this.scope.MakeId = (x,y) => MakeHtmlID(x,y)

            this.scope.NewFilterDrop = (dragID: string, dropID: string, dragClass: string) => {
                if (dragClass != this.va.keyDragClass) return
                let keyname = ParseHtmlID(dragID)
                let key = this.va.possiblekeys.first(x => x.name === keyname)
                this.newFilter(key)
            }

            this.scope.NewRecardDrop = (dragID: string, dropID: string, dragClass: string) => {
                if (dragClass != this.va.keyDragClass) return
                let keyname = ParseHtmlID(dragID)
                let key = this.va.possiblekeys.first(x => x.name === keyname)
                this.newRecCard(key)
            }

            this.scope.RecardDropKey = (dragID: string, dropID: string, dragClass: string) => {
                if (dragClass != this.va.keyDragClass) return
                let keyname = ParseHtmlID(dragID)
                let key = this.va.possiblekeys.first(x => x.name === keyname)

                let args = ParseHtmlIDFull(dropID)
                let cardId = parseInt( args[1] )
                let card = this.va.curmetafilter.reccards.first(x => x.Id === cardId)
                if (card === null) return

                this.RecCardAddKey(card, key, args[0])
                this.validateReccard(card)
            }

            this.scope.NewWildcardDrop = (dragID: string, dropID: string, dragClass: string) => {
                if (dragClass != this.va.keyDragClass) return
                let keyname = ParseHtmlID(dragID)
                let key = this.va.possiblekeys.first(x => x.name === keyname)
                this.newWildCard(key)
            }

            this.scope.GetTypeOperators = (type: string) => this.typeOperators.take(type)
            this.scope.GetTypeOperatorsNames = (type: string) => this.typeOperatorsSQL.take(type)

            this.scope.ValidateItem = (item, type: string) => {
                switch (type) {
                    case "wildcard":
                        this.validateWildcard(item)
                        break
                    case "reccard":
                        this.validateReccard(item)
                        break
                    case "filter":
                        this.validateFilter(item)
                }
            }

            this.scope.GetValidationInfo = (item: IValidable) => this.getValidationString(item,"this item")

            this.scope.RemoveItem = (item: INgViewModel) => this.RemoveItem(item)
            this.scope.RestoreItem = (item: INgViewModel) => this.RestoreItem(item)

            this.scope.DeleteFiltValue = (filter: FilterVM, index: number) => {
                filter.ValsOps.splice(index, 1)
            }

            this.scope.AddFiltValue = (filter: FilterVM) => {
                filter.ValsOps.push(new ValOp())
            }

            this.scope.WildCardUpdateCode = (wc: WildcardVM) => {
                this.updateWildCardCode(wc)
                this.va.curmetafilter.wildcards.forEach(x => this.validateWildcard(x))
            }

            this.scope.SaveCurMFilt = () => this.saveOrUpdateCurMFilter()

            //------------------- Inner Init

            this.initUrlModuleFromRowObj(data.urls)
            this.refetchTables()
            this.refetchMfilters()     
            this.fetchtoarr(true, { urlalias: "getreservedcards" }, this.va.reservedwildcards, false);

        }

        //BaseTables i.e. RecepientFilterTableName

        refetchTables = (onSucces?) => {
            this.fetchtoarr(true, {urlalias: "gettables"}, this.va.basetables, true)
        }

        refetchMfilters = (onSucces?) => {
            this.fetchtoarr(true, { urlalias: "getmfilters"}, this.va.metafilters,true)
        }

        setCurBaseTable = (table: BaseTableVM) => {
            this.va.curbasetable = table
            if (table !== null)
                this.refetchPossibleKeysAndValidate(table)
            else
                this.clearPossibleKeys()
        }

        //VALIDATION

        validateEntities = () => {
            let filt = this.va.curmetafilter
            let entitiesInvalid = false
            filt.ValidationErrors = []
            let itemNameInvalid = isEmptyOrSpaces(filt.Name)
            if (itemNameInvalid)
                filt.ValidationErrors.push("Recepient filter name cannot be empty");
            let noRecepients = filt.reccards.filter(x => !x.ng_ToDelete).length === 0
            if (noRecepients)
                filt.ValidationErrors.push("Recepient filter should have at least one recepient card");
            filt.wildcards.forEach(x => {
                this.validateWildcard(x)
                if (x.Invalid && !entitiesInvalid)
                    entitiesInvalid = true
            })
            filt.filters.forEach(x => {
                this.validateFilter(x)
                if (x.Invalid && !entitiesInvalid)
                    entitiesInvalid = true
            })
            filt.reccards.forEach(x => {
                this.validateReccard(x)
                if (x.Invalid && !entitiesInvalid)
                    entitiesInvalid = true
            })
            filt.Invalid = entitiesInvalid || itemNameInvalid || noRecepients
        }

        validateWildcard = (item: WildcardVM) => {
            item.ValidationErrors = []
            if (item.ng_ToDelete) {
                item.Invalid = false
                return
            }
                
            let itemNameInvalid = isEmptyOrSpaces(item.Name)
            if (itemNameInvalid)
                item.ValidationErrors.push("name cannot be empty");
            let codeEmpty = isEmptyOrSpaces(item._Code)
            if (codeEmpty)
                item.ValidationErrors.push("code cannot be empty");
            let keysInvalid = !this.validateWithKeys(item, (item, key) => key.name === item.Key)
            if (keysInvalid)
                item.ValidationErrors.push("base table has no such key [" + item.Key + "]");
            let codeDublicated = !this.validateDublCodeWildcard(item)
            if (codeDublicated)
                item.ValidationErrors.push("wildcard code is not uniq: " + item.Code);
            item.Invalid = keysInvalid || itemNameInvalid || codeDublicated || codeEmpty
        }

        validateDublCodeWildcard = (wc: WildcardVM) => {
            let cards = this.va.curmetafilter.wildcards
            for (let i = 0; i < cards.length; i++) {
                if (cards[i].Code === wc.Code && cards[i] !== wc)
                    return false
            }
            cards = this.va.reservedwildcards
            for (let i = 0; i < cards.length; i++) {
                if (cards[i].Code === wc.Code)
                    return false
            }
            return true;
        }

        validateReccard = (item: RecepientCardVM) => {
            item.ValidationErrors = []

            if (item.ng_ToDelete) {
                item.Invalid = false
                return
            }

            let itemNameInvalid = isEmptyOrSpaces(item.Name)
            if (itemNameInvalid)
                item.ValidationErrors.push("name cannot be empty");
            let someKeyIsEmpty = isEmptyOrSpaces(item.EmailKey) || isEmptyOrSpaces(item.NameKey) || isEmptyOrSpaces(item.PhoneKey)
            if (someKeyIsEmpty)
                item.ValidationErrors.push("recepient card can't have empty keys");

            let keysInvalid = false
            if (!someKeyIsEmpty) {
                let mailInvalid = !this.validateWithKeys(item, (item, key) => key.name === item.EmailKey)
                if (mailInvalid)
                    item.ValidationErrors.push("base table has no such key [" + item.EmailKey + "]");
                let nameInvalid = !this.validateWithKeys(item, (item, key) => key.name === item.NameKey)
                if (nameInvalid)
                    item.ValidationErrors.push("base table has no such key [" + item.NameKey + "]");
                let phoneInvalid = !this.validateWithKeys(item, (item, key) => key.name === item.PhoneKey)
                if (phoneInvalid)
                    item.ValidationErrors.push("base table has no such key [" + item.PhoneKey + "]");
                keysInvalid = mailInvalid || nameInvalid || phoneInvalid
            }
            item.Invalid = keysInvalid || itemNameInvalid || someKeyIsEmpty
        }

        validateFilter = (item: FilterVM) => {
            item.ValidationErrors = []

            if (item.ng_ToDelete) {
                item.Invalid = false
                return
            }

            let itemNameInvalid = isEmptyOrSpaces(item.Name)
            if (itemNameInvalid)
                item.ValidationErrors.push("name cannot be empty");

            let keysInvalid = !this.validateWithKeys(item, (item, key) => key.name === item.Key && key.type === item.Type)
            if (keysInvalid)
                item.ValidationErrors.push("base table has no such key [" + item.Key + "] with type [" + item.Type + "]");

            item.Invalid = keysInvalid || itemNameInvalid
        }

        validateWithKeys<T>(item: T, validator: (item: T, key: KeyVM) => boolean): boolean {
            for (let i = 0; i < this.va.possiblekeys.length; i++) {
                if (validator(item, this.va.possiblekeys[i]))
                    return true
            }
            return false
        }

        getMetaFilterValidationString = (filt: MetaFilterVM) => {
            let output = ""
            let tmp: string
            tmp = filt.ValidationErrors.join(";\n ")
            if (!isEmptyOrSpaces(tmp))
                output += tmp + ".\n"

            tmp = ""
            filt.reccards.forEach(x => tmp += this.getValidationString(x, 'Recepient Card "' + x.Name + '"'))
            if (!isEmptyOrSpaces(tmp))
                output += tmp + ".\n"

            tmp = ""
            filt.wildcards.forEach(x => output += this.getValidationString(x, 'Wildcard "' + x.Name + '"'))
            if (!isEmptyOrSpaces(tmp))
                output += tmp + ".\n"

            tmp = ""
            filt.filters.forEach(x => output += this.getValidationString(x, 'Subfilter "' + x.Key + '"'))
            if (!isEmptyOrSpaces(tmp))
                output += tmp + ".\n"
            return output
        }

        getValidationString = (item: IValidable, name: string, addValid = false) => {
            if (IsNullOrUndefined(item))
                throw new Error("can't get validation message from null or undefined")

            //it is metafilter silly type check
            if (!IsNullOrUndefined((item as MetaFilterVM).BaseTableId))
                return this.getMetaFilterValidationString(item as MetaFilterVM)

            if (IsNullOrUndefined(item.ValidationErrors) || item.ValidationErrors.length === 0){
                if (addValid) 
                    return name + " has no noticed validation errors"
                else 
                    return ""
            }

            let output = name + " has " + item.ValidationErrors.length + " errors: \n"
            output += item.ValidationErrors.join(";\n ")
            return output
        }

        refetchPossibleKeysAndValidate = (table: BaseTableVM) => {
            let cb = () => {
                if (this.va.curmetafilter !== null)
                    this.validateEntities()
            }
            this.fetchtoarr(true, { urlalias: "getcolomns", params: {id:table.Id}, onSucces:cb},this.va.possiblekeys,true)
        }

        clearPossibleKeys = () => {
            this.va.possiblekeys = []
        }

        //MFilters i.e. MetaFilters i.e. RecepientFilters

        turnMFilterCreate = () => {
            this.setCurBaseTable(null)
            this.va.curmetafilter = new MetaFilterVM()
            this.va.curmetafilter.Id = -1
            this.va.showTableChoose = true
        }


        turnMFilterEdit = (mfilt: MetaFilterVM) => {
            this.va.showTableChoose = false
            let table = FindById(this.va.basetables, mfilt.BaseTableId)
            this.setCurBaseTable(table)

            this.va.curmetafilter = CloneShallow(mfilt)
            this.va.curmetafilter.filters = []
            this.va.curmetafilter.wildcards = []
            this.va.curmetafilter.reccards = []

            //todo this is a bit bad : tblRecepientFilterId, avoid this, use convention approach at least
            let params = new FetchParams().addFilt("tblRecepientFilterId", mfilt.Id)
            let cb = new ConcurentRequestHandler(() => {
                this.refetchPossibleKeysAndValidate(table)
                this.va.curmetafilter.wildcards.forEach(wc => {
                    wc._Code = wc.Code.substr(1,wc.Code.length-2)
                })
                this.va.curmetafilter.filters.forEach(ele => {
                    if (!this.typeOperators.cont(ele.Type))
                        this.fetchTypeOperators(ele.Type)
                    formatValsOps(ele.ValsOps,ele.Type)
                })
            }, true)

            this.fetchtoarr(true, { urlalias: "getfilters", params: params, onSucces: cb },
                this.va.curmetafilter.filters, true);
            
            this.fetchtoarr(true, { urlalias: "getwildcards", params: params, onSucces: cb },
                this.va.curmetafilter.wildcards, true);

            this.fetchtoarr(true, { urlalias: "getreccards", params: params, onSucces: cb },
                this.va.curmetafilter.reccards, true);
        }



        closeMFiltEditor = () => {
            this.va.curmetafilter = null
        }

        saveOrUpdateCurMFilter = () => {
            this.validateEntities()
            if (this.va.curmetafilter.Invalid) {
                let erorrs = this.getValidationString(this.va.curmetafilter, this.va.curmetafilter.Name)
                this.ShowNotification("Validation Errors", "you should fix errors first: \n" + erorrs, {glicon:"ban-circle",nclass:"error"})
                return
            }

            let mode = this.va.curmetafilter.Id === -1 ? "cr" : "up"
            this.request(true, {
                urlalias: "mngmfilter",
                params: {
                    mode: mode,
                    models: [this.va.curmetafilter]
                },
                onSucces: () => {
                    this.closeMFiltEditor()
                    this.refetchMfilters()
                }
            })
        }

        //Common for sub-items: (wildcards, recepient cards, filters)

        RemoveItem = (item: INgViewModel) => {
            item.ng_ToDelete = true;
        }
        RestoreItem = (item: INgViewModel) => {
            item.ng_ToDelete = false;
        }

        //RecepientCards

        newRecCard = (key: KeyVM) => {
            this.va.SHOW_RCARDS = true
            if (IsNullOrUndefined(this.va.curmetafilter.filters))
                this.va.curmetafilter.reccards = []
            let id = this.va.curmetafilter.reccards.max(x => x.Id) + 1
            let rc: RecepientCardVM = {
                Id: id,
                Name: key.name,
                NameKey: key.name,
                EmailKey: "",
                PhoneKey: "",
                RecepientFilterId: this.va.curmetafilter.Id,
                Invalid: false,
                ValidationErrors: [],
                ng_JustCreated: true,
                ng_ToDelete: false
            }
            this.va.curmetafilter.reccards.unshift(rc)
        }

        RecCardAddKey = (card: RecepientCardVM, key: KeyVM, reccardField:string) => {
            switch (reccardField) {
                case this.va.reccardDropName:
                    card.NameKey = key.name
                    break
                case this.va.reccardDropEmail:
                    card.EmailKey = key.name
                    break
                case this.va.reccardDropPhone:
                    card.PhoneKey = key.name
                    break
            }
        }

        //Wildcards

        newWildCard = (key: KeyVM) => {
            this.va.SHOW_WCARDS = true
            let id = this.va.curmetafilter.wildcards.max(x => x.Id) + 1
            let uniqcode = key.name
            let wc: WildcardVM = {
                Id: id,
                Name: key.name,
                Key: key.name,
                Code: "{"+uniqcode+"}",
                RecepientFilterId: this.va.curmetafilter.Id,
                ng_JustCreated: true,
                ng_ToDelete: false,
                _Code: uniqcode,
                Invalid: false,
                ValidationErrors: [],
                FilterValueContainers: []
            }
            if (IsNullOrUndefined(this.va.curmetafilter.wildcards))
                this.va.curmetafilter.wildcards = []
            this.va.curmetafilter.wildcards.unshift(wc)
            this.validateWildcard(wc)
        }

        updateWildCardCode = (wc: WildcardVM) => {
            wc.Code = "{" + wc._Code + "}"
        }

        //Filters

        typeOperators: col.IDictionary<string[]> = new col.Dictionary<string[]>()
        typeOperatorsSQL: col.IDictionary<string[]> = new col.Dictionary<string[]>()

        fetchTypeOperators = (typeName: string) => {
            this.request(true, {
                urlalias: "getoperators",
                params: { typename: typeName},
                onSucces: (response) => {
                    this.typeOperators.add(typeName, response.data.items)
                    let names = []
                    response.data.items.forEach(x => names.push(x.SQLString)) 
                    this.typeOperatorsSQL.add(typeName, names)
                }
            })
        }

        newFilter = (key: KeyVM) => {
            this.va.SHOW_FILTS = true
            if (!this.typeOperators.cont(key.type))
                this.fetchTypeOperators(key.type)
            let filt:FilterVM = {
                Id: -1,
                Name: key.name,
                Key: key.name,
                RecepientFilterId: this.va.curmetafilter.Id,
                //Operator: ["="],
                //Value: [""],
                ValsOps: [new ValOp()],
                Type: key.type,
                Invalid: false,
                ValidationErrors: [],
                ng_JustCreated: true,
                ng_ToDelete: false,
                allowMultipleSelection: false,
                allowUserInput: false,
                autoUpdatedList: false
            }
            if (IsNullOrUndefined(this.va.curmetafilter.filters))
                this.va.curmetafilter.filters = []
            this.va.curmetafilter.filters.unshift(filt)
        }



        //Styling, others



    }
}