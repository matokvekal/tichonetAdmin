using System;
using System.Reflection;
using Business_Logic.MessagesModule;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace ticonet.Controllers.Ng.ViewModels {

    public class FilterVM : INgViewModel {
        public static POCOReflector<tblFilter, FilterVM> tblFilterPR =
            POCOReflector<tblFilter, FilterVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.Name = o.Name,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Key = o.Key,
                (o, m) => {
                    //TODO REFACTOR
                    try {
                        var vals = JsonConvert.DeserializeObject<string[]>(o.ValuesJSON);
                        var ops = JsonConvert.DeserializeObject<string[]>(o.OperatorsJSON);
                        m.ValsOps = new ValueOperatorPair[vals.Length];
                        for (int i = 0; i < vals.Length; i++) {
                            m.ValsOps[i] = new ValueOperatorPair(vals[i], ops[i], o.Type);
                        }
                    }
                    catch {
                        m.ValsOps = new ValueOperatorPair[1];
                    }
                },
                (o, m) => m.Type = o.Type,
                (o, m) => m.allowMultipleSelection = o.allowMultipleSelection ?? false,
                (o, m) => m.allowUserInput = o.allowUserInput ?? false,
                (o, m) => m.autoUpdatedList = o.autoUpdatedList ?? false
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