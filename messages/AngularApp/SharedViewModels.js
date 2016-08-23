var AngularApp;
(function (AngularApp) {
    var Controllers;
    (function (Controllers) {
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
                this.reccards = [];
                this.Name = "New Filter";
                this.BaseTableId = -1;
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
        var ValOp = (function () {
            function ValOp() {
                this.Value = "";
                this.Operator = "=";
            }
            return ValOp;
        }());
        Controllers.ValOp = ValOp;
        var FilterVM = (function () {
            function FilterVM() {
                //local
                this.Invalid = false;
            }
            return FilterVM;
        }());
        Controllers.FilterVM = FilterVM;
        var FilterValueContainer = (function () {
            function FilterValueContainer() {
                this.Value = [];
            }
            return FilterValueContainer;
        }());
        Controllers.FilterValueContainer = FilterValueContainer;
        var WildcardVM = (function () {
            function WildcardVM() {
                this.Invalid = false;
            }
            return WildcardVM;
        }());
        Controllers.WildcardVM = WildcardVM;
        var RecepientCardVM = (function () {
            function RecepientCardVM() {
                //local
                this.Invalid = false;
            }
            return RecepientCardVM;
        }());
        Controllers.RecepientCardVM = RecepientCardVM;
        var FiltOperator = (function () {
            function FiltOperator() {
            }
            return FiltOperator;
        }());
        Controllers.FiltOperator = FiltOperator;
        var TemplateVM = (function () {
            function TemplateVM() {
                this.RecepientFilterId = -1;
                this.Name = "New Template";
                this.FilterValueContainers = [];
                this.ChoosenReccards = [];
            }
            return TemplateVM;
        }());
        Controllers.TemplateVM = TemplateVM;
        function FindById(arr, Id) {
            return arr.first(function (x) { return x.Id === Id; });
        }
        Controllers.FindById = FindById;
    })(Controllers = AngularApp.Controllers || (AngularApp.Controllers = {}));
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=SharedViewModels.js.map