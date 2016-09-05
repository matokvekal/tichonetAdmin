using System;
using System.Reflection;
using Business_Logic.MessagesModule;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Business_Logic.MessagesModule.EntitiesExtensions;
using System.Linq;
using Business_Logic.SqlContext;

namespace ticonet.Controllers.Ng.ViewModels {

    public class FilterVM : INgViewModel {
        public static POCOReflectorInje<tblFilter, FilterVM, ISqlLogic> tblFilterPR =
            POCOReflectorInje<tblFilter, FilterVM, ISqlLogic>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.Name = o.Name,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Key = o.Key,

                (o, m) => m.Type = o.Type,
                (o, m) => m.allowMultipleSelection = o.allowMultipleSelection ?? false,
                (o, m) => m.allowUserInput = o.allowUserInput ?? false,
                (o, m) => m.autoUpdatedList = o.autoUpdatedList ?? false
            ).AddInjected(
                (o, m, sql) => {
                    //TODO REMOVE REDUNTANT ValueOperatorPair Class
                    m.ValsOps = tblFilterHelper.GetValueOperatorPairs(o, sql)
                                .Select(x => new ValueOperatorPair(x.Value.ToString(), x.Operator, o.Type))
                                .ToArray();

                }
            );

        public int Id { get; set; }
        public string Name { get; set; }
        public int RecepientFilterId { get; set; }
        public string Key { get; set; }
        public ValueOperatorPair[] ValsOps { get; set; }

        public string Type { get; set; }

        public bool allowMultipleSelection { get; set; }
        public bool allowUserInput { get; set; }

        public bool autoUpdatedList { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }
}