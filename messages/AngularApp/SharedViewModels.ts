namespace AngularApp.Controllers {

    export interface INgViewModel {
        ng_JustCreated:boolean
        ng_ToDelete:boolean
    }

    export interface IIndeficated {
        Id: number
    }

    export interface IValidable {
        Invalid: boolean
        ValidationErrors: string[]
    }

    export class BaseTableVM implements IIndeficated, INgViewModel {
        Id: number
        Name: string
        ReferencedTableName: string
        ng_JustCreated: boolean
        ng_ToDelete: boolean
    }

    export class MetaFilterVM implements IIndeficated, INgViewModel, IValidable {
        Id: number
        filters: FilterVM[] = []
        wildcards: WildcardVM[] = []
        reccards: RecepientCardVM[] = []

        Name: string = "New Filter"
        BaseTableId: number = -1

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        //local
        //validate filter enitities, Invalid Filter can't be saved
        ValidationErrors: string[]

        Invalid = false
    }

    export class KeyVM {
        name: string
        type: string
    }

    export class ValOp {
        Value: any
        Operator: string

        constructor() {
            this.Value = ""
            this.Operator = "="
        }
    }

    export class FilterVM implements IIndeficated, INgViewModel, IValidable {
        Id: number
        Name: string
        RecepientFilterId: number
        Key: string
        ValsOps: ValOp[]
        Type: string

        allowMultipleSelection:boolean
        allowUserInput: boolean
        autoUpdatedList:boolean

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        //local
        Invalid = false
        ValidationErrors: string[]

        //Show:? boolean
    }

    export class FilterValueContainer {
        FilterId: number
        Values: any[] = []
    }

    export class WildcardVM implements IIndeficated, INgViewModel, IValidable {
        Id: number
        RecepientFilterId: number
        Name: string
        Code: string
        Key: string

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        //local
        _Code: string
        Invalid = false
        ValidationErrors: string[]

        FilterValueContainers: FilterValueContainer[]
    }

    export class RecepientCardVM implements IIndeficated, INgViewModel, IValidable {
        Id: number
        RecepientFilterId: number
        Name: string

        NameKey: string
        EmailKey: string
        PhoneKey: string

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        //local
        Invalid = false
        ValidationErrors: string[]
    }

    export class FiltOperator {
        SQLString: string
        ShortString: string
        Operator: number
        RawInt: number
    }

    export class TemplateVM implements IIndeficated, INgViewModel {
        Id: number
        RecepientFilterId: number = -1
        Name: string = "New Template"
        IsSms: boolean
        MsgHeader: string
        MsgBody: string

        ng_JustCreated: boolean
        ng_ToDelete: boolean

        FilterValueContainers: FilterValueContainer[] = []
        ChoosenReccards: number[] = []
    }

    export function FindById<T extends IIndeficated>(arr: T[], Id: number) {
        return arr.first(x => x.Id === Id)
    }

}