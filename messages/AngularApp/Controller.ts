namespace AngularApp {
    import col = TSNetLike.Collections
    import fnc = TSNetLike.Functors

    export interface IRequestArgs {
        urlalias: string,
        params?: Object,
        before?: () => void,
        onSucces?: ((response?) => void) | ConcurentRequestHandler,
        onFailed?: ((response?) => void) | ConcurentRequestHandler,
    }

    /**
     This Class used for simultaneously running requests to server.
     It runs it's callback only when all binded requests will be ended.
     */
    export class ConcurentRequestHandler {
        private _registered = 0
        /** 
        callOnlyOnce = true, means if callback WAS CALLED already, it won't be called again.
        callOnlyOnce = false, if all requests ended already, callback will be called, even if it was called already.
        */
        constructor(protected callback: (response?) => void, private callOnlyOnce = true) {}

        RegisterRequestStart = () => {
            this._registered++
        }

        TryCall = (response) => {
            this._registered--
            if (this._registered < 1) {
                if (this.callOnlyOnce && this._registered == 0)
                    fnc.F(this.callback,response)
                else if (!this.callOnlyOnce)
                    fnc.F(this.callback, response)
            }

        }
    }

    export function isEmptyOrSpaces(str: string) {
        return typeof str === 'undefined' || str === null || str.match(/^ *$/) !== null;
    }

    function IsConcurentRequestHandler(cb) {
        return !(IsNullOrUndefined(cb) || IsNullOrUndefined(cb.TryCall))
    }

    function RunCallbackOrHandler(callback: ((response?) => void) | ConcurentRequestHandler, response) {
        //argument type check
        if (IsNullOrUndefined(callback)) return
        let cb = callback as any
        if (IsConcurentRequestHandler(cb))
            cb.TryCall(response)
        else
            fnc.F(cb, response)
    }

    export class FetchParams {
        Skip = null
        Count = null
        filters = []

        addSkip = (val: number) => {
            this.Skip = val
            return this
        }
        addCount = (val: number) => {
            this.Count = val
            return this
        }
        addFilt = (key: string, value?: any, operator?: string) => {
            //let val = typeof value === 'undefined' ? null : value.toString()
            this.filters.push({ key: key, val: value,op:operator})
            return this
        }
    }

    function CloneRequestArgs(data: IRequestArgs): IRequestArgs {
        return {
            urlalias: data.urlalias,
            params: data.params,
            before: data.before,
            onSucces: data.onSucces,
            onFailed: data.onFailed
        }
    }

    export function CloneShallow<T>(original: T) {
        let clone:any = {}
        for (let key in original) {
            if (original.hasOwnProperty(key)) {
                clone[key] = original[key]
            }
        }
        return clone as T
    }

    /**this doesnt handles recoursive references!*/
    export function CloneDeep<T>(original: T) {
        let clone: any = {}
        for (let key in original) {
            if (original.hasOwnProperty(key)) {
                if (original[key] instanceof Array) {
                    clone[key] = [];
                    let arr:any[] = clone[key];
                    original[key].forEach(ele => arr.push(CloneDeep(ele)))
                }
                else if (typeof original[key] === "object")
                    clone[key] = CloneDeep(original[key])
                else
                    clone[key] = original[key]
            }
        }
        return clone as T
    }

    export function ParseHtmlID(fullID: string, separator: string = "___::::___") {
        return fullID.split(separator)[1]
    }

    /**
    Returns Array:
    array[0] - prefix
    array[1] - id
     */
    export function ParseHtmlIDFull(fullID: string, separator: string = "___::::___") {
        return fullID.split(separator)
    }

    export function MakeHtmlID (prefix: string, id: string, separator: string = "___::::___") {
        return prefix+separator+id
    }

    export function IsNullOrUndefined(obj) {
        return typeof obj === 'undefined' || obj === null
    }

    export abstract class Controller<TViewAgent> {
        abstract init (data: any): void
        abstract buildVa(): TViewAgent

        static $inject = ['$rootScope', '$scope', '$http']

        /**ViewAgent*/
        va: TViewAgent

        constructor(protected rootScope, protected scope, protected http) {
            this.scope.holdview = false
            this.va = this.buildVa()
            this.scope.va = this.va
            this.scope.init = (data) => this.init(data)
            this.scope.notif = (header, body, type, showdelay) =>
                this.ShowNotification(header, body, type, showdelay)

            scope.$on('$destroy', () => {
                //here we Dispose All Resources used by controller
                if (this.TurnHoldViewOnOthersControllers_on)
                    this.ControllerHoldedView_unsubcribe()
            })
            
        }

        private urls: col.IDictionary<string>
        protected initUrlModule =
            (urls: col.IDictionary<string>) => {
                this.urls = urls
            }
        protected initUrlModuleFromRowObj =
            (urls: Object) =>{
                this.urls = new col.Dictionary<string>(urls)
            }
        private url = (alias: string) => {
            return this.urls.take(alias)
        }

        private TurnHoldViewCount = 0
        private turnHoldView =
            (state) => {
                if (state) this.TurnHoldViewCount++
                else this.TurnHoldViewCount--
                if (this.TurnHoldViewCount <= 0) {
                    this.TurnHoldViewCount = 0
                    this.scope.holdview = false
                }
                else {
                    this.scope.holdview  = true
                }
                this.rootScope.$broadcast('ControllerHoldedView', { controller: this, state: state });
            }

        private TurnHoldViewOnOthersControllers_on = false
        private ControllerHoldedView_unsubcribe: () => void
        protected TurnHoldViewOnOthersControllers = () => {
            if (this.TurnHoldViewOnOthersControllers_on === true) return
            this.TurnHoldViewOnOthersControllers_on = true
            this.ControllerHoldedView_unsubcribe = this.rootScope.$on('ControllerHoldedView', (event, args) => {
                if (args.controller === this) return
                this.turnHoldView(args.state)
            });
        }

        /**This action works if page has Notification controller*/
        protected ShowNotification = (header: string, body: string, type: Controllers.NotifType, showdelay?:number) => {
            let msg = new Controllers.NotifMessage(header, body, type, showdelay)
            this.rootScope.$broadcast(
                Controllers.NotificationController.MSGEVENT,{ message: msg });
        }

        protected request_msgHandlerSucces: (msg: string) => void = null

        protected request_msgHandlerFail: (msg: string) => void = null

        protected request = (holdTillResponse: boolean, data: IRequestArgs) => {
            if (IsConcurentRequestHandler(data.onSucces))
                (data.onSucces as ConcurentRequestHandler).RegisterRequestStart()
            if (IsConcurentRequestHandler(data.onFailed))
                (data.onFailed as ConcurentRequestHandler).RegisterRequestStart()

            if (holdTillResponse)
                this.turnHoldView(true)
            fnc.F(data.before)
            this.http({ method: 'POST', url: this.url(data.urlalias), data: data.params }).
                then(
                    (response) => {
                        if (this.request_msgHandlerSucces !== null
                            && !IsNullOrUndefined(response.data.message))
                            this.request_msgHandlerSucces(response.data.message)
                        RunCallbackOrHandler(data.onSucces,response)
                        if (holdTillResponse)
                            this.turnHoldView(false)
                    },
                    (response) => {
                        if (!IsNullOrUndefined(response.data)) {
                            if (this.request_msgHandlerFail !== null
                                && !IsNullOrUndefined(response.data.message))
                                this.request_msgHandlerFail(response.data.message)
                            RunCallbackOrHandler(data.onSucces, response)
                        }
                        else if (this.request_msgHandlerFail !== null)
                            this.request_msgHandlerFail("no connection to server")
                        if (holdTillResponse)
                            this.turnHoldView(false)
                    }
                );
        }

        /**looks on response.data.items*/
        protected fetchtodict = <V> (holdTillResponse: boolean, data: IRequestArgs,
            container: col.IDictionary<V>,
            keyValueSelector: (obj: any) => col.IKeyValuePair<string, V>,
            clearContainer?: boolean) =>
        {
            if (IsConcurentRequestHandler(data.onSucces))
                (data.onSucces as ConcurentRequestHandler).RegisterRequestStart()
            if (IsConcurentRequestHandler(data.onFailed))
                (data.onFailed as ConcurentRequestHandler).RegisterRequestStart()

            let successCB = data.onSucces
            data = CloneRequestArgs(data)
            data.onSucces = (response) => {
                if (clearContainer) container.clear()
                container.addrange(response.data.items, keyValueSelector)
                RunCallbackOrHandler(successCB, response)
            }
            this.request(holdTillResponse, data)
        }

        /**looks on response.data.items*/
        protected fetchtoarr =
        (holdTillResponse: boolean, data: IRequestArgs, container: any[], clearContainer?: boolean) =>
        {
            if (IsConcurentRequestHandler(data.onSucces))
                (data.onSucces as ConcurentRequestHandler).RegisterRequestStart()
            if (IsConcurentRequestHandler(data.onFailed))
                (data.onFailed as ConcurentRequestHandler).RegisterRequestStart()

            let successCB = data.onSucces
            data = CloneRequestArgs(data)
            data.onSucces = (response) => {
                if (clearContainer) container.splice(0, container.length)
                response.data.items.forEach(e => container.push(e))
                RunCallbackOrHandler(successCB, response)
            }
            this.request(holdTillResponse,data)
        }

        /**looks on response.data.items*/
        protected updatetoarr =
        (holdTillResponse: boolean, data: IRequestArgs, container: any[], equalityPredicate: (o1, o2) => boolean, pushnew: boolean = true) =>
        {
            if (IsConcurentRequestHandler(data.onSucces))
                (data.onSucces as ConcurentRequestHandler).RegisterRequestStart()
            if (IsConcurentRequestHandler(data.onFailed))
                (data.onFailed as ConcurentRequestHandler).RegisterRequestStart()
            let successCB = data.onSucces
            data = CloneRequestArgs(data)
            data.onSucces = (response) => {
                response.data.items.forEach((e1) => {
                    let index = -1
                    for (let i = 0; i < container.length; i++) {
                        if (equalityPredicate(e1, container[i])) {
                            index = i
                            break
                        }
                    }
                    if (index != -1)
                        container[index] = e1
                    else if (pushnew)
                        container.push(e1)
                })
                RunCallbackOrHandler(successCB, response)
            }
            this.request(holdTillResponse, data)
        }
    }



}