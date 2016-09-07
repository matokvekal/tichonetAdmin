using Business_Logic.MessagesModule.EntitiesExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule {
    public partial class tblMessageBatch : IMessagesModuleEntity, IErrorLoged {
        void IErrorLoged.AddError(string errorMessage) {
            Errors = string.IsNullOrEmpty(Errors) ? errorMessage : Errors + "; " + errorMessage;
        }
    }
}
