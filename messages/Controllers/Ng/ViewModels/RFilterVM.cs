using System;
using System.Reflection;
using Business_Logic.MessagesModule;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace ticonet.Controllers.Ng.ViewModels {
    public class RFilterVM : INgViewModel {
        public static POCOReflector<tblRecepientFilter, RFilterVM> tblRecepientFilterPR =
            POCOReflector<tblRecepientFilter, RFilterVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.Name = o.Name,
                (o, m) => m.BaseTableId = o.tblRecepientFilterTableNameId
            );

        public int Id { get; set; }
        public string Name { get; set; }
        public int BaseTableId { get; set; }

        //for ng

        public FilterVM[] filters { get; set; }
        public WildcardVM[] wildcards { get; set; }
        public RecepientcardVM[] reccards { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }
}