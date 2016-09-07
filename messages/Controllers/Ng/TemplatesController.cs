using System;
using System.Linq;
using ticonet.ParentControllers;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesModule;
using System.Web.Mvc;
using Ninject;
using Business_Logic.SqlContext;
using Business_Logic.MessagesModule.Mechanisms;
using Business_Logic.MessagesModule.DataObjects;

namespace ticonet.Controllers.Ng{

    [Authorize]
    public class TemplatesController : NgController<TemplateVM> {
        ISqlLogic sqllogic;

        [Inject]
        public TemplatesController(ISqlLogic logic) {
            sqllogic = logic;
        }

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

        protected override FetchResult<TemplateVM> _fetch(int? Skip, int? Count, NgControllerInstruct[] filters) {
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

        string ToStringSafe(object obj){
            if (obj == null)
                return "";
            return obj.ToString();
        }

        public JsonResult MockMessage (int templateId, int? MaxCount) {
            using (var l = new MessagesModuleLogic()) {
                var tmpl = l.Get<tblTemplate>(templateId);
                if (tmpl == null)
                    return NgResultToJsonResult(NgResult.Fail("Server Error: cannot find template, try save it and re-open."));

                var items = TASK_PROTOTYPE.GetDemoMessages(l,tmpl,sqllogic,tmpl.IsSms, MaxCount ?? 0);
                return NgResultToJsonResult(FetchResult<Message>.Succes(items, items.Count));
            }
        }
    }
}