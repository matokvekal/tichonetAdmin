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
                this.metafilters = [];
                this.curmetafilter = null;
                this.curmetafilterBaseTableName = function () {
                    var t = FindById(_this.basetables, _this.curmetafilter.BaseTableId);
                    return t === undefined ? "" : t.Name;
                };
                this.wildcardCreator_ID = '__wildcardCreator';
                this.filterCreator_ID = '__filterCreator';
                this.keyIdPrefix = '_tablekey';
                this.keyDragClass = 'tablekey';
            }
            return MFiltersVA;
        }());
        var BaseTableVM = (function () {
            function BaseTableVM() {
            }
            return BaseTableVM;
        }());
        Controllers.BaseTableVM = BaseTableVM;
        var MetaFilterVM = (function () {
            function MetaFilterVM() {
                this.filters = [];
                this.wildcards = [];
                this.Name = "New Filter";
                this.BaseTableId = -1;
                //this arrays used to check changes on server, after fetch there are empty
                this.newfilters = [];
                this.newwildcards = [];
                this.removedfilters = [];
                this.removedwildcards = [];
                //local
                //not sended ?
                //TODO
                //validate filter enitities, Invalid Filter can't be saved
                this.Invalid = false;
            }
            return MetaFilterVM;
        }());
        Controllers.MetaFilterVM = MetaFilterVM;
        var KeyVM = (function () {
            function KeyVM() {
            }
            return KeyVM;
        }());
        Controllers.KeyVM = KeyVM;
        var FilterVM = (function () {
            function FilterVM() {
                //local
                this.Invalid = false;
            }
            return FilterVM;
        }());
        Controllers.FilterVM = FilterVM;
        var WildcardVM = (function () {
            function WildcardVM() {
                this.Invalid = false;
            }
            return WildcardVM;
        }());
        Controllers.WildcardVM = WildcardVM;
        var FiltOperator = (function () {
            function FiltOperator() {
            }
            return FiltOperator;
        }());
        Controllers.FiltOperator = FiltOperator;
        function FindById(arr, Id) {
            return arr.first(function (x) { return x.Id === Id; });
        }
        Controllers.FindById = FindById;
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
                        _this.refetchPossibleKeys(table);
                    else
                        _this.clearPossibleKeys();
                };
                this.validateEntities = function () {
                    _this.va.curmetafilter.wildcards.forEach(function (wc) {
                        return wc.Invalid = !_this.validateWithKeys(wc, function (item, key) { return key.name === item.Key; });
                    });
                    _this.va.curmetafilter.filters.forEach(function (wc) {
                        return wc.Invalid = !_this.validateWithKeys(wc, function (item, key) { return key.name === item.Key && key.type === item.Type; });
                    });
                };
                this.refetchPossibleKeys = function (table) {
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
                    _this.va.curmetafilter = new MetaFilterVM();
                    _this.va.curmetafilter.Id = -1;
                    _this.va.showTableChoose = true;
                };
                this.turnMFilterEdit = function (mfilt) {
                    _this.va.showTableChoose = false;
                    var table = FindById(_this.va.basetables, mfilt.BaseTableId);
                    _this.setCurBaseTable(table);
                    //TODO clone DEEP ?
                    _this.va.curmetafilter = AngularApp.CloneShallow(mfilt);
                    _this.va.curmetafilter.filters = [];
                    _this.va.curmetafilter.wildcards = [];
                    _this.fetchtoarr(true, {
                        urlalias: "getfilters",
                        //todo this is a very bad : tblRecepientFilterId, avoid this, use convention approach at least
                        params: new AngularApp.FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                        onSucces: function () { return _this.refetchPossibleKeys(table); }
                    }, _this.va.curmetafilter.filters, true);
                    _this.fetchtoarr(true, {
                        urlalias: "getwildcards",
                        params: new AngularApp.FetchParams().addFilt("tblRecepientFilterId", mfilt.Id),
                        onSucces: function () { return _this.refetchPossibleKeys(table); }
                    }, _this.va.curmetafilter.wildcards, true);
                };
                this.closeMFiltEditor = function () {
                    _this.va.curmetafilter = null;
                };
                this.saveOrUpdateCurMFilter = function () {
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
                //Wildcards
                this.newWildCard = function (key) {
                    var wc = new WildcardVM();
                    wc.Id = -1;
                    wc.Name = key.name;
                    wc.Key = key.name;
                    wc.Code = "{CODE}";
                    wc.RecepientFilterId = _this.va.curmetafilter.Id;
                    if (AngularApp.IsNullOrUndefined(_this.va.curmetafilter.newwildcards))
                        _this.va.curmetafilter.newwildcards = [];
                    _this.va.curmetafilter.newwildcards.push(wc);
                    _this.va.curmetafilter.wildcards.push(wc);
                };
                this.deleteWildCard = function (wc) {
                    if (wc.Id != -1)
                        _this.va.curmetafilter.removedwildcards.push(wc);
                    else
                        _this.va.curmetafilter.newwildcards.remove(wc);
                    _this.va.curmetafilter.wildcards.remove(wc);
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
                    if (!_this.typeOperators.cont(key.type))
                        _this.fetchTypeOperators(key.type);
                    var filt = {
                        Id: -1,
                        Key: key.name,
                        Operator: "=",
                        RecepientFilterId: _this.va.curmetafilter.Id,
                        Value: "",
                        Type: key.type,
                        Invalid: false
                    };
                    if (AngularApp.IsNullOrUndefined(_this.va.curmetafilter.newfilters))
                        _this.va.curmetafilter.newfilters = [];
                    _this.va.curmetafilter.newfilters.push(filt);
                    _this.va.curmetafilter.filters.push(filt);
                };
                this.deleteFilter = function (filt) {
                    if (filt.Id != -1)
                        _this.va.curmetafilter.removedfilters.push(filt);
                    else
                        _this.va.curmetafilter.newfilters.remove(filt);
                    _this.va.curmetafilter.filters.remove(filt);
                };
                //Styling, others
                this.glyphiconforSQLTYPE = function (typeName) {
                    switch (typeName) {
                        case "int":
                            return "glyphicon glyphicon-certificate";
                        case "nvarchar":
                            return "glyphicon glyphicon-font";
                        case "datetime":
                        case "date":
                            return "glyphicon glyphicon-calendar";
                        case "time":
                            return "glyphicon glyphicon-time";
                        case "bit":
                            return "glyphicon glyphicon-check";
                    }
                    return "glyphicon glyphicon-briefcase";
                };
                this.inputTypeForSQLType = function (typeName) {
                    switch (typeName) {
                        case "int":
                            return "number";
                        case "nvarchar":
                            return "text";
                        case "datetime":
                        case "date":
                            return "datetime-local";
                        case "time":
                            return "time";
                        case "bit":
                            return "checkbox";
                    }
                    return "text";
                };
            }
            MFiltersController.prototype.buildVa = function () { return new MFiltersVA; };
            MFiltersController.prototype.init = function (data) {
                //------------------- Scope Init
                var _this = this;
                this.scope.NewMFilt = function () { return _this.turnMFilterCreate(); };
                this.scope.EditMFilt = function (mfilt) { return _this.turnMFilterEdit(mfilt); };
                this.scope.RemoveMfilt = function (mfilt) { return alert("NOT IMPLEMENTED =/"); };
                this.scope.CloseEditor = function () { return _this.closeMFiltEditor(); };
                this.scope.TypeToIcon = function (type) { return _this.glyphiconforSQLTYPE(type); };
                this.scope.InputType = function (type) { return _this.inputTypeForSQLType(type); };
                this.scope.SetCurBaseTable = function (t) { return _this.setCurBaseTable(t); };
                this.scope.ShowTableChoose = function () { return _this.va.showTableChoose = true; };
                this.scope.SetCurTableToCurMFilt = function () {
                    _this.va.curmetafilter.BaseTableId = _this.va.curbasetable.Id;
                    var table = FindById(_this.va.basetables, _this.va.curmetafilter.BaseTableId);
                    _this.refetchPossibleKeys(table);
                    _this.va.showTableChoose = false;
                };
                this.scope.SetCurTableFromCurMFilt = function () {
                    var table = FindById(_this.va.basetables, _this.va.curmetafilter.BaseTableId);
                    _this.va.curbasetable = table === undefined ? null : table;
                    _this.refetchPossibleKeys(table);
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
                this.scope.NewWildcardDrop = function (dragID, dropID, dragClass) {
                    if (dragClass != _this.va.keyDragClass)
                        return;
                    var keyname = AngularApp.ParseHtmlID(dragID);
                    var key = _this.va.possiblekeys.first(function (x) { return x.name === keyname; });
                    _this.newWildCard(key);
                };
                this.scope.GetTypeOperators = function (type) { return _this.typeOperators.take(type); };
                this.scope.GetTypeOperatorsNames = function (type) { return _this.typeOperatorsSQL.take(type); };
                this.scope.RemoveFilter = function (filter) { return _this.deleteFilter(filter); };
                this.scope.RemoveWildcard = function (wc) { return _this.deleteWildCard(wc); };
                this.scope.WildCardUpdateCode = function (wc) { return _this.updateWildCardCode(wc); };
                this.scope.SaveCurMFilt = function () { return _this.saveOrUpdateCurMFilter(); };
                //------------------- Inner Init
                this.initUrlModuleFromRowObj(data.urls);
                this.refetchTables();
                this.refetchMfilters();
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