using System;
using System.Reflection;
using Business_Logic.MessagesModule;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace ticonet.Controllers.Ng.ViewModels {

    public class RFilterTableVM : INgViewModel {
        public static POCOReflector<tblRecepientFilterTableName, RFilterTableVM> tblRecepientFilterTableNamePR =
            POCOReflector<tblRecepientFilterTableName, RFilterTableVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.Name = o.Name,
                (o, m) => m.ReferencedTableName = o.ReferncedTableName
            );

        public int Id { get; set; }
        public string Name { get; set; }

        public string ReferencedTableName { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }

}