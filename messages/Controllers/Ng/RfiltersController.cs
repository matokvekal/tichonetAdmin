using Business_Logic.MessagesContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ticonet.Controllers.Ng.ViewModels;
using ticonet.ParentControllers;

namespace ticonet.Controllers.Ng {
    public class RfiltersController : NgController<RFilterVM> {
        protected override NgResult _create(RFilterVM[] models) {
            //TODO OPTIMIZE

            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    //Create RFilter
                    var filt = l.Create<tblRecepientFilter>();
                    filt.Name = model.Name;
                    filt.tblRecepientFilterTableNameId = model.BaseTableId;
                    l.Add(filt);
                    model.Id = filt.Id;
                    CreateAndUpdateRFilterParts(model, l);
                }
            }
            return NgResult.Succes();
        }


        protected override NgResult _update(RFilterVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var filt = l.Get<tblRecepientFilter>(model.Id);
                    filt.Name = model.Name;
                    filt.tblRecepientFilterTableNameId = model.BaseTableId;
                    l.SaveChanges(filt);
                    CreateAndUpdateRFilterParts(model, l);
                }
            }
            return NgResult.Succes();
        }

        protected void CreateAndUpdateRFilterParts (RFilterVM model, MessagesModuleLogic l) {
            //TODO OPTIMIZE

            //Add new parts
            if (model.newfilters != null) {
                foreach (var item in model.newfilters) {
                    var ent = l.Create<tblFilter>();
                    ent.Key = item.Key;
                    ent.Operator = item.Operator;
                    ent.Type = item.Type;
                    ent.Value = item.Value;
                    ent.tblRecepientFilterId = model.Id;
                    l.Add(ent);
                }
            }

            if (model.newwildcards != null) {
                foreach (var item in model.newwildcards) {
                    var ent = l.Create<tblWildcard>();
                    ent.Key = item.Key;
                    ent.Name = item.Name;
                    ent.Code = item.Code;
                    ent.tblRecepientFilterId = model.Id;
                    l.Add(ent);
                }
            }

            //Update existing parts (exlude new!, removed parts is excluded already)
            //TODO -1 check is unserious..... either do some optimization on client side, either here
            if (model.filters != null) {
                var excludeNewFilters = model.filters.Where(x => x.Id != -1);
                foreach (var item in excludeNewFilters) {
                    var ent = l.Get<tblFilter>(item.Id);
                    ent.Key = item.Key;
                    ent.Operator = item.Operator;
                    ent.Type = item.Type;
                    ent.Value = item.Value;
                    ent.tblRecepientFilterId = model.Id;
                    l.SaveChanges(ent);
                }
            }

            if (model.wildcards != null) {
                var excludeNewWildcards = model.wildcards.Where(x => x.Id != -1);
                foreach (var item in excludeNewWildcards) {
                    var ent = l.Get<tblWildcard>(item.Id);
                    ent.Key = item.Key;
                    ent.Name = item.Name;
                    ent.Code = item.Code;
                    ent.tblRecepientFilterId = model.Id;
                    l.SaveChanges(ent);
                }
            }

            //Delete removed parts
            if (model.removedfilters != null) {
                foreach (var item in model.removedfilters)
                    l.Delete<tblFilter>(item.Id);
            }
            if (model.removedwildcards != null) {
                foreach (var item in model.removedwildcards)
                    l.Delete<tblWildcard>(item.Id);
            }
        }

        protected override NgResult _delete(RFilterVM[] models) {
            throw new NotImplementedException();
        }

        protected override FetchResult<RFilterVM> _fetch(int? Skip, int? Count, QueryFilter[] filters) {
            using (var l = new MessagesModuleLogic()) {
                var items = l.GetAll<tblRecepientFilter>()
                    .Select(x => VMConstructor.MakeFromObj(x,RFilterVM.tblRecepientFilterBND));
                return FetchResult<RFilterVM>.Succes(items,items.Count());
            }
        }

    }
}