using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesContext;
using ticonet.ParentControllers;

namespace ticonet.Controllers.Ng
{
    public class WildcardsController : NgController<WildcardVM> {
        protected override NgResult _create(WildcardVM[] models) {
            throw new NotImplementedException();
        }

        protected override NgResult _delete(WildcardVM[] models) {
            throw new NotImplementedException();
        }

        protected override FetchResult<WildcardVM> _fetch(int? Skip, int? Count, QueryFilter[] filters) {
            using (var l = new MessagesModuleLogic()) {
                int fullQueryCount;
                var queryResult = l.GetFiltered<tblWildcard>(Skip, Count, filters, out fullQueryCount)
                    .Select(x => PocoConstructor.MakeFromObj(x, WildcardVM.tblWildcardPR));
                return FetchResult<WildcardVM>.Succes(queryResult, fullQueryCount);
            }
        }

        protected override NgResult _update(WildcardVM[] models) {
            throw new NotImplementedException();
        }
    }
}