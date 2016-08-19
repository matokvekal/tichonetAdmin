using System;
using System.Linq;
using ticonet.ParentControllers;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesContext;
using System.Web.Mvc;
using Ninject;
using Business_Logic.SqlContext;
using System.Text;
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

        string ToStringSafe(object obj){
            if (obj == null)
                return "";
            return obj.ToString();
        }

        public JsonResult MockMessage (int templateId) {
            tblTemplate templ;
            tblFilter[] filters;
            var filtsToValops = new Dictionary<int, ValueOperatorPair[]>();

            tblWildcard[] wildcards;
            tblRecepientCard[] reccards;

            FilterValueContainer[] userFilterValues;

            string table;
            string msg_h;
            string msg_b;

            //GET NEEDED DATA
            using (var l = new MessagesModuleLogic()) {
                templ = l.Get<tblTemplate>(templateId);
                if (templ == null)
                    return NgResultToJsonResult(NgResult.Fail("Save Template First"));
                userFilterValues = JsonConvert.DeserializeObject<FilterValueContainer[]>(templ.FilterValueContainersJSON);
                msg_h = templ.MsgHeader ?? string.Empty;
                msg_b = templ.MsgBody ?? string.Empty;
                filters = templ.tblRecepientFilter.tblFilters.ToArray();

                foreach (var filt in filters){
                    if (filt.autoUpdatedList.HasValue && filt.autoUpdatedList.Value) {
                        //if user-inputed
                        var tableName = templ.tblRecepientFilter.tblRecepientFilterTableName.ReferncedTableName;
                        var type = sqlLogic.GetColomnType(tableName, filt.Key);
                        var valops = sqlLogic.FetchDataDistinct(new[] { filt.Key }, tableName)
                            .Select(x => new ValueOperatorPair(x[filt.Key].ToString(), "=", type))
                            //HERE WE USE STRING CHECK
                            .Where(x => userFilterValues.Any(y => y.FilterId==filt.Id && y.Value !=null && y.Value.Any( z => ToStringSafe(z) == x.Value.ToString() )))
                            .ToArray();
                        filtsToValops.Add(filt.Id, valops);
                    }
                    else {
                        //if no user-inputed
                        var vals = JsonConvert.DeserializeObject<string[]>(filt.Value);
                        var ops = JsonConvert.DeserializeObject<string[]>(filt.Operator);
                        var valops = new ValueOperatorPair[vals.Length];
                        for (int i = 0; i < vals.Length; i++) {
                            valops[i] = new ValueOperatorPair(vals[i], ops[i], filt.Type);
                        }
                        filtsToValops.Add(filt.Id, valops);
                    }

                }
                wildcards = templ.tblRecepientFilter.tblWildcards.ToArray();
                reccards = templ.tblRecepientFilter.tblRecepientCards.ToArray();
                table = templ.tblRecepientFilter.tblRecepientFilterTableName.ReferncedTableName;
            }
            //BUILD CONDITION
            var Condition = SqlPredicate.BuildAndNode();
            if (filters.Length > 0) {
                foreach (var f in filters) {
                    var orNode = SqlPredicate.BuildOrNode();
                    var valops = filtsToValops[f.Id];
                    foreach(var valop in valops) {
                        orNode.Append(SqlPredicate.BuildEndNode(f.Key, valop.Operator, valop.Value, f.Type));
                    }
                    Condition.Append(orNode);
                }
            }

            //BUILD FIELDS
            var colomns = wildcards.Select(x => x.Key)
                .Concat(reccards.Select(x => x.EmailKey))
                .Concat(reccards.Select(x => x.NameKey))
                .Concat(reccards.Select(x => x.PhoneKey)).Distinct();

            //FETCH SQL DATA
            var data = sqlLogic.FetchData(colomns,table,"dbo", Condition);

            //FILL FIELDS

            var messages = new List<Dictionary<string, string>>();
            foreach (var kv in data) {
                var header = msg_h;
                var body = msg_b;

                foreach (var card in wildcards) {
                    //if(!string.IsNullOrWhiteSpace(header))
                        header = card.Apply(header, kv);
                    //if (!string.IsNullOrWhiteSpace(body))
                        body = card.Apply(body, kv);
                }

                //applying recepients
                foreach (var reccard in reccards) {
                    var dict = new Dictionary<string, string>();
                    var nheader = reccard.Apply(header, kv);
                    var nbody = reccard.Apply(body, kv);

                    dict.Add("header", nheader);
                    dict.Add("body", nbody);
                    messages.Add(dict);
                }
            }
            //
            return NgResultToJsonResult(FetchResult<Dictionary<string, string>>.Succes(messages,messages.Count));
        }
    }
}