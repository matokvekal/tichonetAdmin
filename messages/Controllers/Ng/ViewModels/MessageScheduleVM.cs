using Business_Logic.MessagesModule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ticonet.Controllers.Ng.ViewModels {

    public class MessageScheduleVM : INgViewModel {
        public static POCOReflector<tblMessageSchedule, MessageScheduleVM> tblMessageSchedulePR =
            POCOReflector<tblMessageSchedule, MessageScheduleVM>.Create(
                    (o, m) => m.Id = o.Id,
                    (o, m) => m.TemplateId = o.tblTemplateId,
                    (o, m) => m.Name = o.Name,

                    (o, m) => m.BatchesCount = o.tblMessageBatches.Count(),

                    (o, m) => m.ScheduleDate = o.ScheduleDate,
                    (o, m) => m.RepeatMode = o.RepeatMode,
                    (o, m) => m.IsActive = o.IsActive,
                    (o, m) => m.InArchive = o.InArchive.HasValue ? o.InArchive.Value : false,

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

        public static POCOReflector<MessageScheduleVM, tblMessageSchedule> ReflectToTblMessageSchedule =
            POCOReflector<MessageScheduleVM, tblMessageSchedule>.Create(
                    (o, m) => m.tblTemplateId = o.TemplateId,
                    (o, m) => m.Name = o.Name,

                    (o, m) => m.ScheduleDate = o.ScheduleDate,
                    (o, m) => m.RepeatMode = o.RepeatMode,
                    (o, m) => m.IsActive = o.IsActive,
                    (o, m) => m.InArchive = o.InArchive,

                    (o, m) => m.IsSms = o.IsSms,
                    (o, m) => m.MsgHeader = o.MsgHeader,
                    (o, m) => m.MsgBody = o.MsgBody,
                    (o, m) => m.FilterValueContainersJSON = JsonConvert.SerializeObject(o.FilterValueContainers),
                    (o, m) => m.ChoosenReccardIdsJSON = JsonConvert.SerializeObject(o.ChoosenReccards)
                );

        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string Name { get; set; }

        public int BatchesCount { get; set; }

        public DateTime? ScheduleDate { get; set; }
        public string RepeatMode { get; set; }

        public bool IsActive { get; set; }
        public bool InArchive { get; set; }

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