using Business_Logic.MessagesContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ticonet.Controllers.Ng.ViewModels;
using ticonet.ParentControllers;
using Ninject;
using Business_Logic.SqlContext;

namespace ticonet.Controllers.Ng {

    public class FiltersController : NgController<FilterVM> {
        ISqlLogic sqllogic;

        [Inject]
        public FiltersController(ISqlLogic logic) {
            sqllogic = logic;
        }

        protected override NgResult _create(FilterVM[] models) {
            throw new NotImplementedException();
        }

        protected override NgResult _delete(FilterVM[] models) {
            throw new NotImplementedException();
        }

        protected override FetchResult<FilterVM> _fetch(int? Skip, int? Count, QueryFilter[] filters) {
            IEnumerable<FilterVM> filts;
            IDictionary<int, string> filtsToTables = new Dictionary<int, string>();
            int allQueryCount;
            using (var l = new MessagesModuleLogic()) {
                var result = l.GetFiltered<tblFilter>(Skip, Count, filters, out allQueryCount);
                filts = result.Select(x => PocoConstructor.MakeFromObj(x, FilterVM.tblFilterPR)).ToArray();
                result.ForEach(x => filtsToTables.Add(x.Id, x.tblRecepientFilter.tblRecepientFilterTableName.ReferncedTableName));
            }

            //TODO REFACTOR
            
            foreach(var f in filts) {
                if (f.autoUpdatedList) {
                    var type = sqllogic.GetColomnType(filtsToTables[f.Id], f.Key);
                    var valops = sqllogic.FetchDataDistinct(new[] { f.Key }, filtsToTables[f.Id])
                        .Select(x => new ValueOperatorPair(x[f.Key].ToString(), "=", type)).ToArray();
                    f.ValsOps = valops;
                }
            }

            return FetchResult<FilterVM>.Succes(filts, allQueryCount);
        }

        protected override NgResult _update(FilterVM[] models) {
            throw new NotImplementedException();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetOperatorsForType(string typename) {
            if (string.IsNullOrWhiteSpace(typename))
                return NgResultToJsonResult(NgResult.Fail("Typename is undefined"));
            var items = SqlOperator.GetAllowedForSqlType(typename);
            return NgResultToJsonResult(FetchResult<SqlOperator>.Succes(items, items.Count()));
        }
    }
}