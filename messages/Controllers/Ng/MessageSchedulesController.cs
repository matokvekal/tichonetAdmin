using Business_Logic.MessagesModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ticonet.Controllers.Ng.ViewModels;
using ticonet.ParentControllers;

namespace ticonet.Controllers.Ng {

    [Authorize]
    public class MessageSchedulesController : NgController<MessageScheduleVM> {

        protected override NgResult _create(MessageScheduleVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Create<tblMessageSchedule>();
                    MessageScheduleVM.ReflectToTblMessageSchedule.Run(model, item);
                    l.Add(item);
                }
            }
            return NgResult.Succes(models.Count() + " new schedules was added");
        }

        protected override NgResult _delete(MessageScheduleVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    l.Delete<tblMessageSchedule>(model.Id);
                }
            }
            return NgResult.Succes(models.Count() + " schedules was removed");
        }

        protected override FetchResult<MessageScheduleVM> _fetch(int? Skip, int? Count, NgControllerInstruct[] filters) {
            using (var l = new MessagesModuleLogic()) {
                IQueryable<tblMessageSchedule> baseQuery = null;

                //Special instructions filtration. 
                //this SHOULD be incapsulated sometime.
                //Special instructions is query filters that not relies on main enitity POCO parametrs
                //such as tblMessageSchedule in this case, and we need special logick to handle it, 
                var specialInstructions = filters == null ? null : filters.Where(x => x.isSpecial);
                NgControllerInstruct filter;
                if (specialInstructions !=null 
                    && (filter = specialInstructions.FirstOrDefault(x => x.key == "TemplateName"))!=null) {
                    baseQuery = l.GetFilteredQueryable<tblTemplate>
                        (new[] {
                             new NgControllerInstruct {key = "Name", op = filter.op, val = filter.val }
                        }).SelectMany(x => x.tblMessageSchedules);
                }

                int count;
                var queryResult = l.GetFiltered(Skip,Count,filters, out count, baseQuery)
                    .Select(x => PocoConstructor.MakeFromObj(x, MessageScheduleVM.tblMessageSchedulePR));
                return FetchResult<MessageScheduleVM>.Succes(queryResult, count);
            }
        }

        protected override NgResult _update(MessageScheduleVM[] models) {
            using (var l = new MessagesModuleLogic()) {
                foreach (var model in models) {
                    var item = l.Get<tblMessageSchedule>(model.Id);
                    MessageScheduleVM.ReflectToTblMessageSchedule.Run(model, item);
                    l.SaveChanges(item);
                }
            }
            return NgResult.Succes(models.Count() + " schedules was modified");
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetRepeatModes() {
            var items = tblMessageScheduleHelper.GetAllowedRepeatModeNames();
            return NgResultToJsonResult(FetchResult<string>.Succes(items, items.Length));
        }
    }

}