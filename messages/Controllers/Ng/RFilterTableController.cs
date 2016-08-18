using System;
using System.Linq;
using System.Web.Mvc;
using ticonet.ParentControllers;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesContext;
using Business_Logic.SqlContext;
using System.Collections.Generic;
using Ninject;

namespace ticonet.Controllers.Ng {


    public class RFilterTableController : NgController<RFilterTableVM> {

        protected readonly ISqlLogic sqlLogic;

        [Inject]
        public RFilterTableController (ISqlLogic sqlLogic) {
            this.sqlLogic = sqlLogic;
        }

        protected override NgResult _create(RFilterTableVM[] models) {
            throw new NotImplementedException();
        }

        protected override NgResult _delete(RFilterTableVM[] models) {
            throw new NotImplementedException();
        }

        protected override FetchResult<RFilterTableVM> _fetch(int? Skip, int? Count, QueryFilter[] filters) {
            using (var l = new MessagesModuleLogic()) {
                var items = l.GetAll<tblRecepientFilterTableName>().Select(x =>
                    VMConstructor.MakeFromObj(x, RFilterTableVM.tblRecepientFilterTableNamePR)
                    );
                if (items.Count() == 0)
                    return FetchResult<RFilterTableVM>.Fail("There is no base tables in database");
                return FetchResult<RFilterTableVM>.Succes(items, items.Count());
            }
        }

        protected override NgResult _update(RFilterTableVM[] models) {
            throw new NotImplementedException();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetColumns (int id) {
            string tablename;
            using (var l = new MessagesModuleLogic()) {
                var item = l.Get<tblRecepientFilterTableName>(id);
                if (item == null)
                    return NgResultToJsonResult( NgResult.Fail("There is no binded table in database with id:" + id) );
                tablename = item.ReferncedTableName;
            }
            var data = sqlLogic.GetColomnsInfos(tablename);

            return NgResultToJsonResult(FetchResult<IDictionary<string, string>>.Succes(data, data.Count));
        }
    }

}
