namespace AngularApp.Controllers {
    import col = TSNetLike.Collections

    class MFiltersVA {
        basetables: BaseTableVM[] = []
        curbasetable: BaseTableVM
        showTableChoose: boolean = false

        possiblekeys: KeyVM[] = []

        metafilters: MetaFilterVM[] = []
        curmetafilter: MetaFilterVM = null
        curmetafilterBaseTableName = () => {
            let t = FindById(this.basetables, this.curmetafilter.BaseTableId)
            return t === undefined ? "" : t.Name
        }

        wildcardCreator_ID = '__wildcardCreator'
        filterCreator_ID = '__filterCreator'

        keyIdPrefix = '_tablekey'
        keyDragClass = 'tablekey'
    }

    export interface IIndeficated {
        Id:number
    }

    export class BaseTableVM implements IIndeficated {
        Id: number
        Name: string
        ReferencedTableName: string
    }

    export class MetaFilterVM implements IIndeficated {
        Id: number
        filters: FilterVM[] = []
        wildcards: WildcardVM[] = []

        Name: string = "New Filter"
        BaseTableId: number = -1

        //this arrays used to check changes on server, after fetch there are empty
        newfilters: FilterVM[] = []
        newwildcards: WildcardVM[] = []

        removedfilters: FilterVM[] = []
        removedwildcards: WildcardVM[] = []
        //local
        //not sended ?

        //TODO
        //validate filter enitities, Invalid Filter can't be saved
        Invalid = false
    }

    export class KeyVM {
        name: string
        type: string
    }

    export class FilterVM implements IIndeficated {
        Id: number
        RecepientFilterId: number
        Key: string
        Value: string
        Operator: string
        Type: string

        //local
        Invalid = false
    }

    export class WildcardVM {
        Id: number
        RecepientFilterId: number
        Name: string
        Code: string
        Key: string

        //local
        _Code: string

        Invalid = false
    }

    export class FiltOperator {
        SQLString: string
        ShortString: string
        Operator: number
        RawInt: number
    }

    export function FindById<T extends IIndeficated>(arr: T[], Id: number) {
        return arr.first(x => x.Id === Id)
    }

