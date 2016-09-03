using Business_Logic.MessagesModule;
using Newtonsoft.Json;
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
                    model.Id = filt.Id;
                    CreateAndUpdateRFilterParts(model, l);
                }
            }
            return NgResult.Succes();
        }

        protected void ManagerRFilterPart<TEnitity,TNgViewModel> (MessagesModuleLogic l, IEnumerable<TNgViewModel> items, Action<TEnitity,TNgViewModel> updater) 
            where TEnitity: class,IMessagesModuleEntity
            where TNgViewModel: class,INgViewModel
        {
            if (items == null) return;
            //TODO OPTIMIZE work with DB
            foreach (var item in items) {
                TEnitity ent = null;
                if (item.ng_ToDelete)
                    l.Delete<TEnitity>(item.Id);
                else if (item.ng_JustCreated)
                    ent = l.Create<TEnitity>();
                else
                    ent = l.Get<TEnitity>(item.Id);
                if (ent != null) {
                    updater(ent, item);
                    if (item.ng_JustCreated)
                        l.Add(ent);
                    else
                        l.SaveChanges(ent);
                }
            }
        }

        protected void CreateAndUpdateRFilterParts (RFilterVM model, MessagesModuleLogic l) {
            //!!!! IMPLEMENT POCO_AUTOBINDING MECHANISM IF YOU GOING TO DO SAME THINGS IN OTHER PLACE
            ManagerRFilterPart<tblFilter, FilterVM>(l, model.filters, (e, m) => {
                e.Key = m.Key;
                e.Name = m.Name;
                e.autoUpdatedList = m.autoUpdatedList;
                if (!m.autoUpdatedList){
                    string[] ops = new string[m.ValsOps.Length];
                    string[] vals = new string[m.ValsOps.Length];
                    for (int i = 0; i < m.ValsOps.Length; i++) {
                        ops[i] = m.ValsOps[i].Operator;
                        vals[i] = m.ValsOps[i].Value == null ? "": m.ValsOps[i].Value.ToString();
                    }
                    e.OperatorsJSON = JsonConvert.SerializeObject(ops);
                    e.ValuesJSON = JsonConvert.SerializeObject(vals);
                }
                else {
                    e.OperatorsJSON = "[]";
                    e.ValuesJSON = "[]";
                }
                e.allowMultipleSelection = m.allowMultipleSelection;
                e.allowUserInput = m.allowUserInput;
                e.Type = m.Type;
                e.tblRecepientFilterId = model.Id;
            });

            //!!!! IMPLEMENT POCO_AUTOBINDING MECHANISM IF YOU GOING TO DO SAME THINGS IN OTHER PLACE
            ManagerRFilterPart<tblWildcard, WildcardVM>(l, model.wildcards, (e, m) => {
                e.Name = m.Name;
                e.Key = m.Key;
                e.Code = m.Code;
                e.tblRecepientFilterId = model.Id;
            });

            //!!!! IMPLEMENT POCO_AUTOBINDING MECHANISM IF YOU GOING TO DO SAME THINGS IN OTHER PLACE
            ManagerRFilterPart<tblRecepientCard, RecepientcardVM>(l, model.reccards, (e, m) => {
                e.Name = m.Name;
                e.NameKey = m.NameKey;
                e.PhoneKey = m.PhoneKey;
                e.EmailKey = m.EmailKey;
                e.tblRecepientFilterId = model.Id;
            });

        }

        protected override NgResult _delete(RFilterVM[] models) {
            throw new NotImplementedException();
        }

        protected override FetchResult<RFilterVM> _fetch(int? Skip, int? Count, NgControllerInstruct[] filters) {
            using (var l = new MessagesModuleLogic()) {
                var items = l.GetAll<tblRecepientFilter>()
                    .Select(x => PocoConstructor.MakeFromObj(x,RFilterVM.tblRecepientFilterPR));
                return FetchResult<RFilterVM>.Succes(items,items.Count());
            }
        }

    }
}