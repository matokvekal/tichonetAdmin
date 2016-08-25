using System;
using System.Collections.Generic;
using Business_Logic.MessagesModule;
using System.Linq;
using System.Web;

namespace ticonet.Controllers.Ng.ViewModels {
    public class RecepientcardVM : INgViewModel {
        public static POCOReflector<tblRecepientCard, RecepientcardVM> tblRecepientCardPR =
            POCOReflector<tblRecepientCard, RecepientcardVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Name = o.Name,
                (o, m) => m.NameKey = o.NameKey,
                (o, m) => m.EmailKey = o.EmailKey,
                (o, m) => m.PhoneKey = o.PhoneKey
            );

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }
        public string Name { get; set; }
        public string NameKey { get; set; }
        public string EmailKey { get; set; }
        public string PhoneKey { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }
}