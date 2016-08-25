var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
        var col = TSNetLike.Collections;
        var MFiltersVA = (function () {
            function MFiltersVA() {
                var _this = this;
                this.basetables = [];
                this.showTableChoose = false;
                this.possiblekeys = [];
                this.reservedwildcards = [];
                this.metafilters = [];
                this.curmetafilter = null;
                this.curmetafilterBaseTableName = function () {
                    var t = Controllers.FindById(_this.basetables, _this.curmetafilter.BaseTableId);
                    return t === undefined ? "" : t.Name;
                };
                this.wildcardCreator_ID = '__wildcardCreator';
                this.filterCreator_ID = '__filterCreator';
                this.reccardCreator_ID = '__recepientcardCreator';
                this.reccardDropName = "___reccardDropName";
                this.reccardDropEmail = "___reccardDropEmail";
                this.reccardDropPhone = "___reccardDropPhone";
                this.keyIdPrefix = '_tablekey';
                this.keyDragClass = 'tablekey';
                //just for folding
                this.SHOW_FILTS = false;
                this.SHOW_RCARDS = false;
                this.SHOW_WCARDS = false;
            }
            return MFiltersVA;
        }());
        var MFiltersController = (function (_super) {
            __extends(MFiltersController, _super);
            function MFiltersController($rootScope, $scope, $http) {
                var _this = this;
                _super.call(this, $rootScope, $scope, $http);
                //BaseTables i.e. RecepientFilterTableName
                this.refetchTables = function (onSucces) {
                    _this.fetchtoarr(true, { urlalias: "gettables" }, _this.va.basetables, true);
                };
                this.refetchMfilters = function (onSucces) {
                    _this.fetchtoarr(true, { urlalias: "getmfilters" }, _this.va.metafilters, true);
                };
                this.setCurBaseTable = function (table) {
                    _this.va.curbasetable = table;
                    if (table !== null)
                        _this.refetchPossibleKeysAndValidate(table);
                    else
                        _this.clearPossibleKeys();
                };
                //VALIDATION
                this.validateEntities = function () {
                    var filt = _this.va.curmetafilter;
                    var entitiesInvalid = false;
                    filt.ValidationErrors = [];
                    var itemNameInvalid = AngularApp.isEmptyOrSpaces(filt.Name);
                    if (itemNameInvalid)
                        filt.ValidationErrors.push("Recepient filter name cannot be empty");
                    var noRecepients = filt.reccards.filter(function (x) { return !x.ng_ToDelete; }).length === 0;
                    if (noRecepients)
                        filt.ValidationErrors.push("Recepient filter should have at least one recepient card");
                    filt.wildcards.forEach(function (x) {
                        _this.validateWildcard(x);
                        if (x.Invalid && !entitiesInvalid)
                            entitiesInvalid = true;
                    });
                    filt.filters.forEach(function (x) {
                        _this.validateFilter(x);
                        if (x.Invalid && !entitiesInvalid)
                            entitiesInvalid = true;
                    });
                    filt.reccards.forEach(function (x) {
                        _this.validateReccard(x);
                        if (x.Invalid && !entitiesInvalid)
                            entitiesInvalid = true;
                    });
                    filt.Invalid = entitiesInvalid || itemNameInvalid || noRecepients;
                };
                this.validateWildcard = function (item) {
                    item.ValidationErrors = [];
                    if (item.ng_ToDelete) {
                        item.Invalid = false;
                        return;
                    }
                    var itemNameInvalid = AngularApp.isEmptyOrSpaces(item.Name);
                    if (itemNameInvalid)
                        item.ValidationErrors.push("name cannot be empty");
                    var codeEmpty = AngularApp.isEmptyOrSpaces(item._Code);
                    if (codeEmpty)
                        item.ValidationErrors.push("code cannot be empty");
                    var keysInvalid = !_this.validateWithKeys(item, function (item, key) { return key.name === item.Key; });
                    if (keysInvalid)
                        item.ValidationErrors.push("base table has no such key [" + item.Key + "]");
                    var codeDublicated = !_this.validateDublCodeWildcard(item);
                    if (codeDublicated)
                        item.ValidationErrors.push("wildcard code is not uniq: " + item.Code);
                    item.Invalid = keysInvalid || itemNameInvalid || codeDublicated || codeEmpty;
                };
                this.validateDublCodeWildcard = function (wc) {
                    var cards = _this.va.curmetafilter.wildcards;
                    for (var i = 0; i < cards.length; i++) {
                        if (cards[i].Code === wc.Code && cards[i] !== wc)
                            return false;
                    }
                    cards = _this.va.reservedwildcards;
                    for (var i = 0; i < cards.length; i++) {
                        if (cards[i].Code === wc.Code)
                            return false;
                    }
                    return true;
                };
                this.validateReccard = function (item) {
                    item.ValidationErrors = [];
                    if (item.ng_ToDelete) {
                        item.Invalid = false;
                        return;
                    }
                    var itemNameInvalid = AngularApp.isEmptyOrSpaces(item.Name);
                    if (itemNameInvalid)
                        item.ValidationErrors.push("name cannot be empty");
                    var someKeyIsEmpty = AngularApp.isEmptyOrSpaces(item.EmailKey) || AngularApp.isEmptyOrSpaces(item.NameKey) || AngularApp.isEmptyOrSpaces(item.PhoneKey);
                    if (someKeyIsEmpty)
                        item.ValidationErrors.push("recepient card can't have empty keys");
                    var keysInvalid = false;
                    if (!someKeyIsEmpty) {
                        var mailInvalid = !_this.validateWithKeys(item, function (item, key) { return key.name === item.EmailKey; });
                        if (mailInvalid)
                            item.ValidationErrors.push("base table has no such key [" + item.EmailKey + "]");
                        var nameInvalid = !_this.validateWithKeys(item, function (item, key) { return key.name === item.NameKey; });
                        if (nameInvalid)
                            item.ValidationErrors.push("base table has no such key [" + item.NameKey + "]");
                        var phoneInvalid = !_this.validateWithKeys(item, function (item, key) { return key.name === item.PhoneKey; });
                        if (phoneInvalid)
                            item.ValidationErrors.push("base table has no such key [" + item.PhoneKey + "]");
                        keysInvalid = mailInvalid || nameInvalid || phoneInvalid;
                    }
                    item.Invalid = keysInvalid || itemNameInvalid || someKeyIsEmpty;
                };
                this.validateFilter = function (item) {
                    item.ValidationErrors = [];
                    if (item.ng_ToDelete) {
                        item.Invalid = false;
                        return;
                    }
                    var itemNameInvalid = AngularApp.isEmptyOrSpaces(item.Name);
                    if (itemNameInvalid)
                        item.ValidationErrors.push("name cannot be empty");
                    var keysInvalid = !_this.validateWithKeys(item, function (item, key) { return key.name === item.Key && key.type === item.Type; });
                    if (keysInvalid)
                        item.ValidationErrors.push("base table has no such key [" + item.Key + "] with type [" + item.Type + "]");
                    item.Invalid = keysInvalid || itemNameInvalid;
                };
                this.getMetaFilterValidationString = function (filt) {
                    var output = "";
                    var tmp;
                    tmp = filt.ValidationErrors.join(";\n ");
                    if (!AngularApp.isEmptyOrSpaces(tmp))
                        output += tmp + ".\n";
                    tmp = "";
                    filt.reccards.forEach(function (x) { return tmp += _this.getValidationString(x, 'Recepient Card "' + x.Name + '"'); });
                    if (!AngularApp.isEmptyOrSpaces(tmp))
                        output += tmp + ".\n";
                    tmp = "";
                    filt.wildcards.forEach(function (x) { return output += _this.getValidationString(x, 'Wildcard "' + x.Name + '"'); });
                    if (!AngularApp.isEmptyOrSpaces(tmp))
                        output += tmp + ".\n";
                    tmp = "";
                    filt.filters.forEach(function (x) { return output += _this.getValidationString(x, 'Subfilter "' + x.Key + '"'); });
                    if (!AngularApp.isEmptyOrSpaces(tmp))
                        output += tmp + ".\n";
                    return output;
                };
                this.getValidationString = function (item, name, addValid) {
                    if (addValid === void 0) { addValid = false; }
                    if (AngularApp.IsNullOrUndefined(item))
                        throw new Error("can't get validation message from null or undefined");
                    //it is metafilter silly type check
                    if (!AngularApp.IsNullOrUndefined(item.BaseTableId))
                        return _this.getMetaFilterValidationString(item);
                    if (AngularApp.IsNullOrUndefined(item.ValidationErrors) || item.ValidationErrors.length === 0) {
                        if (addValid)
                            return name + " has no noticed validation errors";
                        else
                            return "";
                    }
                    var output = name + " has " + item.ValidationErrors.length + " errors: \n";
                    output += item.ValidationErrors.join(";\n ");
                    return output;
                };
                this.refetchPossibleKeysAndValidate = function (table) {
                    var cb = function () {
                        if (_this.va.curmetafilter !== null)
                            _this.validateEntities();
                    };
                    _this.fetchtoarr(true, { urlalias: "getcolomns", params: { id: table.Id }, onSucces: cb }, _this.va.possiblekeys, true);
                };
                this.clearPossibleKeys = function () {
                    _this.va.possiblekeys = [];
                };
                //MFilters i.e. MetaFilters i.e. RecepientFilters
                this.turnMFilterCreate = function () {
                    _this.setCurBaseTable(null);
                    _this.va.curmetafilter = new Controllers.MetaFilterVM();
                    _this.va.curmetafilter.Id = -1;
                    _this.va.showTableChoose = true;
                };
                this.turnMFilterEdit = function (mfilt) {
                    _this.va.showTableChoose = false;
                    var table = Controllers.FindById(_this.va.basetables, mfilt.BaseTableId);
                    _this.setCurBaseTable(table);
                    _this.va.curmetafilter = AngularApp.CloneShallow(mfilt);
                    _this.va.curmetafilter.filters = [];
                    _this.va.curmetafilter.wildcards = [];
                    _this.va.curmetafilter.reccards = [];
                    //todo this is a bit bad : tblRecepientFilterId, avoid this, use convention approach at least
                    var params = new AngularApp.FetchParams().addFilt("tblRecepientFilterId", mfilt.Id);
                    var cb = new AngularApp.ConcurentRequestHandler(function () {
                        _this.refetchPossibleKeysAndValidate(table);
                        _this.va.curmetafilter.wildcards.forEach(function (wc) {
                            wc._Code = wc.Code.substr(1, wc.Code.length - 2);
                        });
                        _this.va.curmetafilter.filters.forEach(function (ele) {
                            if (!_this.typeOperators.cont(ele.Type))
                                _this.fetchTypeOperators(ele.Type);
                            Controllers.formatValsOps(ele.ValsOps, ele.Type);
                        });
                    }, true);
                    _this.fetchtoarr(true, { urlalias: "getfilters", params: params, onSucces: cb }, _this.va.curmetafilter.filters, true);
                    _this.fetchtoarr(true, { urlalias: "getwildcards", params: params, onSucces: cb }, _this.va.curmetafilter.wildcards, true);
                    _this.fetchtoarr(true, { urlalias: "getreccards", params: params, onSucces: cb }, _this.va.curmetafilter.reccards, true);
                };
                this.closeMFiltEditor = function () {
                    _this.va.curmetafilter = null;
                };
                this.saveOrUpdateCurMFilter = function () {
                    _this.validateEntities();
                    if (_this.va.curmetafilter.Invalid) {
                        var erorrs = _this.getValidationString(_this.va.curmetafilter, _this.va.curmetafilter.Name);
                        _this.ShowNotification("Validation Errors", "you should fix errors first: \n" + erorrs, { glicon: "ban-circle", nclass: "error" });
                        return;
                    }
                    var mode = _this.va.curmetafilter.Id === -1 ? "cr" : "up";
                    _this.request(true, {
                        urlalias: "mngmfilter",
                        params: {
                            mode: mode,
                            models: [_this.va.curmetafilter]
                        },
                        onSucces: function () {
                            _this.closeMFiltEditor();
                            _this.refetchMfilters();
                        }
                    });
                };
                //Common for sub-items: (wildcards, recepient cards, filters)
                this.RemoveItem = function (item) {
                    item.ng_ToDelete = true;
                };
                this.RestoreItem = function (item) {
                    item.ng_ToDelete = false;
                };
                //RecepientCards
                this.newRecCard = function (key) {
                    _this.va.SHOW_RCARDS = true;
                    if (AngularApp.IsNullOrUndefined(_this.va.curmetafilter.filters))
                        _this.va.curmetafilter.reccards = [];
                    var id = _this.va.curmetafilter.reccards.max(function (x) { return x.Id; }) + 1;
                    var rc = {
                        Id: id,
                        Name: key.name,
                        NameKey: key.name,
                        EmailKey: "",
                        PhoneKey: "",
                        RecepientFilterId: _this.va.curmetafilter.Id,
                        Invalid: false,
                        ValidationErrors: [],
                        ng_JustCreated: true,
                        ng_ToDelete: false
                    };
                    _this.va.curmetafilter.reccards.unshift(rc);
                };
                this.RecCardAddKey = function (card, key, reccardField) {
                    switch (reccardField) {
                        case _this.va.reccardDropName:
                            card.NameKey = key.name;
                            break;
                        case _this.va.reccardDropEmail:
                            card.EmailKey = key.name;
                            break;
                        case _this.va.reccardDropPhone:
                            card.PhoneKey = key.name;
                            break;
                    }
                };
                //Wildcards
                this.newWildCard = function (key) {
                    _this.va.SHOW_WCARDS = true;
                    var id = _this.va.curmetafilter.wildcards.max(function (x) { return x.Id; }) + 1;
                    var uniqcode = key.name;
                    var wc = {
                        Id: id,
                        Name: key.name,
                        Key: key.name,
                        Code: "{" + uniqcode + "}",
                        RecepientFilterId: _this.va.curmetafilter.Id,
                        ng_JustCreated: true,
                        ng_ToDelete: false,
                        _Code: uniqcode,
                        Invalid: false,
                        ValidationErrors: [],
                        FilterValueContainers: []
                    };
                    if (AngularApp.IsNullOrUndefined(_this.va.curmetafilter.wildcards))
                        _this.va.curmetafilter.wildcards = [];
                    _this.va.curmetafilter.wildcards.unshift(wc);
                    _this.validateWildcard(wc);
                };
                this.updateWildCardCode = function (wc) {
                    wc.Code = "{" + wc._Code + "}";
                };
                //Filters
                this.typeOperators = new col.Dictionary();
                this.typeOperatorsSQL = new col.Dictionary();
                this.fetchTypeOperators = function (typeName) {
                    _this.request(true, {
                        urlalias: "getoperators",
                        params: { typename: typeName },
                        onSucces: function (response) {
                            _this.typeOperators.add(typeName, response.data.items);
                            var names = [];
                            response.data.items.forEach(function (x) { return names.push(x.SQLString); });
                            _this.typeOperatorsSQL.add(typeName, names);
                        }
                    });
                };
                this.newFilter = function (key) {
                    _this.va.SHOW_FILTS = true;
                    if (!_this.typeOperators.cont(key.type))
                        _this.fetchTypeOperators(key.type);
                    var filt = {
                        Id: -1,
                        Name: key.name,
                        Key: key.name,
                        RecepientFilterId: _this.va.curmetafilter.Id,
                        //Operator: ["="],
                        //Value: [""],
                        ValsOps: [new Controllers.ValOp()],
                        Type: key.type,
                        Invalid: false,
                        ValidationErrors: [],
                        ng_JustCreated: true,
                        ng_ToDelete: false,
                        allowMultipleSelection: false,
                        allowUserInput: false,
                        autoUpdatedList: false
                    };
                    if (AngularApp.IsNullOrUndefined(_this.va.curmetafilter.filters))
                        _this.va.curmetafilter.filters = [];
                    _this.va.curmetafilter.filters.unshift(filt);
                };
            }
            MFiltersController.prototype.buildVa = function () { return new MFiltersVA; };
            MFiltersController.prototype.init = function (data) {
                var _this = this;
                //------------------- RequestMsgs
                this.request_msgHandlerSucces = function (msg) {
                    _this.ShowNotification("Info", msg, { glicon: "info-sign", nclass: "info" }, 3000);
                };
                this.request_msgHandlerFail = function (msg) {
                    _this.ShowNotification("Error", msg, { glicon: "ban-circle", nclass: "error" });
                };
                //------------------- Scope Init
                this.scope.NewMFilt = function () { return _this.turnMFilterCreate(); };
                this.scope.EditMFilt = function (mfilt) { return _this.turnMFilterEdit(mfilt); };
                this.scope.RemoveMfilt = function (mfilt) { return alert("NOT IMPLEMENTED =/"); };
                this.scope.CloseEditor = function () { return _this.closeMFiltEditor(); };
                this.scope.TypeToIcon = function (type) { return Controllers.glyphiconforSQLTYPE(type); };
                this.scope.InputType = function (type) { return Controllers.inputTypeForSQLType(type); };
                this.scope.SetCurBaseTable = function (t) { return _this.setCurBaseTable(t); };
                this.scope.ShowTableChoose = function () { return _this.va.showTableChoose = true; };
                this.scope.SetCurTableToCurMFilt = function () {
                    _this.va.curmetafilter.BaseTableId = _this.va.curbasetable.Id;
                    var table = Controllers.FindById(_this.va.basetables, _this.va.curmetafilter.BaseTableId);
                    _this.refetchPossibleKeysAndValidate(table);
                    _this.va.showTableChoose = false;
                };
                this.scope.SetCurTableFromCurMFilt = function () {
                    var table = Controllers.FindById(_this.va.basetables, _this.va.curmetafilter.BaseTableId);
                    _this.va.curbasetable = table === undefined ? null : table;
                    _this.refetchPossibleKeysAndValidate(table);
                    _this.va.showTableChoose = false;
                };
                this.scope.IsCurMFiltHasTable = function (table) {
                    return _this.va.curmetafilter.BaseTableId === table.Id;
                };
                this.scope.IsCurMFiltHasAnyTable = function () {
                    return _this.va.curmetafilter.BaseTableId != -1;
                };
                this.scope.MakeId = function (x, y) { return AngularApp.MakeHtmlID(x, y); };
                this.scope.NewFilterDrop = function (dragID, dropID, dragClass) {
                    if (dragClass != _this.va.keyDragClass)
                        return;
                    var keyname = AngularApp.ParseHtmlID(dragID);
                    var key = _this.va.possiblekeys.first(function (x) { return x.name === keyname; });
                    _this.newFilter(key);
                };
                this.scope.NewRecardDrop = function (dragID, dropID, dragClass) {
                    if (dragClass != _this.va.keyDragClass)
                        return;
                    var keyname = AngularApp.ParseHtmlID(dragID);
                    var key = _this.va.possiblekeys.first(function (x) { return x.name === keyname; });
                    _this.newRecCard(key);
                };
                this.scope.RecardDropKey = function (dragID, dropID, dragClass) {
                    if (dragClass != _this.va.keyDragClass)
                        return;
                    var keyname = AngularApp.ParseHtmlID(dragID);
                    var key = _this.va.possiblekeys.first(function (x) { return x.name === keyname; });
                    var args = AngularApp.ParseHtmlIDFull(dropID);
                    var cardId = parseInt(args[1]);
                    var card = _this.va.curmetafilter.reccards.first(function (x) { return x.Id === cardId; });
                    if (card === null)
                        return;
                    _this.RecCardAddKey(card, key, args[0]);
                    _this.validateReccard(card);
                };
                this.scope.NewWildcardDrop = function (dragID, dropID, dragClass) {
                    if (dragClass != _this.va.keyDragClass)
                        return;
                    var keyname = AngularApp.ParseHtmlID(dragID);
                    var key = _this.va.possiblekeys.first(function (x) { return x.name === keyname; });
                    _this.newWildCard(key);
                };
                this.scope.GetTypeOperators = function (type) { return _this.typeOperators.take(type); };
                this.scope.GetTypeOperatorsNames = function (type) { return _this.typeOperatorsSQL.take(type); };
                this.scope.ValidateItem = function (item, type) {
                    switch (type) {
                        case "wildcard":
                            _this.validateWildcard(item);
                            break;
                        case "reccard":
                            _this.validateReccard(item);
                            break;
                        case "filter":
                            _this.validateFilter(item);
                    }
                };
                this.scope.GetValidationInfo = function (item) { return _this.getValidationString(item, "this item"); };
                this.scope.RemoveItem = function (item) { return _this.RemoveItem(item); };
                this.scope.RestoreItem = function (item) { return _this.RestoreItem(item); };
                this.scope.DeleteFiltValue = function (filter, index) {
                    filter.ValsOps.splice(index, 1);
                };
                this.scope.AddFiltValue = function (filter) {
                    filter.ValsOps.push(new Controllers.ValOp());
                };
                this.scope.WildCardUpdateCode = function (wc) {
                    _this.updateWildCardCode(wc);
                    _this.va.curmetafilter.wildcards.forEach(function (x) { return _this.validateWildcard(x); });
                };
                this.scope.SaveCurMFilt = function () { return _this.saveOrUpdateCurMFilter(); };
                //------------------- Inner Init
                this.initUrlModuleFromRowObj(data.urls);
                this.refetchTables();
                this.refetchMfilters();
                this.fetchtoarr(true, { urlalias: "getreservedcards" }, this.va.reservedwildcards, false);
            };
            MFiltersController.prototype.validateWithKeys = function (item, validator) {
                for (var i = 0; i < this.va.possiblekeys.length; i++) {
                    if (validator(item, this.va.possiblekeys[i]))
                        return true;
                }
                return false;
            };
            return MFiltersController;
        }(AngularApp.Controller));
        Controllers.MFiltersController = MFiltersController;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=FiltersController.js.map