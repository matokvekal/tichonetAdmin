using System;
using System.Linq;
using ticonet.ParentControllers;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesContext;
using System.Web.Mvc;
using Ninject;
using Business_Logic.SqlContext;
using System.Text;
using Business_Logic.SqlContext.DynamicQuery;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ticonet.Controllers.Ng{

    public class TemplatesController : NgController<TemplateVM> {

        protected override NgResult _create(TemplateVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    l.Save(new tblTemplate {
                        Name = model.Name,
                        IsSms = model.IsSms,
                        MsgHeader = model.MsgHeader,
                        MsgBody = model.MsgBody,
                        tblRecepientFilterId = model.RecepientFilterId,
                        FilterValueContainersJSON = JsonConvert.SerializeObject(model.FilterValueContainers)
                }, ItemSaveBehaviour.AddOnly);
                }
            }
            return NgResult.Succes(models.Count() + " new templates was added");
        }

        protected override NgResult _delete(TemplateVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    l.Delete<tblTemplate>(model.Id);
                }
            }
            return NgResult.Succes(models.Count() + " templates was removed");
        }

        protected override FetchResult<TemplateVM> _fetch(int? Skip, int? Count, QueryFilter[] filters) {
            using (var l = new MessagesModuleLogic()) {
                var queryResult = l.GetAll<tblTemplate>()
                    .Select(x => VMConstructor.MakeFromObj(x, TemplateVM.tblTemplatePR));
                return FetchResult<TemplateVM>.Succes(queryResult,queryResult.Count());
            }
        }

        protected override NgResult _update(TemplateVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Get<tblTemplate>(model.Id);
                    item.Id = model.Id;
                    item.Name = model.Name;
                    item.IsSms = model.IsSms;
                    item.MsgHeader = model.MsgHeader;
                    item.MsgBody = model.MsgBody;
                    item.tblRecepientFilterId = model.RecepientFilterId;
                    item.FilterValueContainersJSON = JsonConvert.SerializeObject(model.FilterValueContainers);
                    l.SaveChanges(item);
                }
            }
            return NgResult.Succes(models.Count() + " templates was modified");
        }

        //TODO REMOVE TEST
        ISqlLogic sqlLogic;

        [Inject]
        public TemplatesController (ISqlLogic logic) {
            sqlLogic = logic;
        }

        public JsonResult MockMessage (int templateId) {
            //GET NEEDED DATA
            tblTemplate templ;
            tblFilter[] filters;
            tblWildcard[] wildcards;
            string table;
            string msg_h;
            string msg_b;
            using (var l = new MessagesModuleLogic()) {
                templ = l.Get<tblTemplate>(templateId);
                if (templ == null)
                    return NgResultToJsonResult(NgResult.Fail("Save Template First"));
                msg_h = templ.MsgHeader;
                msg_b = templ.MsgBody;
                filters = templ.tblRecepientFilter.tblFilters.ToArray();
                wildcards = templ.tblRecepientFilter.tblWildcards.ToArray();
                table = templ.tblRecepientFilter.tblRecepientFilterTableName.ReferncedTableName;
            }
            //BUILD CONDITION
            StringBuilder cond = new StringBuilder();
            cond.Append("WHERE ");
            foreach(var f in filters) {
                //TODO HERE A PLACE TO ATTACK!
                cond.Append(f.Key);
                cond.Append(" ");
                DQOperator op = new DQOperator(f.Operator);
                cond.Append(op.SQLString);
                cond.Append(" ");
                if (f.Type == "nvarchar") {
                    cond.Append("'");
                    //TODO HERE A PLACE TO ATTACK!
                    cond.Append(f.Value);
                    cond.Append("'");
                }
                else
                    //TODO HERE A PLACE TO ATTACK!
                    cond.Append(f.Value);

                cond.Append(" AND ");
            }
            if (filters.Length > 0)
                cond.Remove(cond.Length - 5, 5);


            //BUILD FIELDS
            var colomns = wildcards.Select(x => x.Key);

            //FETCH SQL DATA
            var data = sqlLogic.FetchData(colomns,table,"dbo",cond.ToString());

            //FILL FIELDS
            var messages = new List<Dictionary<string, string>>();
            foreach (var kv in data) {
                var dict = new Dictionary<string, string>();
                var header = msg_h;
                var body = msg_b;
                foreach (var card in wildcards) {
                    header = card.Apply(header, kv);
                    body = card.Apply(body, kv);
                }
                dict.Add("header", header);
                dict.Add("body", body);
                messages.Add(dict);
            }
            //
            return NgResultToJsonResult(FetchResult<Dictionary<string, string>>.Succes(messages,messages.Count));
        }
    }
}