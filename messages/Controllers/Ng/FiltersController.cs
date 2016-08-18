using Business_Logic.MessagesContext;
using Business_Logic.SqlContext.DynamicQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ticonet.Controllers.Ng.ViewModels;
using ticonet.ParentControllers;

namespace ticonet.Controllers.Ng {

    public class FiltersController : NgController<FilterVM> {
        protected override NgResult _create(FilterVM[] models) {
            throw new NotImplementedException();
        }

        protected override NgResult _delete(FilterVM[] models) {
            throw new NotImplementedException();
        }

        protected override FetchResult<FilterVM> _fetch(int? Skip, int? Count, QueryFilter[] filters) {
            using (var l = new MessagesModuleLogic()) {
                int allQueryCount;
                var result = l.GetFiltered<tblFilter>(Skip, Count, filters, out allQueryCount)
                    .Select(x => VMConstructor.MakeFromObj(x, FilterVM.tblFilterPR));

                return FetchResult<FilterVM>.Succes(result, allQueryCount);
            }
        }

        protected override NgResult _update(FilterVM[] models) {
            throw new NotImplementedException();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetOperatorsForType(string typename) {
            if (string.IsNullOrWhiteSpace(typename))
                return NgResultToJsonResult(NgResult.Fail("Typename is undefined"));
            var items = DQOperator.GetAllowedForSqlType(typename);
            return NgResultToJsonResult(FetchResult<DQOperator>.Succes(items, items.Count()));
        }
    }
}