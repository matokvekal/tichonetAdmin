using System;
using System.Linq;
using ticonet.ParentControllers;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesModule;
using System.Web.Mvc;
using Ninject;
using Business_Logic.SqlContext;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Business_Logic.MessagesModule.InnerLibs.Text2Graph;

namespace ticonet.Controllers.Ng{

    [Authorize]
    public class TemplatesController : NgController<TemplateVM> {

        protected override NgResult _create(TemplateVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Create<tblTemplate>();
                    TemplateVM.ReflectToTblTemplate.Run(model, item);
                    l.Add(item);
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
                    .Select(x => PocoConstructor.MakeFromObj(x, TemplateVM.tblTemplatePR));
                return FetchResult<TemplateVM>.Succes(queryResult,queryResult.Count());
            }
        }

        protected override NgResult _update(TemplateVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Get<tblTemplate>(model.Id);
                    TemplateVM.ReflectToTblTemplate.Run(model, item);
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
                    //if user-inputed
                    if (filt.allowUserInput.HasValue && filt.allowUserInput.Value) {
                        //if user selected value from existing values from DB
                        if (filt.autoUpdatedList.HasValue && filt.autoUpdatedList.Value) {
                            var tableName = templ.tblRecepientFilter.tblRecepientFilterTableName.ReferncedTableName;
                            var type = sqlLogic.GetColomnType(tableName, filt.Key);
                            var valops = sqlLogic.FetchDataDistinct(new[] { filt.Key }, tableName)
                                .Select(x => new ValueOperatorPair(x[filt.Key].ToString(), "=", type))
                                //HERE WE USE STRING CHECK
                                .Where(x => userFilterValues.Any(y => y.FilterId == filt.Id && y.Values != null && y.Values.Any(z => ToStringSafe(z) == x.Value.ToString())))
                                .ToArray();
                            filtsToValops.Add(filt.Id, valops);
                        }
                        //if strongly-enter by user
                        else {
                            //TODO
                            //here's no situation when we have a multi-selected values
                            var ops = JsonConvert.DeserializeObject<string[]>(filt.OperatorsJSON);
                            var valop = userFilterValues.First(x => x.FilterId == filt.Id);
                            filtsToValops.Add(filt.Id, new[]
                                { new ValueOperatorPair (valop.Values == null? null : valop.Values[0].ToString(),ops[0],filt.Type) }
                            );
                        }
                    }
                    //if no user-inputed
                    else {
                        var vals = JsonConvert.DeserializeObject<string[]>(filt.ValuesJSON);
                        var ops = JsonConvert.DeserializeObject<string[]>(filt.OperatorsJSON);
                        var valops = new ValueOperatorPair[vals.Length];
                        for (int i = 0; i < vals.Length; i++) {
                            valops[i] = new ValueOperatorPair(vals[i], ops[i], filt.Type);
                        }
                        filtsToValops.Add(filt.Id, valops);
                    }

                }
                wildcards = templ.tblRecepientFilter.tblWildcards.ToArray();

                int[] allowedReccardsIds = JsonConvert.DeserializeObject<int[]>(templ.ChoosenReccardIdsJSON);

                reccards = templ.tblRecepientFilter.tblRecepientCards.Where(x => allowedReccardsIds.Any(y => y == x.Id)).ToArray();
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
            var sqlData = sqlLogic.FetchData(colomns, table, "dbo", Condition);

            //Create Message Producer

            var MPwildcards = wildcards.SelectMany(x => x.ToKeyValues());
        
            var specs = new DefaultMarkUpSpecification { NewLineSymbol = "\n" };
            var MP = new MessageProducer(templ.MsgHeader,templ.MsgBody,null,specs);

            List<Message> messages = new List<Message>();

            //Produce
            foreach (var rc in reccards) {
                var data = sqlData.Where(x => !String.IsNullOrWhiteSpace(x[rc.EmailKey].ToString())).GroupBy(x => x[rc.EmailKey].ToString());
                var cards = MPwildcards.Concat(rc.ToKeyValues());
                MP.ChangeWildCards(cards);
                foreach (var d in data)
                    messages.Add(MP.Produce(d, MessageType.Email));
            }

            //Send to client

            return NgResultToJsonResult(FetchResult<Message>.Succes(messages,messages.Count));
        }
    }
}