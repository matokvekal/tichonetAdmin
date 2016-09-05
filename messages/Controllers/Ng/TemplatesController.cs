using System;
using System.Linq;
using ticonet.ParentControllers;
using ticonet.Controllers.Ng.ViewModels;
using Business_Logic.MessagesModule;
using System.Web.Mvc;
using Ninject;
using Business_Logic.SqlContext;

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

        public JsonResult MockMessage (int templateId) {
            throw new NotImplementedException();
        }
    }
}