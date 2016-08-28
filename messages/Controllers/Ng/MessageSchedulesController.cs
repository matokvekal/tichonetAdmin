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

        protected override FetchResult<MessageScheduleVM> _fetch(int? Skip, int? Count, QueryFilter[] filters) {
            using (var l = new MessagesModuleLogic()) {
                var queryResult = l.GetAll<tblMessageSchedule>()
                    .Select(x => PocoConstructor.MakeFromObj(x, MessageScheduleVM.tblMessageSchedulePR));
                return FetchResult<MessageScheduleVM>.Succes(queryResult, queryResult.Count());
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