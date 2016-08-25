using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesModule;
using ticonet.ParentControllers;

namespace ticonet.Controllers {
    public class RecepientCardsController : NgController<RecepientcardVM> {
        protected override NgResult _create(RecepientcardVM[] models) {
            throw new NotImplementedException();
        }

        protected override NgResult _delete(RecepientcardVM[] models) {
            throw new NotImplementedException();
        }

        protected override FetchResult<RecepientcardVM> _fetch(int? Skip, int? Count, QueryFilter[] filters) {
            using (var l = new MessagesModuleLogic()) {
                int fullQueryCount;
                var queryResult = l.GetFiltered<tblRecepientCard>(Skip, Count, filters, out fullQueryCount)
                    .Select(x => PocoConstructor.MakeFromObj(x, RecepientcardVM.tblRecepientCardPR));
                return FetchResult<RecepientcardVM>.Succes(queryResult, fullQueryCount);
            }
        }

        protected override NgResult _update(RecepientcardVM[] models) {
            throw new NotImplementedException();
        }

        //TODO make a parametr to this action: should it send only recepients_reserved, or program reserved, or all
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetReservedCodes() {
            var items = new List<WildcardVM>() {
                new WildcardVM {Code = tblRecepientCard.NameCode, Name = "Recepient Name", Id=-10 },
                new WildcardVM {Code = tblRecepientCard.EmailCode, Name = "Recepient Email",Id=-11 },
                new WildcardVM {Code = tblRecepientCard.PhoneCode, Name = "Recepient Phone",Id=-12 }
            };
            return NgResultToJsonResult(FetchResult<WildcardVM>.Succes(items, items.Count));
        }
    }
}