using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule {
    public partial class MessageContext {

        public MessageContext(string nameOrConnectionString)
            : base(nameOrConnectionString) {
        }

    }
}