    export class MFiltersController extends Controller<MFiltersVA> {
        constructor($rootScope, $scope, $http) {
            super($rootScope, $scope, $http)
        }
        buildVa(): MFiltersVA { return new MFiltersVA }
        init(data): void {

            //------------------- Scope Init

            this.scope.NewMFilt = () => this.turnMFilterCreate()

            this.scope.EditMFilt = (mfilt: MetaFilterVM) => this.turnMFilterEdit(mfilt)

            this.scope.RemoveMfilt = (mfilt: MetaFilterVM) => alert("NOT IMPLEMENTED =/")

            this.scope.CloseEditor = () => this.closeMFiltEditor()

            this.scope.TypeToIcon = (type: string) => this.glyphiconforSQLTYPE(type)

            this.scope.InputType = (type: string) => this.inputTypeForSQLType(type)

            this.scope.SetCurBaseTable = (t: BaseTableVM) => this.setCurBaseTable(t)

            this.scope.ShowTableChoose = () => this.va.showTableChoose = true

            this.scope.SetCurTableToCurMFilt = () => {
                this.va.curmetafilter.BaseTableId = this.va.curbasetable.Id
                let table = FindById(this.va.basetables, this.va.curmetafilter.BaseTableId)
                this.refetchPossibleKeys(table)
                this.va.showTableChoose = false
            }

            this.scope.SetCurTableFromCurMFilt = () => {
                let table = FindById(this.va.basetables, this.va.curmetafilter.BaseTableId)
                this.va.curbasetable = table === undefined ? null : table
                this.refetchPossibleKeys(table)
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

            this.scope.NewWildcardDrop = (dragID: string, dropID: string, dragClass: string) => {
                if (dragClass != this.va.keyDragClass) return
                let keyname = ParseHtmlID(dragID)
                let key = this.va.possiblekeys.first(x => x.name === keyname)
                this.newWildCard(key)
            }

            this.scope.GetTypeOperators = (type: string) => this.typeOperators.take(type)
            this.scope.GetTypeOperatorsNames = (type: string) => this.typeOperatorsSQL.take(type)

            this.scope.RemoveFilter = (filter: FilterVM) => this.deleteFilter(filter)
            this.scope.RemoveWildcard = (wc: WildcardVM) => this.deleteWildCard(wc)

            this.scope.WildCardUpdateCode = (wc: WildcardVM) => this.updateWildCardCode(wc)     

            this.scope.SaveCurMFilt = () => this.saveOrUpdateCurMFilter()

            //------------------- Inner Init

            this.initUrlModuleFromRowObj(data.urls)
            this.refetchTables()
            this.refetchMfilters()     
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
                this.refetchPossibleKeys(table)
            else
                this.clearPossibleKeys()
        }

        validateEntities = () => {
            this.va.curmetafilter.wildcards.forEach(wc =>
                wc.Invalid = !this.validateWithKeys(wc, (item, key) => key.name === item.Key)
            )
            this.va.curmetafilter.filters.forEach(wc =>
                wc.Invalid = !this.validateWithKeys(wc, (item, key) => key.name === item.Key && key.type === item.Type)
            )
        }

        refetchPossibleKeys = (table: BaseTableVM) => {
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

            //TODO clone DEEP ?
            this.va.curmetafilter = CloneShallow(mfilt)
            this.va.curmetafilter.filters = []
            this.va.curmetafilter.wildcards = []

            this.fetchtoarr(true,
                {
                    urlalias: "getfilters",
                    //todo this is a very bad : tblRecepientFilterId, avoid this, use convention approach at least
                    params: new FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                    onSucces: () => this.refetchPossibleKeys(table)
                },
                this.va.curmetafilter.filters, true);
            this.fetchtoarr(true,
                {
                    urlalias: "getwildcards",
                    params: new FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                    onSucces: () => this.refetchPossibleKeys(table)
                },
                this.va.curmetafilter.wildcards, true);
        }

        closeMFiltEditor = () => {
            this.va.curmetafilter = null
        }

        saveOrUpdateCurMFilter = () => {
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

        //Wildcards

        newWildCard = (key: KeyVM) => {
            let wc: WildcardVM = new WildcardVM()
            wc.Id = -1
            wc.Name = key.name
            wc.Key = key.name
            wc.Code = "{CODE}"
            wc.RecepientFilterId = this.va.curmetafilter.Id
            if (IsNullOrUndefined( this.va.curmetafilter.newwildcards) )
                this.va.curmetafilter.newwildcards = []
            this.va.curmetafilter.newwildcards.push(wc)
            this.va.curmetafilter.wildcards.push(wc)
        }

        deleteWildCard = (wc: WildcardVM) => {
            if (wc.Id != -1)
                this.va.curmetafilter.removedwildcards.push(wc)
            else
                this.va.curmetafilter.newwildcards.remove(wc)
            this.va.curmetafilter.wildcards.remove(wc)
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
            if (!this.typeOperators.cont(key.type))
                this.fetchTypeOperators(key.type)
            let filt:FilterVM = {
                Id: -1,
                Key: key.name,
                Operator: "=",
                RecepientFilterId: this.va.curmetafilter.Id,
                Value: "",
                Type: key.type,
                Invalid: false
            }
            if (IsNullOrUndefined(this.va.curmetafilter.newfilters))
                this.va.curmetafilter.newfilters = []
            this.va.curmetafilter.newfilters.push(filt)
            this.va.curmetafilter.filters.push(filt)
        }

        deleteFilter = (filt: FilterVM) => {
            if (filt.Id != -1)
                this.va.curmetafilter.removedfilters.push(filt)
            else
                this.va.curmetafilter.newfilters.remove(filt)
            this.va.curmetafilter.filters.remove(filt)
        }

        validateWithKeys<T>(item: T, validator: (item: T, key: KeyVM) => boolean): boolean {
            for (let i = 0; i < this.va.possiblekeys.length; i++) {
                if (validator(item, this.va.possiblekeys[i]))
                    return true
            }
            return false
        }


        //Styling, others

        glyphiconforSQLTYPE = (typeName: string) => {
            switch (typeName) {
                case "int":
                    return "glyphicon glyphicon-certificate"
                case "nvarchar":
                    return "glyphicon glyphicon-font"
                case "datetime":
                case "date":
                    return "glyphicon glyphicon-calendar"
                case "time":
                    return "glyphicon glyphicon-time"
                case "bit":
                    return "glyphicon glyphicon-check"
            }
            return "glyphicon glyphicon-briefcase"
        }

        inputTypeForSQLType = (typeName: string) => {
            switch (typeName) {
                case "int":
                    return "number"
                case "nvarchar":
                    return "text"
                case "datetime":
                case "date":
                    return "datetime-local"
                case "time":
                    return "time"
                case "bit":
                    return "checkbox"
            }
            return "text"
        }

    }
}