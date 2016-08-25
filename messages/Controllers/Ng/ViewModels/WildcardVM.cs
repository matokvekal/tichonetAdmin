using Business_Logic.MessagesModule;
using Newtonsoft.Json;

namespace ticonet.Controllers.Ng.ViewModels {

    public class WildcardVM : INgViewModel {

        public static POCOReflector<tblWildcard, WildcardVM> tblWildcardPR =
            POCOReflector<tblWildcard, WildcardVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Name = o.Name,
                (o, m) => m.Code = o.Code,
                (o, m) => m.Key = o.Key
            );

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }

}