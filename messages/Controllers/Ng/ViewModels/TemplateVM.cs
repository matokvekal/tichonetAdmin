using Business_Logic.MessagesModule;
using Newtonsoft.Json;

namespace ticonet.Controllers.Ng.ViewModels {

    public class TemplateVM : INgViewModel {
        public static POCOReflector<tblTemplate, TemplateVM> tblTemplatePR =
            POCOReflector<tblTemplate, TemplateVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Name = o.Name,
                (o, m) => m.IsSms = o.IsSms,
                (o, m) => m.MsgHeader = o.MsgHeader,
                (o, m) => m.MsgBody = o.MsgBody,
                (o, m) => {
                    if (o.FilterValueContainersJSON == null)
                        m.FilterValueContainers = new FilterValueContainer[] { new FilterValueContainer() };
                    else
                        m.FilterValueContainers = JsonConvert.DeserializeObject<FilterValueContainer[]>(o.FilterValueContainersJSON);
                },
                (o, m) => {
                    if (o.ChoosenReccardIdsJSON == null)
                        m.ChoosenReccards = new int[0];
                    else
                        m.ChoosenReccards = JsonConvert.DeserializeObject<int[]>(o.ChoosenReccardIdsJSON);
                }
            );

        public static POCOReflector<TemplateVM, tblTemplate> ReflectToTblTemplate =
            POCOReflector<TemplateVM, tblTemplate>.Create(
                (o, m) => m.tblRecepientFilterId = o.RecepientFilterId,
                (o, m) => m.Name = o.Name,
                (o, m) => m.IsSms = o.IsSms,
                (o, m) => m.MsgHeader = o.MsgHeader,
                (o, m) => m.MsgBody = o.MsgBody,
                (o, m) => m.FilterValueContainersJSON = JsonConvert.SerializeObject(o.FilterValueContainers),
                (o, m) => m.ChoosenReccardIdsJSON = JsonConvert.SerializeObject(o.ChoosenReccards)
            );

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }

        public string Name { get; set; }
        public bool IsSms { get; set; }

        public string MsgHeader { get; set; }
        public string MsgBody { get; set; }
        public FilterValueContainer[] FilterValueContainers { get; set; }
        public int[] ChoosenReccards { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }
}